using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamynixLevlingGuide.Model
{
    public class QuestCategory
    {
        public string Category { get; set; }
        public List<Quest> Quests { get; set; }
    }
}
