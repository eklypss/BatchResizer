﻿using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using NLog;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace BatchResizer.Service
{
    public class ResizeService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Resizes images in specified folder to specified <see cref="Size"/> and <see cref="ISupportedImageFormat"/>.
        /// </summary>
        /// <param name="folderPath">Path of the folder</param>
        /// <param name="size">Target size of the images</param>
        /// <param name="imageFormat">Target format of the images</param>
        public void ResizeImages(string folderPath, Size size, ISupportedImageFormat imageFormat)
        {
            _logger.Debug($"Trying to resize files in {folderPath} to size: {size.Width}x{size.Height} and save as format: {imageFormat.DefaultExtension}");
            using (ImageFactory imageFactory = new ImageFactory(true, false))
            {
                foreach (var image in Directory.GetFiles(folderPath))
                {
                    if (IsImageFile(image))
                    {
                        _logger.Debug($"Resizing: {image}");
                        var savePath = Path.Combine(folderPath, "Resized", Path.GetFileName(image) + "." + imageFormat.DefaultExtension);
                        imageFactory.Load(image).Resize(size).Format(imageFormat).Save(savePath);
                    }
                    else _logger.Debug($"{image} is not a valid image file.");
                }
            }
            Process.Start(Path.Combine(folderPath, "Resized"));
        }

        /// <summary>
        /// Checks whether the file in filePath is a valid image file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns></returns>
        private bool IsImageFile(string filePath)
        {
            _logger.Debug($"Checking whether {filePath} is a valid image file.");
            if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg") || filePath.EndsWith(".gif") || filePath.EndsWith(".tiff")) return true;
            else return false;
        }
    }
}