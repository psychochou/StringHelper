using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace Shared
{
    class Images
    {

        public static void ConvertToIco(string fileName)
        {
            Image image = Image.FromFile(fileName);
            Image newImage = image.GetThumbnailImage(32, 32, null, new IntPtr());

            using (var bitmap = new Bitmap(newImage))
            {
                bitmap.SetResolution(72, 72);
                using (var ico = Icon.FromHandle(bitmap.GetHicon()))
                {
                    var targetFileName = fileName.ChangeExtension(".ico");

                    using (var fs = new FileStream(targetFileName, FileMode.OpenOrCreate))
                    {
                        ico.Save(fs);
                    }
                }
            }
        }

        private static Font FindBestFitFont(Graphics g, String text, Font font, Size proposedSize)
        {
            // Compute actual size, shrink if needed
            while (true)
            {
                SizeF size = g.MeasureString(text, font);

                // It fits, back out
                if (size.Height <= proposedSize.Height &&
                     size.Width <= proposedSize.Width) { return font; }

                // Try a smaller font (90% of old size)
                Font oldFont = font;
                font = new Font(font.Name, (float)(font.Size * .9), font.Style);
                oldFont.Dispose();
            }
        }
        public static void Generate(string letter, string fileName)
        {
            int size = 48;
            var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);

            var g = Graphics.FromImage(bitmap);
            g.Clear(Color.Red);

             
            Font font = new Font("Arial", 24, FontStyle.Regular);

            font = FindBestFitFont(g, letter, font, new Size(size, size));
            SizeF sizeF = g.MeasureString(letter.ToString(), font);
            g.DrawString(letter, font, Brushes.White, (size - sizeF.Width) / 2, (size - sizeF.Height) / 2);



            using (var ico = Icon.FromHandle(bitmap.GetHicon()))
            {
                var targetFileName = fileName.ChangeExtension(".ico");

                using (var fs = new FileStream(targetFileName, FileMode.OpenOrCreate))
                {
                    ico.Save(fs);
                }
            }
        }
    }

}
