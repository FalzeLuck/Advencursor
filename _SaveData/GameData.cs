﻿using Advencursor._Animation;
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
        public int stage { get; set; } = 1;

        //Setting
        public float volumeMusic { get; set; } = 1f;
        public float volumeEffect { get; set; } = 1f;


        public int gems { get; set; } = 0;
        public int pityCounter { get; set; } = 0;

        public void SaveData()
        {
            GameData data = this;

            string serializedData = JsonSerializer.Serialize(data);
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedData));
            File.WriteAllText("gamedata.dat", encoded);
        }

        public void LoadData()
        {
            if (File.Exists("gamedata.dat"))
            {
                string encoded = File.ReadAllText("gamedata.dat");
                string deserializedData = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                GameData data = JsonSerializer.Deserialize<GameData>(deserializedData);
                this.isFirstTime = data.isFirstTime;
                this.stage = data.stage;
                this.gems = data.gems;
                this.pityCounter = data.pityCounter;
            }
            else
            {
                return;
            }
        }
    }
}
