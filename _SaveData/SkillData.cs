﻿using Advencursor._Skill.Food_Set;
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
        [JsonIgnore]
        public Dictionary<string, float> ThunderCore { get; set; }
        [JsonIgnore]
        public Dictionary<string,float> ThunderShuriken {  get; set; }
        [JsonIgnore]
        public Dictionary<string, float> ThunderSpeed { get; set; }
        [JsonIgnore]
        public Dictionary<string, float> IamStorm { get; set; }

        //Food Set Dictionary
        [JsonIgnore]
        public Dictionary<string, float> FoodTrap { get; set; }
        [JsonIgnore]
        public Dictionary<string, float> PoisonTrap { get; set; }
        [JsonIgnore]
        public Dictionary<string, float> EmergencyFood { get; set; }
        [JsonIgnore]
        public Dictionary<string, float> Invincibility { get; set; }

        //Fire Set Dictionary
        [JsonIgnore]
        public Dictionary<string, float> FireDomain { get; set; }
        [JsonIgnore]
        public Dictionary<string, float> FireBall { get; set; }
        [JsonIgnore]
        public Dictionary<string, float> FireBomb { get; set; }
        [JsonIgnore]
        public Dictionary<string, float> FireEmperor { get; set; }

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
                {"Cooldown", 18f }
            };
            ThunderShuriken = new Dictionary<string, float>()
            {
                {"Damage Multiplier", 0.5f },
                {"Max Amount",4f },
                {"Radius",200f },
                {"Duration", 8f },
                {"Speed", 5f},
                {"Cooldown", 20f }
            };
            ThunderSpeed = new Dictionary<string, float>()
            {
                {"Damage Multiplier", 0.5f },
                {"Duration", 8f },
                {"Speed Multiplier", 2f},
                {"Cooldown", 15f }
            };
            IamStorm = new Dictionary<string, float>()
            {
                {"Damage Multiplier", 1f },
                {"Max Hit",9f },
                {"Cooldown", 60f }
            };
            FoodTrap = new Dictionary<string, float>()
            {
                {"Duration", 5f },
                {"Cooldown", 12f }
            };
            PoisonTrap = new Dictionary<string, float>()
            {
                {"Duration", 5f },
                {"Cooldown", 15f }
            };
            EmergencyFood = new Dictionary<string, float>()
            {
                {"Heal Percentage", 5f },
                {"Attack Multiplier", 1.7f },
                {"Duration", 8f },
                {"Cooldown", 14f }
            };
            Invincibility = new Dictionary<string, float>()
            {
                {"Duration",15f },
                {"Heal Percentage", 50f },
                {"Attack Add", 800f },
                {"Crit Rate Add",20f },
                {"Crit Dam Add",200f },
                {"Cooldown", 60f }
            };
            FireDomain = new Dictionary<string, float>()
            {
                {"Damage Amplifier", 0.25f },
                {"Duration", 7f },
                {"Heal Percentage", 1f },
                {"Heal Interval", 1f },
                {"Radius", 600f },
                {"Cooldown", 20f }
            };
            FireBall = new Dictionary<string, float>()
            {
                {"Damage Multiplier", 1.5f },
                {"Ball Amount",8f },
                {"Speed", 1000f},
                {"Cooldown", 15f }
            };
            FireBomb = new Dictionary<string, float>()
            {
                {"Damage Multiplier", 2.5f },
                {"Radius", 600f },
                {"Countdown Interval", 1 },
                {"Cooldown", 20f }
            };
            FireEmperor = new Dictionary<string, float>()
            {
                {"Bomb Multiplier", 7.5f },
                {"Hp Buff Attack",0.5f },
                {"Slash Multiplier",1.5f },
                {"Radius",600f },
                {"Cooldown",60f }
            };

            skillNameForDamageMultipliers.Add("Thunder Core", ThunderCore);
            skillNameForDamageMultipliers.Add("Thunder Shuriken", ThunderShuriken);
            skillNameForDamageMultipliers.Add("Thunder Speed", ThunderSpeed);
            skillNameForDamageMultipliers.Add("I AM the Storm", IamStorm);
            skillNameForDamageMultipliers.Add("Yadom Bait", FoodTrap);
            skillNameForDamageMultipliers.Add("HAHAHA It's A Trap", PoisonTrap);
            skillNameForDamageMultipliers.Add("Emergency Food", EmergencyFood);
            skillNameForDamageMultipliers.Add("Nah I'd win", Invincibility);
            skillNameForDamageMultipliers.Add("Fire Domain", FireDomain);
            skillNameForDamageMultipliers.Add("Katon goukakyuu no jutsu", FireBall);
            skillNameForDamageMultipliers.Add("E-X-P-L-O-S-I-O-N!", FireBomb);
            skillNameForDamageMultipliers.Add("Flame Emperor", FireEmperor);
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
            File.WriteAllText(path, serializedData);
        }

        public void LoadData()
        {
            if (File.Exists(path))
            {
                string deserializedData = File.ReadAllText(path);
                SkillData data = JsonSerializer.Deserialize<SkillData>(deserializedData);
                this.skillNameForDamageMultipliers = data.skillNameForDamageMultipliers;
                //UpdateDictionariesFromSkillMultipliers();
            }
            else
            {
                SaveData();
            }
        }
        /*private void UpdateDictionariesFromSkillMultipliers()
        {
            ThunderCore = skillNameForDamageMultipliers["Thunder Core"];
            ThunderShuriken = skillNameForDamageMultipliers["Thunder Shuriken"];
            ThunderSpeed = skillNameForDamageMultipliers["Thunder Speed"];
            IamStorm = skillNameForDamageMultipliers["I AM the Storm"];
            FoodTrap = skillNameForDamageMultipliers["Yadom Bait"];
            PoisonTrap = skillNameForDamageMultipliers["HAHAHA It's A Trap"];
            EmergencyFood = skillNameForDamageMultipliers["Emergency Food"];
            Invincibility = skillNameForDamageMultipliers["Nah I'd win"];
            FireDomain = skillNameForDamageMultipliers["Fire Domain"];
            FireBall = skillNameForDamageMultipliers["Katon goukakyuu no jutsu"];
            FireBomb = skillNameForDamageMultipliers["E-X-P-L-O-S-I-O-N!"];
            FireEmperor = skillNameForDamageMultipliers["Flame Emperor"];
        }*/
    }
}
