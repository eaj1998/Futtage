//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Windows.Forms;
//using Futtage.Core.Models;
//using Futtage.Presentation.Common;

//namespace Futtage.Presentation.Views
//{
//    public partial class VideoSelectionUserControl : UserControl, IVideoSelectionView
//    {
//        private ModernButton _btnSelectFiles;
//        private ModernButton _btnAuthenticate;
//        private ModernButton _btnNextStep;
//        private VideoListControl _videoListControl;
//        private PictureBox _pictureBoxProfile;
//        private Label _lblUserName;
//        private Label _lblUserEmail;
//        private Label _lblAuthStatus;
//        private ModernProgressBar _progressBar;
//        private Panel _progressPanel;

//        public List<VideoInfo> SelectedVideos => _videoListControl.Videos;
//        public VideoInfo CurrentlySelectedVideo => _videoListControl.SelectedVideo;

//        private bool _isGoogleAuthenticated;
//        public bool IsGoogleAuthenticated
//        {
//            get => _isGoogleAuthenticated;
//            set
//            {
//                _isGoogleAuthenticated = value;
//                UpdateAuthenticationUI();
//            }
//        }

//        public string AuthenticatedUserName { get; set; }
//        public string AuthenticatedUserEmail { get; set; }
//        public string AuthenticatedUserAvatar { get; set; }

//        // Eventos
//        public event EventHandler<List<string>> FilesSelected;
//        public event EventHandler<VideoInfo> VideoSelectionChanged;
//        public event EventHandler<VideoMoveEventArgs> VideoMoveRequested;
//        public event EventHandler<VideoInfo> VideoRemovalRequested;
//        public event EventHandler AuthenticationRequested;
//        public event EventHandler NextStepRequested;

//        public VideoSelectionUserControl()
//        {
//            InitializeComponent();
//            SetupEventHandlers();
//        }

//        private void InitializeComponent()
//        {
//            SuspendLayout();

//            // Layout principal
//            var mainTableLayout = new TableLayoutPanel
//            {
//                Dock = DockStyle.Fill,
//                ColumnCount = 1,
//                RowCount = 4
//            };

//            // Configurar proporções das linhas
//            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Botões superiores
//            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Lista de vídeos
//            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Informações de autenticação
//            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Botões inferiores

//            // Painel de botões superiores
//            var topButtonPanel = CreateTopButtonPanel();
//            mainTableLayout.Controls.Add(topButtonPanel, 0, 0);

//            // Controle de lista de vídeos
//            _videoListControl = new VideoListControl
//            {
//                Dock = DockStyle.Fill
//            };
//            mainTableLayout.Controls.Add(_videoListControl, 0, 1);

//            // Painel de autenticação
//            var authPanel = CreateAuthenticationPanel();
//            mainTableLayout.Controls.Add(authPanel, 0, 2);

//            // Painel de botões inferiores
//            var bottomButtonPanel = CreateBottomButtonPanel();
//            mainTableLayout.Controls.Add(bottomButtonPanel, 0, 3);

//            // Painel de progresso (inicialmente oculto)
//            _progressPanel = CreateProgressPanel();

//            Controls.Add(mainTableLayout);
//            Controls.Add(_progressPanel);

//            ResumeLayout();
//        }

//        private Panel CreateTopButtonPanel()
//        {
//            var panel = new Panel
//            {
//                Dock = DockStyle.Fill,
//                Padding = new Padding(10)
//            };

//            _btnSelectFiles = new ModernButton
//            {
//                Text = "📁 Selecionar Arquivos de Vídeo...",
//                Dock = DockStyle.Fill,
//                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
//                BorderRadius = 8,
//                ProgressColor = Color.FromArgb(0, 122, 255)
//            };

//            panel.Controls.Add(_btnSelectFiles);
//            return panel;
//        }

//        private Panel CreateAuthenticationPanel()
//        {
//            var panel = new Panel
//            {
//                Dock = DockStyle.Fill,
//                Padding = new Padding(10),
//                BackColor = Color.FromArgb(248, 249, 250)
//            };

//            var tableLayout = new TableLayoutPanel
//            {
//                Dock = DockStyle.Fill,
//                ColumnCount = 3,
//                RowCount = 2
//            };

//            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60)); // Avatar
//            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Info
//            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Botão

