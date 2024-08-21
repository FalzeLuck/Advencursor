using Advencursor._Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

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
            Items.Add(item);
            player.AddSkill(item.keys, item.skill);
        }
    }

    public class Item
    {
        Texture2D texture;
        public string name { get; set; }
        public Skill skill { get; set; }
        public Keys keys { get; set; }

        public Item(string name,Skill skill,Keys keys) 
        {
            this.name = name;
            this.skill = skill;
            this.keys = keys;
        }
    }
}
