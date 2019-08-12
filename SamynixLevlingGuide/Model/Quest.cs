using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Utilities.StringUtil;

namespace SamynixLevlingGuide.Model
{
    public class Quest
    {
        public int? QuestId { get; set; }

        public string QuestKey { get; set; }
        public string Title { get; set; }

        public int ChainLevel { get; set; }
        public int QuestLevel { get; set; }

        public string Category { get; set; }

        public Brush TextBrush { get; set; } = Brushes.Yellow;

        public bool HasBeenDelivered { get; set; }
        public bool HasBeenMarkedForSkip { get; internal set; }

        public static string GetQuestKey(string aTitle, int aChainLevel)
        {
            return TrimUtil.RemoveAllSpecialCharactersAndMakeLowercase(aTitle) + aChainLevel;
        }

        public static string GetQuestTitle(string aTitle, int aChainLevel)
        {
            return aChainLevel > 0 ? $"{aTitle} (Part {aChainLevel + 1})" : aTitle;
        }

    }
}
