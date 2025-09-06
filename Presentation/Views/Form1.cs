using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Futtage.Core.Models;
using Futtage.Presentation.Presenters;

namespace Futtage
{
    public partial class Form1 : Form
    {
        private MainPresenter? _presenter;

        // Estado da UI
        private bool _isProcessing = false;

        public Form1()
        {
            InitializeComponent();
        }

        // Método para ser chamado pelo Program.cs
        public void SetPresenter(MainPresenter presenter)
        {
            _presenter = presenter;
        }

        // REMOVIDO MÉTODO DUPLICADO Form1_Load - será usado apenas o do Designer

        #region Eventos dos Botões - Integrados com Presenter

        private async void btnSelecionarArquivo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== INÍCIO btnSelecionarArquivo_Click ===");
            System.Diagnostics.Debug.WriteLine($"Presenter é null? {_presenter == null}");

            if (_presenter != null)
            {
                System.Diagnostics.Debug.WriteLine("Chamando presenter.OnSelectFilesClickedAsync()");
                try
                {
                    await _presenter.OnSelectFilesClickedAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERRO na seleção de arquivos: {ex.Message}");
                    MessageBox.Show($"Erro na seleção: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERRO: Presenter não configurado - usando fallback");
                await SelectFilesOriginalBehavior();
            }

            System.Diagnostics.Debug.WriteLine("=== FIM btnSelecionarArquivo_Click ===");
        }

        private async void btnAutenticacao_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== INÍCIO btnAutenticacao_Click ===");
            System.Diagnostics.Debug.WriteLine($"Presenter é null? {_presenter == null}");

