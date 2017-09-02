
namespace Sparkle.Helpers
{
    using Sparkle.Entities.Networks;
    using Sparkle.UI;
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Web;

    public static class FileHelper
    {
        const string EXTENTION = ".jpg";

        public static string SaveTimelinePicture(string username, Bitmap source, string userContentsDirectory, string networkName)
        {
            string name = Guid.NewGuid().ToString();
            string pathForUser = userContentsDirectory + "/Networks/" + networkName + "/Peoples/" + username + "/";

            string resizedPicturePath = CheckFilename(pathForUser, name + "l", EXTENTION);
            string originalPicturePath = CheckFilename(pathForUser, name + "o", EXTENTION);

            int oldWidth = source.Width;
            int oldHeight = source.Height;

            const float MaxLargeWidth = 1024;
            const float MaxLargeHeight = 768;
            float newWidth;
            float newHeight;
            float widthPercent = MaxLargeWidth / oldWidth;
            float heigthPercent = MaxLargeHeight / oldHeight;

            if (source.Width < source.Height)
            {
                newWidth = oldWidth * widthPercent;
                newHeight = oldHeight * widthPercent;
            }
            else
            {
                newWidth = oldWidth * heigthPercent;
                newHeight = oldHeight * heigthPercent;
            }

            // do not enlarge picture
            if (newWidth > source.Width || newHeight > source.Height)
            {
                newWidth = source.Width;
                newHeight = source.Height;
            }

            // Original
            try
            {
                source.Save(originalPicturePath);
            }
            catch (ExternalException ex)
            {
                Trace.WriteLine("FileHelper.SaveTimelinePicture: Failed to save picture to '" + originalPicturePath + "': " + ex.Message);
                throw;
            }

            // Resized
            Bitmap newImgProfile = GetNewImage((int)newWidth, (int)newHeight, source);
            source.Dispose();

            // save picture with parameters
            // How to: Set JPEG Compression Level https://msdn.microsoft.com/en-us/library/bb882583%28v=vs.110%29.aspx
            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
            newImgProfile.Save(resizedPicturePath, jpgEncoder, encoderParameters);

            return "/Content/Networks/" + networkName + "/Peoples/" + username + "/" + name + "l" + EXTENTION;
        }

        public static byte[] GetTimelinePicture(
            string userContentsDirectory,
            string networkName,
            string username,
            string pictureName,
            HttpServerUtilityBase server)
        {
            string basePathForUser = "/Networks/" + networkName + "/Peoples/" + username + "/";
            string pathForUser = userContentsDirectory + basePathForUser;
            string fileName = pictureName + "l" + EXTENTION;
            string filePath = Path.Combine(pathForUser, fileName);
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            else
            {
                pathForUser = server.MapPath(basePathForUser);
                filePath = Path.Combine(pathForUser, fileName);
                if (File.Exists(filePath))
                {
                    return File.ReadAllBytes(filePath);
                }
                else
                {
                    return null;
                }
            }
        }

