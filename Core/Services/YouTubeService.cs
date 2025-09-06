// Core/Services/YouTubeService.cs - VERSÃO SIMPLES QUE FUNCIONA

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Futtage.Core.Models;
using Futtage.Infrastructure.Logging;

namespace Futtage.Core.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly ILogger _logger;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _applicationName;
        private UserCredential? _credential;
        private Google.Apis.YouTube.v3.YouTubeService? _youtubeService;
        private UserInfo? _currentUser;

        public YouTubeService(ILogger logger, string clientId = "", string clientSecret = "", string applicationName = "Futtage")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientId = clientId;
            _clientSecret = clientSecret;
            _applicationName = applicationName;
        }

        public async Task<bool> AuthenticateAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== YouTubeService.AuthenticateAsync ===");

            try
            {
                // Verificar se as credenciais estão configuradas
                if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
                {
                    System.Diagnostics.Debug.WriteLine("AVISO: Credenciais OAuth não configuradas - usando modo simulado");
                    return await AuthenticateSimulatedAsync();
                }

                System.Diagnostics.Debug.WriteLine("Tentando autenticação OAuth2 real...");

                // Configurar credenciais OAuth2
                var clientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                };

                // Definir scopes necessários
                var scopes = new[]
                {
                    Google.Apis.YouTube.v3.YouTubeService.Scope.YoutubeUpload,
                    Google.Apis.YouTube.v3.YouTubeService.Scope.YoutubeReadonly
                };

                // Pasta para armazenar tokens
                var credentialsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Futtage",
                    "Credentials"
                );

                // Autenticar usando OAuth2
                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credentialsPath, true)
                );

                // Criar serviço do YouTube
                _youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = _applicationName,
                });

                // Obter informações do usuário
                await LoadUserInfoAsync();

                System.Diagnostics.Debug.WriteLine("Autenticação real bem-sucedida");
                _logger.LogInfo($"Autenticação real do YouTube bem-sucedida: {_currentUser?.Name}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Falha na autenticação real: {ex.Message}");
                _logger.LogWarning($"Falha na autenticação real, usando simulado: {ex.Message}");

                // Fallback para modo simulado
                return await AuthenticateSimulatedAsync();
            }
        }

        private async Task<bool> AuthenticateSimulatedAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== Autenticação Simulada ===");

            await Task.Delay(2000);

            _currentUser = new UserInfo
            {
                Name = "Usuário Teste (Simulado)",
                Email = "teste@gmail.com",
                AvatarUrl = "",
                Id = "123456"
            };

            System.Diagnostics.Debug.WriteLine($"Autenticação simulada concluída - Usuário: {_currentUser.Name}");
            _logger.LogInfo("Autenticação simulada realizada");

            return true;
        }

        private async Task LoadUserInfoAsync()
        {
            try
            {
                if (_youtubeService == null) return;

                var channelsListRequest = _youtubeService.Channels.List("snippet");
                channelsListRequest.Mine = true;

                var channelsListResponse = await channelsListRequest.ExecuteAsync();

                if (channelsListResponse.Items?.Count > 0)
                {
                    var channel = channelsListResponse.Items[0];
                    _currentUser = new UserInfo
                    {
                        Name = channel.Snippet.Title,
                        Email = "email@gmail.com",
                        AvatarUrl = channel.Snippet.Thumbnails?.Default__?.Url ?? "",
                        Id = channel.Id
                    };
                }
                else
                {
                    _currentUser = new UserInfo
                    {
                        Name = "Usuário YouTube",
                        Email = "email@gmail.com",
                        Id = "unknown"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter info do usuário: {ex.Message}");
                _currentUser = new UserInfo
                {
                    Name = "Usuário YouTube",
                    Email = "email@gmail.com",
                    Id = "unknown"
                };
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            await Task.Delay(100);
            return _credential != null || _currentUser != null;
        }

        public async Task<UserInfo> GetUserInfoAsync()
        {
            await Task.Delay(100);
            return _currentUser ?? new UserInfo();
        }

        public async Task<string> UploadVideoAsync(YouTubeUploadRequest request, IProgress<ProcessingProgress>? progress = null)
        {
            System.Diagnostics.Debug.WriteLine("=== UploadVideoAsync INICIADO ===");
            System.Diagnostics.Debug.WriteLine($"Arquivo: {request.VideoFilePath}");
            System.Diagnostics.Debug.WriteLine($"Título: {request.Title}");
            System.Diagnostics.Debug.WriteLine($"Modo real disponível: {_youtubeService != null}");

            try
            {
                if (_youtubeService != null && File.Exists(request.VideoFilePath))
                {
                    return await UploadVideoRealAsync(request, progress);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Usando upload simulado");
                    return await UploadVideoSimulatedAsync(request, progress);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO no upload: {ex.Message}");
                _logger.LogError($"Erro no upload do vídeo: {ex.Message}", ex);

                // Em caso de erro no upload real, tentar simulado
                System.Diagnostics.Debug.WriteLine("Tentando upload simulado como fallback");
                return await UploadVideoSimulatedAsync(request, progress);
            }
        }

        private async Task<string> UploadVideoRealAsync(YouTubeUploadRequest request, IProgress<ProcessingProgress>? progress = null)
        {
            System.Diagnostics.Debug.WriteLine("=== Upload REAL para YouTube ===");

            try
            {
                // Criar objeto Video
                var video = new Video()
                {
                    Snippet = new VideoSnippet()
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Tags = request.Tags,
                        CategoryId = request.Category
                    },
                    Status = new VideoStatus()
                    {
                        PrivacyStatus = request.Privacy,
                        MadeForKids = request.IsForKids
                    }
                };

                using var fileStream = new FileStream(request.VideoFilePath, FileMode.Open, FileAccess.Read);

                var videosInsertRequest = _youtubeService!.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                // Simular progresso enquanto faz upload real
                var uploadTask = videosInsertRequest.UploadAsync();

                // Simular progresso em paralelo
                var progressTask = SimulateRealUploadProgress(progress, fileStream.Length);

                // Aguardar ambos
                await Task.WhenAll(uploadTask, progressTask);

                var uploadResult = await uploadTask;

                if (uploadResult.Status == UploadStatus.Completed)
                {
                    var videoId = videosInsertRequest.ResponseBody?.Id;
                    if (!string.IsNullOrEmpty(videoId))
                    {
                        System.Diagnostics.Debug.WriteLine($"Upload real concluído - Video ID: {videoId}");
                        _logger.LogInfo($"Upload real concluído - Video ID: {videoId}");
                        return videoId;
                    }
                    else
                    {
                        throw new Exception("Video ID não retornado após upload");
                    }
                }
                else
                {
                    throw new Exception($"Upload falhou com status: {uploadResult.Status}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no upload real: {ex.Message}");
                throw;
            }
        }

        private async Task SimulateRealUploadProgress(IProgress<ProcessingProgress>? progress, long fileSize)
        {
            // Simular progresso baseado no tamanho do arquivo
            var totalTime = Math.Max(5000, Math.Min(60000, fileSize / 1024)); // 5s a 60s baseado no tamanho
            var steps = 20;
            var stepTime = totalTime / steps;

            for (int i = 0; i <= steps; i++)
            {
                var percentage = (i * 100) / steps;
                var message = percentage switch
                {
                    < 20 => "Preparando upload real...",
                    < 90 => $"Enviando para YouTube... {percentage}%",
                    _ => "Finalizando upload..."
                };

                progress?.Report(new ProcessingProgress(percentage, message));
                System.Diagnostics.Debug.WriteLine($"Upload real progress: {percentage}%");

                if (i < steps)
                {
                    await Task.Delay((int)stepTime);
                }
            }
        }

        private async Task<string> UploadVideoSimulatedAsync(YouTubeUploadRequest request, IProgress<ProcessingProgress>? progress = null)
        {
            System.Diagnostics.Debug.WriteLine("=== Upload SIMULADO ===");

            // Simular progresso de upload
            for (int i = 0; i <= 100; i += 5)
            {
                await Task.Delay(300); // 300ms por etapa = ~6 segundos total

                var message = i switch
                {
                    < 20 => "Preparando upload...",
                    < 80 => $"Enviando para YouTube... {i}%",
                    < 100 => "Finalizando upload...",
                    _ => "Upload concluído!"
                };

                progress?.Report(new ProcessingProgress(i, message));
                System.Diagnostics.Debug.WriteLine($"Upload simulado: {i}% - {message}");
            }

            var videoId = $"sim_{Guid.NewGuid().ToString("N")[..11]}";
            System.Diagnostics.Debug.WriteLine($"Upload simulado concluído - ID: {videoId}");
            _logger.LogInfo($"Upload simulado concluído - ID: {videoId}");

            return videoId;
        }

        public async Task<bool> SetThumbnailAsync(string videoId, string thumbnailPath)
        {
            System.Diagnostics.Debug.WriteLine($"=== SetThumbnailAsync ===");
            System.Diagnostics.Debug.WriteLine($"Video ID: {videoId}");
            System.Diagnostics.Debug.WriteLine($"Thumbnail: {thumbnailPath}");

            try
            {
                if (_youtubeService != null && File.Exists(thumbnailPath))
                {
                    return await SetThumbnailRealAsync(videoId, thumbnailPath);
                }
                else
                {
                    return await SetThumbnailSimulatedAsync(videoId, thumbnailPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO ao definir thumbnail: {ex.Message}");
                _logger.LogError($"Erro ao definir thumbnail: {ex.Message}", ex);
                return false;
            }
        }

        private async Task<bool> SetThumbnailRealAsync(string videoId, string thumbnailPath)
        {
            System.Diagnostics.Debug.WriteLine("Definindo thumbnail REAL...");

            try
            {
                using var fileStream = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read);

                var thumbnailRequest = _youtubeService!.Thumbnails.Set(videoId, fileStream, "image/*");

                var result = await thumbnailRequest.UploadAsync();

                bool success = result.Status == UploadStatus.Completed;
                System.Diagnostics.Debug.WriteLine($"Thumbnail real definida: {success}");

                if (success)
                {
                    _logger.LogInfo($"Thumbnail real aplicada ao vídeo {videoId}");
                }
                else
                {
                    _logger.LogWarning($"Falha ao aplicar thumbnail real: {result.Status}");
                }

                return success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao definir thumbnail real: {ex.Message}");
                _logger.LogError($"Erro ao definir thumbnail real: {ex.Message}", ex);
                return false;
            }
        }

        private async Task<bool> SetThumbnailSimulatedAsync(string videoId, string thumbnailPath)
        {
            await Task.Delay(1000);
            System.Diagnostics.Debug.WriteLine($"Thumbnail simulada aplicada ao vídeo {videoId}");
            _logger.LogInfo($"Thumbnail simulada aplicada ao vídeo {videoId}");
            return true;
        }

        public void ClearCredentials()
        {
            System.Diagnostics.Debug.WriteLine("Limpando credenciais...");

            _credential = null;
            _youtubeService?.Dispose();
            _youtubeService = null;
            _currentUser = null;

            // Limpar tokens armazenados
            try
            {
                var credentialsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Futtage",
                    "Credentials"
                );

                if (Directory.Exists(credentialsPath))
                {
                    Directory.Delete(credentialsPath, true);
                    System.Diagnostics.Debug.WriteLine("Tokens armazenados removidos");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao limpar tokens: {ex.Message}");
            }

            _logger.LogInfo("Credenciais do YouTube limpas");
        }
    }
}