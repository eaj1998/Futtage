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
using System.Text.Json.Serialization;
using static Futtage.Core.Services.FuttageYouTubeService;
using System.Text.Json;
using Google.Apis.Auth.OAuth2.Responses;

namespace Futtage.Core.Services
{
    public class FuttageYouTubeService : IYouTubeService
    {
        private readonly ILogger _logger;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _applicationName;
        private UserCredential? _credential;
        private YouTubeService? _youtubeService;
        private bool _isAuthenticated = false;
        private UserInfo? _currentUser;

        private readonly string _credentialsFileName = "credentials.json";
        private readonly string _dataStoreName = "FuttageCredentials";

        public FuttageYouTubeService(ILogger logger, string clientId = "", string clientSecret = "", string applicationName = "Futtage")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientId = clientId;
            _clientSecret = clientSecret;
            _applicationName = applicationName;
        }

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                //if (!File.Exists(_credentialsFileName))
                //{
                //    throw new FileNotFoundException($"Arquivo de credenciais não encontrado: {_credentialsFileName}");
                //}

                await CleanupBeforeAuthentication();

                if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
                {
                    throw new InvalidOperationException(
                        "Credenciais OAuth não configuradas. Configure ClientId e ClientSecret no appsettings.json ou variáveis de ambiente.");
                }

                _logger.LogInfo("Iniciando autenticação OAuth2 com Google");

