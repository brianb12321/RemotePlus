using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;

namespace RemotePlusLibrary.Core
{
    public class CustomConsoleTraceListener : ConsoleTraceListener
    {
        public override void Write(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            try
            {
                XDocument doc = XDocument.Parse(message);
                base.Write(doc.ToString());
            }
            catch
            {
                base.Write(message);
            }
        }

        public override void WriteLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            try
            {
                XDocument doc = XDocument.Parse(message);
                base.WriteLine(doc.ToString());
            }
            catch
            {
                base.WriteLine(message);
            }
        }
        public override void Fail(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                XDocument doc = XDocument.Parse(message);
                base.Fail(doc.ToString());
            }
            catch
            {
                base.Fail(message);
            }
        }
        public override void Fail(string message, string detailMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                XDocument doc = XDocument.Parse(message);
                base.Fail(doc.ToString(), detailMessage);
            }
            catch
            {
                base.Fail(message, detailMessage);
            }
        }
    }
}