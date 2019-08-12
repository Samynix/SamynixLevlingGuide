using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SamynixLevlingGuide.Tags
{
    public static class TextColors
    {
        public static Color Text => Colors.Black;
        public static Color SetHome => Color.FromRgb(254, 220, 0);
        public static Color BankWithdraw => Color.FromRgb(237, 32, 36);
        public static Color BankDeposit => Color.FromRgb(106, 189, 69);
        public static Color NamedMob => Color.FromRgb(0, 128, 128);
        public static Color AQ => Color.FromRgb(247, 143, 30);
        public static Color TurnInQuest => Color.FromRgb(193, 30, 93);
        public static Color DoQuest => Color.FromRgb(186, 82, 159);
        public static Color WorkOnQuest => Color.FromRgb(65, 185, 235);
        public static Color SkipQuest => Color.FromRgb(255, 228, 193);

        public static Dictionary<Color, string> ColorsAndDescriptions {
            get
            {
                return new Dictionary<Color, string>
                {
                    { SetHome, "Hearthstone" },
                    { BankWithdraw, "Withdraw from bank" },
                    { BankDeposit, "Deposit in bank" },
                    { NamedMob, "Mob/Item" },
                    { AQ, "Accept quest" },
                    { TurnInQuest, "Turn in quest" },
                    { DoQuest, "Quest should be done after this" },
                    { WorkOnQuest, "Work on quest" },
                    { SkipQuest, "Skip quest" }
                };
            }
        }


        public static Color GetColorFromTag(SimpleTags.Tag aTag)
        {
            switch (aTag)
            {
                case SimpleTags.Tag.SetHome:
                    return SetHome;
                case SimpleTags.Tag.BankWithdraw:
                    return BankWithdraw;
                case SimpleTags.Tag.BankDeposit:
                    return BankDeposit;
                case SimpleTags.Tag.NamedMob:
                    return NamedMob;
                case SimpleTags.Tag.AcceptQuest:
                    return AQ;
                case SimpleTags.Tag.ReturnQuest:
                    return TurnInQuest;
                case SimpleTags.Tag.DoQuest:
                    return DoQuest;
                case SimpleTags.Tag.WorkOnQuest:
                    return WorkOnQuest;
                case SimpleTags.Tag.SkipQuest:
                    return SkipQuest;
                default:
                    return Text;
            }
        }

        //public static bool TryGetColorFromTag(string aTag, out Color aColor)
        //{
        //    if (aTag.ToLower().Contains(nameof(SetHome).ToLower()))
        //    {
        //        aColor = SetHome;
        //        return true;
        //    }

        //    if (aTag.ToLower().Contains(nameof(BankWithdraw).ToLower()))
        //    {
        //        aColor = BankWithdraw;
        //        return true;
        //    }

        //    if (aTag.ToLower().Contains(nameof(BankDeposit).ToLower()))
        //    {
        //        aColor = BankDeposit;
        //        return true;
        //    }

        //    if (aTag.ToLower().Contains(nameof(NamedMob).ToLower()))
        //    {
        //        aColor = NamedMob;
        //        return true;
        //    }

        //    if (aTag.ToLower().Contains(nameof(AQ).ToLower()))
        //    {
        //        aColor = AQ;
        //        return true;
        //    }

        //    if (aTag.ToLower().Contains(nameof(TurnInQuest).ToLower()))
        //    {
        //        aColor = TurnInQuest;
        //        return true;
        //    }

        //    if (aTag.ToLower().Contains(nameof(DoQuest).ToLower()))
        //    {
        //        aColor = DoQuest;
        //        return true;
        //    }

        //    if (aTag.ToLower().Contains(nameof(WorkOnQuest).ToLower()))
        //    {
        //        aColor = WorkOnQuest;
        //        return true;
        //    }

        //    if (aTag.ToLower().Contains(nameof(SkipQuest).ToLower()))
        //    {
        //        aColor = SkipQuest;
        //        return true;
        //    }

        //    aColor = Text;
        //    return false;
        //}



    }
}
