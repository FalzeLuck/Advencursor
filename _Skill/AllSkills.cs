using Advencursor._Skill.Thunder_Set;
using Advencursor._Skill.Food_Set;
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
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",cooldown: 22)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",cooldown:45)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed",cooldown:34)},
            {"I am the Storm", new Skill_R_IamStorm("I am the Storm",cooldown:2.5f * 60)},
            {"Food Trap",new Skill_Q_FoodTrap("Food Trap",cooldown:30) },
            {"Poison Trap",new Skill_W_PoisonTrap("Poison Trap",cooldown : 20) },
            {"Emergency Food",new Skill_E_EmergencyFood("Emergency Food",cooldown : 50) },
            {"Invincibility",new Skill_R_Invincibility("Invincibility",cooldown : 3 * 60) },
        };
        public static void Reset()
        {
            allSkills = new Dictionary<string, Skill>
            {
            {"null", new Skill("null",0.1f) },
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",cooldown: 22)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",cooldown:45)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed",cooldown:34)},
            {"I am the Storm", new Skill_R_IamStorm("I am the Storm",cooldown:2.5f * 60)},
            {"Food Trap",new Skill_Q_FoodTrap("Food Trap",cooldown:30) },
            {"Poison Trap",new Skill_W_PoisonTrap("Poison Trap",cooldown : 20) },
            {"Emergency Food",new Skill_E_EmergencyFood("Emergency Food",cooldown : 50) },
            {"Invincibility",new Skill_R_Invincibility("Invincibility",cooldown : 3 * 60) },
            };
        }

        public static Dictionary<string, Texture2D> allSkillTextures = new Dictionary<string, Texture2D>
        {
            {"null", new Texture2D(Globals.graphicsDevice,1,1) },
            {"Thunder Core", Globals.Content.Load<Texture2D>("Item/SetThunder/Q_Texture")},
            {"Thunder Shuriken", Globals.Content.Load<Texture2D>("Item/SetThunder/W_Texture")},
            {"Thunder Speed", Globals.Content.Load<Texture2D>("Item/SetThunder/E_Texture")},
            {"I am the Storm", Globals.Content.Load<Texture2D>("Item/SetThunder/R_Texture")},
            {"Food Trap",Globals.Content.Load<Texture2D>("Item/SetThunder/Q_Texture")},
            {"Poison Trap",Globals.Content.Load<Texture2D>("Item/SetThunder/W_Texture")},
            {"Emergency Food",Globals.Content.Load<Texture2D>("Item/SetThunder/E_Texture")},
            {"Invincibility",Globals.Content.Load<Texture2D>("Item/SetThunder/R_Texture")},
        };

        

    }
}
