using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamynixLevlingGuide.Tags
{
    public static class TagParser
    {
        public static bool ParseTag(string aLine, ref int aIndex, out string aTag, out string aTagContent, out Dictionary<string, string> aDictionaryOfAttributes, bool isIncrementIndex = true)
        {
            var lineLower = aLine.ToLower();

            aTag = string.Empty;
            aTagContent = string.Empty;
            aDictionaryOfAttributes = new Dictionary<string, string>();
            if (lineLower[aIndex] == '<')
            {
                int endOfStartTagIndex = lineLower.IndexOf('>', aIndex);
                if (endOfStartTagIndex < aIndex)
                {
                    return false;
                }

                string pattern = @"(\w*)=""(\w*\.png|\w*|.*)""";
                RegexOptions options = RegexOptions.Singleline;
                foreach (Match m in Regex.Matches(aLine.Substring(aIndex, endOfStartTagIndex - aIndex), pattern, options))
                {
                    aDictionaryOfAttributes[m.Groups[1].Value.ToLower()] = m.Groups[2].Value;
                }

                if (aLine[endOfStartTagIndex-1] == '/')
                {
                    aTag = lineLower.Substring(aIndex + 1, endOfStartTagIndex - aIndex - 2);
                    if (isIncrementIndex)
                    {
                        aIndex = endOfStartTagIndex;
                    }

                    return true;
                }

                if (aDictionaryOfAttributes.Any())
                {
                    int indexOfSpace = aLine.IndexOf(' ', aIndex);
                    aTag = lineLower.Substring(aIndex + 1, indexOfSpace - aIndex - 1);
                }
                else
                {
                    aTag = lineLower.Substring(aIndex + 1, endOfStartTagIndex - aIndex - 1);
                }
            
                var aTextStartIndex = endOfStartTagIndex + 1;
                int endTagStartIndex = lineLower.IndexOf("</" + aTag, aTextStartIndex);
                if (endTagStartIndex < aTextStartIndex)
                {
                    return false;
                }

                int endTagEndIndex = lineLower.IndexOf('>', endTagStartIndex);
                if (endTagEndIndex < aTextStartIndex)
                {
                    return false;
                }
                var aTextEndIndex = endTagStartIndex;

              
                aTagContent = aLine.Substring(aTextStartIndex, aTextEndIndex - aTextStartIndex).Trim(Environment.NewLine.ToCharArray());
                if (isIncrementIndex)
                {
                    aIndex = endTagEndIndex;
                }

                return true;
            }

            return false;
        }
    }
}
