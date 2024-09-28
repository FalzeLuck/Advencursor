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
    public class Skill_E_EmergencyFood : Skill
    {
        private float buffTime;
        private float oldAttack;
        private bool isusing = false;

        private Animation auraHeal;
        private Animation auraDamage;
        private Vector2 position;
        public Skill_E_EmergencyFood(string name, float cooldown) : base(name, cooldown)
        {
            rarity = 3;
            setSkill = "Food";
            auraHeal = new Animation(Globals.Content.Load<Texture2D>("Item/SetFood/E_Effect2"), 1, 4, 8, false);
            auraDamage = new Animation(Globals.Content.Load<Texture2D>("Item/SetFood/E_Effect1"), 1, 4, 8, true);
        }

        public override void Use(Player player)
        {
            base.Use(player);
            player.Status.Heal(player.Status.MaxHP * 5 / 100);
            oldAttack = player.Status.Attack;
            player.Status.SetAttack(oldAttack * 1.7f);
            isusing = true;
            buffTime = 5f;
            auraDamage.Reset();
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            buffTime -= deltaTime;
            if (buffTime > 0)
            {
                position = player.position;
                auraHeal.Update();
                auraDamage.Update();
            }


            if (buffTime <= 0f && isusing)
            {
                player.Status.SetAttack(oldAttack);
                isusing = false;
            }
        }

        public override void Draw()
        {
            if (buffTime > 0)
            {
                if (!auraHeal.IsComplete)
                {
                    auraHeal.Draw(position);
                }
                else
                {
                    auraDamage.Draw(position);
                }
            }
        }
    }
}
