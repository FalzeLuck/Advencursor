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

namespace Advencursor._SaveData
{
    [Serializable]
    public class SkillData
    {
        private Dictionary<string, float> skillNameForMultipliers;
        public SkillData()
        {
            skillNameForMultipliers = new Dictionary<string, float>()
            {
                {"null", 0f },
            {"Thunder Shuriken", 0.5f},
            {"Thunder Speed",0.5f },
            {"I AM the Storm", 1f},
            {"HAHAHA It's A Trap", 1f},
            };

            if (!File.Exists("skilldata.json"))
            {
                SaveData();
            }
            else LoadData();

        }


        public void SaveData()
        {
            SkillData data = this;

            string serializedData = JsonSerializer.Serialize(data);
            //string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedData));
            File.WriteAllText("skilldata.json", serializedData);
        }

        public void LoadData()
        {
            if (File.Exists("skilldata.json"))
            {
                string deserializedData = File.ReadAllText("skilldata.json");
                //string deserializedData = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                SkillData data = JsonSerializer.Deserialize<SkillData>(deserializedData);
            }
            else
            {
                return;
            }
        }
    }
}
