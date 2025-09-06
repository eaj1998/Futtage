using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Futtage.Core.Models;
using Futtage.Core.Services;
using Futtage.Infrastructure.Logging;
using Futtage.Presentation.Views;
using Google.Apis.YouTube.v3;

namespace Futtage.Presentation.Presenters
{
    public class VideoSelectionPresenter
    {
        private readonly IVideoSelectionView _view;
        private readonly IVideoProcessingService _videoService;
        private readonly IYouTubeService _youTubeService;
        private readonly ILogger _logger;

        public event EventHandler<List<VideoInfo>> VideosReady;
        public event EventHandler ReadyToAdvance;

        public VideoSelectionPresenter(
            IVideoSelectionView view,
            IVideoProcessingService videoService,
            IYouTubeService youTubeService,
            ILogger logger)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _videoService = videoService ?? throw new ArgumentNullException(nameof(videoService));
            _youTubeService = youTubeService ?? throw new ArgumentNullException(nameof(youTubeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            SubscribeToViewEvents();
            InitializeAsync();
        }

        private void SubscribeToViewEvents()
        {
            _view.FilesSelected += OnFilesSelected;
            _view.VideoSelectionChanged += OnVideoSelectionChanged;
            _view.VideoMoveRequested += OnVideoMoveRequested;
            _view.VideoRemovalRequested += OnVideoRemovalRequested;
            _view.AuthenticationRequested += OnAuthenticationRequested;
            _view.NextStepRequested += OnNextStepRequested;
        }

        private async void InitializeAsync()
        {
            try
            {
                // Verificar se já está autenticado
                var isAuthenticated = await _youTubeService.IsAuthenticatedAsync();
                if (isAuthenticated)
                {
                    var userInfo = await _youTubeService.GetUserInfoAsync();
                    _view.UpdateAuthenticationStatus(true, userInfo.Name, userInfo.Email, userInfo.AvatarUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao verificar autenticação: {ex.Message}", ex);
            }
        }

        private async void OnFilesSelected(object sender, List<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
                return;

            _view.ShowProgress("Analisando arquivos de vídeo...");

            try
            {
                var validFiles = filePaths.Where(f =>
                    File.Exists(f) &&
                    _videoService.ValidateVideoFile(f)
                ).ToList();

                if (!validFiles.Any())
                {
                    _view.HideProgress();
                    _view.ShowError("Nenhum arquivo de vídeo MP4 válido foi encontrado.");
                    return;
                }

                var videoInfos = await _videoService.GetMultipleVideoInfoAsync(validFiles);

                // Ordenar por data de criação
                var sortedVideos = videoInfos
                    .Where(v => v.IsValid)
                    .OrderBy(v => v.CreationDate)
                    .ToList();

                _view.HideProgress();

                if (sortedVideos.Any())
                {
                    _view.AddVideos(sortedVideos);
                    _view.ShowSuccess($"{sortedVideos.Count} arquivo(s) adicionado(s) com sucesso!");

                    VideosReady?.Invoke(this, sortedVideos);
                }
                else
                {
                    _view.ShowError("Não foi possível processar nenhum dos arquivos selecionados.");
                }
            }
            catch (Exception ex)
            {
                _view.HideProgress();
                _view.ShowError($"Erro ao processar arquivos: {ex.Message}");
                _logger.LogError($"Erro ao processar arquivos selecionados: {ex.Message}", ex);
            }
        }

        private void OnVideoSelectionChanged(object sender, VideoInfo selectedVideo)
        {
            _logger.LogDebug($"Vídeo selecionado: {selectedVideo?.FilePath ?? "nenhum"}");
        }

        private void OnVideoMoveRequested(object sender, VideoMoveEventArgs e)
        {
            try
            {
                var currentVideos = _view.SelectedVideos.ToList();
                var currentIndex = currentVideos.IndexOf(e.Video);
                var newIndex = currentIndex + e.Direction;

                if (newIndex < 0 || newIndex >= currentVideos.Count)
                    return;

                // Trocar posições
                currentVideos.RemoveAt(currentIndex);
                currentVideos.Insert(newIndex, e.Video);

                _view.UpdateVideoOrder(currentVideos);

                _logger.LogDebug($"Vídeo movido: {e.Video.FilePath} (direção: {e.Direction})");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Erro ao mover vídeo: {ex.Message}");
                _logger.LogError($"Erro ao mover vídeo: {ex.Message}", ex);
            }
        }

        private void OnVideoRemovalRequested(object sender, VideoInfo video)
        {
            try
            {
                _view.RemoveVideo(video);
                _logger.LogDebug($"Vídeo removido: {video.FilePath}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Erro ao remover vídeo: {ex.Message}");
                _logger.LogError($"Erro ao remover vídeo: {ex.Message}", ex);
            }
        }

        private async void OnAuthenticationRequested(object sender, EventArgs e)
        {
            _view.ShowProgress("Conectando com o Google...");

            try
            {
                var success = await _youTubeService.AuthenticateAsync();

                if (success)
                {
                    var userInfo = await _youTubeService.GetUserInfoAsync();
                    _view.UpdateAuthenticationStatus(true, userInfo.Name, userInfo.Email, userInfo.AvatarUrl);
                    _view.ShowSuccess("Autenticação realizada com sucesso!");

                    _logger.LogInfo($"Usuário autenticado: {userInfo.Email}");
                }
                else
                {
                    _view.UpdateAuthenticationStatus(false);
                    _view.ShowError("Falha na autenticação. Tente novamente.");
                }
            }
            catch (Exception ex)
            {
                _view.UpdateAuthenticationStatus(false);
                _view.ShowError($"Erro durante autenticação: {ex.Message}");
                _logger.LogError($"Erro na autenticação: {ex.Message}", ex);
            }
            finally
            {
                _view.HideProgress();
            }
        }

        private void OnNextStepRequested(object sender, EventArgs e)
        {
            var selectedVideos = _view.SelectedVideos;

            if (selectedVideos.Count < 2)
            {
                _view.ShowError("É necessário selecionar pelo menos 2 vídeos para continuar.");
                return;
            }

            if (!_view.IsGoogleAuthenticated)
            {
                _view.ShowError("É necessário fazer login com sua conta Google para continuar.");
                return;
            }

            try
            {
                ReadyToAdvance?.Invoke(this, EventArgs.Empty);
                _logger.LogInfo($"Avançando para próxima etapa com {selectedVideos.Count} vídeos");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Erro ao avançar: {ex.Message}");
                _logger.LogError($"Erro ao avançar para próxima etapa: {ex.Message}", ex);
            }
        }

        public List<VideoInfo> GetSelectedVideos()
        {
            return _view.SelectedVideos;
        }

        public void Dispose()
        {
            // Desinscrever eventos
            _view.FilesSelected -= OnFilesSelected;
            _view.VideoSelectionChanged -= OnVideoSelectionChanged;
            _view.VideoMoveRequested -= OnVideoMoveRequested;
            _view.VideoRemovalRequested -= OnVideoRemovalRequested;
            _view.AuthenticationRequested -= OnAuthenticationRequested;
            _view.NextStepRequested -= OnNextStepRequested;
        }
    }
}