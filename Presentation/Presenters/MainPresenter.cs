using System.Windows.Forms;
using Futtage.Core.Models;
using Futtage.Core.Services;
using Futtage.Infrastructure.Logging;

namespace Futtage.Presentation.Presenters
{
    public class MainPresenter : IDisposable
    {
        private readonly IVideoProcessingService _videoService;
        private readonly IYouTubeService _youTubeService;
        private readonly IFileService _fileService;
        private readonly ILogger _logger;

        private List<VideoInfo> _selectedVideos = new List<VideoInfo>();
        private string _concatenatedVideoPath = string.Empty;
        private string _finalVideoPath = string.Empty;
        private string _selectedThumbnailPath = string.Empty;
        private bool _isAuthenticated = false;
        private UserInfo? _currentUser;

        private Form1? _mainForm;

        public MainPresenter(
            IVideoProcessingService videoService,
            IYouTubeService youTubeService,
            IFileService fileService,
            ILogger logger)
        {
            _videoService = videoService ?? throw new ArgumentNullException(nameof(videoService));
            _youTubeService = youTubeService ?? throw new ArgumentNullException(nameof(youTubeService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize(Form mainForm)
        {
            _mainForm = mainForm as Form1 ?? throw new ArgumentException("Form deve ser do tipo Form1", nameof(mainForm));
            _logger.LogInfo("MainPresenter inicializado");

            // Verificar autenticação existente
            Task.Run(async () => await CheckExistingAuthenticationAsync());
        }

        private async Task CheckExistingAuthenticationAsync()
        {
            try
            {
                _isAuthenticated = await _youTubeService.IsAuthenticatedAsync();
                if (_isAuthenticated)
                {
                    _currentUser = await _youTubeService.GetUserInfoAsync();
                    UpdateAuthenticationUI();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao verificar autenticação existente: {ex.Message}", ex);
            }
        }

        public async Task OnSelectFilesClickedAsync()
        {
            try
            {
                var selectedFiles = await _fileService.SelectVideoFilesAsync();
                if (selectedFiles.Any())
                {
                    await ProcessSelectedFilesAsync(selectedFiles);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Erro ao selecionar arquivos: {ex.Message}");
                _logger.LogError($"Erro na seleção de arquivos: {ex.Message}", ex);
            }
        }

        private async Task ProcessSelectedFilesAsync(List<string> filePaths)
        {
            ShowProgress("Analisando arquivos de vídeo...");

            try
            {
                var validFiles = filePaths.Where(f =>
                    _fileService.FileExists(f) &&
                    _videoService.ValidateVideoFile(f)
                ).ToList();

                if (!validFiles.Any())
                {
                    HideProgress();
                    ShowError("Nenhum arquivo de vídeo válido foi encontrado.");
                    return;
                }

                var videoInfos = await _videoService.GetMultipleVideoInfoAsync(validFiles);

                var validVideos = videoInfos
                    .Where(v => v.IsValid)
                    .OrderBy(v => v.CreationDate)
                    .ToList();

                HideProgress();

                if (validVideos.Any())
                {
                    _selectedVideos.AddRange(validVideos);
                    UpdateVideoListUI();
                    ShowSuccess($"{validVideos.Count} arquivo(s) adicionado(s) com sucesso!");
                }
                else
                {
                    ShowError("Não foi possível processar nenhum dos arquivos selecionados.");
                }
            }
            catch (Exception ex)
            {
                HideProgress();
                ShowError($"Erro ao processar arquivos: {ex.Message}");
                _logger.LogError($"Erro ao processar arquivos: {ex.Message}", ex);
            }
            finally
            {
                HideProgress();
            }
        }

        public async Task OnAuthenticationClickedAsync()
        {
            try
            {
                ShowProgress("Conectando com o Google...");

                var success = await _youTubeService.AuthenticateAsync();

                if (success)
                {
                    _currentUser = await _youTubeService.GetUserInfoAsync();
                    _isAuthenticated = true;

                    UpdateAuthenticationUI();
                    ShowSuccess("Autenticação realizada com sucesso!");
                    _logger.LogInfo($"Usuário autenticado: {_currentUser.Email}");
                }
                else
                {
                    ShowError("Falha na autenticação. Tente novamente.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Erro durante autenticação: {ex.Message}");
                _logger.LogError($"Erro na autenticação: {ex.Message}", ex);
            }
            finally
            {
                HideProgress();
            }
        }

        public async Task OnLogoutClickedAsync()
        {
            try
            {
                if (!_isAuthenticated)
                {
                    ShowError("Você não está conectado.");
                    return;
                }

                var result = MessageBox.Show(
                    $"Deseja desconectar da conta {_currentUser?.Email}?",
                    "Confirmar Logout - Futtage",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                ShowProgress("Desconectando...");

                await _youTubeService.LogoutAsync();

                _isAuthenticated = false;
                _currentUser = null;

                UpdateAuthenticationUI();

                var hasProcessedVideos = !string.IsNullOrEmpty(_concatenatedVideoPath) ||
                                       !string.IsNullOrEmpty(_finalVideoPath);

                if (hasProcessedVideos)
                {
                    var resetResult = MessageBox.Show(
                        "Você fez logout durante um processo de edição. Deseja começar um novo projeto?",
                        "Novo Projeto - Futtage",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (resetResult == DialogResult.Yes)
                    {
                        ResetProject();
                    }
                    else
                    {
                        ShowWarning("Para fazer upload do vídeo, você precisará autenticar novamente.");
                    }
                }

                ShowSuccess("Logout realizado com sucesso!");
                _logger.LogInfo("Usuário desconectado");
            }
            catch (Exception ex)
            {
                ShowError($"Erro durante logout: {ex.Message}");
                _logger.LogError($"Erro no logout: {ex.Message}", ex);
            }
            finally
            {
                HideProgress();
            }
        }

        private void ResetProject()
        {
            try
            {
                _fileService?.DeleteTempFiles();

                _selectedVideos.Clear();
                _concatenatedVideoPath = string.Empty;
                _finalVideoPath = string.Empty;
                _selectedThumbnailPath = string.Empty;

                NavigateToTab(0);

                UpdateVideoListUI();
                UpdateProjectStateUI();

                _logger.LogInfo("Projeto resetado");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao resetar projeto: {ex.Message}", ex);
                ShowError("Erro ao resetar projeto. Reinicie o aplicativo se necessário.");
            }
        }

        public bool IsAuthenticated => _isAuthenticated;

        public UserInfo? CurrentUser => _currentUser;

        public bool CanAdvanceToNextStep()
        {
            return _selectedVideos.Count >= 2 && _isAuthenticated;
        }

        public void OnNextStepClicked()
        {
            if (_selectedVideos.Count < 2)
            {
                ShowError("É necessário selecionar pelo menos 2 vídeos para continuar.");
                return;
            }

            if (!_isAuthenticated)
            {
                ShowError("É necessário fazer login com sua conta Google para continuar.");
                return;
            }

            NavigateToTab(1);
        }

        public async Task OnConcatenateVideosClickedAsync()
        {
            if (_selectedVideos.Count < 2)
            {
                ShowError("É necessário pelo menos 2 vídeos para concatenação.");
                return;
            }

            try
            {
                ShowProgress("Preparando concatenação...");

                var outputPath = await _fileService.SelectOutputPathAsync("video_concatenado.mp4");
                if (string.IsNullOrEmpty(outputPath))
                {
                    HideProgress();
                    return;
                }

                var videoPaths = _selectedVideos.Select(v => v.FilePath).ToList();
                var progress = new Progress<ProcessingProgress>(p =>
                {
                    ShowProgress(p.Message, p.Percentage);
                });

                _concatenatedVideoPath = await _videoService.ConcatenateAsync(videoPaths, outputPath, progress);
                _finalVideoPath = _concatenatedVideoPath;

                HideProgress();
                ShowSuccess("Vídeos concatenados com sucesso!");

                NotifyVideoConcatenated();
                await Task.Delay(500);
                NavigateToTab(2);
            }
            catch (Exception ex)
            {
                HideProgress();
                ShowError($"Erro ao concatenar vídeos: {ex.Message}");
                _logger.LogError($"Erro na concatenação: {ex.Message}", ex);
            }
            finally
            {
                HideProgress();
            }
        }
        public void OnThumbnailTabEntered()
        {
            if (string.IsNullOrEmpty(_selectedThumbnailPath))
            {
                try
                {
                    if (_mainForm != null)
                    {
                        if (_mainForm.InvokeRequired)
                        {
                            _mainForm.Invoke(new Action(() =>
                            {
                                _mainForm.LoadDefaultThumbnailIfNeeded();
                            }));
                        }
                        else
                        {
                            _mainForm.LoadDefaultThumbnailIfNeeded();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Erro ao carregar thumbnail padrão: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            _logger.LogInfo("MainPresenter finalizado");
            _fileService?.DeleteTempFiles();
        }      

        public async Task OnCutVideoClickedAsync(string startTime, string endTime)
        {
            if (string.IsNullOrEmpty(_concatenatedVideoPath))
            {
                ShowError("Nenhum vídeo concatenado disponível para corte.");
                return;
            }

            try
            {
                if (!TimeSpan.TryParse(startTime, out var start) || !TimeSpan.TryParse(endTime, out var end))
                {
                    ShowError("Formato de tempo inválido. Use HH:MM:SS");
                    return;
                }

                if (start >= end)
                {
                    ShowError("Tempo de início deve ser menor que tempo de fim.");
                    return;
                }

                ShowProgress("Preparando corte...");

                var outputPath = GenerateCutOutputPath(_concatenatedVideoPath);
                var progress = new Progress<ProcessingProgress>(p =>
                {
                    ShowProgress(p.Message, p.Percentage);
                });

                _finalVideoPath = await _videoService.TrimAsync(_concatenatedVideoPath, start, end, outputPath, progress);

                HideProgress();
                ShowSuccess($"Vídeo cortado com sucesso!\nSalvo em: {_finalVideoPath}");

                NotifyVideoCut();

                await Task.Delay(500);
                NavigateToTab(3);
            }
            catch (Exception ex)
            {
                HideProgress();
                ShowError($"Erro ao cortar vídeo: {ex.Message}");
                _logger.LogError($"Erro no corte: {ex.Message}", ex);
            }
            finally
            {
                HideProgress();
            }
        }

        private string GenerateCutOutputPath(string originalPath)
        {
            try
            {
                var directory = Path.GetDirectoryName(originalPath) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                var fileName = Path.GetFileNameWithoutExtension(originalPath);
                var extension = Path.GetExtension(originalPath);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var cutFileName = $"{fileName}_cortado_{timestamp}{extension}";

                return Path.Combine(directory, cutFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao gerar caminho de saída: {ex.Message}", ex);

                var fallbackPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads",
                    $"video_cortado_{DateTime.Now:yyyyMMdd_HHmmss}.mp4"
                );

                return fallbackPath;
            }
        }

        public void OnSkipCutClicked()
        {
            _finalVideoPath = _concatenatedVideoPath;
            NotifyVideoCut();
            NavigateToTab(3);
        }

        private void NotifyVideoCut()
        {
            try
            {
                if (_mainForm != null)
                {
                    if (_mainForm.InvokeRequired)
                    {
                        _mainForm.Invoke(new Action(() =>
                        {
                            _mainForm.ForceUpdateProcessingState();
                            _mainForm.LoadDefaultThumbnailIfNeeded();
                        }));
                    }
                    else
                    {
                        _mainForm.ForceUpdateProcessingState();
                        _mainForm.LoadDefaultThumbnailIfNeeded();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao notificar vídeo cortado: {ex.Message}", ex);
            }
        }

        public async Task OnSelectThumbnailClickedAsync()
        {
            try
            {
                var thumbnailPath = await _fileService.SelectImageFileAsync();
                if (!string.IsNullOrEmpty(thumbnailPath))
                {
                    _selectedThumbnailPath = thumbnailPath;
                    UpdateThumbnailUI(thumbnailPath);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Erro ao selecionar thumbnail: {ex.Message}");
                _logger.LogError($"Erro na seleção de thumbnail: {ex.Message}", ex);
            }
        }

        public void OnSkipThumbnailClicked()
        {
            NavigateToTab(4);
        }

        public async Task OnUploadToYouTubeClickedAsync()
        {
            if (string.IsNullOrEmpty(_finalVideoPath))
            {
                ShowError("Nenhum vídeo disponível para upload.");
                return;
            }

            if (!_isAuthenticated)
            {
                ShowError("É necessário estar autenticado para fazer upload.");
                return;
            }

            if (!_fileService.FileExists(_finalVideoPath))
            {
                ShowError($"Arquivo de vídeo não encontrado: {_finalVideoPath}");
                return;
            }

            try
            {
                var uploadRequest = await GetUploadDetailsFromUserAsync();
                if (uploadRequest == null) return;

                ShowProgress("Preparando upload para o YouTube...");

                var progress = new Progress<ProcessingProgress>(p =>
                {
                    ShowProgress(p.Message, p.Percentage);
                });

                var videoId = await _youTubeService.UploadVideoAsync(uploadRequest, progress);

                if (!string.IsNullOrEmpty(_selectedThumbnailPath) && _fileService.FileExists(_selectedThumbnailPath))
                {
                    ShowProgress("Aplicando thumbnail...", 95);

                    try
                    {
                        await _youTubeService.SetThumbnailAsync(videoId, _selectedThumbnailPath);
                    }
                    catch (Exception thumbnailEx)
                    {
                        _logger.LogWarning($"Erro ao aplicar thumbnail: {thumbnailEx.Message}");
                    }
                }

                HideProgress();
                ShowUploadSuccess(videoId, uploadRequest);
                _logger.LogInfo($"Upload completo - Video ID: {videoId}");
            }
            catch (Exception ex)
            {
                HideProgress();
                ShowError($"Erro durante upload: {ex.Message}");
                _logger.LogError($"Erro no upload: {ex.Message}", ex);
            }
            finally
            {
                HideProgress();
            }
        }

        private async Task<YouTubeUploadRequest?> GetUploadDetailsFromUserAsync()
        {
            try
            {
                YouTubeUploadRequest? result = null;

                await Task.Run(() =>
                {
                    if (_mainForm != null)
                    {
                        if (_mainForm.InvokeRequired)
                        {
                            _mainForm.Invoke(new Action(() =>
                            {
                                var creationDate = _selectedVideos.FirstOrDefault()?.CreationDate ?? DateTime.Now;

                                using var detalhesForm = new FormDetalhesVideo(creationDate);

                                if (detalhesForm.ShowDialog() == DialogResult.OK)
                                {
                                    result = new YouTubeUploadRequest
                                    {
                                        VideoFilePath = _finalVideoPath,
                                        Title = detalhesForm.TituloDoVideo,
                                        Description = detalhesForm.DescricaoDoVideo,
                                        ThumbnailPath = _selectedThumbnailPath,
                                        IsForKids = detalhesForm.IsConteudoInfantil,
                                        Privacy = detalhesForm.PrivacyStatus,
                                        Tags = new[] { "Futtage", "VideoEdit", "YouTube" },
                                        Category = "22" // People & Blogs
                                    };
                                }
                            }));
                        }
                        else
                        {
                            var creationDate = _selectedVideos.FirstOrDefault()?.CreationDate ?? DateTime.Now;

                            using var detalhesForm = new FormDetalhesVideo(creationDate);

                            if (detalhesForm.ShowDialog() == DialogResult.OK)
                            {
                                result = new YouTubeUploadRequest
                                {
                                    VideoFilePath = _finalVideoPath,
                                    Title = detalhesForm.TituloDoVideo,
                                    Description = detalhesForm.DescricaoDoVideo,
                                    ThumbnailPath = _selectedThumbnailPath,
                                    IsForKids = detalhesForm.IsConteudoInfantil,
                                    Privacy = detalhesForm.PrivacyStatus,
                                    Tags = new[] { "Futtage", "VideoEdit", "YouTube" },
                                    Category = "22" // People & Blogs
                                };
                            }
                        }
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter detalhes do upload: {ex.Message}", ex);
                return null;
            }
        }

        #region Métodos de Navegação e UI

        private void NavigateToTab(int tabIndex)
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        _mainForm.NavigateToTab(tabIndex);
                    }));
                }
                else
                {
                    _mainForm.NavigateToTab(tabIndex);
                }
            }
        }

        public bool CanNavigateToTab(int tabIndex)
        {
            return tabIndex switch
            {
                0 => true,
                1 => _selectedVideos.Count >= 2,
                2 => !string.IsNullOrEmpty(_concatenatedVideoPath),
                3 => !string.IsNullOrEmpty(_finalVideoPath),
                4 => _isAuthenticated && !string.IsNullOrEmpty(_finalVideoPath),
                _ => false
            };
        }

        private string GetPrivacyDisplayName(string privacy)
        {
            return privacy switch
            {
                "private" => "Privado",
                "unlisted" => "Não listado",
                "public" => "Público",
                _ => privacy
            };
        }

        private void ShowUploadSuccess(string videoId, YouTubeUploadRequest request)
        {
            try
            {
                if (_mainForm != null)
                {
                    if (_mainForm.InvokeRequired)
                    {
                        _mainForm.Invoke(new Action(() =>
                        {
                            var message = $"✅ Upload realizado com sucesso!\n\n" +
                                         $"📹 Título: {request.Title}\n" +
                                         $"🆔 Video ID: {videoId}\n" +
                                         $"🔒 Privacidade: {GetPrivacyDisplayName(request.Privacy)}\n" +
                                         $"👶 Conteúdo infantil: {(request.IsForKids ? "Sim" : "Não")}\n\n" +
                                         $"🔗 Link: https://youtu.be/{videoId}\n\n" +
                                         $"Deseja abrir o vídeo no navegador?";

                            var result = MessageBox.Show(message, "Upload Concluído - Futtage",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                    {
                                        FileName = $"https://youtu.be/{videoId}",
                                        UseShellExecute = true
                                    });
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"Erro ao abrir navegador: {ex.Message}", ex);

                                    try
                                    {
                                        Clipboard.SetText($"https://youtu.be/{videoId}");
                                        MessageBox.Show("Link copiado para a área de transferência!", "Link Copiado",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    catch
                                    {
                                        // Falha silenciosa
                                    }
                                }
                            }
                        }));
                    }
                    else
                    {
                        var message = $"✅ Upload realizado com sucesso!\n\n" +
                                     $"📹 Título: {request.Title}\n" +
                                     $"🆔 Video ID: {videoId}\n" +
                                     $"🔒 Privacidade: {GetPrivacyDisplayName(request.Privacy)}\n" +
                                     $"👶 Conteúdo infantil: {(request.IsForKids ? "Sim" : "Não")}\n\n" +
                                     $"🔗 Link: https://youtu.be/{videoId}\n\n" +
                                     $"Deseja abrir o vídeo no navegador?";

                        var result = MessageBox.Show(message, "Upload Concluído - Futtage",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = $"https://youtu.be/{videoId}",
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Erro ao abrir navegador: {ex.Message}", ex);

                                try
                                {
                                    Clipboard.SetText($"https://youtu.be/{videoId}");
                                    MessageBox.Show("Link copiado para a área de transferência!", "Link Copiado",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                catch
                                {
                                    // Falha silenciosa
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao mostrar sucesso do upload: {ex.Message}", ex);
            }
        }

        #endregion

        #region Métodos Privados Corrigidos

        private void ShowProgress(string message, int percentage = 0)
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() => _mainForm.ShowProgress(message, percentage)));
                }
                else
                {
                    _mainForm.ShowProgress(message, percentage);
                }
            }
        }

        private void HideProgress()
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() => _mainForm.HideProgress()));
                }
                else
                {
                    _mainForm.HideProgress();
                }
            }
        }

        private void ShowError(string message)
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        MessageBox.Show(message, "Erro - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    MessageBox.Show(message, "Erro - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowSuccess(string message)
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        MessageBox.Show(message, "Sucesso - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
                else
                {
                    MessageBox.Show(message, "Sucesso - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void ShowWarning(string message)
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        MessageBox.Show(message, "Aviso - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }));
                }
                else
                {
                    MessageBox.Show(message, "Aviso - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void UpdateVideoListUI()
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() => _mainForm.UpdateVideoList(_selectedVideos)));
                }
                else
                {
                    _mainForm.UpdateVideoList(_selectedVideos);
                }
            }
        }

        private void UpdateAuthenticationUI()
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        _mainForm.UpdateAuthenticationStatus(
                            _isAuthenticated,
                            _currentUser?.Name,
                            _currentUser?.Email,
                            _currentUser?.AvatarUrl);
                    }));
                }
                else
                {
                    _mainForm.UpdateAuthenticationStatus(
                        _isAuthenticated,
                        _currentUser?.Name,
                        _currentUser?.Email,
                        _currentUser?.AvatarUrl);
                }
            }
        }

        private void UpdateProjectStateUI()
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        _mainForm.ForceUpdateProcessingState();
                    }));
                }
                else
                {
                    _mainForm.ForceUpdateProcessingState();
                }
            }
        }

        private void UpdateThumbnailUI(string thumbnailPath)
        {
            if (_mainForm != null)
            {
                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() => _mainForm.SetThumbnail(thumbnailPath)));
                }
                else
                {
                    _mainForm.SetThumbnail(thumbnailPath);
                }
            }
        }

        private void NotifyVideoConcatenated()
        {
            try
            {
                if (_mainForm != null)
                {
                    if (_mainForm.InvokeRequired)
                    {
                        _mainForm.Invoke(new Action(() =>
                        {
                            _mainForm.ForceUpdateProcessingState();
                            _mainForm.SetCuttingDefaults("00:00:00", "00:05:00");
                        }));
                    }
                    else
                    {
                        _mainForm.ForceUpdateProcessingState();
                        _mainForm.SetCuttingDefaults("00:00:00", "00:05:00");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao notificar vídeo concatenado: {ex.Message}", ex);
            }
        }

        #endregion      
    }
}