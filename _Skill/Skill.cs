using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill
{
    public class Skill
    {
        public string name { get; set; }
        public int damage { get; set; }
        public float cooldown { get; set; }
        public float cooldownTimer { get; private set; }

        public Skill(string name, int damage, float cooldown) 
        {
            this.name = name;
            this.damage = damage;
            this.cooldown = cooldown;
            cooldownTimer = 0;
        }


        public bool CanUse()
        {
            return cooldownTimer <= 0;
        }

        public void Use()
        {
            cooldownTimer = cooldown;
        }

        public void Update(float deltaTime)
        {
            if(cooldownTimer > 0) 
                cooldownTimer -= deltaTime;

            if(cooldownTimer < 0)
            {
                cooldownTimer = 0;
            }
        }


    }
}
