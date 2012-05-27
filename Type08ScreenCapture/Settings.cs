using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Type08ScreenCapture
{
    using System.Xml.Serialization;
    using System.IO;

    public class Settings
    {
        public string Location { get; set; }
        public string Prefix { get; set; }
        public string Extention { get; set; }
        public bool BaloonEnabled { get; set; }
        public bool SoundEnabled { get; set; }
        public bool IncludeCaret { get; set; }

        public Settings()
        {
            Location = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            Prefix = "スクリーンショット";
            Extention = ".png";
            BaloonEnabled = false;
            SoundEnabled = false;
            IncludeCaret = false;
        }

        static private XmlSerializer serializer = new XmlSerializer(typeof(Settings));
        static private string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "type08.settings");

        public static Settings FromFile()
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    var value = serializer.Deserialize(stream) as Settings;

                    if (value == null)
                        throw new Exception("Failure in deserializing.");
                    else
                        return value;
                }
            }
            catch
            {
                return new Settings();
            }
        }

        public void ToFile()
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(stream, this);
                }
            }
            catch
            {
                /* Do Nothing */
            }
        }
    }
}
