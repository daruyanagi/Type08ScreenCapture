using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using System.IO;
using System.Windows.Input;
using WinForms = System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Media;

namespace Type08ScreenCapture
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private HotKey mHotKey = null;
        private WinForms.NotifyIcon mNotifyIcon = null;
        private WinForms.ContextMenuStrip mContextMenuStrip = null;
        private Settings settings = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // プロパティの初期化
            settings = Settings.FromFile();

            // ホットキーを登録
            mHotKey = new HotKey(Key.PrintScreen, KeyModifier.Win, (_) => { Capture(); });

            // 通知アイコンのコンテクストメニューを作成
            mContextMenuStrip = new WinForms.ContextMenuStrip();

            /* [キャプチャー]メニュー */ 
            {
                var item = new WinForms.ToolStripMenuItem("キャプチャー");
                item.ShortcutKeyDisplayString = "Alt + Windows";
                item.Click += (_sender, _e) =>
                {
                    Capture();
                };
                mContextMenuStrip.Items.Add(item);
            }

            mContextMenuStrip.Items.Add(new WinForms.ToolStripSeparator());

            /* [バルーンで通知]メニュー */
            {
                var item = new WinForms.ToolStripMenuItem("バルーンで通知");
                item.Name = "menuItemNotificationBaloonEnabled";
                item.Click += (_sender, _e) =>
                {
                    settings.BaloonEnabled = !settings.BaloonEnabled;
                };
                mContextMenuStrip.Items.Add(item);
            }

            /* [サウンドで通知]メニュー */
            {
                var item = new WinForms.ToolStripMenuItem("サウンドで通知");
                item.Name = "menuItemNotificationSoundEnabled";
                item.Click += (_sender, _e) =>
                {
                    settings.SoundEnabled = !settings.SoundEnabled;
                };
                mContextMenuStrip.Items.Add(item);
            }

            /* [キャプチャーにカーソルを含める]メニュー */
            {
                var item = new WinForms.ToolStripMenuItem("キャプチャーにカーソルを含める");
                item.Name = "menuItemIncludeCaret";
                item.Click += (_sender, _e) =>
                {
                    settings.IncludeCaret = !settings.IncludeCaret;
                };
                mContextMenuStrip.Items.Add(item);
            }

            mContextMenuStrip.Items.Add(new WinForms.ToolStripSeparator());

            /* [保存フォルダを開く]メニュー */
            {
                var item = new WinForms.ToolStripMenuItem("保存フォルダを開く");
                item.Click += (_sender, _e) =>
                {
                    Process.Start(settings.Location);
                };
                mContextMenuStrip.Items.Add(item);
            }

            /* [バージョン情報（&A）]メニュー */
            {
                var item = new WinForms.ToolStripMenuItem("バージョン情報（&A）");
                item.Click += (_sender, _e) =>
                {
                    MainWindow.Visibility = Visibility.Visible;
                };
                mContextMenuStrip.Items.Add(item);
            }

            mContextMenuStrip.Items.Add(new WinForms.ToolStripSeparator());

            /* [閉じる（&X）]メニュー */
            {
                var item = new WinForms.ToolStripMenuItem("閉じる（&X）");
                item.Click += (_sender, _e) =>
                {
                    Application.Current.Shutdown();
                };
                mContextMenuStrip.Items.Add(item);
            }

            /* メニューと設定の同期 */
            mContextMenuStrip.VisibleChanged += (_sender, _e) =>
            {
                Func<string, WinForms.ToolStripMenuItem> MenuItem = (_) =>
                    {
                        return mContextMenuStrip.Items.Find(_, true)[0]
                            as WinForms.ToolStripMenuItem;
                    };

                MenuItem("menuItemNotificationBaloonEnabled").Checked = settings.BaloonEnabled;
                MenuItem("menuItemNotificationSoundEnabled").Checked = settings.SoundEnabled;
                MenuItem("menuItemIncludeCaret").Checked = settings.IncludeCaret;
            };

            // 通知アイコンを作成
            mNotifyIcon = new WinForms.NotifyIcon()
            {
                Text = WinForms.Application.ProductName,
                BalloonTipTitle = WinForms.Application.ProductName,
                BalloonTipIcon = WinForms.ToolTipIcon.Info,
                Icon = Icon.ExtractAssociatedIcon(Environment.GetCommandLineArgs()[0]),
                ContextMenuStrip = mContextMenuStrip,
                Visible = true,
            };

            mNotifyIcon.DoubleClick += (_sender, _e) => { Capture(); };
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            settings.ToFile();

            if (mHotKey != null) mHotKey.Dispose();
            if (mNotifyIcon != null) mNotifyIcon.Dispose();
            if (mContextMenuStrip != null) mContextMenuStrip.Dispose();
        }

        internal string GetFileNameToSave(string location, string prefix, string extension)
        {
            var filename = Path.Combine(location, prefix + extension);
            var i = 0;

            while (File.Exists(filename))
            {
                filename = Path.Combine(location,
                    string.Format("{0} ({1}){2}", prefix, ++i, extension));
            }

            return filename;
        }

        public const int AW_HIDE = 0x10000;
        public const int AW_ACTIVATE = 0x20000;
        public const int AW_SLIDE = 0x40000;
        public const int AW_BLEND = 0x80000;
        public const int AW_HOR_POSITIVE = 0x00000001;
        public const int AW_HOR_NEGATIVE = 0x00000002;
        public const int AW_VER_POSITIVE = 0x00000004;
        public const int AW_VER_NEGATIVE = 0x00000008;
        public const int AW_CENTER = 0x00000010;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);

        internal void Capture()
        {
            using (var bitmap = settings.IncludeCaret
                ? ScreenSupport.CaptureDesktopWithCursor()
                : ScreenSupport.CaptureDesktop())
            {
                var extension = settings.Extention.ToLower();
                var filename = GetFileNameToSave(settings.Location, settings.Prefix, extension);

                switch (extension)
                {
                    case ".png":
                        bitmap.Save(filename, ImageFormat.Png);
                        break;
                    case ".jpg":
                        bitmap.Save(filename, ImageFormat.Jpeg);
                        break;
                    case ".gif":
                        bitmap.Save(filename, ImageFormat.Gif);
                        break;
                    case ".bmp":
                        bitmap.Save(filename, ImageFormat.Bmp);
                        break;
                    default:
                        throw new Exception(string.Format("{0} is not supported.", extension));
                }

                if (settings.BaloonEnabled)
                {
                    mNotifyIcon.BalloonTipText = string.Format("{0} を保存しました。", filename);
                    mNotifyIcon.ShowBalloonTip(1000);
                }
            }

            using (var form = new System.Windows.Forms.Form()
            {
                BackColor = Color.Black,
                ShowInTaskbar = false,
                TopMost = true,
                // WindowState = System.Windows.Forms.FormWindowState.Maximized, <- 諸悪の根源！
                Left = 0,
                Top = 0,
                Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None,
            })
            {
                int result = 0;
                const int interval = 200;

                result = AnimateWindow(form.Handle, interval, AW_BLEND | AW_ACTIVATE);
                if (result != 0) Debug.WriteLine(Marshal.GetLastWin32Error());
                form.Show();

                if (settings.SoundEnabled && File.Exists("PrintScreen.wav"))
                    new SoundPlayer("PrintScreen.wav").Play();

                result = AnimateWindow(form.Handle, interval, AW_BLEND | AW_HIDE);
                if (result != 0) Debug.WriteLine(Marshal.GetLastWin32Error());
                form.Hide();
            }
        }
    }
}
