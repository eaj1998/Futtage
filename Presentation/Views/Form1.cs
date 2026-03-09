// Presentation/Views/Form1.cs - ARQUIVO COMPLETO CORRIGIDO
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
using Futtage.Presentation.Common;

namespace Futtage
{
    public partial class Form1 : Form
    {
        private MainPresenter? _presenter;

        private bool _isProcessing = false;

        private ModernProgressBar? _modernProgressBar;
        private Label? _progressLabel;
        private Panel? _progressPanel;
        private Button? _btnCancel;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;

            if (btnLogout != null)
            {
                btnLogout.Enabled = false;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.TopMost = false;
            this.Activate();
        }

        public void SetPresenter(MainPresenter presenter)
        {
            _presenter = presenter;
        }

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
                if (_presenter != null)
                {
                    _presenter.RemoveVideo(lstArquivosSelecionados.SelectedIndex);
                }
                else
                {
                    lstArquivosSelecionados.Items.RemoveAt(lstArquivosSelecionados.SelectedIndex);
                    UpdateUIState();
                }
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

            // Ações específicas ao entrar em cada aba
            switch (e.TabPageIndex)
            {
                case 2: // Aba de corte
                    BeginInvoke(new Action(OnCuttingTabEntered));
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
                btnLogout.Enabled = true;

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
                btnLogout.Enabled = false;
            }

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

            System.Diagnostics.Debug.WriteLine("Forçando atualização de estado...");
            UpdateUIState();

            System.Diagnostics.Debug.WriteLine($"Valores de corte definidos - Início: {startTime}, Fim: {endTime}");
        }

        #endregion

        #region Métodos de Interface de Progresso

