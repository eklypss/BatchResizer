using System.Drawing;
using System.Windows.Forms;
using BatchResizer.Command;
using BatchResizer.Enum;
using BatchResizer.Service;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using NLog;
using Prism.Mvvm;

namespace BatchResizer.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private string _selectedFolder;
        private int _imageTargetHeight;
        private int _imageTargetWidth;
        private int _imageResizePercentage = 100;
        private int _selectedImageFormatIndex = -1;
        private int _selectedResizeModeIndex = -1;
        private ResizeService _resizeService;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ISupportedImageFormat ImageFormat { get; set; }
        public ResizeModes ResizeMode { get; set; } = ResizeModes.NotSet;
        public RelayCommand ResizeCommand { get; set; }
        public RelayCommand BrowseCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="MainViewModel"/> class and initializes <see
        /// cref="ResizeCommand"/>, <see cref="BrowseCommand"/> and <see cref="ResizeService"/>.
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
                _logger.Debug($"SelectedFolder was changed to: {value}.");
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
                _logger.Debug($"ImageTargetHeight was changed to: {value}.");
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
                _logger.Debug($"ImageTargetWidth was changed to: {value}.");
            }
        }

        /// <summary>
        /// Property/notifier for _imageResizePercentage.
        /// </summary>
        public int ImageResizePercentage
        {
            get { return _imageResizePercentage; }
            set
            {
                SetProperty(ref _imageResizePercentage, value, "ImageResizePercentage");
                RaisePropertyChanged("CanResize");
                _logger.Debug($"ImageResizePercentage was changed to: {value}%.");
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
                _logger.Debug($"SelectedImageFormatIndex was changed to: {value}.");
            }
        }

        /// <summary>
        /// Property/notifier for <see cref="_selectedResizeModeIndex"/>
        /// </summary>
        public int SelectedResizeModeIndex
        {
            get { return _selectedResizeModeIndex; }
            set
            {
                SetProperty(ref _selectedResizeModeIndex, value, "SelectedResizeModeIndex");
                ResizeMode = (ResizeModes)value;
                RaisePropertyChanged("IsSpecifiedResizeModeSelected");
                RaisePropertyChanged("IsPercentageResizeModeSelected");
                _logger.Debug($"SelectedResizeModeIndex was changed to: {value}.");
            }
        }

        /// <summary>
        /// Boolean that determines whether the <see cref="ResizeCommand"/> can be executed or not,
        /// depending if <see cref="SelectedFolder"/> is a valid path.
        /// </summary>
        public bool CanResize
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SelectedFolder) || SelectedImageFormatIndex == -1 || ResizeMode == ResizeModes.NotSet) return false;
                switch (ResizeMode)
                {
                    case ResizeModes.Specified:
                    {
                        if (ImageTargetHeight > Settings.MAX_IMAGE_HEIGHT || ImageTargetWidth > Settings.MAX_IMAGE_WIDTH || ImageTargetHeight < Settings.MIN_IMAGE_HEIGHT || ImageTargetWidth < Settings.MIN_IMAGE_WIDTH) return false;
                        return true;
                    }
                    case ResizeModes.Percentage:
                    {
                        if (ImageResizePercentage > Settings.MAX_RESIZE_PERCENTAGE || ImageResizePercentage < Settings.MIN_RESIZE_PERCENTAGE) return false;
                        return true;
                    }
                    default:
                    {
                        _logger.Warn("Could not determine ResizeMode, returning false.");
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Boolean that determines whether <see cref="ResizeMode"/> equals to <see cref="ResizeModes.Specified"/>.
        /// </summary>
        public bool IsSpecifiedResizeModeSelected
        {
            get { return (ResizeMode == ResizeModes.Specified) ? true : false; }
        }

        /// <summary>
        /// Boolean that determines whether <see cref="ResizeMode"/> equals to <see cref="ResizeModes.Percentage"/>.
        /// </summary>
        public bool IsPercentageResizeModeSelected
        {
            get { return (ResizeMode == ResizeModes.Percentage) ? true : false; }
        }

        /// <summary>
        /// Performs the resizing of the images.
        /// </summary>
        private void Resize()
        {
            _logger.Debug("ResizeCommand was called.");
            try
            {
                switch (ResizeMode)
                {
                    case ResizeModes.Specified:
                    {
                        _resizeService.ResizeImagesToSize(SelectedFolder, new Size(ImageTargetHeight, ImageTargetWidth), ImageFormat);
                        break;
                    }
                    case ResizeModes.Percentage:
                    {
                        _resizeService.ResizeImagesToPercentage(SelectedFolder, ImageResizePercentage, ImageFormat);
                        break;
                    }
                }
            }
            catch
            {
                _logger.Error($"Error occured during resizing.");
            }
            finally
            {
                _logger.Debug("ResizeCommand finished.");
            }
        }

        /// <summary>
        /// Creates and displays the <see cref="FolderBrowserDialog"/> for the <see
        /// cref="BrowseCommand"/> used by the Browse button.
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
                    _logger.Debug($"Specified folder was selected: {selectFolderDialog.SelectedPath}.");
                }
                else
                {
                    _logger.Warn($"Unexpected DialogResult received: {dialogResult.ToString()}.");
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
            _logger.Debug($"Getting ImageFormat for index: {index}.");
            switch (index)
            {
                case 0: return new BitmapFormat();
                case 1: return new GifFormat() { AnimationProcessMode = AnimationProcessMode.First };
                case 2: return new JpegFormat() { Quality = 100 };
                case 3: return new PngFormat();
                case 4: return new TiffFormat();
                default: return new PngFormat();
            }
        }
    }
}