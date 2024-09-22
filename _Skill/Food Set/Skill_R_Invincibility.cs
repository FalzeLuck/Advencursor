using Advencursor._Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill.Food_Set
{
    public class Skill_R_Invincibility : Skill
    {
        private float buffTime;
        private float oldAttack;
        private float oldCritRate;
        private float oldCritDam;
        private bool isusing = false;
        public Skill_R_Invincibility(string name, float cooldown) : base(name, cooldown)
        {

        }

        public override void Use(Player player)
        {
            base.Use(player);
            player.Status.Heal(player.Status.MaxHP * 50 / 100);
            oldAttack = player.Status.Attack;
            oldCritRate = player.Status.CritRate;
            oldCritDam = player.Status.CritDam;
            player.Status.SetAttack(oldAttack + 800);
            isusing = true;
            buffTime = 15f;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            buffTime -= deltaTime;

            if (buffTime <= 0f && isusing)
            {
                player.Status.SetAttack(oldAttack);
                player.Status.SetCritRate(oldCritRate);
                player.Status.SetCritDamage(oldCritDam);
                isusing = false;
            }
        }
    }
}
