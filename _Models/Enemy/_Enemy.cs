﻿using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Particles;
using Advencursor._Particles.Emitter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy
{
    public abstract class _Enemy : Sprite
    {

        public MovementAI movementAI { get; set; }
        public Rectangle parryZone;
        public bool isAttacking;
        protected Texture2D shadowTexture;

        public bool isAmp = false;
        public float ampMultiplier = 0f;
        public float baseAmp = 1f;
        public float burnDuration = 0f;
        public bool isBurn => burnDuration > 0;
        private bool isBurningParticle = false;
        private SpriteEmitter spriteEmitter;
        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        public float collisionCooldown { get; set; }

        public _Enemy(Texture2D texture, Vector2 position, int health, int attack) : base(texture, position)
        {
            Status = new(health, attack);
            animations = new Dictionary<string, Animation>();
            ped = new()
            {
                particleData = new ParticleData()
                {
                    lifespan = 1f,
                    colorEnd = Color.Red,
                    colorStart = Color.Orange,
                    opacityStart = 0.4f,
                    opacityEnd = 0,
                    sizeEnd = 12f,
                    sizeStart = 48f,
                    speed = 75,
                    angle = 270,
                    rotation = 0f,
                    rangeMax = 300f,
                },
                angleVariance = 30,
                lifeSpanMin = 0.5f,
                lifeSpanMax = 1.0f,
                speedMin = 100f,
                speedMax = 150f,
                rotationMin = 0,
                rotationMax = 0,
                interval = 0.1f,
                emitCount = 10
            };
            spriteEmitter = new SpriteEmitter(() => this.position);
            pe = new(spriteEmitter, ped);
        }

        public override void Update(GameTime gameTime)
        {
            collisionCooldown -= TimeManager.TimeGlobal;
            burnDuration -= TimeManager.TimeGlobal;
            Vector2 playerPosition = new(InputManager._mousePosition.X, InputManager._mousePosition.Y);
            FlipAuto(playerPosition);
        }

        public void FlipAuto(Vector2 playerPosition,bool reverse = false)
        {
            if (!reverse)
            {
                if (playerPosition.X > position.X)
                {
                    foreach (var anim in animations.Values)
                    {
                        anim.IsFlip = false;
                    }
                }
                else
                {
                    foreach (var anim in animations.Values)
                    {
                        anim.IsFlip = true;
                    }
                }
            }
            else
            {
                if (playerPosition.X < position.X)
                {
                    foreach (var anim in animations.Values)
                    {
                        anim.IsFlip = false;
                    }
                }
                else
                {
                    foreach (var anim in animations.Values)
                    {
                        anim.IsFlip = true;
                    }
                }
            }
        }

        public void UpdateParryZone()
        {
            parryZone = collision;
            int increaseamount = 300;
            int newX = parryZone.X - increaseamount / 2;
            int newY = parryZone.Y - increaseamount / 2;
            int newWidth = parryZone.Width + increaseamount;
            int newHeight = parryZone.Height + increaseamount;
            parryZone = new Rectangle(newX, newY, newWidth, newHeight);
        }

        public void DrawBurn()
        {
            if (isBurn && !isBurningParticle)
            {
                ParticleManager.AddParticleEmitter(pe);
                isBurningParticle = true;
            }
            else if (!isBurn || !Status.IsAlive())
            {
                ParticleManager.RemoveParticleEmitter(pe);
                isBurningParticle= false;
            }
        }

        public void RemoveBurn()
        {
            isBurningParticle = true;
            ParticleManager.RemoveParticleEmitter(pe);
        }
        public void BurnDamage(Sprite player)
        {
            if (isBurn)
            {
                if (player is Player)
                {
                    Status.TakeDamageNoCrit(player.Status.Attack, player, Color.Orange,"Burning ");
                    burnDuration = 0;
                }
            }
        }
        public virtual void TakeDamage(float fixedDamage, Sprite fromwho,Color color = default)
        {
            Status.TakeDamageNoCrit(fixedDamage * (baseAmp + ampMultiplier), fromwho,color);
            BurnDamage(fromwho);
        }

        public virtual void TakeDamage(float multiplier, Player player, bool throughImmune = false, bool NoCrit = false,Color color = default)
        {
            if (throughImmune)
            {
                Status.TakeDamageNoImmune(multiplier * player.Status.Attack * (baseAmp + ampMultiplier), player,NoCrit,color);
            }
            else
            {
                Status.TakeDamage(multiplier * player.Status.Attack * (baseAmp + ampMultiplier), player,color);
            }
            BurnDamage(player);
        }

        public virtual void TakeDamage(Player player, float fixedDamage, bool throughImmune = false, bool NoCrit = false, Color color = default)
        {
            if (throughImmune)
            {
                Status.TakeDamageNoImmune(fixedDamage * (baseAmp + ampMultiplier), player , NoCrit,color);
            }
            else
            {
                if (NoCrit)
                {
                    Status.TakeDamageNoCrit(fixedDamage * (baseAmp + ampMultiplier), player,Color.White);
                }
                else
                    Status.TakeDamage(fixedDamage * (baseAmp + ampMultiplier), player, color);
            }
            BurnDamage(player);
        }

        

        public virtual void CollisionCooldownReset(float timer)
        {
            collisionCooldown = timer;
        }

        public virtual void Die()
        {
            indicator = "Die";
        }

    }
}
