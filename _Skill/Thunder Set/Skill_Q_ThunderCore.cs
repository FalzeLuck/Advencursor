using Advencursor._Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill.Thunder_Set
{
    public class Skill_Q_ThunderCore : Skill
    {
        private float buffTime;
        public  Skill_Q_ThunderCore(string name, float cooldown) : base(name, cooldown)
        {
            buffTime = 8f;
        }

        public override void Use(Player player)
        {
            base.Use(player);
            buffTime = 0f;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            buffTime += deltaTime;

            if (buffTime > 0f)
            {
                player.isBuff = true;
                player.buffIndicator = "Thunder_";
            }
            if (buffTime > 8f)
            {
                player.isBuff = false;
                player.buffIndicator = "Normal_";
            }
        }
    }
}
