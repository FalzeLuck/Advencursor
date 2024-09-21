using Advencursor._AI;
using Advencursor._Managers;
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
    public class Skill_W_PoisonTrap : Skill
    {
        private float buffTime;
        private Sprite poisonFood;

        private List<float> collisionCooldown = new List<float>();
        public Skill_W_PoisonTrap(string name, float cooldown) : base(name, cooldown)
        {
            buffTime = 0f;
            poisonFood = new(Globals.Content.Load<Texture2D>("Item/SetThunder/W_Texture"), Vector2.Zero);
        }

        public override void Use(Player player)
        {
            base.Use(player);
            poisonFood.position = player.position;
            buffTime = 3f;

            collisionCooldown = new List<float>(new float[100]);
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);

            if (buffTime > 0)
            {
                buffTime -= deltaTime;
                poisonFood.CollisionUpdate();
                for (int i = 0; i < Globals.EnemyManager.Count; i++)
                {
                    if (collisionCooldown[i] <= 0)
                    {
                        if (Globals.EnemyManager[i].collision.Intersects(poisonFood.collision))
                        {
                            Globals.EnemyManager[i].TakeDamage(1, player);
                            collisionCooldown[i] = 1f;
                        }
                    }
                }

                for (int i = 0; i < collisionCooldown.Count; i++)
                {
                    collisionCooldown[i] -= TimeManager.TimeGlobal;
                }
            }
        }

        public override void Draw()
        {
            if (buffTime > 0)
                poisonFood.Draw();
        }
    }
}
