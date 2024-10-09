using Advencursor._Skill.Food_Set;
using Advencursor._Skill.Thunder_Set;
using Advencursor._Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Diagnostics;

namespace Advencursor._SaveData
{
    [Serializable]
    public class SkillData
    {
        public Dictionary<string, Dictionary<string,float>> skillNameForDamageMultipliers {  get; set; } = new Dictionary<string, Dictionary<string,float>>();

        //Thunder Set Dictionary
        public Dictionary<string, float> ThunderCore { get; set; }
        public Dictionary<string,float> ThunderShuriken {  get; set; }
        public Dictionary<string, float> ThunderSpeed { get; set; }
        public Dictionary<string, float> IamStorm { get; set; }

        //Food Set Dictionary
        public Dictionary<string, float> FoodTrap { get; set; }
        public Dictionary<string, float> PoisonTrap { get; set; }
        public Dictionary<string, float> EmergencyFood { get; set; }
        public Dictionary<string, float> Invincibility { get; set; }

        public const string path = "skillData.json";
        public SkillData()
        {
            InitializeDictionaries();
        }

        private void InitializeDictionaries()
        {
            ThunderCore = new Dictionary<string, float>()
            {
                {"Duration", 8f },
            };
            ThunderShuriken = new Dictionary<string, float>()
            {
                {"Damage Multiplier", 0.5f },
                {"Max Amount",4f },
                {"Radius",200f },
                {"Duration", 8f },
                {"Speed", 5f},
            };
            ThunderSpeed = new Dictionary<string, float>()
            {
                {"Damage Multiplier", 0.5f },
                {"Duration", 8f },
                {"Speed Multiplier", 2f},
            };
            IamStorm = new Dictionary<string, float>()
            {
                {"Damage Multiplier", 1f },
                {"Max Hit",9f },
            };
            FoodTrap = new Dictionary<string, float>()
            {
                {"Duration", 5f },
            };
            PoisonTrap = new Dictionary<string, float>()
            {
                {"Duration", 5f },
            };
            EmergencyFood = new Dictionary<string, float>()
            {
                {"Heal Percentage", 5f },
                {"Attack Multiplier", 1.7f },
                {"Duration", 8f },
            };
            Invincibility = new Dictionary<string, float>()
            {
                {"Duration",15f },
                {"Heal Percentage", 50f },
                {"Attack Add", 800f },
                {"Crit Rate Add",20f },
                {"Crit Dam Add",200f },
            };

            skillNameForDamageMultipliers.Add("Thunder Core", ThunderCore);
            skillNameForDamageMultipliers.Add("Thunder Shuriken", ThunderShuriken);
            skillNameForDamageMultipliers.Add("Thunder Speed", ThunderSpeed);
            skillNameForDamageMultipliers.Add("I AM the Storm", IamStorm);
            skillNameForDamageMultipliers.Add("Yadom Bait", FoodTrap);
            skillNameForDamageMultipliers.Add("HAHAHA It's A Trap", PoisonTrap);
            skillNameForDamageMultipliers.Add("Emergency Food", EmergencyFood);
            skillNameForDamageMultipliers.Add("Nah I'd win", Invincibility);
        }

        public float GetMultiplierNumber(string skillName,string multiplierName)
        {
            var tempDict = skillNameForDamageMultipliers[skillName];

            return tempDict[multiplierName];
        }

        public void SaveData()
        {
            SkillData data = this;
            string serializedData = JsonSerializer.Serialize<SkillData>(data);
            Trace.WriteLine(serializedData);
            File.WriteAllText(path, serializedData);
        }

        public void LoadData()
        {
            if (File.Exists(path))
            {
                string deserializedData = File.ReadAllText(path);
                SkillData data = JsonSerializer.Deserialize<SkillData>(deserializedData);

                this.skillNameForDamageMultipliers = data.skillNameForDamageMultipliers;
                UpdateDictionariesFromSkillMultipliers();
            }
            else
            {
                SaveData();
            }
        }
        private void UpdateDictionariesFromSkillMultipliers()
        {
            ThunderCore = skillNameForDamageMultipliers["Thunder Core"];
            ThunderShuriken = skillNameForDamageMultipliers["Thunder Shuriken"];
            ThunderSpeed = skillNameForDamageMultipliers["Thunder Speed"];
            IamStorm = skillNameForDamageMultipliers["I AM the Storm"];
            FoodTrap = skillNameForDamageMultipliers["Yadom Bait"];
            PoisonTrap = skillNameForDamageMultipliers["HAHAHA It's A Trap"];
            EmergencyFood = skillNameForDamageMultipliers["Emergency Food"];
            Invincibility = skillNameForDamageMultipliers["Nah I'd win"];
        }
    }
}
