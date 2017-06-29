using System.Drawing;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.Gui
{
    // NOTE: This will not be sent over the wire. This is merely used to serialize the palette to the client config file.
    [DataContract]
    public class Theme
    {
        [DataMember]
        public Color BackgroundColor { get; set; }
        [DataMember]
        public Color TextBoxBackgroundColor { get; set; }
        [DataMember]
        public Color TextForgroundColor { get; set; }
        [DataMember]
        public Color TextBoxForegroundColor { get; set; }
        [DataMember]
        public Color TreeViewBackgroundColor { get; set; }
        [DataMember]
        public Color TreeViewForegrondColor { get; set; }
        [DataMember]
        public Color ConsoleBackgroundColor { get; set; }
        [DataMember]
        public Color ConsoleForegroundColor { get; set; }
        [DataMember]
        public bool ThemeEnabled { get; set; }

        public static Theme AwesomeWhite
        {
            get
            {
                return new Theme()
                {
                    BackgroundColor = Color.White,
                    TextBoxBackgroundColor = Color.White,
                    TextForgroundColor = Color.Black,
                    TextBoxForegroundColor = Color.Black,
                    TreeViewBackgroundColor = Color.White,
                    TreeViewForegrondColor = Color.Black,
                };
            }
        }
        public static Theme CoolDark
        {
            get
            {
                return new Theme()
                {
                    BackgroundColor = Color.DimGray,
                    TextBoxBackgroundColor = Color.DimGray,
                    TextForgroundColor = Color.White,
                    TextBoxForegroundColor = Color.White,
                    TreeViewForegrondColor = Color.White,
                    TreeViewBackgroundColor = Color.DimGray,
                };
            }
        }
    }
}