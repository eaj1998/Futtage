using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Upload;
using System.IO;
using System.Linq;

namespace Futtage
{
    public partial class Form1 : Form
    {
        private string? caminhoCapaPersonalizada = null;
        private string? caminhoVideoJuntado = null;
        private string? caminhoVideoFinal = null;

        private bool juncaoConcluida = false;

        private FormAguarde formAguardeAtual;
        private long totalBytesUpload;
        private YouTubeService youtubeService;

        private bool passo1Completo = false;
        private bool passo2Completo = false;

        public Form1()
        {
            InitializeComponent();
            AtualizarEstadoDosBotoes();
            btnFazerUpload.Enabled = false;
            try
            {
                byte[] iconBytes = Properties.Resources.app_icon;

                using (MemoryStream ms = new MemoryStream(iconBytes))
                {
                    this.Icon = new Icon(ms);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar o ícone do aplicativo. Erro: " + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnSelecionarArquivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Arquivos de Vídeo MP4 (*.mp4)|*.mp4|Todos os arquivos (*.*)|*.*",
                Title = "Selecione os vídeos para juntar"
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] arquivosSelecionados = openFileDialog1.FileNames;
                var infosDeArquivos = arquivosSelecionados.Select(caminho => new FileInfo(caminho));
                var arquivosOrdenados = infosDeArquivos.OrderBy(f => f.CreationTime);

                lstArquivosSelecionados.Items.Clear();
                foreach (var arquivoInfo in arquivosOrdenados)
                {
                    lstArquivosSelecionados.Items.Add(arquivoInfo.FullName);
                }

                juncaoConcluida = false;
                caminhoVideoJuntado = null;
                caminhoVideoFinal = null;
            }

            AtualizarEstadoDosBotoes();
        }

        private async void btnJuntarVideos_Click(object sender, EventArgs e)
        {
            var listaDeArquivos = new System.Text.StringBuilder();
            foreach (var item in lstArquivosSelecionados.Items)
            {
                listaDeArquivos.AppendLine($"file '{item.ToString().Replace('\\', '/')}'");
            }

            string caminhoDoArquivoLista = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mylist.txt");
            File.WriteAllText(caminhoDoArquivoLista, listaDeArquivos.ToString());

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Vídeo MP4 (*.mp4)|*.mp4",
                Title = "Salvar vídeo juntado como...",
                FileName = "video_juntado.mp4"
            };

            if (saveDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.formAguardeAtual = new FormAguarde();
                this.formAguardeAtual.ConfigurarVisibilidadeDaBarra(false); // Modo genérico, sem barra de progresso
                this.formAguardeAtual.StartPosition = FormStartPosition.Manual;
                int startX = this.Location.X + (this.Width - this.formAguardeAtual.Width) / 2;
                int startY = this.Location.Y + (this.Height - this.formAguardeAtual.Height) / 2;
                this.formAguardeAtual.Location = new Point(startX, startY);
                this.formAguardeAtual.Show(this);

                try
                {
                    await Task.Run(() =>
                    {
                        string caminhoDoVideoSaida = saveDialog.FileName;
                        string comando = $"-y -f concat -safe 0 -i \"{caminhoDoArquivoLista}\" -c copy \"{caminhoDoVideoSaida}\"";
                        var startInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg.exe", comando)
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using (var process = System.Diagnostics.Process.Start(startInfo))
                        {
                            process.WaitForExit();
                        }
                    });

                    MessageBox.Show("Vídeos juntados com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.caminhoVideoJuntado = saveDialog.FileName;
                    this.caminhoVideoFinal = this.caminhoVideoJuntado;
                    this.juncaoConcluida = true;

                    tabControlPrincipal.SelectedTab = tabPageCorte;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao juntar os vídeos: " + ex.Message, "Erro");
                }
                finally
                {
                    this.formAguardeAtual.Close();
                    if (File.Exists(caminhoDoArquivoLista)) File.Delete(caminhoDoArquivoLista);
                }
            }
        }
        private async void btnCortarVideo_Click(object sender, EventArgs e)
        {
            string videoDeEntrada = this.caminhoVideoJuntado;
            string tempoInicio = txtInicioCorte.Text;
            string tempoFim = txtFimCorte.Text;

            if (string.IsNullOrEmpty(tempoInicio) || string.IsNullOrEmpty(tempoFim))
            {
                MessageBox.Show("Por favor, preencha os tempos de início e fim.", "Aviso");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Vídeo MP4 (*.mp4)|*.mp4",
                FileName = Path.GetFileNameWithoutExtension(videoDeEntrada) + "_cortado.mp4"
            };

            if (saveDialog.ShowDialog(this) == DialogResult.OK)
            {
                string videoDeSaida = saveDialog.FileName;
                string comando = $"-ss {tempoInicio} -i \"{videoDeEntrada}\" -to {tempoFim} -c copy \"{videoDeSaida}\"";

                this.formAguardeAtual = new FormAguarde();
                this.formAguardeAtual.ConfigurarVisibilidadeDaBarra(false);
                this.formAguardeAtual.StartPosition = FormStartPosition.Manual;
                int startX = this.Location.X + (this.Width - this.formAguardeAtual.Width) / 2;
                int startY = this.Location.Y + (this.Height - this.formAguardeAtual.Height) / 2;
                this.formAguardeAtual.Location = new Point(startX, startY);
                this.formAguardeAtual.Show(this);

                try
                {
                    await Task.Run(() =>
                    {
                        var startInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg.exe", comando)
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using (var process = System.Diagnostics.Process.Start(startInfo))
                        {
                            process.WaitForExit();
                        }
                    });

                    MessageBox.Show("Vídeo cortado com sucesso!", "Sucesso");

                    this.caminhoVideoFinal = videoDeSaida;

                    tabControlPrincipal.SelectedTab = tabPageCapa;
                    btnFazerUpload.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao cortar o vídeo: " + ex.Message, "Erro");
                }
                finally
                {
                    this.formAguardeAtual.Close();
                }
            }
        }
        private void btnPularCorte_Click(object sender, EventArgs e)
        {
            tabControlPrincipal.SelectedTab = tabPageUpload;
            btnFazerUpload.Enabled = true;
        }

