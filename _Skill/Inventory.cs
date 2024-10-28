using Advencursor._Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Advencursor._SaveData;

namespace Advencursor._Skill
{
    public class Inventory
    {
        public List<Item> Items { get; private set; }

        
        public Inventory()
        {
            Items = new List<Item>()
            {
                new Item(AllSkills.itemNameViaSkillName["null"],AllSkills.allSkills["null"],Keys.None)
            };
        }

        public void SaveInventory()
        {
            string json = JsonSerializer.Serialize(Items);
            string encodedJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            File.WriteAllText("inventory.dat", encodedJson);
        }

        public void LoadInventory(Texture2D defaultTexture)
        {
            if (!File.Exists("inventory.dat")) return;

            string encodedJson = File.ReadAllText("inventory.dat");
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(encodedJson));
            List<Item> deserializedItems = JsonSerializer.Deserialize<List<Item>>(json);

            foreach (var item in deserializedItems)
            {
                item.SetTexture();
            }

            Items.Clear();
            Items.AddRange(deserializedItems);
            ClearNull();
        }

        public void ClearNull()
        {
            Items.RemoveAll(item => item.name == "null");
        }

        public void SortItem()
        {
            Dictionary<Keys, int> keyOrder = new Dictionary<Keys, int>
            {
                { Keys.Q, 1 },
                { Keys.W, 2 },
                { Keys.E, 3 },
                { Keys.R, 4 }
            };

            Items = Items
                .OrderByDescending(item => item.itemSet)
                .ThenBy(item => keyOrder.ContainsKey(item.keys) ? keyOrder[item.keys] : int.MaxValue)
                .ThenByDescending(item => item.statValue)
                .ToList();
        }
        public void RemoveDuplicatesKeepHighestStat()
        {
            
            var itemsByName = Items.GroupBy(item => item.name);

            List<Item> highestStatItems = new List<Item>();

            foreach (var group in itemsByName)
            {
                var maxStatItem = group.OrderByDescending(item => item.statValue).First();
                highestStatItems.Add(maxStatItem);
            }

            Items = highestStatItems;
        }
    }

    public class Item
    {
        [JsonIgnore]
        public Texture2D texture { get; private set; }


        public string name { get; set; }
        public Skill skill { get; set; }
        public string itemSet { get; set; }
        public Keys keys { get; set; }
        public string statDesc { get; set; }
        public float statValue { get; set; }

        public Item(string name, Skill skill, Keys keys)
        {
            this.name = name;
            this.skill = skill;
            itemSet = skill.setSkill;
            this.keys = keys;
            texture = AllSkills.allSkillTextures["null"];
            SetTexture();
            GenerateStat();
        }

        private void GenerateStat()
        {
            if (keys == Keys.Q)
            {
                statDesc = "Health";
                statValue = BiasedRandomFloat(1000, 3000, 3.0f);
            }
            else if (keys == Keys.W)
            {
                statDesc = "Attack";
                statValue = BiasedRandomFloat(20, 40, 4.0f);
            }
            else if (keys == Keys.E)
            {
                statDesc = "Critical Rate";
                statValue = BiasedRandomFloat(4.7f, 31.3f, 4.0f);
            }
            else if (keys == Keys.R)
            {
                statDesc = "Critical Damage";
                statValue = BiasedRandomFloat(9.3f, 62.2f, 4.0f);
            }
        }

        private float BiasedRandomFloat(float min, float max, float bias = 2.0f)
        {
            float randomFactor = (float)Globals.random.NextDouble();

            randomFactor = (float)Math.Pow(randomFactor, bias);

            return min + (max - min) * randomFactor;
        }

        public void GenerateStatCheat()
        {
            if (keys == Keys.Q)
            {
                statDesc = "Health";
                statValue = 10000;
            }
            else if (keys == Keys.W)
            {
                statDesc = "Attack";
                statValue = 5000;
            }
            else if (keys == Keys.E)
            {
                int index = Globals.random.Next(1, 2);
                if (index == 1)
                {
                    statDesc = "Critical Rate";
                    statValue = 50;
                }
                else
                {
                    statDesc = "Critical Damage";
                    statValue = 100;
                }
            }
        }

        public void SetTexture() { texture = AllSkills.allSkillTextures[skill.name]; }


    }
}
