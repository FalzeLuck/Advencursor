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
        private List<double> angle = new List<double>();
        private float rotation_speed = 0.15f;
        private List<Vector2> position = new List<Vector2>();
        private const float collisionCooldownTime = 0.5f;
        private List<float> collisionCooldown = new List<float>();

        private float maxDuration = 1.5f;
        private float stayDuration;

        //For Multiplier
        private Player player;
        private float skillMultiplier = 1.2f;
        public Skill_W_ThunderShuriken(string name, float cooldown) : base(name, cooldown)
        {
        }

        public override void Use(Player player)
        {
            base.Use(player);
            isUsing = true;
            stayDuration = 0f;
            int collisionCooldownindex = 0;
            for (int i = 0; i < maxAmount; i++)
            {
                animations.Add(new(Globals.Content.Load<Texture2D>("Animation/LightningShuriken"), 1, 4, TimeManager.framerate, true));
                angle.Add((i * 90 * Math.PI)/180);
                position.Add(Vector2.Zero);
                collisionCooldownindex++;
            }

            collisionCooldown = new List<float>(new float[(Globals.EnemyManager.Count * collisionCooldownindex) + 100]);
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

            for (int i = 0; i < Globals.EnemyManager.Count; i++)
            {
                for (int j = 0; j < Math.Min(animations.Count, position.Count); j++)
                {
                    int index = (animations.Count * i) + j;
                    if (collisionCooldown[index] <= 0)
                    {
                        if (Globals.EnemyManager[i].collision.Intersects(animations[j].GetCollision(position[j])))
                        {
                            Globals.EnemyManager[i].TakeDamage((int)(player.Status.Attack * skillMultiplier), player);
                            Globals.EnemyManager[i].Status.Paralysis(1.5f);
                            collisionCooldown[index] = collisionCooldownTime;
                        }
                    }
                }
            }

            for (int i = 0; i < collisionCooldown.Count; i++)
            {
                collisionCooldown[i] -= TimeManager.TimeGlobal;
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
