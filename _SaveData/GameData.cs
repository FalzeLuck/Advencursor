using Advencursor._Animation;
using Advencursor._Combat;
using Advencursor._Skill;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Advencursor._SaveData
{
    [Serializable]
    public class GameData
    {
        public bool isFirstTime {  get; set; } = true;
        public int stage {  get; set; }

        public void SaveData()
        {
            GameData data = this;

            string serializedData = JsonSerializer.Serialize(data);
            File.WriteAllText("gamedata.json", serializedData);
        }

        public void LoadData()
        {
            if (File.Exists("gamedata.json"))
            {
                string deserializedData = File.ReadAllText("gamedata.json");
                GameData data = JsonSerializer.Deserialize<GameData>(deserializedData);
                this.isFirstTime = data.isFirstTime;
                this.stage = data.stage;
            }
            else
            {
                return;
            }
        }
    }
}
