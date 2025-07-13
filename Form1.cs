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
        private string? caminhoDoVideoPronto = null;
        private FormAguarde formAguardeAtual;
        private long totalBytesUpload;

        private YouTubeService youtubeService;
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

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Arquivos de Vídeo MP4 (*.mp4)|*.mp4|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Title = "Selecione os vídeos";

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                lstArquivosSelecionados.Items.Clear();

                string[] arquivosSelecionados = openFileDialog1.FileNames;

                var infosDeArquivos = arquivosSelecionados.Select(caminho => new FileInfo(caminho));

                var arquivosOrdenados = infosDeArquivos.OrderBy(f => f.CreationTime);

                foreach (var arquivoInfo in arquivosOrdenados)
                {
                    lstArquivosSelecionados.Items.Add(arquivoInfo.FullName);
                }

                //MessageBox.Show(openFileDialog1.FileNames.Length.ToString() + " arquivo(s) selecionado(s) e ordenado(s) por data!", "Sucesso");
            }

            AtualizarEstadoDosBotoes();
        }

        private async void btnJuntarVideos_Click(object sender, EventArgs e)
        {
            if (lstArquivosSelecionados.Items.Count < 2)
            {
                MessageBox.Show("Por favor, selecione pelo menos dois arquivos de vídeo para juntar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var listaDeArquivos = new System.Text.StringBuilder();
            foreach (var item in lstArquivosSelecionados.Items)
            {
                string caminhoCorrigido = item.ToString().Replace('\\', '/');
                listaDeArquivos.AppendLine($"file '{caminhoCorrigido}'");
            }

            string caminhoDoArquivoLista = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mylist.txt");
            System.IO.File.WriteAllText(caminhoDoArquivoLista, listaDeArquivos.ToString());

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Vídeo MP4 (*.mp4)|*.mp4";
            saveDialog.Title = "Salvar vídeo final como...";
            saveDialog.FileName = "video_final.mp4";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                this.caminhoDoVideoPronto = null;
                btnFazerUpload.Enabled = false;
                FormAguarde formAguarde = new FormAguarde();

                try
                {

                    formAguarde.StartPosition = FormStartPosition.Manual;

                    int startX = this.Location.X + (this.Width - formAguarde.Width) / 2;
                    int startY = this.Location.Y + (this.Height - formAguarde.Height) / 2;

                    formAguarde.Location = new Point(startX, startY);
                    formAguarde.ConfigurarVisibilidadeDaBarra(false);
                    formAguarde.Show(this);

                    string caminhoDoVideoFinal = saveDialog.FileName;
                    string comando = $"-y -f concat -safe 0 -i \"{caminhoDoArquivoLista}\" -c copy \"{caminhoDoVideoFinal}\"";

                    var processo = new System.Diagnostics.Process();
                    processo.StartInfo.FileName = "ffmpeg.exe";
                    processo.StartInfo.Arguments = comando;
                    processo.StartInfo.CreateNoWindow = true;
                    processo.StartInfo.UseShellExecute = false;
                    processo.StartInfo.RedirectStandardOutput = true;
                    processo.StartInfo.RedirectStandardError = true;

                    processo.Start();

                    var outputTask = processo.StandardOutput.ReadToEndAsync();
                    var errorTask = processo.StandardError.ReadToEndAsync();

                    await processo.WaitForExitAsync();

                    formAguarde.Close();

                    string output = await outputTask;
                    string error = await errorTask;

                    if (processo.ExitCode == 0)
                    {
                        MessageBox.Show($"Vídeo salvo com sucesso em:\n{caminhoDoVideoFinal}", "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.caminhoDoVideoPronto = caminhoDoVideoFinal;
                        btnFazerUpload.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("O FFmpeg encontrou um erro durante a execução.\n\n" +
                                        "Mensagem de Erro:\n" + error,
                                        "Erro do FFmpeg", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    if (formAguarde != null && !formAguarde.IsDisposed)
                    {
                        formAguarde.Close();
                    }
                    MessageBox.Show("Ocorreu um erro CRÍTICO.\n\nErro: " + ex.Message, "Erro de Execução", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (System.IO.File.Exists(caminhoDoArquivoLista))
                    {
                        System.IO.File.Delete(caminhoDoArquivoLista);
                    }
                }
            }
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
                int indiceSelecionado = lstArquivosSelecionados.SelectedIndex;

                lstArquivosSelecionados.Items.RemoveAt(indiceSelecionado);

                if (lstArquivosSelecionados.Items.Count > 0)
                {
                    if (indiceSelecionado >= lstArquivosSelecionados.Items.Count)
                    {
                        lstArquivosSelecionados.SelectedIndex = lstArquivosSelecionados.Items.Count - 1;
                    }
                    else
                    {
                        lstArquivosSelecionados.SelectedIndex = indiceSelecionado;
                    }
                }
            }
        }

        private void lstArquivosSelecionados_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarEstadoDosBotoes();
        }

        private void AtualizarEstadoDosBotoes()
        {
            if (btnFazerUpload.Enabled)
            {
                btnFazerUpload.Enabled = false;
                this.caminhoDoVideoPronto = null;
            }

            btnJuntarVideos.Enabled = (lstArquivosSelecionados.Items.Count >= 2);

            bool itemSelecionado = (lstArquivosSelecionados.SelectedIndex != -1);

            btnExcluirItem.Enabled = itemSelecionado;

            btnMoverCima.Enabled = (itemSelecionado && lstArquivosSelecionados.SelectedIndex > 0);

            btnMoverBaixo.Enabled = (itemSelecionado && lstArquivosSelecionados.SelectedIndex < lstArquivosSelecionados.Items.Count - 1);

            btnMoverBaixo.Enabled = (itemSelecionado && lstArquivosSelecionados.SelectedIndex < lstArquivosSelecionados.Items.Count - 1);

            
        }

        private async void btnFazerUpload_Click(object sender, EventArgs e)
        {
            string? caminhoDoVideo = this.caminhoDoVideoPronto;

            if (string.IsNullOrEmpty(caminhoDoVideo) || !File.Exists(caminhoDoVideo))
            {
                MessageBox.Show("Nenhum vídeo pronto para upload foi encontrado ou o arquivo foi movido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnFazerUpload.Enabled = false;
                return;
            }
            string primeiroVideoPath = lstArquivosSelecionados.Items[0].ToString() ?? new DateTime().ToString();

            FileInfo infoDoPrimeiroVideo = new FileInfo(primeiroVideoPath);

            DateTime dataPrimeiroVideo = infoDoPrimeiroVideo.CreationTime;

            using (FormDetalhesVideo formDetalhes = new FormDetalhesVideo(dataPrimeiroVideo))
            {
                if (formDetalhes.ShowDialog(this) == DialogResult.OK)
                {
                    string titulo = formDetalhes.TituloDoVideo;
                    string descricao = formDetalhes.DescricaoDoVideo;
                    bool paraCriancas = formDetalhes.IsConteudoInfantil;

                    this.formAguardeAtual = new FormAguarde();
                    this.formAguardeAtual.StartPosition = FormStartPosition.Manual;
                    int startX = this.Location.X + (this.Width - this.formAguardeAtual.Width) / 2;
                    int startY = this.Location.Y + (this.Height - this.formAguardeAtual.Height) / 2;
                    this.formAguardeAtual.Location = new Point(startX, startY);
                    this.formAguardeAtual.ConfigurarVisibilidadeDaBarra(true);
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
                            video.Snippet = new VideoSnippet
                            {
                                Title = titulo,
                                Description = descricao,
                                CategoryId = "22" 
                            };
                            video.Status = new VideoStatus
                            {
                                PrivacyStatus = "public",
                                SelfDeclaredMadeForKids = paraCriancas
                            };

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
                else
                {
                    MessageBox.Show("Upload cancelado pelo usuário.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        void OnProgressChanged(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    if (this.formAguardeAtual != null && !this.formAguardeAtual.IsDisposed)
                    {
                        int porcentagem = (int)(((double)progress.BytesSent / this.totalBytesUpload) * 100);

                        this.formAguardeAtual.AtualizarProgresso(porcentagem);
                    }
                    break;
                case UploadStatus.Failed:
                    MessageBox.Show($"Ocorreu um erro durante o upload: {progress.Exception}");
                    break;
            }
        }

        private async void OnResponseReceived(Video video)
        {
            MessageBox.Show($"Upload do VÍDEO concluído! ID: {video.Id}", "Sucesso - Etapa 1/2", MessageBoxButtons.OK, MessageBoxIcon.Information);

            try
            {
                MessageBox.Show("Iniciando upload da capa padrão...", "Etapa 2/2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                using (var stream = new MemoryStream(Properties.Resources.capa_padrao))
                {
                    var thumbnailsSetRequest = this.youtubeService.Thumbnails.Set(video.Id, stream, "image/png");
                    await thumbnailsSetRequest.UploadAsync();
                }

                MessageBox.Show("Capa personalizada enviada com sucesso!", "Sucesso Final!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Falha ao enviar a capa personalizada.\n\n" +
                                "Possível Causa: O canal do YouTube não é verificado ou o recurso da imagem não foi encontrado.\n" +
                                "Erro Detalhado: " + ex.Message, "Erro na Capa", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
