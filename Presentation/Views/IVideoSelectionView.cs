using System;
using System.Collections.Generic;
using Futtage.Core.Models;

namespace Futtage.Presentation.Views
{
    public interface IVideoSelectionView
    {
        // Propriedades
        List<VideoInfo> SelectedVideos { get; }
        VideoInfo CurrentlySelectedVideo { get; }
        bool IsGoogleAuthenticated { get; set; }
        string AuthenticatedUserName { get; set; }
        string AuthenticatedUserEmail { get; set; }
        string AuthenticatedUserAvatar { get; set; }

        // Eventos
        event EventHandler<List<string>> FilesSelected;
        event EventHandler<VideoInfo> VideoSelectionChanged;
        event EventHandler<VideoMoveEventArgs> VideoMoveRequested;
        event EventHandler<VideoInfo> VideoRemovalRequested;
        event EventHandler AuthenticationRequested;
        event EventHandler NextStepRequested;

        // Métodos
        void AddVideos(List<VideoInfo> videos);
        void RemoveVideo(VideoInfo video);
        void UpdateVideoOrder(List<VideoInfo> newOrder);
        void ShowProgress(string message);
        void HideProgress();
        void ShowError(string message);
        void ShowSuccess(string message);
        void EnableControls(bool enabled);
        void UpdateAuthenticationStatus(bool isAuthenticated, string userName = null, string userEmail = null, string avatarUrl = null);
    }

    public class VideoMoveEventArgs : EventArgs
    {
        public VideoInfo Video { get; }
        public int Direction { get; }

        public VideoMoveEventArgs(VideoInfo video, int direction)
        {
            Video = video;
            Direction = direction;
        }
    }
}