            if (_presenter != null)
            {
                System.Diagnostics.Debug.WriteLine("Chamando presenter.OnAuthenticationClickedAsync()");
                try
                {
                    await _presenter.OnAuthenticationClickedAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERRO na autenticação: {ex.Message}");
                    MessageBox.Show($"Erro na autenticação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERRO: Presenter não configurado");
                MessageBox.Show("Presenter não configurado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            System.Diagnostics.Debug.WriteLine("=== FIM btnAutenticacao_Click ===");
        }

        private void btnProximoPasso1_Click(object sender, EventArgs e)
        {
            if (_presenter != null)
            {
                _presenter.OnNextStepClicked();
            }
            else
            {
                // Fallback
                if (lstArquivosSelecionados.Items.Count >= 2)
                {
                    tabControlPrincipal.SelectedIndex = 1;
                }
            }
        }

        private async void btnJuntarVideos_Click(object sender, EventArgs e)
        {
            if (_presenter != null)
            {
                await _presenter.OnConcatenateVideosClickedAsync();
            }
            else
            {
                MessageBox.Show("Presenter não configurado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnCortarVideo_Click(object sender, EventArgs e)
        {
            if (_presenter != null)
            {
                await _presenter.OnCutVideoClickedAsync(txtInicioCorte.Text, txtFimCorte.Text);
            }
            else
            {
                MessageBox.Show("Presenter não configurado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPularCorte_Click(object sender, EventArgs e)
        {
            if (_presenter != null)
            {
                _presenter.OnSkipCutClicked();
            }
            else
            {
                tabControlPrincipal.SelectedIndex = 3; // Fallback
            }
        }

        private async void btnSelecionarCapa_Click(object sender, EventArgs e)
        {
            if (_presenter != null)
            {
                await _presenter.OnSelectThumbnailClickedAsync();
            }
            else
            {
                // Fallback para comportamento original
                await SelectThumbnailOriginalBehavior();
            }
        }

        private void btnPulaPassoFinal_Click(object sender, EventArgs e)
        {
            if (_presenter != null)
            {
                _presenter.OnSkipThumbnailClicked();
            }
            else
            {
                tabControlPrincipal.SelectedIndex = 4; // Fallback
            }
        }

        private async void btnFazerUpload_Click(object sender, EventArgs e)
        {
            if (_presenter != null)
            {
                await _presenter.OnUploadToYouTubeClickedAsync();
            }
            else
            {
                MessageBox.Show("Presenter não configurado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos de Navegação e Lista

        private void btnMoverCima_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(-1);
        }

        private void btnMoverBaixo_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(1);
        }

        private void btnExcluirItem_Click(object sender, EventArgs e)
        {
            if (lstArquivosSelecionados.SelectedIndex >= 0)
            {
                lstArquivosSelecionados.Items.RemoveAt(lstArquivosSelecionados.SelectedIndex);
                UpdateUIState();
            }
        }

        private void lstArquivosSelecionados_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMoveButtonsState();
        }

        private void btnVoltarPasso1_Click(object sender, EventArgs e)
        {
            tabControlPrincipal.SelectedIndex = 0;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Validar navegação entre abas
            if (_presenter != null)
            {
                if (!_presenter.CanNavigateToTab(e.TabPageIndex))
                {
                    e.Cancel = true;
                    ShowNavigationError(e.TabPageIndex);
                    return;
                }
            }
            else
            {
                // Fallback - validação básica
                switch (e.TabPageIndex)
                {
                    case 1: // Concatenação
                        if (lstArquivosSelecionados.Items.Count < 2)
                        {
                            e.Cancel = true;
                            MessageBox.Show("Selecione pelo menos 2 vídeos antes de continuar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;
                }
            }

            // NOVO: Ações específicas ao entrar em cada aba
            switch (e.TabPageIndex)
            {
                case 2: // Aba de corte
                    BeginInvoke(new Action(OnCuttingTabEntered)); // Usar BeginInvoke para executar após a mudança de aba
                    break;
                case 3: // Aba de thumbnail
                    BeginInvoke(new Action(() => {
                        if (_presenter != null)
                        {
                            _presenter.OnThumbnailTabEntered();
                        }
                        else
                        {
                            LoadDefaultThumbnail();
                        }
                    }));
                    break;
            }
        }


        private void tabPageCapa_Enter(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== tabPageCapa_Enter INICIADO ===");

            if (_presenter != null)
            {
                System.Diagnostics.Debug.WriteLine("Chamando presenter.OnThumbnailTabEntered()");
                _presenter.OnThumbnailTabEntered();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Presenter não disponível - carregando thumbnail padrão diretamente");
                LoadDefaultThumbnailIfNeeded();
            }

            // Forçar atualização dos controles
            UpdateThumbnailTabControls();

            System.Diagnostics.Debug.WriteLine("=== tabPageCapa_Enter FINALIZADO ===");
        }

        private void txtInicioCorte_TextChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        private void txtFimCorte_TextChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        #endregion

        #region Métodos Públicos para o Presenter Atualizar a UI

        public void UpdateVideoList(List<VideoInfo> videos)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<List<VideoInfo>>(UpdateVideoList), videos);
                return;
            }

            lstArquivosSelecionados.Items.Clear();
            foreach (var video in videos)
            {
                var displayText = $"{video.FileName} ({video.FormattedDuration})";
                lstArquivosSelecionados.Items.Add(displayText);
            }

            // CORRIGIDO: Forçar atualização do estado após adicionar vídeos
            UpdateUIState();
            System.Diagnostics.Debug.WriteLine($"Vídeos na lista: {lstArquivosSelecionados.Items.Count}");
        }

        public void UpdateAuthenticationStatus(bool isAuthenticated, string? userName = null, string? userEmail = null, string? avatarUrl = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, string?, string?, string?>(UpdateAuthenticationStatus),
                    isAuthenticated, userName, userEmail, avatarUrl);
                return;
            }

            if (isAuthenticated)
            {
                lblContaLogada.Text = userName ?? "Usuário Autenticado";
                btnAutenticacao.Text = "✅ Conectado";
                btnAutenticacao.Enabled = false;
                btnAutenticacao.BackColor = Color.LightGreen;

                // Carregar avatar se disponível
                if (!string.IsNullOrEmpty(avatarUrl))
                {
                    LoadUserAvatar(avatarUrl);
                }
            }
            else
            {
                lblContaLogada.Text = "Desconhecido";
                btnAutenticacao.Text = "Login com o Google";
                btnAutenticacao.Enabled = true;
                btnAutenticacao.BackColor = SystemColors.Control;
                pictureBoxProfile.Image = null;
            }

            // CORRIGIDO: Forçar atualização do estado após mudança de autenticação
            UpdateUIState();
            System.Diagnostics.Debug.WriteLine($"Autenticado: {isAuthenticated}, Botão texto: {btnAutenticacao.Text}");
        }

        public void SetThumbnail(string imagePath)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetThumbnail), imagePath);
                return;
            }

            try
            {
                if (File.Exists(imagePath))
                {
                    using var img = Image.FromFile(imagePath);
                    pictureBoxCapa.Image = new Bitmap(img);
                    lblCaminhoCapa.Text = Path.GetFileName(imagePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar thumbnail: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowProgress(string message, int percentage = 0)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, int>(ShowProgress), message, percentage);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"=== ShowProgress: {message} ({percentage}%) ===");

            _isProcessing = true;
            this.Text = $"Futtage - {message}";
            this.Cursor = Cursors.WaitCursor;

            System.Diagnostics.Debug.WriteLine($"_isProcessing definido como: {_isProcessing}");

            // Desabilitar controles durante processamento
            EnableControls(false);

            System.Diagnostics.Debug.WriteLine("Controles desabilitados durante processamento");
        }

        public void HideProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(HideProgress));
                return;
            }