        private void InitializeProgressControls()
        {
            // Painel de progresso
            _progressPanel = new Panel
            {
                Size = new Size(680, 60),
                Location = new Point(14, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            // Label de status
            _progressLabel = new Label
            {
                Text = "Preparando...",
                Location = new Point(10, 8),
                Size = new Size(660, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            // Barra de progresso moderna
            _modernProgressBar = new ModernProgressBar
            {
                Location = new Point(10, 30),
                Size = new Size(550, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ProgressColor = Color.FromArgb(0, 122, 255),
                ShowPercentage = true
            };

            // Botão Cancelar
            _btnCancel = new Button
            {
                Text = "Cancelar",
                Location = new Point(570, 28),
                Size = new Size(100, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnCancel.FlatAppearance.BorderSize = 0;
            _btnCancel.Click += BtnCancel_Click;

            _progressPanel.Controls.Add(_progressLabel);
            _progressPanel.Controls.Add(_modernProgressBar);
            _progressPanel.Controls.Add(_btnCancel);

            // Adicionar ao formulário principal
            this.Controls.Add(_progressPanel);
            _progressPanel.BringToFront();
        }

        public void ShowProgress(string message, int percentage = 0)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, int>(ShowProgress), message, percentage);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"=== ShowProgress: {message} ({percentage}%) ===");

            // Inicializar controles se necessário
            if (_progressPanel == null)
            {
                InitializeProgressControls();
            }

            _isProcessing = true;

            // Atualizar controles de progresso
            if (_progressLabel != null)
                _progressLabel.Text = message;

            if (_modernProgressBar != null)
            {
                _modernProgressBar.Value = percentage;
                _modernProgressBar.ProgressText = percentage > 0 ? $"{percentage}%" : "";
            }

            // Mostrar painel e atualizar UI
            if (_progressPanel != null)
            {
                _progressPanel.Visible = true;
                _progressPanel.BringToFront();
            }

            UpdateFormTitle(message);
            this.Cursor = Cursors.WaitCursor;

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

            _isProcessing = false;

            // Ocultar painel de progresso
            if (_progressPanel != null)
            {
                _progressPanel.Visible = false;
            }

            UpdateFormTitle("");
            this.Cursor = Cursors.Default;

            // Reabilitar controles
            EnableControls(true);

            System.Diagnostics.Debug.WriteLine("Controles reabilitados após processamento");
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            _presenter?.CancelCurrentOperation();
        }

        #endregion

        #region Métodos Auxiliares

        private void UpdateFormTitle(string operation = "")
        {
            var baseTitle = "Futtage - Editor de Vídeo e Upload para YouTube";
            this.Text = string.IsNullOrEmpty(operation) ? baseTitle : $"{baseTitle} - {operation}";
        }

        private bool ValidateTimeFormat(string timeText)
        {
            return TimeSpan.TryParse(timeText, out _);
        }

        private void UpdateUIState()
        {
            var hasVideos = lstArquivosSelecionados.Items.Count > 0;
            var hasEnoughVideos = lstArquivosSelecionados.Items.Count >= 2;
            var isAuthenticated = btnAutenticacao.Text.Contains("✅");
      
            var currentTab = tabControlPrincipal.SelectedIndex;
            var tabNames = new[] { "Seleção", "Concatenação", "Corte", "Thumbnail", "Upload" };
            if (currentTab >= 0 && currentTab < tabNames.Length)
            {
                if (!_isProcessing)
                {
                    UpdateFormTitle($"Passo {currentTab + 1}: {tabNames[currentTab]}");
                }
            }
            
            switch (currentTab)
            {
                case 0: // Seleção
                    btnProximoPasso1.Enabled = hasEnoughVideos && !_isProcessing;
                    btnSelecionarArquivo.Enabled = !_isProcessing;
                    btnAutenticacao.Enabled = !_isProcessing && !isAuthenticated;
                    break;

                case 1: // Concatenação
                    btnJuntarVideos.Enabled = hasEnoughVideos && !_isProcessing;
                    btnVoltarPasso1.Enabled = !_isProcessing;
                    break;

                case 2: // Corte
                    bool cuttingFieldsValid = AreCuttingFieldsValid();
                    btnCortarVideo.Enabled = !_isProcessing && cuttingFieldsValid;
                    btnPularCorte.Enabled = !_isProcessing;
                    txtInicioCorte.Enabled = !_isProcessing;
                    txtFimCorte.Enabled = !_isProcessing;

                    txtInicioCorte.BackColor = ValidateTimeFormat(txtInicioCorte.Text) ?
                        SystemColors.Window : Color.FromArgb(255, 240, 240);
                    txtFimCorte.BackColor = ValidateTimeFormat(txtFimCorte.Text) ?
                        SystemColors.Window : Color.FromArgb(255, 240, 240);
                    break;

                case 3: // Thumbnail
                    btnSelecionarCapa.Enabled = !_isProcessing;
                    btnPulaPassoFinal.Enabled = !_isProcessing;
                    break;

                case 4: // Upload
                    btnFazerUpload.Enabled = !_isProcessing && isAuthenticated;
                    break;
            }

            UpdateMoveButtonsState();
        }

        private async void btnAuthToggle_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter != null)
                {
                    if (_presenter.IsAuthenticated)
                    {
                        await _presenter.OnLogoutClickedAsync();
                    }
                    else
                    {
                        await _presenter.OnAuthenticationClickedAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na autenticação: {ex.Message}", "Erro - Futtage",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void OnCuttingTabEntered()
        {
            if (string.IsNullOrEmpty(txtInicioCorte.Text))
            {
                txtInicioCorte.Text = "00:00:00";
            }

            if (string.IsNullOrEmpty(txtFimCorte.Text))
            {
                txtFimCorte.Text = "00:01:00";
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
                UpdateUIState();
            }
        }

        private void MoveSelectedItem(int direction)
        {
            var selectedIndex = lstArquivosSelecionados.SelectedIndex;
            if (selectedIndex < 0) return;

            var newIndex = selectedIndex + direction;
            if (newIndex < 0 || newIndex >= lstArquivosSelecionados.Items.Count) return;

            if (_presenter != null)
            {
                _presenter.MoveVideo(selectedIndex, newIndex);
                if (newIndex >= 0 && newIndex < lstArquivosSelecionados.Items.Count)
                {
                    lstArquivosSelecionados.SelectedIndex = newIndex;
                }
            }
            else
            {
                var item = lstArquivosSelecionados.Items[selectedIndex];
                lstArquivosSelecionados.Items.RemoveAt(selectedIndex);
                lstArquivosSelecionados.Items.Insert(newIndex, item);
                lstArquivosSelecionados.SelectedIndex = newIndex;
            }
        }

        private void ShowNavigationError(int tabIndex)
        {
            var message = tabIndex switch
            {
                1 => "Selecione pelo menos 2 vídeos antes de continuar para a concatenação.",
                2 => "É necessário concatenar os vídeos primeiro.",
                3 => "É necessário processar o vídeo antes de definir a thumbnail.",
                4 => "É necessário ter um vídeo processado para fazer upload.",
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
                if (pictureBoxCapa.Image == null)
                {
                    System.Diagnostics.Debug.WriteLine("Carregando thumbnail padrão...");

                    var defaultImage = new Bitmap(400, 300);
                    using (var g = Graphics.FromImage(defaultImage))
                    {
                        g.Clear(Color.FromArgb(240, 240, 240));

                        using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                        using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                        {
                            var text = "Thumbnail Padrão";
                            var textSize = g.MeasureString(text, font);
                            var x = (defaultImage.Width - textSize.Width) / 2;
                            var y = (defaultImage.Height - textSize.Height) / 2;

                            g.DrawString(text, font, brush, x, y);
                        }

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
    }
}