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
        public float Health { get; set; }
        public float Attack { get; set; }

        public float CritRate { get; set; }
        public float CritDamage { get; set; }
        public Dictionary<Keys, string> SkillNames { get; set; }
    }
}
