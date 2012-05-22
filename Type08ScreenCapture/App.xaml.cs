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

        public bool NotificationEnabled { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mContextMenuStrip = new WinForms.ContextMenuStrip();
            {
                WinForms.ToolStripItem item;
                NotificationEnabled = true;

                item = new WinForms.ToolStripMenuItem("デスクトップ通知をON/OFF");
                item.Name = "menuItemNotificationEnabled";
                item.Click += (_sender, _e) =>
                {
                    NotificationEnabled = !NotificationEnabled;
                };
                mContextMenuStrip.Items.Add(item);

                item = new WinForms.ToolStripMenuItem("保存フォルダを開く");
                item.Click += (_sender, _e) =>
                {
                    Process.Start(Location);
                };
                mContextMenuStrip.Items.Add(item);

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

            mContextMenuStrip.VisibleChanged += (_s, _e) =>
            {
                var item = mContextMenuStrip.Items.Find("menuItemNotificationEnabled", true)[0] as WinForms.ToolStripMenuItem;
                item.Checked = NotificationEnabled;
            };

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

                        if (NotificationEnabled)
                        {
                            mNotifyIcon.BalloonTipText = string.Format("{0} を保存しました。", filename);
                            mNotifyIcon.ShowBalloonTip(1000);
                        }
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
