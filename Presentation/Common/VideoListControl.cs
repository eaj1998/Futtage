//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Windows.Forms;
//using Futtage.Core.Models;

//namespace Futtage.Presentation.Common
//{
//    public class VideoListControl : UserControl
//    {
//        private ListView _listView;
//        private ImageList _imageList;
//        private ContextMenuStrip _contextMenu;
//        private List<VideoInfo> _videos = new List<VideoInfo>();

//        public event EventHandler<VideoListEventArgs> VideoSelectionChanged;
//        public event EventHandler<VideoListEventArgs> VideoOrderChanged;
//        public event EventHandler<VideoListEventArgs> VideoRemoved;

//        public List<VideoInfo> Videos => _videos.ToList();
//        public VideoInfo SelectedVideo => GetSelectedVideo();

//        public VideoListControl()
//        {
//            InitializeComponent();
//            SetupDragDrop();
//            SetupContextMenu();
//        }

//        private void InitializeComponent()
//        {
//            _imageList = new ImageList
//            {
//                ImageSize = new Size(64, 48),
//                ColorDepth = ColorDepth.Depth32Bit
//            };

//            _listView = new ListView
//            {
//                Dock = DockStyle.Fill,
//                View = View.Details,
//                FullRowSelect = true,
//                GridLines = true,
//                MultiSelect = false,
//                AllowDrop = true,
//                LargeImageList = _imageList,
//                SmallImageList = _imageList
//            };

//            // Configurar colunas
//            _listView.Columns.Add("Arquivo", 200);
//            _listView.Columns.Add("Duração", 80);
//            _listView.Columns.Add("Resolução", 100);
//            _listView.Columns.Add("Tamanho", 80);
//            _listView.Columns.Add("Data Criação", 120);

//            _listView.SelectedIndexChanged += OnSelectionChanged;
//            _listView.DragEnter += OnDragEnter;
//            _listView.DragDrop += OnDragDrop;
//            _listView.ItemDrag += OnItemDrag;
//            _listView.DragOver += OnDragOver;

//            Controls.Add(_listView);
//        }

//        private void SetupContextMenu()
//        {
//            _contextMenu = new ContextMenuStrip();

//            var moveUpItem = new ToolStripMenuItem("Mover para Cima")
//            {
//                Image = Properties.Resources.ArrowUp, // Assumindo que existe
//                ShortcutKeys = Keys.Control | Keys.Up
//            };
//            moveUpItem.Click += (s, e) => MoveSelectedVideo(-1);

//            var moveDownItem = new ToolStripMenuItem("Mover para Baixo")
//            {
//                Image = Properties.Resources.ArrowDown,
//                ShortcutKeys = Keys.Control | Keys.Down
//            };
//            moveDownItem.Click += (s, e) => MoveSelectedVideo(1);

//            var removeItem = new ToolStripMenuItem("Remover")
//            {
//                Image = Properties.Resources.Delete,
//                ShortcutKeys = Keys.Delete
//            };
//            removeItem.Click += (s, e) => RemoveSelectedVideo();

//            var separator = new ToolStripSeparator();

//            var propertiesItem = new ToolStripMenuItem("Propriedades")
//            {
//                Image = Properties.Resources.Properties
//            };
//            propertiesItem.Click += (s, e) => ShowVideoProperties();

//            _contextMenu.Items.AddRange(new ToolStripItem[]
//            {
//                moveUpItem, moveDownItem, separator, removeItem, separator, propertiesItem
//            });

//            _listView.ContextMenuStrip = _contextMenu;
//        }

//        private void SetupDragDrop()
//        {
//            AllowDrop = true;
//        }

//        public void AddVideos(List<VideoInfo> videos)
//        {
//            foreach (var video in videos)
//            {
//                AddVideo(video);
//            }
//        }

//        public void AddVideo(VideoInfo video)
//        {
//            if (video == null) return;

//            _videos.Add(video);

//            var item = new ListViewItem(video.FilePath)
//            {
//                Tag = video
//            };

//            item.SubItems.Add(video.FormattedDuration);
//            item.SubItems.Add(video.Resolution);
//            item.SubItems.Add(video.FormattedFileSize);
//            item.SubItems.Add(video.CreationDate.ToString("dd/MM/yyyy HH:mm"));

//            // Adicionar thumbnail se disponível
//            if (!string.IsNullOrEmpty(video.ThumbnailPath))
//            {
//                try
//                {
//                    var thumbnail = Image.FromFile(video.ThumbnailPath);
//                    _imageList.Images.Add(video.FilePath, thumbnail);
//                    item.ImageKey = video.FilePath;
//                }
//                catch
//                {
//                    // Usar ícone padrão se falhar
//                }
//            }

