using Advencursor._Skill.Thunder_Set;
using Advencursor._Skill.Food_Set;
using Advencursor._Skill.Fire_Set;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Advencursor._SaveData;
using System.Text.Json.Serialization;
using System.IO;

namespace Advencursor._Skill
{
    static class AllSkills
    {
        public static SkillData skillData { get; set; } = new SkillData();

        

        public static Dictionary<string, Skill> allSkills = new Dictionary<string, Skill>
        {
            //Please Copy All to Reset function Below when changing this or you will encounter logic error. - from Past Chotayakorn to Future Chotayakorn
            {"null", new Skill("null",skillData) },
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",skillData)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",skillData)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed", skillData)},
            {"I AM the Storm", new Skill_R_IamStorm("I AM the Storm", skillData)},
            {"Yadom Bait",new Skill_Q_FoodTrap("Yadom Bait", skillData) },
            {"HAHAHA It's A Trap",new Skill_W_PoisonTrap("HAHAHA It's A Trap", skillData) },
            {"Emergency Food",new Skill_E_EmergencyFood("Emergency Food", skillData) },
            {"Nah I'd win",new Skill_R_Invincibility("Nah I'd win", skillData) },
            {"Fire Domain",new Skill_Q_FireDomain("Fire Domain",skillData) },
        };
        public static void Reset()
        {
            allSkills = new Dictionary<string, Skill>
            {
            {"null", new Skill("null",skillData) },
            {"Thunder Core", new Skill_Q_ThunderCore("Thunder Core",skillData)},
            {"Thunder Shuriken", new Skill_W_ThunderShuriken("Thunder Shuriken",skillData)},
            {"Thunder Speed", new Skill_E_ThunderSpeed("Thunder Speed", skillData)},
            {"I AM the Storm", new Skill_R_IamStorm("I AM the Storm", skillData)},
            {"Yadom Bait",new Skill_Q_FoodTrap("Yadom Bait", skillData) },
            {"HAHAHA It's A Trap",new Skill_W_PoisonTrap("HAHAHA It's A Trap", skillData) },
            {"Emergency Food",new Skill_E_EmergencyFood("Emergency Food", skillData) },
            {"Nah I'd win",new Skill_R_Invincibility("Nah I'd win", skillData) },
            {"Fire Domain",new Skill_Q_FireDomain("Fire Domain",skillData) },
            };
        }

        public static Dictionary<string, Texture2D> allSkillTextures = new Dictionary<string, Texture2D>
        {
            {"null", new Texture2D(Globals.graphicsDevice,1,1) },
            {"Thunder Core", Globals.Content.Load<Texture2D>("Item/SetThunder/Q_Texture")},
            {"Thunder Shuriken", Globals.Content.Load<Texture2D>("Item/SetThunder/W_Texture")},
            {"Thunder Speed", Globals.Content.Load<Texture2D>("Item/SetThunder/E_Texture")},
            {"I AM the Storm", Globals.Content.Load<Texture2D>("Item/SetThunder/R_Texture")},
            {"Yadom Bait",Globals.Content.Load<Texture2D>("Item/SetFood/Q_Texture")},
            {"HAHAHA It's A Trap",Globals.Content.Load<Texture2D>("Item/SetFood/W_Texture")},
            {"Emergency Food",Globals.Content.Load<Texture2D>("Item/SetFood/E_Texture")},
            {"Nah I'd win",Globals.Content.Load<Texture2D>("Item/SetFood/R_Texture")},
            {"Fire Domain",Globals.Content.Load<Texture2D>("Item/SetFire/Q_Texture")},
        };


        public static Dictionary<string, string> itemNameViaSkillName = new Dictionary<string, string>
        {
            {"null","null"},
            {"Thunder Core","(Q)Thunder Core Orb"},
            {"Thunder Shuriken","(W)Thunder Shuriken Orb"},
            {"Thunder Speed","(E)Thunder Speed Orb"},
            {"I AM the Storm","(R)Thunder Slash Orb"},
            {"Yadom Bait","(Q)Green Inhaler Orb"},
            {"HAHAHA It's A Trap","(W)Poison Inhaler Orb"},
            {"Emergency Food","(E)Heal Inhaler Orb"},
            {"Nah I'd win","(R)Invincibility Orb"},
            {"Fire Domain","Fire Domain Orb"},
        };

        public static Dictionary<string,Keys> itemKeyViaSkillName = new Dictionary<string, Keys>
        {
            {"null",Keys.None},
            {"Thunder Core",Keys.Q},
            {"Thunder Shuriken",Keys.W},
            {"Thunder Speed",Keys.E},
            {"I AM the Storm",Keys.R},
            {"Yadom Bait",Keys.Q},
            {"HAHAHA It's A Trap",Keys.W},
            {"Emergency Food",Keys.E},
            {"Nah I'd win",Keys.R},
            {"Fire Domain",Keys.Q},
        };
    }
}