//            // Avatar
//            _pictureBoxProfile = new PictureBox
//            {
//                Size = new Size(50, 50),
//                SizeMode = PictureBoxSizeMode.Zoom,
//                Anchor = AnchorStyles.Left | AnchorStyles.Top,
//                BorderStyle = BorderStyle.FixedSingle
//            };
//            tableLayout.Controls.Add(_pictureBoxProfile, 0, 0);
//            tableLayout.SetRowSpan(_pictureBoxProfile, 2);

//            // Informações do usuário
//            var infoPanel = new Panel { Dock = DockStyle.Fill };

//            _lblAuthStatus = new Label
//            {
//                Text = "Status da Conta:",
//                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
//                ForeColor = Color.Gray,
//                Dock = DockStyle.Top,
//                Height = 20
//            };

//            _lblUserName = new Label
//            {
//                Text = "Não autenticado",
//                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
//                Dock = DockStyle.Top,
//                Height = 25
//            };

//            _lblUserEmail = new Label
//            {
//                Text = "",
//                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
//                ForeColor = Color.Gray,
//                Dock = DockStyle.Fill
//            };

//            infoPanel.Controls.Add(_lblUserEmail);
//            infoPanel.Controls.Add(_lblUserName);
//            infoPanel.Controls.Add(_lblAuthStatus);

//            tableLayout.Controls.Add(infoPanel, 1, 0);
//            tableLayout.SetRowSpan(infoPanel, 2);

//            // Botão de autenticação
//            _btnAuthenticate = new ModernButton
//            {
//                Text = "🔐 Fazer Login",
//                Dock = DockStyle.Fill,
//                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
//                BorderRadius = 6
//            };
//            tableLayout.Controls.Add(_btnAuthenticate, 2, 0);
//            tableLayout.SetRowSpan(_btnAuthenticate, 2);

//            panel.Controls.Add(tableLayout);
//            return panel;
//        }

//        private Panel CreateBottomButtonPanel()
//        {
//            var panel = new Panel
//            {
//                Dock = DockStyle.Fill,
//                Padding = new Padding(10, 5, 10, 10)
//            };

//            _btnNextStep = new ModernButton
//            {
//                Text = "➡️ Próximo Passo",
//                Dock = DockStyle.Right,
//                Width = 150,
//                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
//                BorderRadius = 8,
//                ProgressColor = Color.FromArgb(40, 167, 69),
//                Enabled = false
//            };

//            panel.Controls.Add(_btnNextStep);
//            return panel;
//        }

//        private Panel CreateProgressPanel()
//        {
//            var panel = new Panel
//            {
//                Dock = DockStyle.Fill,
//                BackColor = Color.FromArgb(200, 255, 255, 255),
//                Visible = false
//            };

//            _progressBar = new ModernProgressBar
//            {
//                Anchor = AnchorStyles.None,
//                Size = new Size(300, 25),
//                ProgressColor = Color.FromArgb(0, 122, 255)
//            };

//            // Centralizar a barra de progresso
//            _progressBar.Location = new Point(
//                (panel.Width - _progressBar.Width) / 2,
//                (panel.Height - _progressBar.Height) / 2
//            );

//            panel.Controls.Add(_progressBar);
//            panel.Resize += (s, e) =>
//            {
//                _progressBar.Location = new Point(
//                    (panel.Width - _progressBar.Width) / 2,
//                    (panel.Height - _progressBar.Height) / 2
//                );
//            };

//            return panel;
//        }

//        private void SetupEventHandlers()
//        {
//            _btnSelectFiles.Click += (s, e) => SelectFiles();
//            _btnAuthenticate.Click += (s, e) => AuthenticationRequested?.Invoke(this, EventArgs.Empty);
//            _btnNextStep.Click += (s, e) => NextStepRequested?.Invoke(this, EventArgs.Empty);

//            _videoListControl.VideoSelectionChanged += (s, e) =>
//            {
//                VideoSelectionChanged?.Invoke(this, e.Video);
//                UpdateNextButtonState();
//            };

//            _videoListControl.VideoOrderChanged += (s, e) =>
//            {
//                UpdateNextButtonState();
//            };

//            _videoListControl.VideoRemoved += (s, e) =>
//            {
//                VideoRemovalRequested?.Invoke(this, e.Video);
//                UpdateNextButtonState();
//            };

//            _videoListControl.OnVideosDropped += (s, e) =>
//            {
//                FilesSelected?.Invoke(this, e.FilePaths);
//            };
//        }

