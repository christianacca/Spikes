using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


namespace Eca.Commons.Drawing
{
    public static class DrawingUtils
    {
        #region Class Members

        /// <summary>
        /// This method takes in an Image Stream and sizes for width and height and returns a resized image stream keeping its aspect ratio.
        /// </summary>
        /// <remarks>Original source: http://stackoverflow.com/questions/8214562/resize-jpeg-image-to-fixed-width-while-keeping-aspect-ratio-as-it-is </remarks>
        public static Stream ResizeImage(this Stream stream, Size size)
        {
            var image = Image.FromStream(stream);
            int width = image.Width;
            int height = image.Height;
            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float percent = 0, percentWidth = 0, percentHeight = 0;
            percentWidth = (size.Width/(float) width);
            percentHeight = (size.Height/(float) height);
            int destW = 0;
            int destH = 0;

            percent = percentHeight < percentWidth ? percentHeight : percentWidth;

            destW = (int) (width*percent);
            destH = (int) (height*percent);

            var mStream = new MemoryStream();

            if (destW == 0 && destH == 0)
            {
                image.Save(mStream, ImageFormat.Jpeg);
                return mStream;
            }

            using (var bitmap = new Bitmap(destW, destH, PixelFormat.Format48bppRgb))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image,
                                       new Rectangle(destX, destY, destW, destH),
                                       new Rectangle(sourceX, sourceY, width, height),
                                       GraphicsUnit.Pixel);
                }

                bitmap.Save(mStream, ImageFormat.Jpeg);
            }

            mStream.Position = 0;
            return mStream;
        }

        #endregion
    }
}