using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Parser
{
    public static class StringExtensions
    {
        /// <summary>
        /// Return tokens from string base on delimiters
        /// 
        /// Why not "string.Split(...)"?  Because the delimiters are not returned and they
        /// are required for parsing
        /// 
        /// Note: quoted string are supported
        /// 
        /// </summary>
        /// <param name="self">string</param>
        /// <param name="delimiters">list of delimiters to used</param>
        /// <param name="stringComparison">type of string comparison</param>
        /// <returns>return list of fields + tokens</returns>
        public static IReadOnlyList<string> Tokenize(this string self, string[] delimiters, StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (self == null || self == "" || delimiters == null || delimiters.Length == 0)
            {
                return new string[0];
            }

            var tokens = new List<string>();
            var seperatorPos = new HashSet<int>();
            bool insideQuote = false;

            StringBuilder str = new StringBuilder();

            for (int i = 0; i < self.Length; i++)
            {
                if( self[i] == '"')
                {
                    str.Append(self[i]);

                    if(!insideQuote)
                    {
                        insideQuote = true;
                        continue;
                    }

                    insideQuote = false;
                    tokens.Add(str.ToString());
                    str.Clear();
                    continue;
                }

                if (insideQuote)
                {
                    str.Append(self[i]);
                    continue;
                }

                if (self[i] == ' ' && i + 1 < self.Length && self[i + 1] == ' ')
                {
                    continue;
                }

                seperatorPos.Clear();
                for (int seperatorIndex = 0; seperatorIndex < delimiters.Length; seperatorIndex++)
                {
                    string sep = delimiters[seperatorIndex];

                    if (sep.Length > (self.Length - i))
                    {
                        seperatorPos.Remove(seperatorIndex);
                        continue;
                    }

                    if (self.Substring(i, sep.Length).Equals(sep, stringComparison))
                    {
                        seperatorPos.Add(seperatorIndex);
                    }
                }

                if (seperatorPos.Count > 1)
                {
                    var r = seperatorPos
                        .Select(x => new { Sep = delimiters[x], Idx = x })
                        .OrderByDescending(x => x.Sep.Length);

                    foreach (var item in r.Skip(1))
                    {
                        seperatorPos.Remove(item.Idx);
                    }
                }

                switch (seperatorPos.Count)
                {
                    case 0:
                        str.Append(self[i]);
                        break;

                    case 1:
                        if (str.Length > 0)
                        {
                            tokens.Add(str.ToString());
                            str.Clear();
                        }

                        string sep = delimiters[seperatorPos.First()];
                        tokens.Add(self.Substring(i, sep.Length));
                        i += sep.Length - 1;
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            if(insideQuote)
            {
                throw new InvalidOperationException($"Missing ending quote, {str}");
            }

            if (str.Length > 0)
            {
                tokens.Add(str.ToString());
            }

            return tokens;
        }

        /// <summary>
        /// Return indication that the string is quoted or not
        /// </summary>
        /// <param name="self">string</param>
        /// <returns>true or false</returns>
        public static bool IsQuoted(this string self)
        {
            if( string.IsNullOrWhiteSpace(self) || self.Length < 2)
            {
                return false;
            }

            return self[0] == '"' && self[self.Length - 1] == '"';
        }

        /// <summary>
        /// Clean string of non-alpha-numeric characters, except "_"
        /// </summary>
        /// <param name="self"></param>
        /// <returns>cleaned string</returns>
        public static string Clean(this string self)
        {
            var cleaned = self
                .Where(x => char.IsLetterOrDigit(x) && x != '_')
                .ToArray();

            return new string(cleaned);
        }
    }
}