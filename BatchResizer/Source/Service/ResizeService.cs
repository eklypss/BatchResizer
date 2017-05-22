﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using NLog;

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
        public void ResizeImagesToSize(string folderPath, Size size, ISupportedImageFormat imageFormat)
        {
            _logger.Debug($"Trying to resize files in {folderPath} to size: {size.Width}x{size.Height} and save as format: {imageFormat.DefaultExtension}");
            using (ImageFactory imageFactory = new ImageFactory(true, false))
            {
                foreach (var image in Directory.GetFiles(folderPath))
                {
                    if (IsImageFile(image))
                    {
                        var savePath = string.Empty;
                        _logger.Debug($"Resizing: {image}");
                        // bmp acts as the original file extension of the image, not as .bmp
                        if (imageFormat.DefaultExtension == "bmp")
                        {
                            savePath = Path.Combine(folderPath, "Resized", Path.GetFileName(image));
                            imageFactory.Load(image).Resize(size).Save(savePath);
                        }
                        else
                        {
                            savePath = Path.Combine(folderPath, "Resized", Path.GetFileName(image) + "." + imageFormat.DefaultExtension);
                            imageFactory.Load(image).Resize(size).Format(imageFormat).Save(savePath);
                        }
                    }
                    else _logger.Debug($"{image} is not a valid image file.");
                }
            }
            Process.Start(Path.Combine(folderPath, "Resized"));
        }

        public void ResizeImagesToPercentage(string folderPath, float resizePercentage, ISupportedImageFormat imageFormat)
        {
            using (ImageFactory imageFactory = new ImageFactory(true, false))
            {
                foreach (var image in Directory.GetFiles(folderPath))
                {
                    if (IsImageFile(image))
                    {
                        var loadedImage = imageFactory.Load(image);
                        _logger.Debug($"Resizing: {loadedImage.ImagePath}");
                        _logger.Debug($"Original size: {loadedImage.Image.Width}x{loadedImage.Image.Height}.");
                        var savePath = Path.Combine(folderPath, "Resized", Path.GetFileName(image) + "." + imageFormat.DefaultExtension);
                        double percentage = resizePercentage / 100;
                        double height = percentage * loadedImage.Image.Height;
                        double width = percentage * loadedImage.Image.Width;
                        _logger.Debug($"new size: {width}x{height}");
                        Size percentageSize = new Size(Convert.ToInt32(height), Convert.ToInt32(width));
                        loadedImage.Resize(percentageSize).Format(imageFormat).Save(savePath);
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