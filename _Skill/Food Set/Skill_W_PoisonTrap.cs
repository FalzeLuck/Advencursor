using Advencursor._AI;
using Advencursor._Animation;
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
            rarity = 2;
            setSkill = "Food";
            description = "\"Haha, it's a trap!\" Place a poison trap on the ground. When enemies step in, enemies will be rapidly consumed by the poison!";
            buffTime = 0f;
            poisonFood = new(Globals.Content.Load<Texture2D>("Item/SetFood/W_Texture"), Vector2.Zero);
            poisonFood.animations["base"] = new Animation(Globals.Content.Load<Texture2D>("Item/SetFood/W_Effect"),1,8,8,true);
        }

        public override void Use(Player player)
        {
            base.Use(player);
            poisonFood.position = player.position;
            poisonFood.SetOpacity(0.9f);
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
                foreach (var item in poisonFood.animations.Values)
                {
                    item.Update();
                }
                for (int i = 0; i < Globals.EnemyManager.Count; i++)
                {
                    if (collisionCooldown[i] <= 0)
                    {
                        if (Globals.EnemyManager[i].collision.Intersects(poisonFood.collision))
                        {
                            Globals.EnemyManager[i].TakeDamage(1, player, (Globals.EnemyManager[i].Status.MaxHP / 100) + 500,true,true);
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
