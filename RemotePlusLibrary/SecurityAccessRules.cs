using RemotePlusLibrary.Editors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace RemotePlusLibrary
{
    [DataContract]
    public class SecurityAccessRules
    {
        [DataMember]
        [Browsable(true)]
        public bool CanRunProgram { get; set; }
        [DataMember]
        [Browsable(true)]
        public bool CanBeep { get; set; }
        [Browsable(true)]
        [DataMember]
        public bool CanSpeak { get; set; }
        [Browsable(true)]
        [DataMember]
        public bool CanAccessConsole { get; set; }
        [DataMember]
        [Browsable(true)]
        public bool CanPlaySound { get; set; }
        [DataMember]
        [Browsable(true)]
        public bool CanPlaySoundLoop { get; set; }
        [DataMember]
        [Browsable(true)]
        public bool CanPlaySoundSync { get; set; }
        [DataMember]
        [Browsable(true)]
        public bool CanAccessSettings { get; set; }
        [DataMember]
        [Browsable(true)]
        public bool CanShowMessageBox { get; set; }
        [DataMember]
        [Browsable(true)]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        public List<string> ExtensionRules { get; set; }
        public void Reset()
        {
            this.CanAccessConsole = true;
            this.CanBeep = true;
            this.CanPlaySound = true;
            this.CanPlaySoundLoop = true;
            this.CanPlaySoundSync = true;
            this.CanRunProgram = true;
            this.CanShowMessageBox = true;
            this.CanSpeak = true;
            this.CanAccessSettings = true;
            this.ExtensionRules = new List<string>();
        }
        public SecurityAccessRules()
        {
            Reset();
        }
    }
}
