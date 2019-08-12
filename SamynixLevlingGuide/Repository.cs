using SamyLib.Standard.Database;
using SamynixLevlingGuide.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.StringUtil;

namespace SamynixLevlingGuide
{
    public static class Repository
    {
        private const string DbFile = "database.sqlite";

        static Repository()
        {
            SqlDriverFactory.RegisterDriver(0, SqlDriverType.Sqlite, $"Data Source={DbFile};Version=3");
        }

        private static Dictionary<int, QuestDAO> _questsByIdCache;

        private static Dictionary<string, QuestDAO> _questsByTitleAndPartCache;


        public static Dictionary<string, QuestDAO> QuestsByTitleAndPart
        {
            get
            {
                if (_questsByTitleAndPartCache == null)
                {
                    LoadQuestChains();
                }

                return _questsByTitleAndPartCache;
            }
        }

        public static Dictionary<int, QuestDAO> QuestsById
        {
            get
            {
                if (_questsByIdCache == null)
                {
                    LoadQuestChains();
                }

                return _questsByIdCache;
            }
        }

        private static void LoadQuestChains()
        {
            _questsByTitleAndPartCache = new Dictionary<string, QuestDAO>();
            _questsByIdCache = new Dictionary<int, QuestDAO>();

            //TODO error handlgn
            string sSql = $"SELECT * FROM {QuestDAO.TableName} ORDER BY {nameof(QuestDAO.ChainLevel)} ";
            var dataTable = SqlDriverFactory.GetSqlDriver(0).ExecuteQuery(sSql, null);
            var quests = dataTable.AsEnumerable().Select(QuestDAO.FromDataRow);

            foreach (var quest in quests)
            {
                _questsByIdCache[quest.QuestId] = quest;
                _questsByTitleAndPartCache[quest.QuestKey] = quest;
            }
        }
    }
}
