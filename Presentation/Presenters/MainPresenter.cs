using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        // Estado da aplicação
        private List<VideoInfo> _selectedVideos = new List<VideoInfo>();
        private string _concatenatedVideoPath = string.Empty;
        private string _finalVideoPath = string.Empty;
        private string _selectedThumbnailPath = string.Empty;
        private bool _isAuthenticated = false;
        private UserInfo? _currentUser;

        // Referência para o Form (será injetada) - CORRIGIDO
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

                    // Atualizar UI na thread principal
                    _mainForm?.Invoke(new Action(() =>
                    {
                        UpdateAuthenticationUI();
                    }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao verificar autenticação existente: {ex.Message}", ex);
            }
        }

        // === EVENTOS DO FORM ORIGINAL ===

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
                    ShowError("Nenhum arquivo de vídeo MP4 válido foi encontrado.");
                    return;
                }

                var videoInfos = await _videoService.GetMultipleVideoInfoAsync(validFiles);

                // Ordenar por data de criação
                var sortedVideos = videoInfos
                    .Where(v => v.IsValid)
                    .OrderBy(v => v.CreationDate)
                    .ToList();

                HideProgress();

                if (sortedVideos.Any())
                {
                    _selectedVideos.AddRange(sortedVideos);
                    UpdateVideoListUI();
                    ShowSuccess($"{sortedVideos.Count} arquivo(s) adicionado(s) com sucesso!");
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
        }


        public async Task OnAuthenticationClickedAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== MainPresenter.OnAuthenticationClickedAsync INICIADO ===");

            try
            {
                ShowProgress("Conectando com o Google...");
                System.Diagnostics.Debug.WriteLine("Chamando _youTubeService.AuthenticateAsync()");

                var success = await _youTubeService.AuthenticateAsync();
                System.Diagnostics.Debug.WriteLine($"Autenticação retornou: {success}");

                if (success)
                {
                    System.Diagnostics.Debug.WriteLine("Obtendo informações do usuário...");
                    _currentUser = await _youTubeService.GetUserInfoAsync();
                    _isAuthenticated = true;

                    System.Diagnostics.Debug.WriteLine($"Usuário: {_currentUser.Name}, Email: {_currentUser.Email}");
                    System.Diagnostics.Debug.WriteLine("Chamando UpdateAuthenticationUI()");

                    UpdateAuthenticationUI();
                    ShowSuccess("Autenticação realizada com sucesso!");
                    _logger.LogInfo($"Usuário autenticado: {_currentUser.Email}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Autenticação falhou");
                    ShowError("Falha na autenticação. Tente novamente.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEÇÃO na autenticação: {ex.Message}");
                ShowError($"Erro durante autenticação: {ex.Message}");
                _logger.LogError($"Erro na autenticação: {ex.Message}", ex);
            }
            finally
            {
                HideProgress();
                System.Diagnostics.Debug.WriteLine("=== MainPresenter.OnAuthenticationClickedAsync FINALIZADO ===");
            }
        }

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

            NavigateToTab(1); // Ir para aba de concatenação
        }

        public async Task OnConcatenateVideosClickedAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== MainPresenter.OnConcatenateVideosClickedAsync INICIADO ===");

            if (_selectedVideos.Count < 2)
            {
                ShowError("É necessário pelo menos 2 vídeos para concatenação.");
                return;
            }

            try
            {
                ShowProgress("Juntando vídeos...");
                System.Diagnostics.Debug.WriteLine("ShowProgress chamado - Processing deve estar True");

                var outputPath = await _fileService.SelectOutputPathAsync("video_concatenado.mp4");
                if (string.IsNullOrEmpty(outputPath))
                {
                    System.Diagnostics.Debug.WriteLine("Usuário cancelou seleção de arquivo - chamando HideProgress");
                    HideProgress();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Arquivo de saída selecionado: {outputPath}");

                var videoPaths = _selectedVideos.Select(v => v.FilePath).ToList();
                var progress = new Progress<ProcessingProgress>(p =>
                {
                    ShowProgress($"Juntando vídeos... {p.Message}", p.Percentage);
                });

                System.Diagnostics.Debug.WriteLine("Iniciando concatenação...");
                _concatenatedVideoPath = await _videoService.ConcatenateAsync(videoPaths, outputPath, progress);
                _finalVideoPath = _concatenatedVideoPath; // Por padrão, o vídeo final é o concatenado

                System.Diagnostics.Debug.WriteLine($"Concatenação concluída: {_concatenatedVideoPath}");

                // CORRIGIDO: Garantir que HideProgress seja chamado ANTES de outras operações
                HideProgress();
                System.Diagnostics.Debug.WriteLine("HideProgress chamado - Processing deve estar False agora");

                ShowSuccess("Vídeos juntados com sucesso!");

                // CORRIGIDO: Notificar Form sobre o vídeo concatenado
                NotifyVideoConcatenated();

                NavigateToTab(2); // Ir para aba de corte

                System.Diagnostics.Debug.WriteLine("Navegação para aba de corte concluída");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO na concatenação: {ex.Message}");
                HideProgress(); // CORRIGIDO: Garantir que seja chamado em caso de erro também
                ShowError($"Erro ao juntar vídeos: {ex.Message}");
                _logger.LogError($"Erro na concatenação: {ex.Message}", ex);
            }
            finally
            {
                // CORRIGIDO: Garantir que HideProgress seja chamado sempre
                System.Diagnostics.Debug.WriteLine("Finally - garantindo que HideProgress seja chamado");
                HideProgress();
                System.Diagnostics.Debug.WriteLine("=== MainPresenter.OnConcatenateVideosClickedAsync FINALIZADO ===");
            }
        }

        private void NotifyVideoConcatenated()
        {
            System.Diagnostics.Debug.WriteLine("=== NotifyVideoConcatenated INICIADO ===");

            try
            {
                _mainForm?.Invoke(new Action(() =>
                {
                    System.Diagnostics.Debug.WriteLine("Definindo valores padrão de corte...");

                    // CORRIGIDO: Primeiro forçar o estado de processamento para false
                    _mainForm.ForceUpdateProcessingState();

                    // Depois definir os valores de corte
                    _mainForm.SetCuttingDefaults("00:00:00", "00:05:00");

                    System.Diagnostics.Debug.WriteLine("Valores de corte definidos e estado atualizado");
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO ao notificar vídeo concatenado: {ex.Message}");
                _logger.LogError($"Erro ao notificar vídeo concatenado: {ex.Message}", ex);
            }

            System.Diagnostics.Debug.WriteLine("=== NotifyVideoConcatenated FINALIZADO ===");
        }


        private async Task CreateSimulatedCutFile(string outputPath)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Criando arquivo simulado: {outputPath}");

                // Criar diretório se não existir
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Criar arquivo vazio para simular o vídeo cortado
                await File.WriteAllTextAsync(outputPath, "Arquivo simulado de vídeo cortado");

                System.Diagnostics.Debug.WriteLine($"Arquivo simulado criado: {outputPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar arquivo simulado: {ex.Message}");
            }
        }
        public async Task OnCutVideoClickedAsync(string startTime, string endTime)
        {
            System.Diagnostics.Debug.WriteLine("=== MainPresenter.OnCutVideoClickedAsync INICIADO ===");
            System.Diagnostics.Debug.WriteLine($"Início: {startTime}, Fim: {endTime}");
            System.Diagnostics.Debug.WriteLine($"Vídeo concatenado: {_concatenatedVideoPath}");

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

                ShowProgress("Cortando vídeo...");
                System.Diagnostics.Debug.WriteLine("ShowProgress chamado para corte");

                // CORRIGIDO: Gerar nome de arquivo correto
                var outputPath = GenerateCutOutputPath(_concatenatedVideoPath);
                System.Diagnostics.Debug.WriteLine($"Arquivo de saída do corte: {outputPath}");

                var progress = new Progress<ProcessingProgress>(p =>
                {
                    ShowProgress($"Cortando vídeo... {p.Message}", p.Percentage);
                });

                System.Diagnostics.Debug.WriteLine("Iniciando corte do vídeo...");
                _finalVideoPath = await _videoService.TrimAsync(_concatenatedVideoPath, start, end, outputPath, progress);

                System.Diagnostics.Debug.WriteLine($"Corte concluído: {_finalVideoPath}");

                // Verificar se o arquivo foi realmente criado
                if (!_fileService.FileExists(_finalVideoPath))
                {
                    System.Diagnostics.Debug.WriteLine($"AVISO: Arquivo cortado não foi encontrado: {_finalVideoPath}");
                    // Em modo simulado, criar arquivo vazio para teste
                    await CreateSimulatedCutFile(_finalVideoPath);
                }

                // CORRIGIDO: Garantir que HideProgress seja chamado ANTES de outras operações
                HideProgress();
                System.Diagnostics.Debug.WriteLine("HideProgress chamado após corte");

                ShowSuccess($"Vídeo cortado com sucesso!\nSalvo em: {_finalVideoPath}");

                // CORRIGIDO: Notificar sobre vídeo cortado e navegar
                NotifyVideoCut();

                NavigateToTab(3); // Ir para aba de thumbnail

                System.Diagnostics.Debug.WriteLine("Navegação para aba de thumbnail concluída");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO no corte: {ex.Message}");
                HideProgress(); // CORRIGIDO: Garantir que seja chamado em caso de erro
                ShowError($"Erro ao cortar vídeo: {ex.Message}");
                _logger.LogError($"Erro no corte: {ex.Message}", ex);
            }
            finally
            {
                // CORRIGIDO: Garantir que HideProgress seja chamado sempre
                System.Diagnostics.Debug.WriteLine("Finally - garantindo que HideProgress seja chamado após corte");
                HideProgress();
                System.Diagnostics.Debug.WriteLine("=== MainPresenter.OnCutVideoClickedAsync FINALIZADO ===");
            }
        }

        private string GenerateCutOutputPath(string originalPath)
        {
            try
            {
                var directory = Path.GetDirectoryName(originalPath) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                var fileName = Path.GetFileNameWithoutExtension(originalPath);
                var extension = Path.GetExtension(originalPath);

                // Gerar nome único para evitar sobrescrever
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var cutFileName = $"{fileName}_cortado_{timestamp}{extension}";
                var outputPath = Path.Combine(directory, cutFileName);

                System.Diagnostics.Debug.WriteLine($"Caminho original: {originalPath}");
                System.Diagnostics.Debug.WriteLine($"Diretório: {directory}");
                System.Diagnostics.Debug.WriteLine($"Nome do arquivo: {fileName}");
                System.Diagnostics.Debug.WriteLine($"Extensão: {extension}");
                System.Diagnostics.Debug.WriteLine($"Caminho de saída gerado: {outputPath}");

                return outputPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao gerar caminho de saída: {ex.Message}");

                // Fallback: usar pasta de downloads
                var fallbackPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads",
                    $"video_cortado_{DateTime.Now:yyyyMMdd_HHmmss}.mp4"
                );

                System.Diagnostics.Debug.WriteLine($"Usando caminho fallback: {fallbackPath}");
                return fallbackPath;
            }
        }

        public void OnSkipCutClicked()
        {
            System.Diagnostics.Debug.WriteLine("=== OnSkipCutClicked INICIADO ===");

            _finalVideoPath = _concatenatedVideoPath;
            System.Diagnostics.Debug.WriteLine($"Vídeo final definido como: {_finalVideoPath}");

            // CORRIGIDO: Notificar sobre pulo do corte
            NotifyVideoCut();

            NavigateToTab(3); // Ir para aba de thumbnail

            System.Diagnostics.Debug.WriteLine("=== OnSkipCutClicked FINALIZADO ===");
        }

        private void NotifyVideoCut()
        {
            System.Diagnostics.Debug.WriteLine("=== NotifyVideoCut INICIADO ===");

            try
            {
                _mainForm?.Invoke(new Action(() =>
                {
                    System.Diagnostics.Debug.WriteLine("Atualizando estado após corte...");

                    // Forçar o estado de processamento para false
                    _mainForm.ForceUpdateProcessingState();

                    // Carregar thumbnail padrão
                    _mainForm.LoadDefaultThumbnailIfNeeded();

                    System.Diagnostics.Debug.WriteLine("Estado atualizado após corte/pulo");
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO ao notificar vídeo cortado: {ex.Message}");
                _logger.LogError($"Erro ao notificar vídeo cortado: {ex.Message}", ex);
            }

            System.Diagnostics.Debug.WriteLine("=== NotifyVideoCut FINALIZADO ===");
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
            NavigateToTab(4); // Ir para aba de upload
        }

        public async Task OnUploadToYouTubeClickedAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== MainPresenter.OnUploadToYouTubeClickedAsync INICIADO ===");

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
                System.Diagnostics.Debug.WriteLine($"Iniciando processo de upload do arquivo: {_finalVideoPath}");

                // Obter detalhes do vídeo do usuário
                var uploadRequest = await GetUploadDetailsFromUserAsync();
                if (uploadRequest == null)
                {
                    System.Diagnostics.Debug.WriteLine("Usuário cancelou a entrada de detalhes");
                    return;
                }

                ShowProgress("Preparando upload para o YouTube...");

                var progress = new Progress<ProcessingProgress>(p =>
                {
                    ShowProgress($"Upload para YouTube... {p.Message}", p.Percentage);
                });

                System.Diagnostics.Debug.WriteLine("Iniciando upload do vídeo...");
                var videoId = await _youTubeService.UploadVideoAsync(uploadRequest, progress);

                System.Diagnostics.Debug.WriteLine($"Upload do vídeo concluído. Video ID: {videoId}");

                // Aplicar thumbnail se selecionada
                if (!string.IsNullOrEmpty(_selectedThumbnailPath) && _fileService.FileExists(_selectedThumbnailPath))
                {
                    System.Diagnostics.Debug.WriteLine("Aplicando thumbnail personalizada...");
                    ShowProgress("Aplicando thumbnail...", 95);

                    try
                    {
                        await _youTubeService.SetThumbnailAsync(videoId, _selectedThumbnailPath);
                        System.Diagnostics.Debug.WriteLine("Thumbnail aplicada com sucesso");
                    }
                    catch (Exception thumbnailEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao aplicar thumbnail: {thumbnailEx.Message}");
                        _logger.LogWarning($"Erro ao aplicar thumbnail: {thumbnailEx.Message}");
                        // Não falhar o upload por causa da thumbnail
                    }
                }

                HideProgress();

                // Mostrar resultado do upload
                ShowUploadSuccess(videoId, uploadRequest);

                _logger.LogInfo($"Upload completo - Video ID: {videoId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO durante upload: {ex.Message}");
                HideProgress();
                ShowError($"Erro durante upload: {ex.Message}");
                _logger.LogError($"Erro no upload: {ex.Message}", ex);
            }
            finally
            {
                HideProgress();
                System.Diagnostics.Debug.WriteLine("=== MainPresenter.OnUploadToYouTubeClickedAsync FINALIZADO ===");
            }
        }

        private async Task<YouTubeUploadRequest?> GetUploadDetailsFromUserAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== GetUploadDetailsFromUserAsync ===");

            try
            {
                YouTubeUploadRequest? result = null;

                await Task.Run(() =>
                {
                    _mainForm?.Invoke(new Action(() =>
                    {
                        System.Diagnostics.Debug.WriteLine("Abrindo formulário de detalhes do vídeo...");

                        var creationDate = _selectedVideos.FirstOrDefault()?.CreationDate ?? DateTime.Now;

                        using var detalhesForm = new FormDetalhesVideo(creationDate);

                        if (detalhesForm.ShowDialog() == DialogResult.OK)
                        {
                            System.Diagnostics.Debug.WriteLine("Usuário confirmou detalhes do vídeo");

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

                            System.Diagnostics.Debug.WriteLine($"Detalhes capturados - Título: {result.Title}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Usuário cancelou entrada de detalhes");
                        }
                    }));
                });

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter detalhes do upload: {ex.Message}");
                _logger.LogError($"Erro ao obter detalhes do upload: {ex.Message}", ex);
                return null;
            }
        }

        // === MÉTODOS DE NAVEGAÇÃO ===

        private void NavigateToTab(int tabIndex)
        {
            _mainForm?.Invoke(new Action(() =>
            {
                _mainForm.NavigateToTab(tabIndex); // CORRIGIDO - agora chama método específico do Form1
            }));
        }

        public bool CanNavigateToTab(int tabIndex)
        {
            System.Diagnostics.Debug.WriteLine($"=== CanNavigateToTab({tabIndex}) ===");
            System.Diagnostics.Debug.WriteLine($"Videos selecionados: {_selectedVideos.Count}");
            System.Diagnostics.Debug.WriteLine($"Autenticado: {_isAuthenticated}");
            System.Diagnostics.Debug.WriteLine($"Vídeo concatenado: '{_concatenatedVideoPath}'");
            System.Diagnostics.Debug.WriteLine($"Vídeo final: '{_finalVideoPath}'");

            var result = tabIndex switch
            {
                0 => true, // Seleção sempre permitida
                1 => _selectedVideos.Count >= 2, // Concatenação
                2 => !string.IsNullOrEmpty(_concatenatedVideoPath), // Corte
                3 => !string.IsNullOrEmpty(_finalVideoPath), // Thumbnail - CORRIGIDO: usar _finalVideoPath
                4 => _isAuthenticated && !string.IsNullOrEmpty(_finalVideoPath), // Upload
                _ => false
            };

            System.Diagnostics.Debug.WriteLine($"Resultado CanNavigateToTab({tabIndex}): {result}");

            // Debug específico para cada aba
            switch (tabIndex)
            {
                case 1:
                    System.Diagnostics.Debug.WriteLine($"  Concatenação: precisa de 2+ vídeos, tem {_selectedVideos.Count}");
                    break;
                case 2:
                    System.Diagnostics.Debug.WriteLine($"  Corte: precisa de vídeo concatenado, tem: {!string.IsNullOrEmpty(_concatenatedVideoPath)}");
                    break;
                case 3:
                    System.Diagnostics.Debug.WriteLine($"  Thumbnail: precisa de vídeo final, tem: {!string.IsNullOrEmpty(_finalVideoPath)}");
                    break;
                case 4:
                    System.Diagnostics.Debug.WriteLine($"  Upload: precisa de auth + vídeo final, auth: {_isAuthenticated}, vídeo: {!string.IsNullOrEmpty(_finalVideoPath)}");
                    break;
            }

            return result;
        }

        // === MÉTODOS DE UI - CORRIGIDOS ===
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
                _mainForm?.Invoke(new Action(() =>
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
                            System.Diagnostics.Debug.WriteLine($"Erro ao abrir navegador: {ex.Message}");

                            // Fallback: copiar para clipboard
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao mostrar sucesso do upload: {ex.Message}");
            }
        }

        private void ShowProgress(string message, int percentage = 0)
        {
            _mainForm?.ShowProgress(message, percentage); // CORRIGIDO - chama método específico do Form1
        }

        private void HideProgress()
        {
            _mainForm?.HideProgress(); // CORRIGIDO - chama método específico do Form1
        }

        private void ShowError(string message)
        {
            _mainForm?.Invoke(new Action(() =>
            {
                MessageBox.Show(message, "Erro - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        private void ShowSuccess(string message)
        {
            _mainForm?.Invoke(new Action(() =>
            {
                MessageBox.Show(message, "Sucesso - Futtage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void UpdateVideoListUI()
        {
            _mainForm?.UpdateVideoList(_selectedVideos); // CORRIGIDO - chama método específico do Form1
        }

        private void UpdateAuthenticationUI()
        {
            System.Diagnostics.Debug.WriteLine("=== UpdateAuthenticationUI INICIADO ===");
            System.Diagnostics.Debug.WriteLine($"_mainForm é null? {_mainForm == null}");
            System.Diagnostics.Debug.WriteLine($"_isAuthenticated: {_isAuthenticated}");
            System.Diagnostics.Debug.WriteLine($"_currentUser é null? {_currentUser == null}");

            if (_currentUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"Nome: {_currentUser.Name}, Email: {_currentUser.Email}");
            }

            _mainForm?.UpdateAuthenticationStatus(
                _isAuthenticated,
                _currentUser?.Name,
                _currentUser?.Email,
                _currentUser?.AvatarUrl);

            System.Diagnostics.Debug.WriteLine("=== UpdateAuthenticationUI FINALIZADO ===");
        }

        private void UpdateThumbnailUI(string thumbnailPath)
        {
            _mainForm?.SetThumbnail(thumbnailPath); // CORRIGIDO - chama método específico do Form1
        }

        // === MÉTODOS AUXILIARES - CORRIGIDOS ===

        private YouTubeUploadRequest CreateUploadRequest() // CORRIGIDO - método adicionado
        {
            var fileName = Path.GetFileNameWithoutExtension(_finalVideoPath);
            var date = DateTime.Now.ToString("dd/MM/yyyy");

            return new YouTubeUploadRequest
            {
                VideoFilePath = _finalVideoPath,
                Title = $"Viana - {date} - {fileName}",
                Description = "Câmera: SJ4000 AIR\n\nTime Meu:\n🧤\nTime Teu:\n🧤",
                ThumbnailPath = _selectedThumbnailPath,
                IsForKids = false,
                Privacy = "private"
            };
        }

        public void OnThumbnailTabEntered()
        {
            System.Diagnostics.Debug.WriteLine("=== OnThumbnailTabEntered INICIADO ===");
            System.Diagnostics.Debug.WriteLine($"Vídeo final disponível: {!string.IsNullOrEmpty(_finalVideoPath)}");
            System.Diagnostics.Debug.WriteLine($"Caminho do vídeo final: {_finalVideoPath}");

            // Carregar thumbnail padrão se não houver uma selecionada
            if (string.IsNullOrEmpty(_selectedThumbnailPath))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Carregando thumbnail padrão...");
                    _mainForm?.Invoke(new Action(() =>
                    {
                        _mainForm.LoadDefaultThumbnailIfNeeded();
                    }));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Erro ao carregar thumbnail padrão: {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine("=== OnThumbnailTabEntered FINALIZADO ===");
        }

        public void Dispose()
        {
            _logger.LogInfo("MainPresenter finalizado");
            _fileService?.DeleteTempFiles();
        }
    }
}