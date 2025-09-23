<<<<<<< HEAD
=======
﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows.Forms;
using Futtage.Infrastructure.Extensions;
using Futtage.Presentation.Presenters;

>>>>>>> d41ffedcd2d253031af9cc1242882502c952df34
namespace Futtage
{
    internal static class Program
    {
<<<<<<< HEAD
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
=======
        [STAThread]
        static void Main()
        {
            // Configurar aplicação WinForms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            try
            {
                // Configurar tratamento global de erros
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                CheckFFmpegAvailability();

                var configuration = BuildConfiguration();                

                var services = new ServiceCollection();
                services.AddFuttageServices(configuration);

                using var serviceProvider = services.BuildServiceProvider();

                var mainForm = new Form1();
                var presenter = serviceProvider.GetRequiredService<MainPresenter>();

                mainForm.SetPresenter(presenter);
                presenter.Initialize(mainForm);

                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Erro crítico ao iniciar aplicação:\n{ex.Message}\n\n" +
                                 $"Verifique:\n" +
                                 $"• FFmpeg está na pasta do aplicativo\n" +
                                 $"• Credenciais do YouTube estão configuradas\n" +
                                 $"• .NET 8.0 está instalado\n\n" +
                                 $"Detalhes técnicos:\n{ex}";

                MessageBox.Show(errorMessage, "Erro Crítico - Futtage",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                System.Diagnostics.Debug.WriteLine($"❌ ERRO CRÍTICO: {ex}");
            }
        }

        private static void CheckFFmpegAvailability()
        {
            var possiblePaths = new[]
            {
                "ffmpeg.exe",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
                Path.Combine(Environment.CurrentDirectory, "ffmpeg.exe")
            };

            bool ffmpegFound = false;
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    ffmpegFound = true;
                    System.Diagnostics.Debug.WriteLine($"✅ FFmpeg encontrado: {path}");
                    break;
                }
            }

            if (!ffmpegFound)
            {
                var message = "⚠️ FFmpeg não encontrado!\n\n" +
                             "O processamento de vídeo pode não funcionar corretamente.\n\n" +
                             "Para resolver:\n" +
                             "1. Baixe FFmpeg em: https://www.gyan.dev/ffmpeg/builds/\n" +
                             "2. Extraia ffmpeg.exe para a pasta do aplicativo\n" +
                             "3. Reinicie o Futtage\n\n" +
                             "Deseja continuar mesmo assim?";

                var result = MessageBox.Show(message, "FFmpeg Não Encontrado - Futtage",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    Environment.Exit(0);
                }

                System.Diagnostics.Debug.WriteLine("⚠️ FFmpeg não encontrado - continuando sem ele");
            }
        }

        private static IConfiguration BuildConfiguration()
        {
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                var appsettingsPath = Path.Combine(currentDir, "appsettings.json");

                System.Diagnostics.Debug.WriteLine($"🔍 Tentando carregar: {appsettingsPath}");
                System.Diagnostics.Debug.WriteLine($"🔍 Arquivo existe? {File.Exists(appsettingsPath)}");

                if (!File.Exists(appsettingsPath))
                {
                    throw new FileNotFoundException($"appsettings.json não encontrado em: {appsettingsPath}");
                }

                var jsonContent = File.ReadAllText(appsettingsPath);
                System.Diagnostics.Debug.WriteLine($"🔍 Conteúdo do JSON (primeiros 200 chars): {jsonContent[..Math.Min(200, jsonContent.Length)]}");

                var builder = new ConfigurationBuilder()
                    .SetBasePath(currentDir)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                builder.AddEnvironmentVariables("FUTTAGE_");

                var configuration = builder.Build();

                var youtubeSection = configuration.GetSection("YouTube");
                System.Diagnostics.Debug.WriteLine($"🔍 Seção YouTube existe? {youtubeSection.Exists()}");

                if (youtubeSection.Exists())
                {
                    var allKeys = youtubeSection.GetChildren();
                    foreach (var key in allKeys)
                    {
                        System.Diagnostics.Debug.WriteLine($"🔍 Chave encontrada: {key.Key} = {key.Value}");
                    }
                }

                System.Diagnostics.Debug.WriteLine("✅ Configuração carregada com sucesso");
                return configuration;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Erro ao carregar configuração: {ex.Message}");

                MessageBox.Show($"Erro ao carregar appsettings.json:\n{ex.Message}\n\n" +
                               $"Pasta atual: {Directory.GetCurrentDirectory()}\n" +
                               $"Arquivo procurado: appsettings.json",
                               "Erro de Configuração", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw; 
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Thread Exception: {e.Exception}");

            var result = MessageBox.Show(
                $"Ocorreu um erro na aplicação:\n{e.Exception.Message}\n\n" +
                $"Deseja continuar?\n\n" +
                $"Detalhes: {e.Exception.GetType().Name}",
                "Erro - Futtage",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);

            if (result == DialogResult.No)
            {
                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Unhandled Exception: {e.ExceptionObject}");

            MessageBox.Show(
                $"Erro crítico não tratado:\n{e.ExceptionObject}\n\n" +
                $"A aplicação será encerrada.\n\n" +
                $"Por favor, reporte este erro aos desenvolvedores.",
                "Erro Crítico - Futtage",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            Environment.Exit(1);
>>>>>>> d41ffedcd2d253031af9cc1242882502c952df34
        }
    }
}