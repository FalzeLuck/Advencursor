﻿using Advencursor._Managers;
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

namespace Advencursor._Skill.Thunder_Set
{
    public class Skill_R_IamStorm : Skill
    {
        private SpriteEmitter spriteEmitter;

        private ParticleEmitterData lightningPed;
        private ParticleEmitter pe;

        private float skillTime;
        private bool isUsing = false;

        //Star Shape
        private Vector2[] starPoints;
        private int[] starOrder = { 0, 2, 4, 1, 3 };
        private int currentPoint;
        private float speed = 10000;
        Vector2 targetPoint;

        //For Multiplier
        private Player player;
        private float skillMultiplier = 4f;
        public Skill_R_IamStorm(string name, float cooldown) : base(name, cooldown)
        {
            
        }

        public override void Use(Player player)
        {
            base.Use(player);
            lightningPed = new()
            {
                particleData = new LightningParticleData()
                {
                    sizeStart = 150,
                    sizeEnd = 150,
                },
                interval = 0.0005f,
                emitCount = 1,
                angleVariance = 180f,
                speedMax = 0f,
                speedMin = 0f,
                lifeSpanMax = 1f,
                lifeSpanMin = 0.5f,
                rotationMax = 360f,
            };

            this.player = player;
            spriteEmitter = new SpriteEmitter(() => player.position);

            pe = new(spriteEmitter, lightningPed);
            ParticleManager.AddParticleEmitter(pe);
            TimeManager.ChangeGameSpeed(0.2f);

            //Star
            currentPoint = 0;
            Vector2 center = new Vector2(Globals.Viewport.Width / 2, Globals.Viewport.Height / 2);
            float radius = 800f;

            starPoints = CalculateStar(center, radius);

            player.position = starPoints[starOrder[0]];
            targetPoint = starPoints[starOrder[1]];

            skillTime = 3f;
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
                    player.position += dir * speed * TimeManager.TotalSeconds;
                    if(distance<100f)
                        targetPoint = starPoints[starOrder[0]];
                }
                

                
                

                
                if (skillTime <= 0)
                {
                    isUsing = false;
                    ParticleManager.RemoveParticleEmitter(pe);
                    foreach (var enemy in Globals.EnemyManager)
                    {
                        enemy.TakeDamage((int)(player.Status.Attack * skillMultiplier),player);
                    }
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
