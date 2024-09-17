using Advencursor._Skill.Thunder_Set;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill
{
    static class AllSkills
    {
        public static Dictionary<string, Skill> allSkills = new Dictionary<string, Skill>
        {
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",5)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed",6)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",7)},
            {"I am the Storm", new Skill_R_IamStorm("I am the Storm",8)},
        };


    }
}