        private void btnMoverCima_Click(object sender, EventArgs e)
        {
            if (lstArquivosSelecionados.SelectedItem != null && lstArquivosSelecionados.SelectedIndex > 0)
            {
                object itemSelecionado = lstArquivosSelecionados.SelectedItem;
                int indiceSelecionado = lstArquivosSelecionados.SelectedIndex;
                lstArquivosSelecionados.Items.RemoveAt(indiceSelecionado);
                lstArquivosSelecionados.Items.Insert(indiceSelecionado - 1, itemSelecionado);
                lstArquivosSelecionados.SelectedIndex = indiceSelecionado - 1;
            }
        }

        private void btnMoverBaixo_Click(object sender, EventArgs e)
        {
            if (lstArquivosSelecionados.SelectedItem != null && lstArquivosSelecionados.SelectedIndex < lstArquivosSelecionados.Items.Count - 1)
            {
                object itemSelecionado = lstArquivosSelecionados.SelectedItem;
                int indiceSelecionado = lstArquivosSelecionados.SelectedIndex;
                lstArquivosSelecionados.Items.RemoveAt(indiceSelecionado);
                lstArquivosSelecionados.Items.Insert(indiceSelecionado + 1, itemSelecionado);
                lstArquivosSelecionados.SelectedIndex = indiceSelecionado + 1;
            }
        }

        private void btnExcluirItem_Click(object sender, EventArgs e)
        {
            if (lstArquivosSelecionados.SelectedItem != null)
            {
                lstArquivosSelecionados.Items.Remove(lstArquivosSelecionados.SelectedItem);
            }
            AtualizarEstadoDosBotoes();
        }

        private void lstArquivosSelecionados_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarEstadoDosBotoes();
        }

        private void AtualizarEstadoDosBotoes()
        {
            if (juncaoConcluida)
            {
                juncaoConcluida = false;
                caminhoVideoJuntado = null;
                caminhoVideoFinal = null;
            }

            btnJuntarVideos.Enabled = (lstArquivosSelecionados.Items.Count >= 2);

            bool itemSelecionado = (lstArquivosSelecionados.SelectedIndex != -1);
            btnExcluirItem.Enabled = itemSelecionado;
            btnMoverCima.Enabled = (itemSelecionado && lstArquivosSelecionados.SelectedIndex > 0);
            btnMoverBaixo.Enabled = (itemSelecionado && lstArquivosSelecionados.SelectedIndex < lstArquivosSelecionados.Items.Count - 1);
        }

