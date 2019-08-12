using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamynixLevlingGuide.Model
{
    public enum ClassEnum
    {
        Warrior = 1,
        Paladin = 2,
        Hunter = 4,
        Rogue = 8,
        Priest = 16,
        Shaman = 64,
        Mage = 128,
        Warlock = 256,
        Druid = 1024,

        All = Warrior + Paladin + Hunter + Rogue + Priest + Shaman + Mage + Warlock + Druid
    }
}
