using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using System.IO;
using System.Windows.Input;
using WinForms = System.Windows.Forms;

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

        public string Location = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public string Prefix = "スクリーンショット";
        public string Extention = ".bmp";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mContextMenuStrip = new WinForms.ContextMenuStrip();
            {
                WinForms.ToolStripItem item;

                item = new WinForms.ToolStripMenuItem("バージョン情報（&A）");
                item.Click += (_sender, _e) =>
                {
                    MainWindow.Visibility = Visibility.Visible;
                };
                mContextMenuStrip.Items.Add(item);

                item = new WinForms.ToolStripSeparator();
                mContextMenuStrip.Items.Add(item);

                item = new WinForms.ToolStripMenuItem("閉じる（&X）");
                item.Click += (_sender, _e) =>
                {
                    Application.Current.Shutdown();
                };
                mContextMenuStrip.Items.Add(item);
            }

            mNotifyIcon = new WinForms.NotifyIcon()
            {
                Text = WinForms.Application.ProductName,
                BalloonTipTitle = WinForms.Application.ProductName,
                BalloonTipIcon = WinForms.ToolTipIcon.Info,
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Environment.GetCommandLineArgs()[0]),
                ContextMenuStrip = mContextMenuStrip,
                Visible = true,
            };

            mHotKey = new HotKey(
                Key.PrintScreen, KeyModifier.Win,
                (_) =>
                {
                    using (var bitmap = ScreenSupport.CaptureDesktopWithCursor())
                    {
                        var filename = GetFileNameToSave(Location, Prefix, Extention);
                        bitmap.Save(filename);

                        mNotifyIcon.BalloonTipText = string.Format("{0} を保存しました。", filename);
                        mNotifyIcon.ShowBalloonTip(1000);
                    }
                }
            );
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (mHotKey != null) mHotKey.Dispose();
            if (mNotifyIcon != null) mNotifyIcon.Dispose();
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
    }
}
