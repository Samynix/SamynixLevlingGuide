using SamynixLevlingGuide.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Utilities.EnumUtil;

namespace SamynixLevlingGuide.Tags
{
    public class SimpleTags
    {
        public static Tag GetTagFromString(string aTag)
        {
            foreach (var tag in Enum.GetValues(typeof(Tag)).Cast<Tag>().Where(t => t != Tag.Invalid))
            {
                if (IsTag(tag, aTag))
                {
                    return tag;
                }
            }

            return Tag.Invalid;
        }

        public static bool IsTag(Tag aTag, string aTagString)
        {
            var tagDescription = aTag.GetDescription();
            return !string.IsNullOrEmpty(aTagString) && (aTag.ToString().ToLower() == aTagString.ToLower() || tagDescription.ToLower() == aTagString.ToLower());
        }



        public static T GetContent<T>(string aTagContent)
        {
            try
            {
                var typeOf = typeof(T);
                if (typeOf == typeof(Int64))
                {
                    return (T)Convert.ChangeType(Int64.Parse(aTagContent), typeof(T));
                }
                else if (typeOf == typeof(long))
                {
                    return (T)Convert.ChangeType(long.Parse(aTagContent), typeof(T));
                }
                else if (typeOf == typeof(int))
                {
                    return (T)Convert.ChangeType(int.Parse(aTagContent), typeof(T));
                }
                else if (typeOf == typeof(string))
                {
                    return (T)Convert.ChangeType(aTagContent, typeof(T));
                }
                else if (typeOf == typeof(double))
                {
                    return (T)Convert.ChangeType(double.Parse(aTagContent.Replace(',', '.'), CultureInfo.InvariantCulture), typeof(T));
                }
                else if (typeOf == typeof(float))
                {
                    return (T)Convert.ChangeType(float.Parse(aTagContent.Replace(',', '.'),
                        CultureInfo.InvariantCulture), typeof(T));
                }
                else if (typeOf == typeof(bool) || typeOf == typeof(Boolean))
                {
                    return (T)Convert.ChangeType(Convert.ToBoolean(aTagContent), typeof(T));
                }
                else if (typeOf == typeof(DateTime))
                {
                    return (T)Convert.ChangeType(DateTime.Parse(aTagContent.ToString().Replace(',', '.'),
                        CultureInfo.InvariantCulture), typeof(T));
                }
                else if (typeOf == typeof(Point))
                {
                    aTagContent = aTagContent.Trim().TrimStart('(').TrimEnd(')');
                    var xy = aTagContent.Split(',').Select(s => s.Trim()).ToArray();
                    if (xy.Length < 2)
                    {
                        return (T)Convert.ChangeType(new Point(double.NaN, double.NaN), typeof(Point));
                    }

                    double x, y;
                    if (double.TryParse(xy[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) && double.TryParse(xy[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                    {
                        return (T)Convert.ChangeType(new Point(x, y), typeof(Point));
                    }

                    return (T)Convert.ChangeType(new Point(double.NaN, double.NaN), typeof(Point));
                }
                else if (typeOf == typeof(Color))
                {
                    string[] rgba = aTagContent.Split(',');
                    var color = Color.FromArgb(rgba.Length >= 4 ? byte.Parse(rgba[3].Trim()) : (byte)255, byte.Parse(rgba[0].Trim()), byte.Parse(rgba[1].Trim()), byte.Parse(rgba[2].Trim()));
                    return (T)Convert.ChangeType(color, typeof(Color));
                }
                else if (typeOf == typeof(ClassEnum))
                {
                    int classFlag = 0;
                    foreach (var classString in aTagContent.Split(',').Select(s => s.Trim()))
                    {
                        bool isInvert = classString.StartsWith("!");
                        var trimmedClassString = classString.TrimStart('!');
                        if (Enum.GetNames(typeof(ClassEnum)).Any(x => x.ToLower() == trimmedClassString.ToLower()))
                        {
                            var enumValue = (int)Enum.Parse(typeof(ClassEnum), trimmedClassString, true);
                            classFlag += isInvert ? -enumValue : enumValue;
                        }
                    }

                    return (T)Convert.ChangeType((ClassEnum)classFlag, typeof(ClassEnum));
                }
                else
                {
                    throw new NotImplementedException($"Not implemented for type {typeOf.FullName}");
                }
            }
            catch (Exception ex)
            {
                //TODO log
                Console.WriteLine(ex.Message);
                return default(T);
            }
          
        }

        public enum Tag
        {
            [Description("Image")]
            Image,
            StepNumber,
            [Description("Title")]
            Title,
 
            [Description("ST")]
            SubStep,

            [Description("B")]
            Bold,
            [Description("I")]
            Italic,
            [Description("UL")]
            Underline,


            [Description("LE")]
            Legend,
            [Description("Class")]
            Class,
            [Description("TL")]
            TLoc,
            [Description("PIC")]
            Picture,

            [Description("EM")]
            DONTFORGET,

            [Description("SH")]
            SetHome,
            [Description("BW")]
            BankWithdraw,
            [Description("BD")]
            BankDeposit,
            [Description("NM")]
            NamedMob,
            [Description("AQ")]
            AcceptQuest,
            [Description("RQ")]
            ReturnQuest,
            [Description("DQ")]
            DoQuest,
            [Description("WQ")]
            WorkOnQuest,
            [Description("SQ")]
            SkipQuest,
            [Description("BR")]
            LineBreak,
            [Description("P")]
            Place,


            Invalid,
      
        }
    }

    
}
