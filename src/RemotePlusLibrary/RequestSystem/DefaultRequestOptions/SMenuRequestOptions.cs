using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestOptions
{
    [DataContract]
    public class SMenuRequestOptions : SimpleMenuRequestOptions
    {
        /// <summary>
        /// The background color of the header. The default color is Black.
        /// </summary>
        [DataMember]
        public int HeaderBackgrondColor { get; set; } = Color.Black.ToArgb();
        /// <summary>
        /// The background color of the selector. The default color is Brown.
        /// </summary>
        [DataMember]
        public int SelectBackColor { get; set; } = Color.Brown.ToArgb();
        /// <summary>
        /// The background color of the client console. The default color is Black.
        /// </summary>
        [DataMember]
        public int ConsoleBackColor { get; set; } = Color.Black.ToArgb();
        /// <summary>
        /// The foreground color of the header. The default color is White.
        /// </summary>
        [DataMember]
        public int HeaderForeground { get; set; } = Color.White.ToArgb();
        /// <summary>
        /// The foreground color of each menu item. The default color is White.
        /// </summary>
        [DataMember]
        public int MenuItemForeground { get; set; } = Color.White.ToArgb();
        /// <summary>
        /// The foreground color of a menu item when selected. The default color is Black.
        /// </summary>
        [DataMember]
        public int SelectForeground { get; set; } = Color.Black.ToArgb();
        /// <summary>
        /// Determines whether to draw a border around the heading. The default is False.
        /// </summary>
        [DataMember]
        public bool DrawHeadingBorder { get; set; } = false;
    }
}
