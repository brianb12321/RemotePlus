using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    public struct SessionId
    {
        public static SessionId Empty => new SessionId("");
        public string Id { get; }

        public SessionId(string id)
        {
            Id = id;
        }
        public static bool operator == (SessionId left, SessionId right)
        {
            return (left.Id == right.Id);
        }
        public static bool operator !=(SessionId left, SessionId right)
        {
            return (left.Id != right.Id);
        }
    }
}