        public static void CopyDefaultGroupPicture(int suggestId, int groupId)
        {
            string dir = HttpContext.Current.Server.MapPath("/Content/Networks/" + Lang.T("AppNameKey") + "/Groups/");
            EnsureDirectoryExists(dir);

            string suggestFile = HttpContext.Current.Server.MapPath("/Content/Networks/Common/Groups/" + suggestId + ".gif");
            string groupFile = dir + groupId.ToString(CultureInfo.InvariantCulture) + ".gif";
            File.Copy(suggestFile, groupFile);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Saves the resume picture.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string SaveResumePicture(string id, Bitmap source)
        {
            string name = "";
            name = id.Replace("-", "").Replace(".jpg", "");

            // Save original picture
            string fileName = name + EXTENTION;
            SaveOriginal(fileName, source);

            // check directory
            string path = HttpContext.Current.Server.MapPath("/Content/Networks/" + Lang.T("AppNameKey") + "/Resumes/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // save resume picture
            ResizeAndSavePicture(path + fileName, 170, 200, true, source, ImageFormat.Jpeg);

            return "/Content/Networks/" + Lang.T("AppNameKey") + "/Resumes/" + fileName;
        }

        private static string CheckFilename(string directory, string login, string extention)
        {
            string filename = Path.Combine(directory, login + extention);
            CreateDirectory(directory);
            DeleteFile(filename);
            return filename;
        }

        private static Bitmap GetNewImage(int width, int height, Bitmap source)
        {
            Image image = source;
            ////int NewHeight = Height;
            ////int NewWidth = Width;
            ////if (source.Width > source.Height)
            ////{
            ////    NewHeight = (Width * source.Height) / source.Width;
            ////}
            ////if (source.Width < source.Height)
            ////{
            ////    NewWidth = (Height * source.Width) / source.Height; ;
            ////}

            ////return new Bitmap(source.GetThumbnailImage(width, height, null, IntPtr.Zero));

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary> Delete the file</summary>
        /// <param name="filename">filename</param>
        private static void DeleteFile(string filename)
        {
            if (FileExist(filename)) { File.Delete(filename); }
        }

        /// <summary>Check if the file exists</summary>
        /// <param name="filename">filename</param>
        private static bool FileExist(string filename)
        {
            return File.Exists(filename);
        }

        /// <summary>
        /// Create the directory
        /// </summary>
        /// <param name="path">the path of the directory</param>
        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void SaveOriginal(string name, Bitmap source)
        {
            string directory = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["TempImageDirectory"]);

            string fileName = Path.Combine(directory, name);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(fileName))
            {
                int i = 1;
                string tempfilename = fileName.Insert(fileName.LastIndexOf(Path.GetExtension(fileName)), "(" + i + ")");
                while (File.Exists(tempfilename))
                {
                    i++;
                    tempfilename = fileName.Insert(fileName.LastIndexOf(Path.GetExtension(fileName)), "(" + i + ")");
                }

                fileName = tempfilename;
            }

            source.Save(fileName);
        }

        private static bool SavePicture(string fileName, int width, int height, Bitmap source, ImageFormat imageFormat)
        {
            try
            {
                Bitmap newImg = GetNewImage(width, height, source);
                newImg.Save(fileName, imageFormat);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool ResizeAndSavePicture(string fileName, int width, int height, bool fill, Bitmap source, ImageFormat imageFormat)
        {
            float newWidth, newHeight;

            float widthPercent = (float)width / source.Width;
            float heigthPercent = (float)height / source.Height;

            if (fill)
            {
                // size min
                if (source.Width < source.Height)
                {
                    newWidth = source.Width * widthPercent;
                    newHeight = source.Height * widthPercent;
                }
                else
                {
                    newWidth = source.Width * heigthPercent;
                    newHeight = source.Height * heigthPercent;
                }
            }
            else
            {
                // size max
                if (source.Width < source.Height)
                {
                    newWidth = source.Width * heigthPercent;
                    newHeight = source.Height * heigthPercent;
                }
                else
                {
                    newWidth = source.Width * widthPercent;
                    newHeight = source.Height * widthPercent;
                }
            }

            source = GetNewImage((int)newWidth, (int)newHeight, source);

            Bitmap dest = new Bitmap(width, height);

            int marge = (source.Width - width) / 2;

            // background
            var backColor = source.GetPixel(0, 0);
            for (int x = 0; x < dest.Width; x++)
            {
                for (int y = 0; y < dest.Height; y++)
                {
                    dest.SetPixel(x, y, backColor);
                }
            }

            // center image
            var startx = Math.Max((dest.Width - source.Width) / 2, 0);
            var starty = Math.Max((dest.Height - source.Height) / 2, 0);
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    if (x + startx < dest.Width && y + starty < dest.Height)
                    {
                        dest.SetPixel(x + startx, y + starty, source.GetPixel(x, y));
                    }
                }
            }

            return SavePicture(fileName, (int)width, (int)height, dest, imageFormat);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
    }
}
