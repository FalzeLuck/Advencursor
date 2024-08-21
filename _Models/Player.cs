using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Skill;
using Advencursor._Combat;
using System.Diagnostics;

namespace Advencursor._Models
{
    public class Player : Sprite
    {
        public Status Status { get; set; }
        public Dictionary<Keys, Skill> Skills {  get; private set; }
        public Inventory Inventory { get; private set; }





        public Player(Texture2D texture,Vector2 position, int health) : base(texture,position)
        {
            Skills = new Dictionary<Keys, Skill>();
            Status = new(health);
            Inventory = new Inventory();
        }


        public void EquipItem(Item item)
        {
            Inventory.EquipItem(item, this);
        }


        public void AddSkill(Keys key,Skill skill)
        {
            Skills[key] = skill;
        }
        public void RemoveSkill(Keys key) { Skills[key] = null; }

        public void UseSkill(Keys key)
        {
            if (Skills.ContainsKey(key) && Skills[key].CanUse())
            {
                Skills[key].Use();
                ParticleManager.AddParticle(new(position, new()));
                Debug.WriteLine("Player use Q");
            }

            if (Skills.ContainsKey(key) && !Skills[key].CanUse())
            {
                Debug.WriteLine("Player cant use Q");
            }

        }


        public void Update()
        {
            position = new(InputManager._mousePosition.X,InputManager._mousePosition.Y);
        }
    }
}
