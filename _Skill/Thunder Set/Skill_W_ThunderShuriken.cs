using Advencursor._Animation;
using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Models.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advencursor._Skill.Thunder_Set
{
    public class Skill_W_ThunderShuriken : Skill
    {
        private List<Animation> animations = new List<Animation>();

        private float buffTime;
        

        private int maxAmount = 4;
        private int currentAmount = 0;

        private bool isUsing = false;

        private float radius = 200f;
        private List<float> angle = new List<float>();
        private float rotation_speed = 0.05f;
        private List<Vector2> position = new List<Vector2>();
        private const float collisionCooldownTime = 0.5f;

        private float maxDuration = 1.5f;
        private float stayDuration;

        //For Multiplier
        private Player player;
        private float skillMultiplier = 1.2f;
        public Skill_W_ThunderShuriken(string name, float cooldown) : base(name, cooldown)
        {
        }

        public override void Use()
        {
            base.Use();
            isUsing = true;
            stayDuration = 0f;
            for (int i = 0; i < maxAmount; i++)
            {
                animations.Add(new(Globals.Content.Load<Texture2D>("Animation/LightningShuriken"), 1, 2, TimeManager.framerate, true));
                angle.Add(i * 80f);
                position.Add(Vector2.Zero);
            }
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            if (isUsing)
            {
                stayDuration += deltaTime;
                if (stayDuration < maxDuration)
                {
                    for (int i = 0; i < maxAmount; i++)
                    {
                        animations[i].Update();
                        angle[i] += rotation_speed;

                        if (angle[i] > MathHelper.TwoPi)
                        {
                            angle[i] -= MathHelper.TwoPi;
                        }

                        position[i] = new Vector2(
                            player.position.X + radius * (float)Math.Cos(angle[i]),
                            player.position.Y + radius * (float)Math.Sin(angle[i])
                            );
                    }
                }
                else
                {
                    isUsing = false;
                }
            }

            if (!isUsing)
            {
                animations.Clear();
                angle.Clear();
                position.Clear();
            }

            this.player = player;
            CheckCollision();
        }

        private void CheckCollision()
        {
            float deltaTime = TimeManager.TotalSeconds;

            for (int i = 0; i < Math.Min(animations.Count, position.Count); i++)
            {
                if (i >= maxAmount) break;


                foreach (var enemy in Globals.EnemyManager)
                {

                    bool isColliding = enemy.collision.Intersects(animations[i].GetCollision(position[i]));
                    if (isColliding)
                    {
                        if (enemy.collisionCooldown <= 0)
                        {
                            enemy.TakeDamage((int)(player.Status.Attack * skillMultiplier), player);
                            enemy.Status.Paralysis(1.5f);
                            enemy.CollisionCooldownReset(collisionCooldownTime);
                        }
                    }
                }
                
                
            }

        }

        public override void Draw()
        {
            if (isUsing)
            {
                for (int i = 0; i < maxAmount; i++)
                {
                    animations[i].Draw(position[i]);
                }
            }
            
        }
    }
}
