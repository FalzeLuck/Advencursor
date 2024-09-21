using Advencursor._Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill.Food_Set
{
    public class Skill_E_EmergencyFood : Skill
    {
        private float buffTime;
        private float oldAttack;
        private bool isusing = false;
        public Skill_E_EmergencyFood(string name, float cooldown) : base(name, cooldown)
        {
            
        }

        public override void Use(Player player)
        {
            base.Use(player);
            player.Status.Heal(player.Status.MaxHP * 20 / 100);
            oldAttack = player.Status.Attack;
            player.Status.SetAttack(oldAttack * 1.5f);
            isusing = true;
            buffTime = 5f;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            buffTime -= deltaTime;

            if (buffTime <= 0f && isusing)
            {
                player.Status.SetAttack(oldAttack);
                isusing = false;
            }
        }
    }
}
