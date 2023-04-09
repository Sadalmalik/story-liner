using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Self.Architecture.CSV
{
    public static class CSVUtils
    {
        public const char cellSeparator = ',';
        public const char lineSeparator1 = '\r';
        public const char lineSeparator2 = '\n';
        public const char escapeCharacter = '\\';
        public const char stringWrapper = '"';
        private static char[] SPECIAL_CHAR = new char[] { ',', '\n', '"', '«', '»' };
        
        public static List<string[]> FromString(string text, bool forceFillColls = true)
        {
            int i = 0;
            int l = text.Length;
            char Next() { return (i < l) ? text[i++] : (char)0; }
            char Peek() { return (i < l) ? text[i] : (char)0; }
            bool Eof() { return i >= l; }
            
            bool stringMode = false;
            StringBuilder sb = new StringBuilder();
            List<string> line = new List<string>();
            List<string[]> table = new List<string[]>();
            int maxLength = 0;
            
            while (true)
            {
                char c = Next();

                if (Eof())
                {
                    if (sb.Length > 0)
                    {
                        line.Add(sb.ToString());
                        sb.Clear();
                    }

                    if (line.Count > 0)
                    {
                        table.Add(line.ToArray());
                        maxLength = Mathf.Max(maxLength, line.Count);
                        line.Clear();
                    }
                    
                    break;
                }
                
                if (stringMode)
                {
                    if (c == escapeCharacter)
                    {
                        char n = Next();
                        sb.Append(n);
                        continue;
                    } 
                    
                    if (c == stringWrapper)
                    {
                        char n = Peek();
                        if (n == stringWrapper)
                        {
                            sb.Append(n);
                            Next();
                        }
                        else
                        {
                            stringMode = false;
                            continue;
                        }
                    }
                    
                    sb.Append(c);
                }
                else
                {
                    if (c == stringWrapper)
                    {
                        stringMode = true;
                        continue;
                    }

                    if (c == cellSeparator)
                    {
                        line.Add(sb.ToString());
                        sb.Clear();
                        continue;
                    }

                    if (c == lineSeparator1)
                    {
                        continue;
                    }

                    if (c == lineSeparator2)
                    {
                        line.Add(sb.ToString());
                        sb.Clear();
                        table.Add(line.ToArray());
                        maxLength = Mathf.Max(maxLength, line.Count);
                        line.Clear();
                        continue;
                    }
                    
                    sb.Append(c);
                }
            }

            if (forceFillColls)
            {
                for (i = 0; i < table.Count; i++)
                {
                    var row = table[i];
                    Array.Resize(ref row, maxLength);
                    table[i] = row;
                }
            }

            return table;
        }

        public static string ToString(List<string[]> table)
        {
            StringBuilder builder = new StringBuilder();
            bool firstLine = true;
            foreach (string[] line in table)
            {
                if (!firstLine)
                    builder.Append("\n");
                firstLine = false;

                bool firstItem = true;
                foreach (var item in line)
                {
                    if (!firstItem)
                        builder.Append(",");
                    firstItem = false;

                    if (item.IndexOfAny(SPECIAL_CHAR) != -1)
                        builder.AppendFormat("\"{0}\"", item.Replace("\"", "\"\""));
                    else
                        builder.Append(item);
                }
            }
            return builder.ToString();
        }
    }
}