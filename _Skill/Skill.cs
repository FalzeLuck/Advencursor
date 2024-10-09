using Advencursor._Models;
using Advencursor._SaveData;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Advencursor._Skill
{
    [Serializable]
    public class Skill
    {
        public string name { get; set; }
        public int damage { get; set; }
        public string description { get; set; }
        public float cooldown { get; set; }
        public float cooldownTimer { get; private set; }

        public int rarity;

        public string setSkill;
        public SkillData skillData { get; set; }

        public Skill()
        {
            
        }

        public Skill(string name, float cooldown,SkillData skillData) 
        {
            this.name = name;
            this.cooldown = cooldown;
            this.skillData = skillData;
            cooldownTimer = 0;
            description = "";
        }


        public bool CanUse()
        {
            return cooldownTimer <= 0;
        }

        public virtual void Use(Player player)
        {
            cooldownTimer = cooldown;
        }

        public virtual void Update(float deltaTime,Player player)
        {
            if(cooldownTimer > 0) 
                cooldownTimer -= deltaTime;

            if(cooldownTimer < 0)
            {
                cooldownTimer = 0;
            }
        }

        public virtual void Draw() { }



    }
}
