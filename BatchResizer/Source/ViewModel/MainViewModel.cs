using BatchResizer.Command;
using BatchResizer.Service;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using NLog;
using Prism.Mvvm;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BatchResizer.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private string _selectedFolder;
        private int _imageTargetHeight;
        private int _imageTargetWidth;
        private int _selectedImageFormatIndex = -1;
        private ResizeService _resizeService;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ISupportedImageFormat ImageFormat { get; set; }
        public RelayCommand ResizeCommand { get; set; }
        public RelayCommand BrowseCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="MainViewModel"/> class and initializes <see cref="ResizeCommand"/>, <see cref="BrowseCommand"/> and <see cref="ResizeService"/>.
        /// </summary>
        public MainViewModel()
        {
            ResizeCommand = new RelayCommand(Resize);
            BrowseCommand = new RelayCommand(Browse);
            _resizeService = new ResizeService();
            _logger.Debug("Constructor called, commands and ResizeService were created.");
        }

        /// <summary>
        /// Property/notifier for <see cref="_selectedFolder"/>
        /// </summary>
        public string SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                SetProperty(ref _selectedFolder, value, "SelectedFolder");
                RaisePropertyChanged("CanResize");
                _logger.Debug("SelectedFolder was changed to: {0}", value);
            }
        }

        /// <summary>
        /// Property/notifier for <see cref="_imageTargetHeight"/>
        /// </summary>
        public int ImageTargetHeight
        {
            get { return _imageTargetHeight; }
            set
            {
                SetProperty(ref _imageTargetHeight, value, "ImageTargetHeight");
                RaisePropertyChanged("CanResize");
                _logger.Debug("ImageTargetHeight was changed to: {0}", value);
            }
        }

        /// <summary>
        /// Property/notifier for <see cref="_imageTargetWidth"/>
        /// </summary>
        public int ImageTargetWidth
        {
            get { return _imageTargetWidth; ; }
            set
            {
                SetProperty(ref _imageTargetWidth, value, "ImageTargetWidth");
                RaisePropertyChanged("CanResize");
                _logger.Debug("ImageTargetWidth was changed to: {0}", value);
            }
        }

        /// <summary>
        /// Property/notifier for <see cref="_selectedImageFormatIndex"/>
        /// </summary>
        public int SelectedImageFormatIndex
        {
            get { return _selectedImageFormatIndex; }
            set
            {
                SetProperty(ref _selectedImageFormatIndex, value, "SelectedImageFormatIndex");
                RaisePropertyChanged("CanResize");
                ImageFormat = GetImageFormatForIndex(_selectedImageFormatIndex);
                _logger.Debug("SelectedImageFormatIndex was changed to: {0}", value);
            }
        }

        /// <summary>
        /// Boolean that determines whether the <see cref="ResizeCommand"/> can be executed or not, depending if <see cref="SelectedFolder"/> is a valid path.
        /// </summary>
        public bool CanResize
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SelectedFolder) || SelectedImageFormatIndex == -1 || ImageTargetHeight <= 0 || ImageTargetWidth <= 0) return false;
                return true;
            }
        }

        /// <summary>
        /// Performs the resizing of the images.
        /// </summary>
        private void Resize()
        {
            _logger.Debug("ResizeCommand was called.");
            try
            {
                _resizeService.ResizeImages(SelectedFolder, new Size(ImageTargetHeight, ImageTargetWidth), ImageFormat);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured during resizing: {0}", ex.Message);
                _logger.Error("{0} {1}", ex.Source, ex.StackTrace);
            }
            finally
            {
                _logger.Debug("ResizeCommand finished.");
            }
        }

        /// <summary>
        /// Creates and displays the <see cref="FolderBrowserDialog"/> for the <see cref="BrowseCommand"/> used by the Browse button.
        /// </summary>
        private void Browse()
        {
            _logger.Info("BrowseCommand was called.");
            using (FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog())
            {
                var dialogResult = selectFolderDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    SelectedFolder = selectFolderDialog.SelectedPath;
                }
                else
                {
                    _logger.Warn("Unexpected DialogResult received: {0}", dialogResult.ToString());
                }
            }
        }

        /// <summary>
        /// Gets <see cref="ISupportedImageFormat"/> for the <see cref="SelectedImageFormatIndex"/>.
        /// </summary>
        /// <param name="index">The ComboBox index.</param>
        /// <returns></returns>
        private ISupportedImageFormat GetImageFormatForIndex(int index)
        {
            _logger.Debug("Getting ImageFormat for index: {0}", index);
            switch (index)
            {
                case 0: return new GifFormat() { AnimationProcessMode = AnimationProcessMode.First };
                case 1: return new JpegFormat() { Quality = 100 };
                case 2: return new PngFormat();
                case 3: return new TiffFormat();
                default: return new PngFormat();
            }
        }
    }
}