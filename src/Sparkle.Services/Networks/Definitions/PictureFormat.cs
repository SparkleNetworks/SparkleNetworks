
namespace Sparkle.Services.Networks.Definitions
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PictureFormat
    {
        public string FileNameFormat { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public PictureStretchMode StretchMode { get; set; }
        public PictureClipOrigin ClipOrigin { get; set; }
        public ImageFormat ImageFormat { get; set; }
        public ImageQuality ImageQuality { get; set; }
        public Func<object, string[]> FilenameMaker { get; set; }

        public string Name { get; set; }

        public PictureFormat Clone()
        {
            return new PictureFormat
            {
                Name = this.Name,
                FileNameFormat = this.FileNameFormat,
                Width = this.Width,
                Height = this.Height,
                StretchMode = this.StretchMode,
                ClipOrigin = this.ClipOrigin,
                ImageFormat = this.ImageFormat,
                ImageQuality = this.ImageQuality,
            };
        }
    }

    public enum PictureStretchMode
    {
        /// <summary>
        /// The content preserves its original size.
        /// </summary>
        None = 0,

        /// <summary>
        /// The content is resized to fill the destination dimensions.
        /// The aspect ratio is not preserved.
        /// </summary>
        Fill,

        /// <summary>
        /// The content is resized to fit in the destination dimensions while it preserves its native aspect ratio.
        /// </summary>
        Uniform,

        /// <summary>
        /// The content is resized to fill the destination dimensions while it preserves
        /// its native aspect ratio. If the aspect ratio of the destination rectangle
        /// differs from the source, the source content is clipped to fit in the destination
        /// dimensions.
        /// </summary>
        UniformToFill,
    }

    public enum PictureClipOrigin
    {
        TopLeft = 0,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        Center,
    }

    public enum ImageFormat
    {
        Unspecified,
        Bitmap,
        JPEG,
        PNG,
    }

    public enum ImageQuality
    {
        Unspecified,

        /// <summary>
        /// Avoid any image degradation.
        /// </summary>
        Lossless,

        /// <summary>
        /// Allow invisible image degradations.
        /// </summary>
        High,

        /// <summary>
        /// Allow small visible degradations.
        /// </summary>
        Medium,
    }
}
