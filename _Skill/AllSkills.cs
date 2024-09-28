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
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",cooldown: 18)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",cooldown:20)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed",cooldown:15)},
            {"I AM the Storm", new Skill_R_IamStorm("I AM the Storm",cooldown:60)},
            {"Food Trap",new Skill_Q_FoodTrap("Food Trap",cooldown:12) },
            {"HAHAHA It's A Trap",new Skill_W_PoisonTrap("HAHAHA It's A Trap",cooldown : 15) },
            {"Emergency Food",new Skill_E_EmergencyFood("Emergency Food",cooldown : 14) },
            {"Nah I'd win",new Skill_R_Invincibility("Nah I'd win",cooldown : 60) },
        };
        public static void Reset()
        {
            allSkills = new Dictionary<string, Skill>
            {
            {"null", new Skill("null",0.1f) },
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",cooldown: 18)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",cooldown:20)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed",cooldown:15)},
            {"I AM the Storm", new Skill_R_IamStorm("I AM the Storm",cooldown:60)},
            {"Food Trap",new Skill_Q_FoodTrap("Food Trap",cooldown:12) },
            {"HAHAHA It's A Trap",new Skill_W_PoisonTrap("HAHAHA It's A Trap",cooldown : 15) },
            {"Emergency Food",new Skill_E_EmergencyFood("Emergency Food",cooldown : 14) },
            {"Nah I'd win",new Skill_R_Invincibility("Nah I'd win",cooldown : 60) },
            };
        }

        public static Dictionary<string, Texture2D> allSkillTextures = new Dictionary<string, Texture2D>
        {
            {"null", new Texture2D(Globals.graphicsDevice,1,1) },
            {"Thunder Core", Globals.Content.Load<Texture2D>("Item/SetThunder/Q_Texture")},
            {"Thunder Shuriken", Globals.Content.Load<Texture2D>("Item/SetThunder/W_Texture")},
            {"Thunder Speed", Globals.Content.Load<Texture2D>("Item/SetThunder/E_Texture")},
            {"I AM the Storm", Globals.Content.Load<Texture2D>("Item/SetThunder/R_Texture")},
            {"Food Trap",Globals.Content.Load<Texture2D>("Item/SetFood/Q_Texture")},
            {"HAHAHA It's A Trap",Globals.Content.Load<Texture2D>("Item/SetFood/W_Texture")},
            {"Emergency Food",Globals.Content.Load<Texture2D>("Item/SetFood/E_Texture")},
            {"Nah I'd win",Globals.Content.Load<Texture2D>("Item/SetFood/R_Texture")},
        };


        public static Dictionary<string, string> itemNameViaSkillName = new Dictionary<string, string>
        {
            {"Thunder Core","(Q)Thunder Core Orb"},
            {"Thunder Shuriken","(W)Thunder Shuriken Orb"},
            {"Thunder Speed","(E)Thunder Speed Orb"},
            {"I AM the Storm","(R)Thunder Slash Orb"},
            {"Food Trap","(Q)Green Inhaler Orb"},
            {"HAHAHA It's A Trap","(W)Poison Inhaler Orb"},
            {"Emergency Food","(E)Heal Inhaler Orb"},
            {"Nah I'd win","(R)Invincibility Orb"},
        };
    }
}
