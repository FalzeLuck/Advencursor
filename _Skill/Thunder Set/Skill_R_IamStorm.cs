using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Particles.Emitter;
using Advencursor._Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Advencursor._Combat;
using Microsoft.Xna.Framework.Audio;
using Advencursor._SaveData;

namespace Advencursor._Skill.Thunder_Set
{
    public class Skill_R_IamStorm : Skill
    {
        private SpriteEmitter spriteEmitter;
        private SoundManager soundManager = new SoundManager();

        private ParticleEmitterData lightningPed;
        private ParticleEmitterData ped1;
        private ParticleEmitter pe;
        private ParticleEmitter pe1;

        private float skillTime;
        private bool isUsing = false;

        //Star Shape
        private Vector2[] starPoints;
        private int[] starOrder = { 0, 2, 4, 1, 3 };
        private int currentPoint;
        private float speed = 10000;
        Vector2 targetPoint;

        Texture2D star1;
        Texture2D star2;
        Vector2 starOrigin;

        //For Multiplier
        private Player player;
        private float skillMultiplier = 1f;
        private int maxHit = 9;
        private int countHit = 0;
        public Skill_R_IamStorm(string name, SkillData skillData) : base(name, skillData)
        {
            skillMultiplier = skillData.GetMultiplierNumber(name, "Damage Multiplier");
            maxHit = (int)skillData.GetMultiplierNumber(name, "Max Hit");
            rarity = 4;
            setSkill = "Thunder";
            description = "As the power of sharp judgment, I will slash every enemy in the area. Inflicts all enemies with massive damage and inflict Paralysis status for a short period.";
            star1 = Globals.Content.Load<Texture2D>("Item/SetThunder/R_Thunder_1");
            star2 = Globals.Content.Load<Texture2D>("Item/SetThunder/R_Thunder_2");
            starOrigin =  new Vector2(star1.Width / 2, star1.Height / 2);

            SoundEffect slash = Globals.Content.Load<SoundEffect>("Sound/Effect/Knife Swing");
            soundManager.LoadSound("Slash", slash);
        }

        public override void Use(Player player)
        {
            base.Use(player);
            foreach (var enemy in Globals.EnemyManager)
            {
                enemy.TakeDamage(skillMultiplier, player,(enemy.Status.MaxHP*15)/100,true,true);
                enemy.Status.Paralysis(5f);
            }

            lightningPed = new()
            {
                particleData = new LightningParticleData()
                {
                    sizeStart = 150,
                    sizeEnd = 150,
                    opacityEnd = 0,
                },
                interval = 0.0001f,
                emitCount = 1,
                angleVariance = 180f,
                speedMax = 0f,
                speedMin = 0f,
                lifeSpanMax = 0.5f,
                lifeSpanMin = 0.5f,
                rotationMax = 360f,
            };

            ped1 = new()
            {
                particleData = new LightningParticleData()
                {
                    sizeStart = 150,
                    sizeEnd = 150,
                    colorStart = Color.Yellow,
                    opacityEnd=0,
                },
                interval = 0.0001f,
                emitCount = 1,
                angleVariance = 180f,
                speedMax = 0f,
                speedMin = 0f,
                lifeSpanMax = 0.5f,
                lifeSpanMin = 0.5f,
                rotationMax = 360f,
            };

            this.player = player;
            spriteEmitter = new SpriteEmitter(() => player.position);

            pe = new(spriteEmitter, lightningPed);
            pe1 = new(spriteEmitter, ped1);
            ParticleManager.AddParticleEmitter(pe);
            ParticleManager.AddParticleEmitter(pe1);
            TimeManager.ChangeGameSpeed(0.01f);

            //Star
            currentPoint = 0;
            Vector2 center = new Vector2(Globals.Viewport.Width / 2, Globals.Viewport.Height / 2);
            float radius = 800f;

            starPoints = CalculateStar(center, radius);

            player.position = starPoints[starOrder[0]];
            targetPoint = starPoints[starOrder[1]];

            skillTime = 3f;
            countHit = 0;
            player.Immunity(skillTime);
            player.Stop();
            
            isUsing = true;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);

            if (isUsing)
            {
                skillTime -= TimeManager.TotalSeconds;
                
                Vector2 dir = targetPoint - player.position;
                float distance = dir.Length();

                if (starOrder[currentPoint] != 3)
                {

                    if (distance > 100f)
                    {
                        dir.Normalize();
                        player.position += dir * speed * TimeManager.TotalSeconds;
                    }
                    else
                    {
                        currentPoint = (currentPoint + 1) % starOrder.Length;
                        targetPoint = starPoints[starOrder[currentPoint]];
                    }
                }
                else
                {
                    dir.Normalize();
                    if (skillTime > 2f)
                    {
                        player.position += dir * speed * TimeManager.TotalSeconds;
                    }
                    if(distance<100f)
                        targetPoint = starPoints[starOrder[0]];
                }
                
                
                if(skillTime <= 1f && countHit < maxHit)
                {
                    soundManager.PlaySoundCanStack("Slash");
                    foreach (var enemy in Globals.EnemyManager)
                    {
                        enemy.TakeDamage( 2, player,true,false);
                    }
                    Globals.Camera.Shake(0.2f,5f);
                    countHit++;
                }
                else if (skillTime <= 2f && countHit < maxHit)
                {
                    int randomRange = 300;
                    float randomX = Globals.RandomFloat(Globals.Bounds.X / 2 - randomRange, Globals.Bounds.X / 2 + randomRange);
                    float randomY = Globals.RandomFloat(Globals.Bounds.Y / 2 - randomRange, Globals.Bounds.Y / 2 + randomRange);
                    player.position = new Vector2(randomX, randomY);
                    ParticleManager.RemoveParticleEmitter(pe);
                    ParticleManager.RemoveParticleEmitter(pe1);
                }


                if (skillTime <= 0)
                {
                    isUsing = false;
                    player.Immunity(0.5f);
                    player.position = new Vector2(Globals.Viewport.Width / 2, Globals.Viewport.Height / 2);
                    Mouse.SetPosition((int)(player.position.X), (int)(player.position.Y));
                }
            }

            if (!isUsing)
            {
                player.Start();
                TimeManager.ChangeGameSpeed(1f);
            }

        }

        public override void Draw()
        {
            if (isUsing && skillTime <= 2f)
            {
                Globals.SpriteBatch.Draw(star1,  new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2) , null, Color.White, 0, starOrigin, 1, SpriteEffects.None, 0f);
            }
            if (isUsing && skillTime <= 1f)
            {
                Globals.SpriteBatch.Draw(star2, new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2), null, Color.White, 0, starOrigin, 1, SpriteEffects.None, 0f);
            }
        }

        public Vector2[] CalculateStar(Vector2 center, float radius)
        {
            Vector2[] points = new Vector2[5];
            float angle = MathHelper.PiOver2; // Start at the top of the star
            float angleStep = MathHelper.TwoPi / 5; // Full circle divided by 5 points

            for (int i = 0; i < 5; i++)
            {
                points[i] = new Vector2(
                    center.X + (float)Math.Cos(angle) * radius,
                    center.Y - (float)Math.Sin(angle) * radius
                );

                angle += angleStep;
            }

            return points;
        }
    }
}
