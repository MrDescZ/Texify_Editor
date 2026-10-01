using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Textify_Editor.Classes
{
    public enum AppBackdropType
    {
        Auto = 0,
        None = 1,
        Mica = 2,
        Acrylic = 3,
        Tabbed = 4
    }

    public enum AppEolMode
    {
        Lf,
        CrLf,
        Cr
    }

    [Serializable]
    public class AppSettings
    {

        public int AccentColorArgb { get; set; } = Color.FromArgb(0, 122, 204).ToArgb();
        [XmlIgnore]
        public Color AccentColor
        {
            get => Color.FromArgb(AccentColorArgb);
            set => AccentColorArgb = value.ToArgb();
        }

        public AppBackdropType Backdrop { get; set; } = AppBackdropType.Mica;

        public bool ImmersiveDarkTitleBar { get; set; } = true;

        public bool GradientBackgroundEnabled { get; set; } = false;

        public int GradientTopColorArgb { get; set; } = Color.FromArgb(60, 20, 65).ToArgb();
        [XmlIgnore]
        public Color GradientTopColor
        {
            get => Color.FromArgb(GradientTopColorArgb);
            set => GradientTopColorArgb = value.ToArgb();
        }

        public int GradientBottomColorArgb { get; set; } = Color.FromArgb(60, 25, 70).ToArgb();
        [XmlIgnore]
        public Color GradientBottomColor
        {
            get => Color.FromArgb(GradientBottomColorArgb);
            set => GradientBottomColorArgb = value.ToArgb();
        }

        public int KeywordColorArgb { get; set; } = Color.FromArgb(97, 175, 239).ToArgb();
        [XmlIgnore]
        public Color KeywordColor
        {
            get => Color.FromArgb(KeywordColorArgb);
            set => KeywordColorArgb = value.ToArgb();
        }

        public int StringColorArgb { get; set; } = Color.FromArgb(152, 195, 121).ToArgb();
        [XmlIgnore]
        public Color StringColor
        {
            get => Color.FromArgb(StringColorArgb);
            set => StringColorArgb = value.ToArgb();
        }

        public int CommentColorArgb { get; set; } = Color.FromArgb(92, 99, 112).ToArgb();
        [XmlIgnore]
        public Color CommentColor
        {
            get => Color.FromArgb(CommentColorArgb);
            set => CommentColorArgb = value.ToArgb();
        }

        public int NumberColorArgb { get; set; } = Color.FromArgb(209, 154, 102).ToArgb();
        [XmlIgnore]
        public Color NumberColor
        {
            get => Color.FromArgb(NumberColorArgb);
            set => NumberColorArgb = value.ToArgb();
        }

        public int PreprocessorColorArgb { get; set; } = Color.FromArgb(198, 120, 221).ToArgb();
        [XmlIgnore]
        public Color PreprocessorColor
        {
            get => Color.FromArgb(PreprocessorColorArgb);
            set => PreprocessorColorArgb = value.ToArgb();
        }

        public int OperatorColorArgb { get; set; } = Color.FromArgb(86, 182, 194).ToArgb();
        [XmlIgnore]
        public Color OperatorColor
        {
            get => Color.FromArgb(OperatorColorArgb);
            set => OperatorColorArgb = value.ToArgb();
        }

        public string FontFamily { get; set; } = "Cascadia Mono";
        public int FontSize { get; set; } = 11;
        public int TabWidth { get; set; } = 4;
        public int IndentWidth { get; set; } = 4;
        public bool WordWrap { get; set; } = false;
        public bool AutoIndent { get; set; } = true;
        public bool AutoCloseBrackets { get; set; } = true;
        public bool ShowLineNumbers { get; set; } = true;
        public bool ShowWhitespace { get; set; } = false;
        public bool ShowIndentGuides { get; set; } = true;
        public int DefaultZoomPercent { get; set; } = 80;
        public AppEolMode EolMode { get; set; } = AppEolMode.Lf;
        public bool OpenFilesReadOnly { get; set; } = false;

        public bool DiscordRichPresenceEnabled { get; set; } = false;
        public int MaxRecentFiles { get; set; } = 10;


        private static string SettingsDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Textify Editor");

        private static string SettingsFilePath => Path.Combine(SettingsDirectory, "settings.xml");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var serializer = new XmlSerializer(typeof(AppSettings));
                    using var stream = File.OpenRead(SettingsFilePath);
                    if (serializer.Deserialize(stream) is AppSettings loaded)
                        return loaded;
                }
            }
            catch
            {
            }

            var defaults = new AppSettings();
            defaults.Save();
            return defaults;
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(SettingsDirectory);
                var serializer = new XmlSerializer(typeof(AppSettings));
                using var stream = File.Create(SettingsFilePath);
                serializer.Serialize(stream, this);
            }
            catch
            {

            }
        }

        public AppSettings Clone() => (AppSettings)MemberwiseClone();

        public void ResetToDefaults()
        {
            var defaults = new AppSettings();
            foreach (PropertyInfo prop in typeof(AppSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanRead && prop.CanWrite)
                    prop.SetValue(this, prop.GetValue(defaults));
            }
        }
    }
}