﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SecuritySystem.Email.EmailClient.EmailAdvanced {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    public sealed partial class EmailAdvancedSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static EmailAdvancedSettings defaultInstance = ((EmailAdvancedSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new EmailAdvancedSettings())));
        
        public static EmailAdvancedSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AppendPrefix {
            get {
                return ((string)(this["AppendPrefix"]));
            }
            set {
                this["AppendPrefix"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AppendSuffix {
            get {
                return ((string)(this["AppendSuffix"]));
            }
            set {
                this["AppendSuffix"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool TimeStamp {
            get {
                return ((bool)(this["TimeStamp"]));
            }
            set {
                this["TimeStamp"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int MessageSendCount {
            get {
                return ((int)(this["MessageSendCount"]));
            }
            set {
                this["MessageSendCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DisableSendCMD {
            get {
                return ((bool)(this["DisableSendCMD"]));
            }
            set {
                this["DisableSendCMD"] = value;
            }
        }
    }
}
