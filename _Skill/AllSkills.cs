using Advencursor._Skill.Thunder_Set;
using Microsoft.Xna.Framework.Graphics;
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
            //Please Copy All to Reset function Below when changing this or you will encounter logic error. - from Past Chotayakorn to Future Chotayakorn
            {"null", new Skill("null",0.1f) },
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",5)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed",6)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",7)},
            {"I am the Storm", new Skill_R_IamStorm("I am the Storm",8)},
        };
        public static void Reset()
        {
            allSkills = new Dictionary<string, Skill>
            {
            {"null", new Skill("null",0.1f) },
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",5)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed",6)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",7)},
            {"I am the Storm", new Skill_R_IamStorm("I am the Storm",8)},
            };
        }

        public static Dictionary<string, Texture2D> allSkillTextures = new Dictionary<string, Texture2D>
        {
            {"null", new Texture2D(Globals.graphicsDevice,1,1) },
            {"Thunder Core", Globals.Content.Load<Texture2D>("Item/SetThunder/Q_Texture")},
            {"Thunder Speed", Globals.Content.Load<Texture2D>("Item/SetThunder/W_Texture")},
            {"Thunder Shuriken", Globals.Content.Load<Texture2D>("Item/SetThunder/E_Texture")},
            {"I am the Storm", Globals.Content.Load<Texture2D>("Item/SetThunder/R_Texture")},

        };

        

    }
}
