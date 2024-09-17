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

namespace Advencursor._Skill
{
    public class Inventory
    {
        public List<Item> Items {  get; private set; }


        public Inventory() 
        {
            Items = new List<Item>();
        }

        public void EquipItem(Item item,Player player)
        {
            //player.AddSkill(item.keys, item.skill);
        }




        public void SaveInventory(string filePath)
        {
            string json = JsonSerializer.Serialize(Items);
            File.WriteAllText(filePath, json);
        }

        public void LoadInventory(string filePath, Texture2D defaultTexture)
        {
            if(!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            List<Item> deserializedItems = JsonSerializer.Deserialize<List<Item>>(json);

            foreach (var item in deserializedItems)
            {
                item.SetTexture(defaultTexture);
            }

            Items.Clear();
            Items.AddRange(deserializedItems);
        }


    }

    public class Item
    {
        [JsonIgnore]
        public Texture2D texture {  get; private set; }


        public string name { get; set; }
        public Skill skill { get; set; }
        public Keys keys { get; set; }
        public string statDesc { get; set; }
        public float statValue { get; set; }

        public Item(Texture2D texture,string name,Skill skill,Keys keys) 
        {
            this.texture = texture;
            this.name = name;
            this.skill = skill;
            this.keys = keys;
            GenerateStat();
        }

        private void GenerateStat()
        {
            if (keys == Keys.Q)
            {
                statDesc =  "Health";
                statValue = Globals.RandomFloat(3000,10000);
            }
            else if (keys == Keys.W)
            {
                statDesc = "Attack";
                statValue = Globals.RandomFloat(500, 1000);
            }
            else if(keys == Keys.E)
            {
                int index = Globals.random.Next(1, 3);
                if(index == 1)
                {
                    statDesc = "Critical Rate";
                    statValue = Globals.RandomFloat(5, 40);
                }
                else
                {
                    statDesc = "Critical Damage";
                    statValue = Globals.RandomFloat(0, 100);
                }
            }
        }

        public void SetTexture(Texture2D texture) { this.texture = texture; }
    }
}