                var clientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                };

                var scopes = new[]
                {
                    Google.Apis.YouTube.v3.YouTubeService.Scope.YoutubeUpload,
                    Google.Apis.YouTube.v3.YouTubeService.Scope.YoutubeReadonly
                };

                var credentialsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Futtage",
                    "Credentials"
                );

                Directory.CreateDirectory(credentialsPath);

                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credentialsPath, true)
                );

                if (_credential != null)
                {
                    _youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = _credential,
                        ApplicationName = _applicationName,
                    });

                    var isValid = await TestCredentialAsync();
                    if (isValid)
                    {
                        _isAuthenticated = true;
                        _logger.LogInfo("Autenticação realizada com sucesso");
                        await LoadUserInfoAsync();
                        return true;
                    }
                    else
                    {
                        _logger.LogError("Credencial criada mas falhou no teste");
                        await CleanupOnError();
                        return false;
                    }
                }
                _logger.LogError("Credencial nulla autenticação falhou");
                await CleanupOnError();
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro na autenticação: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                if (_credential == null)
                {
                    await LoadSavedCredentialAsync();
                }

                if (_credential != null)
                {
                    return await TestCredentialAsync();
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao verificar autenticação: {ex.Message}", ex);
                if (IsTokenError(ex))
                {
                    _logger.LogWarning("Token corrompido detectado, limpando credenciais...");
                    await CleanupCorruptedCredentials();
                }

                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInfo("Iniciando processo de logout");

                if (_credential?.Token != null && !string.IsNullOrEmpty(_credential.Token.AccessToken))
                {
                    try
                    {
                        await RevokeTokenAsync(_credential.Token.AccessToken);
                        _logger.LogInfo("Token revogado com sucesso");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Erro ao revogar token: {ex.Message}");
                    }
                }

                await ClearStoredCredentialsAsync();

                await CleanupOnError();

                _logger.LogInfo("Logout realizado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro durante logout: {ex.Message}", ex);

                await CleanupOnError();

                throw;
            }
        }

        public async Task<UserInfo> GetUserInfoAsync()
        {
            try
            {
                _logger.LogInfo("=== GetUserInfoAsync chamado ===");

                if (!_isAuthenticated || _youtubeService == null)
                {
                    throw new InvalidOperationException("Usuário não está autenticado");
                }

                // Se já temos as informações e são válidas, retornar
                if (_currentUser != null && !string.IsNullOrEmpty(_currentUser.Name) && _currentUser.Id != "unknown")
                {
                    _logger.LogInfo($"Retornando informações em cache: {_currentUser.Name}");
                    return _currentUser;
                }

                // Carregar informações do usuário usando LoadUserInfoAsync
                await LoadUserInfoAsync();

                // Verificar se conseguiu carregar
                if (_currentUser == null)
                {
                    throw new InvalidOperationException("Não foi possível carregar informações do usuário");
                }

                _logger.LogInfo($"Informações do usuário obtidas: {_currentUser.Name}");
                return _currentUser;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro em GetUserInfoAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<string> UploadVideoAsync(YouTubeUploadRequest request, IProgress<ProcessingProgress>? progress = null)
        {
         
            try
            {
                if (!_isAuthenticated || _youtubeService == null)
                    throw new InvalidOperationException("Não autenticado");

                if (string.IsNullOrEmpty(request.VideoFilePath) || !File.Exists(request.VideoFilePath))
                    throw new FileNotFoundException("Arquivo de vídeo não encontrado", request.VideoFilePath);

                await EnsureValidTokenAsync();

                _logger.LogInfo($"Iniciando upload real para YouTube: {request.Title}");

                var video = new Video()
                {
                    Snippet = new VideoSnippet()
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Tags = request.Tags?.ToList(),
                        CategoryId = request.Category,
                        DefaultLanguage = "pt-BR",
                        DefaultAudioLanguage = "pt-BR"
                    },
                    Status = new VideoStatus()
                    {
                        PrivacyStatus = request.Privacy,
                        MadeForKids = request.IsForKids,
                        SelfDeclaredMadeForKids = request.IsForKids
                    }
                };

                progress?.Report(new ProcessingProgress(0, "Iniciando upload..."));

                using var fileStream = new FileStream(request.VideoFilePath, FileMode.Open, FileAccess.Read);
                var fileSize = fileStream.Length;

                var videosInsertRequest = _youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ChunkSize = 1024 * 1024; 

                var progressTask = SimulateUploadProgress(progress, fileSize);

                videosInsertRequest.ResponseReceived += (video) =>
                {
                    _logger.LogInfo($"Upload concluído. Video ID: {video.Id}");
                };

                var uploadTask = videosInsertRequest.UploadAsync();

                await Task.WhenAll(uploadTask, progressTask);

                var uploadResult = await uploadTask;

                if (uploadResult.Status == UploadStatus.Completed)
                {
                    var videoId = videosInsertRequest.ResponseBody?.Id;
                    if (string.IsNullOrEmpty(videoId))
                        throw new Exception("Video ID não retornado após upload");

                    progress?.Report(new ProcessingProgress(100, "Upload concluído!"));
                    _logger.LogInfo($"Upload bem-sucedido - Video ID: {videoId}");

                    return videoId;
                }
                else if (uploadResult.Status == UploadStatus.Failed)
                {
                    throw new Exception($"Upload falhou: {uploadResult.Exception?.Message ?? "Erro desconhecido"}");
                }
                else
                {
                    throw new Exception($"Upload não foi concluído. Status: {uploadResult.Status}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro no upload: {ex.Message}", ex);

                if (IsTokenError(ex))
                {
                    await CleanupCorruptedCredentials();
                    throw new InvalidOperationException("Token expirado durante upload. Faça login novamente.", ex);
                }

                throw;
            }
        }

        public async Task<bool> SetThumbnailAsync(string videoId, string thumbnailPath)
        {


            try
            {
                if (!_isAuthenticated || _youtubeService == null)
                    throw new InvalidOperationException("Não autenticado");

                if (string.IsNullOrEmpty(thumbnailPath) || !File.Exists(thumbnailPath))
                    throw new FileNotFoundException("Arquivo de thumbnail não encontrado: ", thumbnailPath);

                _logger.LogInfo($"Definindo thumbnail para vídeo {videoId}");

                using var fileStream = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read);

                var thumbnailRequest = _youtubeService.Thumbnails.Set(videoId, fileStream, GetMimeType(thumbnailPath));

                var result = await thumbnailRequest.UploadAsync();

                var success = result.Status == UploadStatus.Completed;

                if (success)
                {
                    _logger.LogInfo($"Thumbnail aplicada com sucesso ao vídeo {videoId}");
                }
                else
                {
                    _logger.LogError($"Falha ao aplicar thumbnail: {result.Status}");
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao definir thumbnail: {ex.Message}", ex);
                if (IsTokenError(ex))
                {
                    await CleanupCorruptedCredentials();
                    throw new InvalidOperationException("Token expirado durante aplicação de thumbnail. Faça login novamente.", ex);
                }

                throw;
            }
        }

        public void ClearCredentials()
        {
            try
            {
                _logger.LogInfo("Limpando credenciais do YouTube");

                _credential = null;
                _youtubeService?.Dispose();
                _youtubeService = null;
                _currentUser = null;

                var credentialsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Futtage",
                    "Credentials"
                );

                if (Directory.Exists(credentialsPath))
                {
                    Directory.Delete(credentialsPath, true);
                    _logger.LogInfo("Tokens armazenados removidos");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao limpar credenciais: {ex.Message}", ex);
            }
        }
        public void Dispose()
        {
            try
            {
                _youtubeService?.Dispose();
                _logger.LogInfo("FuttageYouTubeService finalizado");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro ao finalizar YouTubeService: {ex.Message}");
            }
        }

        #region Métodos Privados
        private async Task<bool> LoadSavedCredentialAsync()
        {
            try
            {
                var credentialsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FuttageCredentials");

                if (Directory.Exists(credentialsPath))
                {
                    var scopes = new[]
                    {
                        YouTubeService.Scope.YoutubeUpload,
                        YouTubeService.Scope.YoutubeReadonly,
                        "https://www.googleapis.com/auth/userinfo.profile",
                        "https://www.googleapis.com/auth/userinfo.email"
                    };

                    _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromFile("credentials.json").Secrets,
                        scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore("FuttageCredentials", true));

                    if (_credential != null)
                    {
                        _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = _credential,
                            ApplicationName = "Futtage Video Editor"
                        });

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar credencial salva: {ex.Message}", ex);
                if (IsTokenError(ex))
                {
                    await CleanupCorruptedCredentials();
                }

                return false;
            }
        }

        private async Task<bool> TestCredentialAsync()
        {
            try
            {
                if (_youtubeService == null || _credential == null)
                    return false;

                await EnsureValidTokenAsync();

                var request = _youtubeService.Channels.List("snippet");
                request.Mine = true;
                request.MaxResults = 1;

                var response = await request.ExecuteAsync();
                return response != null;
            }
            catch (Exception ex)
            {
                if (IsTokenError(ex))
                {
                    await CleanupCorruptedCredentials();
                }
                return false;
            }

        }

        private async Task EnsureValidTokenAsync()
        {
            if (_credential?.Token == null)
            {
                throw new InvalidOperationException("Credencial não está disponível");
            }

            if (_credential.Token.IsExpired(_credential.Flow.Clock))
            {
                _logger.LogInfo("Token expirado, tentando renovar...");

                try
                {
                    await _credential.RefreshTokenAsync(CancellationToken.None);
                    _logger.LogInfo("Token renovado com sucesso");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Falha ao renovar token: {ex.Message}", ex);

                    await CleanupCorruptedCredentials();
                    throw new InvalidOperationException("Token expirado e não pôde ser renovado. Faça login novamente.", ex);
                }
            }
        }

        // NOVO: Verificar se o erro é relacionado a token
        private static bool IsTokenError(Exception ex)
        {
            return ex is TokenResponseException ||
                   ex.Message.Contains("invalid_grant") ||
                   ex.Message.Contains("Token has been expired") ||
                   ex.Message.Contains("invalid_token") ||
                   ex.Message.Contains("unauthorized");
        }

        private async Task CleanupBeforeAuthentication()
        {
            try
            {
                _credential = null;
                _youtubeService?.Dispose();
                _youtubeService = null;
                _isAuthenticated = false;

                _logger.LogInfo("Limpeza pré-autenticação concluída");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro durante limpeza pré-autenticação: {ex.Message}");
            }
        }

        private async Task CleanupCorruptedCredentials()
        {
            try
            {
                _logger.LogWarning("Limpando credenciais corrompidas...");

                _credential = null;
                _youtubeService?.Dispose();
                _youtubeService = null;
                _isAuthenticated = false;

                await ClearStoredCredentialsAsync();

                _logger.LogInfo("Credenciais corrompidas limpas");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao limpar credenciais corrompidas: {ex.Message}", ex);
            }
        }

        private async Task RevokeTokenAsync(string accessToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                var revokeUrl = $"https://oauth2.googleapis.com/revoke?token={accessToken}";

                var response = await httpClient.PostAsync(revokeUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Falha ao revogar token: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro ao revogar token: {ex.Message}");
                throw;
            }

        }

        private async Task ClearStoredCredentialsAsync()
        {
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
                    _logger.LogInfo("Credenciais armazenadas removidas");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro ao limpar credenciais armazenadas: {ex.Message}");
            }
        }
        private async Task LoadUserInfoAsync()
        {
            try
            {
                _logger.LogInfo("Iniciando carregamento de informações do usuário...");

                if (_youtubeService == null)
                {
                    _logger.LogWarning("YouTubeService não está disponível");
                    SetDefaultUserInfo();
                    return;
                }

                // Verificar se o token está válido antes de fazer a chamada
                if (_credential?.Token != null && _credential.Token.IsExpired(_credential.Flow.Clock))
                {
                    _logger.LogInfo("Token expirado, tentando renovar...");
                    try
                    {
                        await _credential.RefreshTokenAsync(CancellationToken.None);
                        _logger.LogInfo("Token renovado com sucesso");
                    }
                    catch (Exception refreshEx)
                    {
                        _logger.LogError($"Falha ao renovar token: {refreshEx.Message}", refreshEx);
                        SetDefaultUserInfo();
                        return;
                    }
                }

                // Fazer a requisição para obter informações do canal
                var channelsListRequest = _youtubeService.Channels.List("snippet,statistics");
                channelsListRequest.Mine = true;
                channelsListRequest.MaxResults = 1;

                _logger.LogInfo("Executando requisição para obter canal do usuário...");
                var channelsListResponse = await channelsListRequest.ExecuteAsync();

                if (channelsListResponse?.Items?.Count > 0)
                {
                    var channel = channelsListResponse.Items[0];
                    var snippet = channel.Snippet;

                    _currentUser = new UserInfo
                    {
                        Name = snippet?.Title ?? "Canal YouTube",
                        Email = snippet?.Title ?? "Canal YouTube", // API do YouTube não retorna email
                        AvatarUrl = snippet?.Thumbnails?.Default__?.Url ??
                                   snippet?.Thumbnails?.Medium?.Url ??
                                   snippet?.Thumbnails?.High?.Url ?? "",
                        Id = channel.Id ?? "unknown"
                    };

                    _logger.LogInfo($"Informações do usuário carregadas com sucesso: {_currentUser.Name} (ID: {_currentUser.Id})");

                    // Log adicional das estatísticas se disponíveis
                    if (channel.Statistics != null)
                    {
                        _logger.LogInfo($"Estatísticas do canal - Inscritos: {channel.Statistics.SubscriberCount}, Videos: {channel.Statistics.VideoCount}");
                    }
                }
                else
                {
                    _logger.LogWarning("Nenhum canal encontrado para o usuário autenticado");
                    SetDefaultUserInfo();
                }
            }
            catch (Google.GoogleApiException googleEx)
            {
                _logger.LogError($"Erro da API do Google: {googleEx.Message} (Code: {googleEx.HttpStatusCode})", googleEx);

                // Se for erro 401 ou 403, pode ser problema de autenticação
                if (googleEx.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("Erro 401 - Token inválido ou expirado");
                    throw new InvalidOperationException("Token de autenticação inválido. Faça login novamente.", googleEx);
                }
                else if (googleEx.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogError("Erro 403 - Acesso negado. Verificar scopes ou permissões");
                    throw new InvalidOperationException("Acesso negado. Verifique as permissões do aplicativo.", googleEx);
                }

                SetDefaultUserInfo();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro inesperado ao carregar informações do usuário: {ex.Message}", ex);
                SetDefaultUserInfo();
                throw;
            }
        }

        // Método auxiliar para definir informações padrão do usuário
        private void SetDefaultUserInfo()
        {
            _currentUser = new UserInfo
            {
                Name = "Usuário YouTube",
                Email = "Não disponível",
                AvatarUrl = "",
                Id = "unknown"
            };

            _logger.LogInfo("Informações padrão do usuário definidas");
        }

        private async Task SimulateUploadProgress(IProgress<ProcessingProgress>? progress, long fileSize)
        {
            if (progress == null) return;

            var totalTime = Math.Max(5000, Math.Min(60000, fileSize / (1024 * 100))); // 5s a 60s
            var steps = 20;
            var stepTime = totalTime / steps;

            for (int i = 0; i <= steps; i++)
            {
                var percentage = (i * 99) / steps; 
                var mbSent = (fileSize * i / steps) / (1024.0 * 1024.0);
                var mbTotal = fileSize / (1024.0 * 1024.0);

                var message = percentage switch
                {
                    < 20 => "Preparando upload...",
                    < 99 => $"Enviando: {mbSent:F1} MB / {mbTotal:F1} MB",
                    _ => "Finalizando upload..."
                };

                progress.Report(new ProcessingProgress(percentage, message));

                if (i < steps)
                {
                    await Task.Delay((int)stepTime);
                }
            }
        }

        private static string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };
        }

        private async Task CleanupOnError()
        {
            try
            {
                _credential = null;
                _youtubeService?.Dispose();
                _youtubeService = null;
                _isAuthenticated = false;

                await Task.CompletedTask; // Para manter a assinatura async
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro durante cleanup: {ex.Message}");
            }
        }

        #endregion     
    }
}