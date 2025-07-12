namespace Futtage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AtualizarEstadoDosBotoes();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Abre a janela de diálogo para o usuário selecionar o(s) arquivo(s)
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                lstArquivosSelecionados.Items.Clear();

                foreach (string arquivo in openFileDialog1.FileNames)
                {
                    lstArquivosSelecionados.Items.Add(arquivo);
                }

                MessageBox.Show(openFileDialog1.FileNames.Length.ToString() + " arquivo(s) selecionado(s)!", "Sucesso");
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
                FormAguarde formAguarde = new FormAguarde();

                try
                {
                   
                    formAguarde.StartPosition = FormStartPosition.Manual;

                    int startX = this.Location.X + (this.Width - formAguarde.Width) / 2;
                    int startY = this.Location.Y + (this.Height - formAguarde.Height) / 2;

                    formAguarde.Location = new Point(startX, startY);

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
            btnJuntarVideos.Enabled = (lstArquivosSelecionados.Items.Count >= 2);

            bool itemSelecionado = (lstArquivosSelecionados.SelectedIndex != -1);

            btnExcluirItem.Enabled = itemSelecionado;

            btnMoverCima.Enabled = (itemSelecionado && lstArquivosSelecionados.SelectedIndex > 0);

            btnMoverBaixo.Enabled = (itemSelecionado && lstArquivosSelecionados.SelectedIndex < lstArquivosSelecionados.Items.Count - 1);
        }

    }
}
