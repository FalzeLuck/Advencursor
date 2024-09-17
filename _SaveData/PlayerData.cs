using Advencursor._Models;
using Advencursor._Skill;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._SaveData
{
    [Serializable]
    public class PlayerData
    {
        public int Health { get; set; }
        public int Attack { get; set; }

        public Dictionary<Keys, string> SkillNames { get; set; }
    }
}
