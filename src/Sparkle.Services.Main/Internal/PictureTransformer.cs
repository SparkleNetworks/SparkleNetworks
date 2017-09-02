
namespace Sparkle.Services.Main.Internal
{
    using Sparkle.Services.Networks.Definitions;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Makes thumbnails of pictures.
    /// </summary>
    public class PictureTransformer
    {
        public MemoryStream FormatPicture(PictureFormat format, Stream stream)
        {
            Bitmap source;
            stream.Seek(0L, SeekOrigin.Begin);
            try
            {
                source = new Bitmap(stream);
            }
            catch (ArgumentException ex)
            {
                throw new FormatException("The stream does not contain a picture", ex);
            }

            return this.FormatPicture(format, source);
        }

        public MemoryStream FormatPicture(PictureFormat format, Bitmap source)
        {
            float width = format.Width, height = format.Height;
            float newWidth, newHeight;

            float widthPercent = width / source.Width;
            float heigthPercent = height / source.Height;

            if (format.StretchMode == PictureStretchMode.UniformToFill)
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
            else if (format.StretchMode == PictureStretchMode.Uniform)
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
            else
            {
                throw new NotSupportedException("Formating picture with StretchMode " + format.StretchMode + " is not supported");
            }

            ////source = GetNewImage((int)newWidth, (int)newHeight, source);
            source = new Bitmap(source.GetThumbnailImage((int)newWidth, (int)newHeight, null, IntPtr.Zero));

            Bitmap dest = new Bitmap(format.Width, format.Height);

            int marge = (source.Width - format.Width) / 2;

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
            this.FillDestWithSource(source, dest, format.StretchMode);

            var imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            var export = new MemoryStream();
            dest.Save(export, GetImageCodecInfo(format), GetImageCodecParams(format));
            export.Seek(0L, SeekOrigin.Begin);
            return export;
        }

        private void FillDestWithSource(Bitmap source, Bitmap dest, PictureStretchMode mode)
        {
            var startx = (int)Math.Max(Math.Abs((dest.Width - source.Width) / 2.0), 0);
            var starty = (int)Math.Max(Math.Abs((dest.Height - source.Height) / 2.0), 0);

            if (mode == PictureStretchMode.Uniform)
            {
                int sx = startx;
                for (int x = 0; x < source.Width; x++)
                {
                    int sy = starty;
                    for (int y = 0; y < source.Height; y++)
                    {
                        if (sx < dest.Width && sy < dest.Height)
                        {
                            dest.SetPixel(sx, sy, source.GetPixel(x, y));
                        }

                        sy++;
                    }

                    sx++;
                }
            }
            else if (mode == PictureStretchMode.UniformToFill)
            {
                int sx = startx;
                for (int x = 0; x < dest.Width; x++)
                {
                    int sy = starty;
                    for (int y = 0; y < dest.Height; y++)
                    {
                        if (sx < source.Width && sy < source.Height)
                        {
                            dest.SetPixel(x, y, source.GetPixel(sx, sy));
                        }

                        sy++;
                    }

                    sx++;
                }
            }
            else
                throw new NotSupportedException("Formating picture with StretchMode " + mode + " is not supported");
        }

        private EncoderParameters GetImageCodecParams(PictureFormat format)
        {
            long quality;
            switch (format.ImageQuality)
            {
                case ImageQuality.Lossless:
                    quality = 100L;
                    break;
                case ImageQuality.High:
                    quality = 92L;
                    break;
                case ImageQuality.Medium:
                    quality = 85L;
                    break;
                default:
                    throw new NotSupportedException("ImageQuality '" + format.ImageQuality + "' does not have a parameter value");
            }

            var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            return parameters;
        }

        private ImageCodecInfo GetImageCodecInfo(PictureFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string mime;
            switch (format.ImageFormat)
            {
                case Sparkle.Services.Networks.Definitions.ImageFormat.Bitmap:
                    mime = "image/bitmap";
                    break;
                case Sparkle.Services.Networks.Definitions.ImageFormat.JPEG:
                    mime = "image/jpeg";
                    break;
                case Sparkle.Services.Networks.Definitions.ImageFormat.PNG:
                    mime = "image/png";
                    break;
                default:
                    throw new NotSupportedException("ImageFormat '" + format.ImageFormat + "' does not have any encoder");
            }

            for (int i = 0; i < codecs.Length; i++)
            {
                var codec = codecs[i];
                if (codec.MimeType == mime)
                {
                    return codec;
                }
            }

            throw new NotSupportedException("ImageFormat '" + format.ImageFormat + "' does not have any encoder");
        }
    }
}
