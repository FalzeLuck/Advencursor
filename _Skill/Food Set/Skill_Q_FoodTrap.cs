using Advencursor._AI;
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
    public class Skill_Q_FoodTrap : Skill
    {
        private float buffTime;
        private Sprite tauntFood;
        public Skill_Q_FoodTrap(string name, float cooldown) : base(name, cooldown)
        {
            buffTime = 0f;
            tauntFood = new(Globals.Content.Load<Texture2D>("Item/SetThunder/R_Texture"), Vector2.Zero);
        }

        public override void Use(Player player)
        {
            base.Use(player);
            tauntFood.position = player.position;
            buffTime = 5f;
            
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);

            if(buffTime > 0)
            {
                buffTime -= deltaTime;
                tauntFood.CollisionUpdate();
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.movementAI = new FollowMovementAI()
                    {
                        target = tauntFood,
                    };
                }
            }
            else
            {
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.movementAI = new FollowMovementAI()
                    {
                        target = player,
                    };
                }
            }
        }

        public override void Draw()
        {
            if(buffTime > 0)
            tauntFood.Draw();
        }
    }
}
