using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities.DatabaseUtil;

namespace SamynixLevlingGuide.Model
{
    public class QuestDAO
    {
        public const string TableName = "QuestChain";



        public int QuestId { get; private set; }
        public int NextQuestId { get; private set; }
        public int ChainLevel { get; private set; }
        public int MinLevel { get; private set; }
        public int QuestLevel { get; private set; }
        public int Type { get; private set; }
        public int ZoneOrSort { get; private set; }
        public string Category { get; private set; }
        public string Title { get; private set; }
        public string Details { get; private set; }
        public string Objectives { get; private set; }
        public string OfferRewardText { get; private set; }
        public string RequsetItemsText { get; private set; }
        public string EndText { get; private set; }


#region Not nn Database
        public string QuestKey { get; private set; }
#endregion Not in Database

        public static QuestDAO FromDataRow(DataRow aDataRow)
        {
            var result = new QuestDAO();
            result.QuestId = aDataRow.GetValue<int>(nameof(QuestId));
            result.NextQuestId = aDataRow.GetValue<int>(nameof(NextQuestId));
            result.ChainLevel = aDataRow.GetValue<int>(nameof(ChainLevel));
            result.MinLevel = aDataRow.GetValue<int>(nameof(MinLevel));
            result.QuestLevel = aDataRow.GetValue<int>(nameof(QuestLevel));
            result.Type = aDataRow.GetValue<int>(nameof(Type));
            result.ZoneOrSort = aDataRow.GetValue<int>(nameof(ZoneOrSort));
            result.Category = aDataRow.GetValue<string>(nameof(Category));
            result.Title = aDataRow.GetValue<string>(nameof(Title));
            result.Details = aDataRow.GetValue<string>(nameof(Details));
            result.Objectives = aDataRow.GetValue<string>(nameof(Objectives));
            result.OfferRewardText = aDataRow.GetValue<string>(nameof(OfferRewardText));
            result.RequsetItemsText = aDataRow.GetValue<string>(nameof(RequsetItemsText));
            result.EndText = aDataRow.GetValue<string>(nameof(EndText));

            result.QuestKey = Quest.GetQuestKey(result.Title, result.ChainLevel);
            if (result.QuestKey.Contains("calloffire"))
            {

            }

            return result;
        }


    }
}