            System.Diagnostics.Debug.WriteLine("=== HideProgress CHAMADO ===");
            System.Diagnostics.Debug.WriteLine($"_isProcessing antes: {_isProcessing}");

            _isProcessing = false;
            this.Text = "Futtage";
            this.Cursor = Cursors.Default;

            System.Diagnostics.Debug.WriteLine($"_isProcessing depois: {_isProcessing}");

            // Reabilitar controles
            EnableControls(true);

            System.Diagnostics.Debug.WriteLine("Controles reabilitados após processamento");
            System.Diagnostics.Debug.WriteLine("=== HideProgress FINALIZADO ===");
        }

        public void NavigateToTab(int tabIndex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(NavigateToTab), tabIndex);
                return;
            }

            if (tabIndex >= 0 && tabIndex < tabControlPrincipal.TabPages.Count)
            {
                tabControlPrincipal.SelectedIndex = tabIndex;
            }
        }

        #endregion

        #region Métodos Auxiliares

        private void UpdateUIState()
        {
            var hasVideos = lstArquivosSelecionados.Items.Count > 0;
            var hasEnoughVideos = lstArquivosSelecionados.Items.Count >= 2;
            var isAuthenticated = btnAutenticacao.Text.Contains("✅");

            // Debug detalhado do estado
            System.Diagnostics.Debug.WriteLine($"=== UpdateUIState ===");
            System.Diagnostics.Debug.WriteLine($"Vídeos: {lstArquivosSelecionados.Items.Count}");
            System.Diagnostics.Debug.WriteLine($"Auth: {isAuthenticated}");
            System.Diagnostics.Debug.WriteLine($"Processing: {_isProcessing}");
            System.Diagnostics.Debug.WriteLine($"Aba atual: {tabControlPrincipal.SelectedIndex}");

            // Atualizar estado dos botões principais
            btnProximoPasso1.Enabled = hasEnoughVideos && isAuthenticated && !_isProcessing;
            btnJuntarVideos.Enabled = hasEnoughVideos && !_isProcessing;

            // Validação específica para aba de corte
            if (tabControlPrincipal.SelectedIndex == 2) // Aba de corte
            {
                bool cuttingFieldsValid = AreCuttingFieldsValid();
                btnCortarVideo.Enabled = !_isProcessing && cuttingFieldsValid;
                btnPularCorte.Enabled = !_isProcessing;

                System.Diagnostics.Debug.WriteLine($"Campos de corte válidos: {cuttingFieldsValid}");
                System.Diagnostics.Debug.WriteLine($"Início: '{txtInicioCorte.Text}', Fim: '{txtFimCorte.Text}'");
                System.Diagnostics.Debug.WriteLine($"Cortar Video habilitado: {btnCortarVideo.Enabled}");
                System.Diagnostics.Debug.WriteLine($"Pular Corte habilitado: {btnPularCorte.Enabled}");
            }

            // NOVO: Validação específica para aba de thumbnail
            if (tabControlPrincipal.SelectedIndex == 3) // Aba de thumbnail
            {
                UpdateThumbnailTabControls();
            }

            // NOVO: Validação específica para aba de upload
            if (tabControlPrincipal.SelectedIndex == 4) // Aba de upload
            {
                btnFazerUpload.Enabled = !_isProcessing && isAuthenticated;
                System.Diagnostics.Debug.WriteLine($"Fazer Upload habilitado: {btnFazerUpload.Enabled}");
            }

            UpdateMoveButtonsState();

            // Debug dos botões principais
            System.Diagnostics.Debug.WriteLine($"Próximo Passo habilitado: {btnProximoPasso1.Enabled}");
            System.Diagnostics.Debug.WriteLine($"Juntar Vídeos habilitado: {btnJuntarVideos.Enabled}");
            System.Diagnostics.Debug.WriteLine($"=== FIM UpdateUIState ===");
        }


        // CORRIGIDO: SetCuttingDefaults com forçar atualização de estado
        public void SetCuttingDefaults(string startTime, string endTime)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, string>(SetCuttingDefaults), startTime, endTime);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"=== SetCuttingDefaults: {startTime} -> {endTime} ===");
            System.Diagnostics.Debug.WriteLine($"_isProcessing no momento: {_isProcessing}");

            txtInicioCorte.Text = startTime;
            txtFimCorte.Text = endTime;

            // CORRIGIDO: Forçar atualização do estado após definir valores
            System.Diagnostics.Debug.WriteLine("Forçando atualização de estado...");
            UpdateUIState();

            System.Diagnostics.Debug.WriteLine($"Valores de corte definidos - Início: {startTime}, Fim: {endTime}");
        }

        private void OnCuttingTabEntered()
        {
            // Definir valores padrão para os campos de corte
            if (string.IsNullOrEmpty(txtInicioCorte.Text))
            {
                txtInicioCorte.Text = "00:00:00";
            }

            if (string.IsNullOrEmpty(txtFimCorte.Text))
            {
                txtFimCorte.Text = "00:01:00"; // 1 minuto como padrão
            }

            UpdateUIState();
        }

        private void UpdateMoveButtonsState()
        {
            var selectedIndex = lstArquivosSelecionados.SelectedIndex;
            var itemCount = lstArquivosSelecionados.Items.Count;

            btnMoverCima.Enabled = selectedIndex > 0 && !_isProcessing;
            btnMoverBaixo.Enabled = selectedIndex >= 0 && selectedIndex < itemCount - 1 && !_isProcessing;
            btnExcluirItem.Enabled = selectedIndex >= 0 && !_isProcessing;
        }

        private void EnableControls(bool enabled)
        {
            // Habilitar/desabilitar controles principais
            btnSelecionarArquivo.Enabled = enabled;
            btnAutenticacao.Enabled = enabled && !btnAutenticacao.Text.Contains("✅");
            btnProximoPasso1.Enabled = enabled;
            btnJuntarVideos.Enabled = enabled;
            btnCortarVideo.Enabled = enabled;
            btnPularCorte.Enabled = enabled;
            btnSelecionarCapa.Enabled = enabled;
            btnPulaPassoFinal.Enabled = enabled;
            btnFazerUpload.Enabled = enabled;

            // Controles de lista
            btnMoverCima.Enabled = enabled;
            btnMoverBaixo.Enabled = enabled;
            btnExcluirItem.Enabled = enabled;
            lstArquivosSelecionados.Enabled = enabled;

            if (enabled)
            {
                UpdateUIState(); // Re-aplicar regras específicas
            }
        }

        private void MoveSelectedItem(int direction)
        {
            var selectedIndex = lstArquivosSelecionados.SelectedIndex;
            if (selectedIndex < 0) return;

            var newIndex = selectedIndex + direction;
            if (newIndex < 0 || newIndex >= lstArquivosSelecionados.Items.Count) return;

            var item = lstArquivosSelecionados.Items[selectedIndex];
            lstArquivosSelecionados.Items.RemoveAt(selectedIndex);
            lstArquivosSelecionados.Items.Insert(newIndex, item);
            lstArquivosSelecionados.SelectedIndex = newIndex;
        }

        private void ShowNavigationError(int tabIndex)
        {
            var message = tabIndex switch
            {
                1 => "Selecione pelo menos 2 vídeos antes de continuar para a concatenação.",
                2 => "É necessário concatenar os vídeos primeiro.",
                3 => "É necessário processar o vídeo antes de definir a thumbnail.",
                4 => "É necessário estar autenticado e ter um vídeo processado para fazer upload.",
                _ => "Não é possível navegar para esta aba no momento."
            };

            MessageBox.Show(message, "Navegação Bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private async void LoadUserAvatar(string avatarUrl)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                var imageBytes = await client.GetByteArrayAsync(avatarUrl);
                using var ms = new MemoryStream(imageBytes);

                if (InvokeRequired)
                {
                    Invoke(new Action(() => {
                        pictureBoxProfile.Image = Image.FromStream(ms);
                    }));
                }
                else
                {
                    pictureBoxProfile.Image = Image.FromStream(ms);
                }
            }
            catch
            {
                // Usar imagem padrão se falhar ou ignorar
            }
        }

        private void LoadDefaultThumbnail()
        {
            try
            {
                // CORRIGIDO - Usar recurso que realmente existe ou criar imagem padrão
                var defaultImage = new Bitmap(400, 300);
                using (var g = Graphics.FromImage(defaultImage))
                {
                    g.Clear(Color.LightGray);
                    g.DrawString("Thumbnail Padrão", SystemFonts.DefaultFont, Brushes.Black, 10, 10);
                }

                pictureBoxCapa.Image = defaultImage;
                lblCaminhoCapa.Text = "Thumbnail padrão";
            }
            catch
            {
                // Ignorar erro se não conseguir criar imagem padrão
            }
        }

        #endregion

        #region Fallback Methods (comportamento original quando presenter não está disponível)

        private async Task SelectFilesOriginalBehavior()
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos de Vídeo MP4 (*.mp4)|*.mp4|Todos os arquivos (*.*)|*.*",
                Multiselect = true,
                Title = "Selecione um arquivo"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    lstArquivosSelecionados.Items.Add(Path.GetFileName(fileName));
                }
                UpdateUIState();
            }
        }

        private async Task SelectThumbnailOriginalBehavior()
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos de Imagem|*.jpg;*.jpeg;*.png;*.bmp|Todos os arquivos (*.*)|*.*",
                Title = "Selecione uma imagem para thumbnail"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SetThumbnail(openFileDialog.FileName);
            }
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isProcessing)
            {
                var result = MessageBox.Show(
                    "Uma operação está em andamento. Tem certeza de que deseja sair?",
                    "Operação em Andamento",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        #region Métodos Públicos Adicionais para o Presenter
        public void ForceUpdateProcessingState()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ForceUpdateProcessingState));
                return;
            }

            System.Diagnostics.Debug.WriteLine("=== ForceUpdateProcessingState ===");
            System.Diagnostics.Debug.WriteLine($"Forçando _isProcessing = false");

            _isProcessing = false;
            UpdateUIState();
        }        

        public void EnableCuttingControls(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(EnableCuttingControls), enabled);
                return;
            }

            btnCortarVideo.Enabled = enabled && !_isProcessing;
            btnPularCorte.Enabled = enabled && !_isProcessing;
            txtInicioCorte.Enabled = enabled && !_isProcessing;
            txtFimCorte.Enabled = enabled && !_isProcessing;
        }

        #endregion
        #region Métodos para Aba de Thumbnail

        public void LoadDefaultThumbnailIfNeeded()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(LoadDefaultThumbnailIfNeeded));
                return;
            }

            System.Diagnostics.Debug.WriteLine("=== LoadDefaultThumbnailIfNeeded ===");

            try
            {
                // Se não há imagem na thumbnail ou está vazia, carregar padrão
                if (pictureBoxCapa.Image == null)
                {
                    System.Diagnostics.Debug.WriteLine("Carregando thumbnail padrão...");

                    // Criar imagem padrão
                    var defaultImage = new Bitmap(400, 300);
                    using (var g = Graphics.FromImage(defaultImage))
                    {
                        g.Clear(Color.FromArgb(240, 240, 240));

                        // Desenhar um retângulo com texto
                        using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                        using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                        {
                            var text = "Thumbnail Padrão";
                            var textSize = g.MeasureString(text, font);
                            var x = (defaultImage.Width - textSize.Width) / 2;
                            var y = (defaultImage.Height - textSize.Height) / 2;

                            g.DrawString(text, font, brush, x, y);
                        }

                        // Desenhar borda
                        using (var pen = new Pen(Color.FromArgb(200, 200, 200), 2))
                        {
                            g.DrawRectangle(pen, 1, 1, defaultImage.Width - 2, defaultImage.Height - 2);
                        }
                    }

                    pictureBoxCapa.Image = defaultImage;
                    lblCaminhoCapa.Text = "Thumbnail padrão carregada";

                    System.Diagnostics.Debug.WriteLine("Thumbnail padrão criada e definida");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Thumbnail já existe, não alterando");
                }

                // Forçar atualização dos controles da aba de thumbnail
                UpdateThumbnailTabControls();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO ao carregar thumbnail padrão: {ex.Message}");
            }
        }

        private void UpdateThumbnailTabControls()
        {
            System.Diagnostics.Debug.WriteLine("=== UpdateThumbnailTabControls ===");
            System.Diagnostics.Debug.WriteLine($"Processing: {_isProcessing}");
            System.Diagnostics.Debug.WriteLine($"Aba atual: {tabControlPrincipal.SelectedIndex}");

            if (tabControlPrincipal.SelectedIndex == 3) // Aba de thumbnail
            {
                btnSelecionarCapa.Enabled = !_isProcessing;
                btnPulaPassoFinal.Enabled = !_isProcessing;

                System.Diagnostics.Debug.WriteLine($"Selecionar Capa habilitado: {btnSelecionarCapa.Enabled}");
                System.Diagnostics.Debug.WriteLine($"Próximo Passo (thumbnail) habilitado: {btnPulaPassoFinal.Enabled}");
            }
        }

        #endregion

    }
}