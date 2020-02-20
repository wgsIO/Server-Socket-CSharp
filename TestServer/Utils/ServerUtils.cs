using System;
using System.Collections.Generic;
using System.Text;

namespace TestServer.Utils
{
    class ServerUtils
    {
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static StringBuilder SubString(StringBuilder input, int index, int length)
        {
            StringBuilder subString = new StringBuilder();
            if (index + length - 1 >= input.Length || index < 0)
            {
                throw new ArgumentOutOfRangeException("Index out of range!");
            }
            int endIndex = index + length;
            for (int i = index; i < endIndex; i++)
            {
                subString.Append(input[i]);
            }
            return subString;
        }

    }
}