//        private void SelectFiles()
//        {
//            using (var openFileDialog = new OpenFileDialog
//            {
//                Filter = "Arquivos de Vídeo MP4 (*.mp4)|*.mp4|Todos os arquivos (*.*)|*.*",
//                Multiselect = true,
//                Title = "Selecione os arquivos de vídeo"
//            })
//            {
//                if (openFileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    FilesSelected?.Invoke(this, openFileDialog.FileNames.ToList());
//                }
//            }
//        }

//        public void AddVideos(List<VideoInfo> videos)
//        {
//            _videoListControl.AddVideos(videos);
//            UpdateNextButtonState();
//        }

//        public void RemoveVideo(VideoInfo video)
//        {
//            _videoListControl.RemoveVideo(video);
//            UpdateNextButtonState();
//        }

//        public void UpdateVideoOrder(List<VideoInfo> newOrder)
//        {
//            _videoListControl.ClearVideos();
//            _videoListControl.AddVideos(newOrder);
//            UpdateNextButtonState();
//        }

//        public void ShowProgress(string message)
//        {
//            if (InvokeRequired)
//            {
//                Invoke(new Action<string>(ShowProgress), message);
//                return;
//            }

//            _progressBar.ProgressText = message;
//            _progressBar.Value = 0;
//            _progressPanel.Visible = true;
//            _progressPanel.BringToFront();
//            EnableControls(false);
//        }

//        public void HideProgress()
//        {
//            if (InvokeRequired)
//            {
//                Invoke(new Action(HideProgress));
//                return;
//            }

//            _progressPanel.Visible = false;
//            EnableControls(true);
//        }

//        public void ShowError(string message)
//        {
//            MessageBox.Show(message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        }

//        public void ShowSuccess(string message)
//        {
//            MessageBox.Show(message, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }

//        public void EnableControls(bool enabled)
//        {
//            _btnSelectFiles.Enabled = enabled;
//            _btnAuthenticate.Enabled = enabled;
//            _btnNextStep.Enabled = enabled && SelectedVideos.Count >= 2 && IsGoogleAuthenticated;
//            _videoListControl.Enabled = enabled;
//        }

//        public void UpdateAuthenticationStatus(bool isAuthenticated, string userName = null, string userEmail = null, string avatarUrl = null)
//        {
//            IsGoogleAuthenticated = isAuthenticated;
//            AuthenticatedUserName = userName;
//            AuthenticatedUserEmail = userEmail;
//            AuthenticatedUserAvatar = avatarUrl;

//            UpdateAuthenticationUI();
//            UpdateNextButtonState();
//        }

//        private void UpdateAuthenticationUI()
//        {
//            if (_isGoogleAuthenticated)
//            {
//                _lblUserName.Text = AuthenticatedUserName ?? "Usuário Autenticado";
//                _lblUserEmail.Text = AuthenticatedUserEmail ?? "";
//                _btnAuthenticate.Text = "✅ Conectado";
//                _btnAuthenticate.Enabled = false;
//                _btnAuthenticate.BackColor = Color.FromArgb(40, 167, 69);

//                // Carregar avatar se disponível
//                if (!string.IsNullOrEmpty(AuthenticatedUserAvatar))
//                {
//                    LoadUserAvatar(AuthenticatedUserAvatar);
//                }
//            }
//            else
//            {
//                _lblUserName.Text = "Não autenticado";
//                _lblUserEmail.Text = "Clique em 'Fazer Login' para conectar sua conta Google";
//                _btnAuthenticate.Text = "🔐 Fazer Login";
//                _btnAuthenticate.Enabled = true;
//                _btnAuthenticate.BackColor = Color.FromArgb(0, 122, 255);
//                _pictureBoxProfile.Image = null;
//            }
//        }

//        private async void LoadUserAvatar(string avatarUrl)
//        {
//            try
//            {
//                using (var client = new System.Net.Http.HttpClient())
//                {
//                    var imageBytes = await client.GetByteArrayAsync(avatarUrl);
//                    using (var ms = new System.IO.MemoryStream(imageBytes))
//                    {
//                        _pictureBoxProfile.Image = Image.FromStream(ms);
//                    }
//                }
//            }
//            catch
//            {
//                // Usar imagem padrão se falhar
//                _pictureBoxProfile.Image = Properties.Resources.DefaultAvatar;
//            }
//        }

//        private void UpdateNextButtonState()
//        {
//            _btnNextStep.Enabled = SelectedVideos.Count >= 2 && IsGoogleAuthenticated;
//        }
//    }
//}