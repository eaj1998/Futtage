using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Futtage.Infrastructure.Extensions;
using Futtage.Infrastructure.Logging;
using Futtage.Presentation.Presenters;
using Futtage.Presentation.Views;

namespace Futtage
{
    internal static class Program
    {
        private static ServiceProvider? _serviceProvider;
        private static ILogger? _logger;

        [STAThread]
        static void Main()
        {
            try
            {
                // Configuração global da aplicação
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetHighDpiMode(HighDpiMode.SystemAware);

                // Configurar tratamento global de exceções
                SetupGlobalExceptionHandling();

                // Configurar e construir container de DI
                var services = new ServiceCollection();
                var configuration = BuildConfiguration();

                services.AddFuttageServices(configuration);
                _serviceProvider = services.BuildServiceProvider();

                _logger = _serviceProvider.GetRequiredService<ILogger>();
                _logger.LogInfo("Aplicação iniciada");

                // Criar e executar formulário principal
                var mainForm = CreateMainForm();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                HandleStartupException(ex);
            }
            finally
            {
                _logger?.LogInfo("Aplicação encerrada");
                _serviceProvider?.Dispose();
            }
        }

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            // Carregar arquivo .env se existir
            var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (File.Exists(envFile))
            {
                try
                {
                    DotNetEnv.Env.Load(envFile);
                }
                catch
                {
                    // Ignorar erro se não conseguir carregar .env
                }
            }

            // Adicionar variáveis de ambiente
            builder.AddEnvironmentVariables();

            return builder.Build();
        }

        private static Form CreateMainForm()
        {
            var mainPresenter = _serviceProvider!.GetRequiredService<MainPresenter>();

            // Criar Form1 e integrar com presenter
            var form = new Form1();
            form.SetPresenter(mainPresenter);

            // Inicializar o presenter com referência ao form
            mainPresenter.Initialize(form);

            return form;
        }

        private static void IntegratePresenterWithForm1(Form1 form, MainPresenter presenter)
        {
            // Este método não é mais necessário, pois a integração
            // é feita diretamente no CreateMainForm
        }

        private static void SetupGlobalExceptionHandling()
        {
            Application.ThreadException += (sender, e) =>
            {
                HandleException(e.Exception, "Application.ThreadException");
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                HandleException(e.ExceptionObject as Exception, "AppDomain.UnhandledException", e.IsTerminating);
            };
        }

        private static void HandleException(Exception? ex, string source, bool isTerminating = false)
        {
            try
            {
                _logger?.LogError($"Exceção não tratada em {source}: {ex?.Message}", ex);

                var message = isTerminating
                    ? $"Erro crítico na aplicação:\n\n{ex?.Message}\n\nA aplicação será encerrada."
                    : $"Ocorreu um erro inesperado:\n\n{ex?.Message}\n\nDeseja continuar?";

                var buttons = isTerminating ? MessageBoxButtons.OK : MessageBoxButtons.YesNo;
                var icon = isTerminating ? MessageBoxIcon.Error : MessageBoxIcon.Warning;

                var result = MessageBox.Show(message, "Erro - Futtage", buttons, icon);

                if (!isTerminating && result == DialogResult.No)
                {
                    Application.Exit();
                }
            }
            catch
            {
                // Falha silenciosa para evitar loops infinitos
                MessageBox.Show("Erro crítico na aplicação. A aplicação será encerrada.",
                    "Erro Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private static void HandleStartupException(Exception ex)
        {
            var message = $"Erro ao inicializar a aplicação:\n\n{ex.Message}\n\nVerifique se todas as dependências estão instaladas.";
            MessageBox.Show(message, "Erro de Inicialização - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}