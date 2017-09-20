using System.Runtime.Serialization;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Specifies the type of login request.
    /// </summary>
    [DataContract]
    public enum AutehnticationSeverity
    {
        /// <summary>
        /// The operation is risky, but not severe.
        /// </summary>
        [EnumMember]
        Risk,
        /// <summary>
        /// The operation is normal like registiration.
        /// </summary>
        [EnumMember]
        Normal,
        /// <summary>
        /// The operation could be dangerous to the server application and/or server.
        /// </summary>
        [EnumMember]
        Danger
    }
}