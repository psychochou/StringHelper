using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace Shared
{
    public static class Colors
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);

        public static (int, int, int) GetColorAt(int x, int y)
        {
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            int a = (int)GetPixel(dc, x, y);
            ReleaseDC(desk, dc);
            int r = (a >> 0) & 0xff;
            int g = (a >> 8) & 0xff;
            int b = (a >> 16) & 0xff;

            return (r, g, b);
        }
        public static (int, int, int) GetColorAt()
        {

            var p = GetCursorPosition();
            return GetColorAt(p.X, p.Y);
        }
        public static string ConvertRGBStringToHex(string value)
        {

            var matches = Regex.Matches(value, "[0-9]{1,3}");

            var r = "";

            var count = 0;

            foreach (Match item in matches)

            {

                r += int.Parse(item.Value).ToString("X2");
                count++;
                if (count % 3 == 0)
                {
                    r += Environment.NewLine;
                    count = 0;
                }
            }
            return r;
        }
    }
}