        private async void btnFazerUpload_Click(object sender, EventArgs e)
        {
            string? caminhoDoVideo = this.caminhoVideoFinal;

            if (string.IsNullOrEmpty(caminhoDoVideo) || !File.Exists(caminhoDoVideo))
            {
                MessageBox.Show("Nenhum vídeo final foi encontrado para o upload.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (FormDetalhesVideo formDetalhes = new FormDetalhesVideo(new FileInfo(caminhoDoVideo).CreationTime))
            {
                if (formDetalhes.ShowDialog(this) == DialogResult.OK)
                {
                    string titulo = formDetalhes.TituloDoVideo;
                    string descricao = formDetalhes.DescricaoDoVideo;
                    bool paraCriancas = formDetalhes.IsConteudoInfantil;

                    this.formAguardeAtual = new FormAguarde();
                    this.formAguardeAtual.ConfigurarVisibilidadeDaBarra(true);
                    this.formAguardeAtual.StartPosition = FormStartPosition.Manual;
                    int startX = this.Location.X + (this.Width - this.formAguardeAtual.Width) / 2;
                    int startY = this.Location.Y + (this.Height - this.formAguardeAtual.Height) / 2;
                    this.formAguardeAtual.Location = new Point(startX, startY);
                    this.formAguardeAtual.Show(this);

                    try
                    {
                        await Task.Run(async () =>
                        {
                            DotNetEnv.Env.Load();
                            var clientSecrets = new ClientSecrets
                            {
                                ClientId = Environment.GetEnvironmentVariable("YOUTUBE_CLIENT_ID"),
                                ClientSecret = Environment.GetEnvironmentVariable("YOUTUBE_CLIENT_SECRET")
                            };
                            UserCredential credenciais = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                                clientSecrets, new[] { YouTubeService.Scope.YoutubeUpload }, "user", CancellationToken.None);

                            this.youtubeService = new YouTubeService(new BaseClientService.Initializer()
                            {
                                HttpClientInitializer = credenciais,
                                ApplicationName = "Futtage Uploader"
                            });

                            var video = new Video();
                            video.Snippet = new VideoSnippet { Title = titulo, Description = descricao, CategoryId = "22" };
                            video.Status = new VideoStatus { PrivacyStatus = "public", SelfDeclaredMadeForKids = paraCriancas };

                            this.totalBytesUpload = new FileInfo(caminhoDoVideo).Length;

                            using (var fileStream = new FileStream(caminhoDoVideo, FileMode.Open))
                            {
                                var videosInsertRequest = this.youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                                videosInsertRequest.ProgressChanged += OnProgressChanged;
                                videosInsertRequest.ResponseReceived += OnResponseReceived;
                                await videosInsertRequest.UploadAsync();
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro durante o processo de upload: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (this.formAguardeAtual != null && !this.formAguardeAtual.IsDisposed)
                        {
                            this.formAguardeAtual.Close();
                        }
                    }
                }
            }
        }

        void OnProgressChanged(IUploadProgress progress)
        {
            if (progress.Status == UploadStatus.Uploading)
            {
                if (this.formAguardeAtual != null && !this.formAguardeAtual.IsDisposed)
                {
                    if (this.totalBytesUpload > 0)
                    {
                        int porcentagem = (int)(((double)progress.BytesSent / this.totalBytesUpload) * 100);
                        this.formAguardeAtual.AtualizarProgresso(porcentagem);
                    }
                }
            }
        }

        private async void OnResponseReceived(Video video)
        {
            MessageBox.Show($"Upload do VÍDEO concluído! ID: {video.Id}", "Sucesso - Etapa 1/2");

            try
            {
                MessageBox.Show("Iniciando upload da capa...", "Etapa 2/2", MessageBoxButtons.OK, MessageBoxIcon.Information);


                using (Stream stream = GetThumbnailStream())
                {
                    var thumbnailsSetRequest = this.youtubeService.Thumbnails.Set(video.Id, stream, "image/png");
                    await thumbnailsSetRequest.UploadAsync();
                }

                MessageBox.Show("Capa enviada com sucesso!", "Sucesso Final!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Falha ao enviar a capa: " + ex.Message, "Erro na Capa", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex < tabControlPrincipal.SelectedIndex) return;

            if (e.TabPage == tabPageJuntar && lstArquivosSelecionados.Items.Count < 2)
            {
                e.Cancel = true;
            }
            else if (e.TabPage == tabPageCorte && !juncaoConcluida)
            {
                e.Cancel = true;
            }
            else if (e.TabPage == tabPageUpload && string.IsNullOrEmpty(caminhoVideoFinal))
            {
                e.Cancel = true;
            }
        }

        private void btnVoltarCorte_Click(object sender, EventArgs e)
        {
            tabControlPrincipal.SelectedTab = tabPageCorte;
        }

        private void btnProximoPasso1_Click(object sender, EventArgs e)
        {
            tabControlPrincipal.SelectedTab = tabPageJuntar;
        }

        private void btnVoltarPasso1_Click(object sender, EventArgs e)
        {
            tabControlPrincipal.SelectedTab = tabPageSelecao;
        }

        private void tabPageCapa_Enter(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(caminhoCapaPersonalizada))
            {
                try
                {
                    byte[] imageBytes = Properties.Resources.capa_padrao;

                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBoxCapa.Image = Image.FromStream(ms);
                    }

                    lblCaminhoCapa.Text = "Usando capa padrão do aplicativo.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possível carregar a imagem de capa padrão. Erro: " + ex.Message);
                    pictureBoxCapa.Image = null;
                }
            }
        }

        private void btnSelecionarCapa_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogCapa = new OpenFileDialog
            {
                Filter = "Arquivos de Imagem (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png",
                Title = "Selecione uma nova capa"
            };

            if (openFileDialogCapa.ShowDialog() == DialogResult.OK)
            {
                this.caminhoCapaPersonalizada = openFileDialogCapa.FileName;

                pictureBoxCapa.Image = Image.FromFile(this.caminhoCapaPersonalizada);
                lblCaminhoCapa.Text = this.caminhoCapaPersonalizada;
            }
        }

        private Stream GetThumbnailStream()
        {
            if (!string.IsNullOrEmpty(caminhoCapaPersonalizada) && File.Exists(caminhoCapaPersonalizada))
            {
                return new FileStream(caminhoCapaPersonalizada, FileMode.Open, FileAccess.Read);
            }
            else
            {
                return new MemoryStream(Properties.Resources.capa_padrao);
            }
        }

        private void btnPulaPassoFinal_Click(object sender, EventArgs e)
        {
            tabControlPrincipal.SelectedTab = tabPageUpload;
        }
    }
}
