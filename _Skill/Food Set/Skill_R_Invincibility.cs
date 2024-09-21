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
        public Skill_R_Invincibility(string name, float cooldown) : base(name, cooldown)
        {

        }

        public override void Use(Player player)
        {
            base.Use(player);
            player.Immunity(8f);
        }
    }
}
