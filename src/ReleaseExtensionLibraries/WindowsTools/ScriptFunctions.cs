﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTools
{
    public class ScriptFunctions
    {
        public static string randChars(long numOfChars)
        {
            Random r = new Random();
            StringBuilder chars = new StringBuilder();
            for(long i = 0; i <= numOfChars; i++)
            {
                chars.Append((char)r.Next(32, 126));
            }
            return chars.ToString();
        }
    }
}