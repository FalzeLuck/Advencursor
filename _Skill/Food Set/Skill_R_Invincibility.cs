using Advencursor._Animation;
using Advencursor._Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private Animation aura;
        private Animation auraDamage;
        private Vector2 position;
        public Skill_R_Invincibility(string name, float cooldown) : base(name, cooldown)
        {
            rarity = 4;
            setSkill = "Food";
            aura = new Animation(Globals.Content.Load<Texture2D>("Item/SetFood/R_Effect"), 1, 4, 8, true);
            auraDamage = new Animation(Globals.Content.Load<Texture2D>("Item/SetFood/E_Effect1"), 2, 4, 2, 8, true);
        }

        public override void Use(Player player)
        {
            base.Use(player);
            player.Status.Heal(player.Status.MaxHP * 50 / 100);
            oldAttack = player.Status.Attack;
            oldCritRate = player.Status.CritRate;
            oldCritDam = player.Status.CritDam;
            player.Status.SetAttack(oldAttack + 800);
            player.Status.SetCritRate(oldCritRate + 20);
            player.Status.SetCritDamage(oldCritDam + 200);
            isusing = true;
            buffTime = 15f;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            buffTime -= deltaTime;
            if (buffTime > 0)
            {
                position = player.position;
                aura.Update();
                auraDamage.Update();
            }
            else if (buffTime <= 0f && isusing)
            {
                
                player.Status.SetAttack(oldAttack);
                player.Status.SetCritRate(oldCritRate);
                player.Status.SetCritDamage(oldCritDam);
                isusing = false;
            }
        }

        public override void Draw()
        {
            if (buffTime > 0)
            {
                //auraDamage.Draw(position);
                aura.Draw(position);
            }
        }
    }
}
