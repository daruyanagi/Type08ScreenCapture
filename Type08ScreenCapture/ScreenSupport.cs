using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Drawing; /* System.Drawing.dll */

namespace Type08ScreenCapture
{
    class ScreenSupport
    {
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        static Bitmap CaptureDesktop()
        {
            SIZE size;
            IntPtr hBitmap;
            IntPtr hDC = Win32Api.GetDC(Win32Api.GetDesktopWindow());
            IntPtr hMemDC = Win32Api.CreateCompatibleDC(hDC);

            size.cx = Win32Api.GetSystemMetrics(Win32Api.SM_CXSCREEN);
            size.cy = Win32Api.GetSystemMetrics(Win32Api.SM_CYSCREEN);

            hBitmap = Win32Api.CreateCompatibleBitmap(hDC, size.cx, size.cy);

            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = (IntPtr)Win32Api.SelectObject(hMemDC, hBitmap);

                Win32Api.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0, Win32Api.SRCCOPY);

                Win32Api.SelectObject(hMemDC, hOld);
                Win32Api.DeleteDC(hMemDC);
                Win32Api.ReleaseDC(Win32Api.GetDesktopWindow(), hDC);
                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                Win32Api.DeleteObject(hBitmap);
                GC.Collect();
                return bmp;
            }

            return null;
        }

        static Bitmap CaptureCursor(ref int x, ref int y)
        {
            Bitmap bmp;
            IntPtr hicon;
            Win32Api.CURSORINFO ci = new Win32Api.CURSORINFO();
            Win32Api.ICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);

            if (Win32Api.GetCursorInfo(out ci))
            {
                if (ci.flags == Win32Api.CURSOR_SHOWING)
                {
                    hicon = Win32Api.CopyIcon(ci.hCursor);
                    if (Win32Api.GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.x - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.y - ((int)icInfo.yHotspot);
                        Icon ic = Icon.FromHandle(hicon);
                        bmp = ic.ToBitmap();

                        return bmp;
                    }
                }
            }
            return null;
        }

        public static Bitmap CaptureDesktopWithCursor()
        {
            int cursorX = 0;
            int cursorY = 0;
            Bitmap desktopBMP;
            Bitmap cursorBMP;
            Bitmap finalBMP;
            Graphics g;
            Rectangle r;

            desktopBMP = CaptureDesktop();
            cursorBMP = CaptureCursor(ref cursorX, ref cursorY);

            if (desktopBMP != null)
            {
                if (cursorBMP != null)
                {
                    r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);
                    g = Graphics.FromImage(desktopBMP);
                    g.DrawImage(cursorBMP, r);
                    g.Flush();

                    return desktopBMP;
                }
                else
                    return desktopBMP;
            }

            return null;
        }

        public static class Win32Api
        {

            #region Class Variables

            public const int SRCCOPY = 13369376;

            public const int SM_CXSCREEN = 0;
            public const int SM_CYSCREEN = 1;

            public const Int32 CURSOR_SHOWING = 0x00000001;

            [StructLayout(LayoutKind.Sequential)]
            public struct ICONINFO
            {
                public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies 
                public Int32 xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot 
                public Int32 yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot 
                public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
                public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public Int32 x;
                public Int32 y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct CURSORINFO
            {
                public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
                public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
                public IntPtr hCursor;          // Handle to the cursor. 
                public POINT ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
            }

            #endregion

            #region user32.dll Functions

            [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll", EntryPoint = "GetDC")]
            public static extern IntPtr GetDC(IntPtr ptr);

            [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
            public static extern int GetSystemMetrics(int abc);

            [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
            public static extern IntPtr GetWindowDC(Int32 ptr);

            [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);


            [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
            public static extern bool GetCursorInfo(out CURSORINFO pci);

            [DllImport("user32.dll", EntryPoint = "CopyIcon")]
            public static extern IntPtr CopyIcon(IntPtr hIcon);

            [DllImport("user32.dll", EntryPoint = "GetIconInfo")]
            public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

            #endregion

            #region gdi32.dll Functions

            [DllImport("gdi32.dll", EntryPoint = "CreateDC")]
            public static extern IntPtr CreateDC(
                IntPtr lpszDriver, string lpszDevice, IntPtr lpszOutput, IntPtr lpInitData);

            [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
            public static extern IntPtr DeleteDC(IntPtr hDc);

            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            public static extern IntPtr DeleteObject(IntPtr hDc);

            [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
            public static extern bool BitBlt(
                IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest,
                IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

            [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

            [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

            [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

            #endregion    
        }
    }
}