//            // Colorir item baseado na validade
//            if (!video.IsValid)
//            {
//                item.BackColor = Color.FromArgb(255, 240, 240);
//                item.ForeColor = Color.Red;
//                item.ToolTipText = video.ErrorMessage;
//            }

//            _listView.Items.Add(item);
//        }

//        public void RemoveVideo(VideoInfo video)
//        {
//            var item = _listView.Items.Cast<ListViewItem>()
//                .FirstOrDefault(i => i.Tag == video);

//            if (item != null)
//            {
//                _videos.Remove(video);
//                _listView.Items.Remove(item);

//                VideoRemoved?.Invoke(this, new VideoListEventArgs(video));
//            }
//        }

//        public void ClearVideos()
//        {
//            _videos.Clear();
//            _listView.Items.Clear();
//            _imageList.Images.Clear();
//        }

//        private void MoveSelectedVideo(int direction)
//        {
//            var selectedItem = _listView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
//            if (selectedItem == null) return;

//            var video = (VideoInfo)selectedItem.Tag;
//            var currentIndex = _videos.IndexOf(video);
//            var newIndex = currentIndex + direction;

//            if (newIndex < 0 || newIndex >= _videos.Count) return;

//            // Atualizar lista
//            _videos.RemoveAt(currentIndex);
//            _videos.Insert(newIndex, video);

//            // Atualizar ListView
//            _listView.Items.RemoveAt(currentIndex);
//            _listView.Items.Insert(newIndex, selectedItem);

//            // Manter seleção
//            selectedItem.Selected = true;
//            selectedItem.EnsureVisible();

//            VideoOrderChanged?.Invoke(this, new VideoListEventArgs(video));
//        }

//        private void RemoveSelectedVideo()
//        {
//            var selectedItem = _listView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
//            if (selectedItem == null) return;

//            var video = (VideoInfo)selectedItem.Tag;
//            RemoveVideo(video);
//        }

//        private void ShowVideoProperties()
//        {
//            var selectedVideo = GetSelectedVideo();
//            if (selectedVideo == null) return;

//            using (var propertiesForm = new VideoPropertiesForm(selectedVideo))
//            {
//                propertiesForm.ShowDialog(this);
//            }
//        }

//        private VideoInfo GetSelectedVideo()
//        {
//            return _listView.SelectedItems.Cast<ListViewItem>()
//                .FirstOrDefault()?.Tag as VideoInfo;
//        }

//        private void OnSelectionChanged(object sender, EventArgs e)
//        {
//            var selectedVideo = GetSelectedVideo();
//            VideoSelectionChanged?.Invoke(this, new VideoListEventArgs(selectedVideo));
//        }

//        private void OnDragEnter(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//            {
//                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
//                if (files.Any(f => f.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)))
//                {
//                    e.Effect = DragDropEffects.Copy;
//                    return;
//                }
//            }

//            e.Effect = DragDropEffects.None;
//        }

//        private void OnDragDrop(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//            {
//                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
//                var videoFiles = files.Where(f => f.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase));

//                // Disparar evento para que o apresentador processe os arquivos
//                OnVideosDropped?.Invoke(this, new VideosDroppedEventArgs(videoFiles.ToList()));
//            }
//        }

//        private void OnItemDrag(object sender, ItemDragEventArgs e)
//        {
//            if (e.Item is ListViewItem item)
//            {
//                DoDragDrop(item, DragDropEffects.Move);
//            }
//        }

//        private void OnDragOver(object sender, DragEventArgs e)
//        {
//            // Permitir reordenação interna
//            if (e.Data.GetDataPresent(typeof(ListViewItem)))
//            {
//                e.Effect = DragDropEffects.Move;

//                // Destacar posição de inserção
//                var targetPoint = _listView.PointToClient(new Point(e.X, e.Y));
//                var targetItem = _listView.GetItemAt(targetPoint.X, targetPoint.Y);

//                // Implementar indicador visual de inserção
//            }
//        }

//        public event EventHandler<VideosDroppedEventArgs> OnVideosDropped;

//        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
//        {
//            switch (keyData)
//            {
//                case Keys.Delete:
//                    RemoveSelectedVideo();
//                    return true;

//                case Keys.Control | Keys.Up:
//                    MoveSelectedVideo(-1);
//                    return true;

//                case Keys.Control | Keys.Down:
//                    MoveSelectedVideo(1);
//                    return true;
//            }

//            return base.ProcessCmdKey(ref msg, keyData);
//        }
//    }

//    public class VideoListEventArgs : EventArgs
//    {
//        public VideoInfo Video { get; }

//        public VideoListEventArgs(VideoInfo video)
//        {
//            Video = video;
//        }
//    }

//    public class VideosDroppedEventArgs : EventArgs
//    {
//        public List<string> FilePaths { get; }

//        public VideosDroppedEventArgs(List<string> filePaths)
//        {
//            FilePaths = filePaths;
//        }
//    }
//}