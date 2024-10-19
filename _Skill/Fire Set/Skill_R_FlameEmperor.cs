using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Particles.Emitter;
using Advencursor._Particles;
using Advencursor._SaveData;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Advencursor._Models.Enemy.Stage1;
using Advencursor._Models.Enemy.Stage2;
using Advencursor._Models.Enemy.Stage3;
using Advencursor._Animation;
using System.Diagnostics;

namespace Advencursor._Skill.Fire_Set
{
    public class Skill_R_FlameEmperor : Skill
    {
        private float slashMultiplier;
        private float slashHpPercentAdd;
        private float bombMultiplier;
        private float radius;

        private float countdownTimer;
        private float duration;
        private SpriteEmitter spriteEmitter;
        private StaticEmitter staticEmitter;
        private List<Vector2> litPoint;
        private List<Sprite> litSprite;
        private List<ParticleEmitter> activeEmitters = new List<ParticleEmitter>();
        private List<float> collisionCooldown = new List<float>();
        private bool isBomb;

        private Animation aura;
        private Animation slashTexture;
        private float delayTimer;
        private float delay = 0.01f;
        private bool startAttack;
        Vector2 offset = new Vector2(75, 0);
        private Rectangle slashCollision;

        private ParticleEmitterData ped;
        private ParticleEmitter pe;
        private ParticleEmitter peBomb;

        private Circle circleCollision;

        private Vector2 previousPlayerPosition;
        private Player player;
        public Skill_R_FlameEmperor(string name, SkillData skillData) : base(name, skillData)
        {
            bombMultiplier = skillData.GetMultiplierNumber(name, "Bomb Multiplier"); 
            slashHpPercentAdd = skillData.GetMultiplierNumber(name, "Hp Buff Attack");
            slashMultiplier = skillData.GetMultiplierNumber(name, "Slash Multiplier");
            radius = skillData.GetMultiplierNumber(name, "Radius");
            litPoint = new List<Vector2>();
            litSprite = new List<Sprite>();
            rarity = 4;
            setSkill = "Fire";
            description = "Fully awaken the power of the solar flame within your body, releasing waves of solar flames that deal damage around you and turning your weapon into a powerful flaming sword, making your attacks more powerful and applying the Burning effect on every hit.";
            aura = new Animation(Globals.Content.Load<Texture2D>("Item/SetFire/R_Effect1"),1,8,8,true);
            aura.SetOpacity(0.5f);
            slashTexture = new Animation(Globals.Content.Load<Texture2D>("Item/SetFire/R_Effect2"), 1, 4, 16, false);
            slashTexture.scale = 2f;
        }

        public override void Use(Player player)
        {
            base.Use(player);
            this.player = player;
            isBomb = false;

            ped = new()
            {
                particleData = new ParticleData()
                {
                    colorEnd = Color.Red,
                    colorStart = Color.Orange,
                    opacityStart = 1,
                    opacityEnd = 0,
                    sizeEnd = 4f,
                    sizeStart = 32f,
                    speed = 75,
                    angle = 270,
                    rotation = 0f,
                    rangeMax = 300f,
                },
                angle = 0,
                angleVariance = 15,
                lifeSpanMin = 0.5f,
                lifeSpanMax = 0.5f,
                speedMin = 50f,
                speedMax = 150f,
                rotationMin = 0,rotationMax = 0,
                interval = 0.05f,
                emitCount = 10
            };



            slashTexture.IsFlip = player.isFlip;
            if (slashTexture.IsFlip)
            {
                slashTexture.offset = offset;
            }
            else
            {
                slashTexture.offset = -offset;
            }

            previousPlayerPosition = player.position;

            duration = 15f;
            collisionCooldown = new List<float>(new float[100]);
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);

            if(duration < 14.5f)
            {
                ParticleManager.RemoveParticleEmitter(peBomb);
            }
            if (duration <= 0)
            {
                litSprite.Clear();
                RemoveFire();
                activeEmitters.Clear();
            }
            if (duration > 0)
            {
                duration -= TimeManager.TimeGlobal;
                aura.Update();
                previousPlayerPosition = player.position;
                
                if (!player.CanNormalAttack())
                {
                    startAttack = true;
                }
                if (slashTexture.IsComplete)
                {
                    for (int i = 0; i < collisionCooldown.Count; i++) collisionCooldown[i] = 0;
                    slashCollision = new Rectangle();
                    slashTexture.Reset();
                    delayTimer = delay;
                    startAttack = false;
                }

                slashTexture.IsFlip = player.isFlip;
                if (slashTexture.IsFlip)
                {
                    slashTexture.offset = offset;
                }
                else
                {
                    slashTexture.offset = -offset;
                }

                if (startAttack)
                {
                    delayTimer -= TimeManager.TimeGlobal;
                }

                if(delayTimer <= 0)
                {
                    slashTexture.Update();
                    slashCollision = slashTexture.GetCollision(player.position);
                    for (int i = 0; i < Globals.EnemyManager.Count; i++)
                    {
                        if(Globals.EnemyManager[i].collision.Intersects(slashCollision) && collisionCooldown[i] <= 0)
                        {
                            Globals.EnemyManager[i].TakeDamage(player,(slashHpPercentAdd/100f + player.Status.Attack) * slashMultiplier,true,false,Color.Orange);
                            Globals.EnemyManager[i].burnDuration = 3f;
                            collisionCooldown[i] = 1;
                        }
                    }
                }

                
                
                if (!isBomb)
                {
                    Explode();
                    isBomb = true;
                }

            }
        }

        public override void Draw()
        {
            if (duration > 0)
            {
                aura.Draw(new Vector2(previousPlayerPosition.X,previousPlayerPosition.Y - 75));
                if (delayTimer <= 0)
                {
                    slashTexture.Draw(previousPlayerPosition);
                    //DrawSlahCollision();
                }
            }
        }

        private void LitFire(Sprite litSprite)
        {
            spriteEmitter = new SpriteEmitter(() => litSprite.position);
            pe = new(spriteEmitter, ped);
            ParticleManager.AddParticleEmitter(pe);
            activeEmitters.Add(pe);
        }

        private void RemoveFire()
        {
            foreach (var emitter in activeEmitters)
            {
                ParticleManager.RemoveParticleEmitter(emitter);
            }
        }

        private void Explode()
        {
            circleCollision = new Circle(previousPlayerPosition, radius);
            staticEmitter = new StaticEmitter(previousPlayerPosition);
            ped = new()
            {
                particleData = new ParticleData()
                {
                    lifespan = 1.5f,
                    colorEnd = Color.Red,
                    colorStart = Color.Orange,
                    opacityStart = 1,
                    opacityEnd = 0,
                    sizeEnd = 4f,
                    sizeStart = 32f,
                    speed = 75,
                    angle = 270,
                    rotation = 0f,
                    rangeMax = radius,
                },
                interval = 0.01f,
                emitCount = 100,
                angleVariance = 180f,
                speedMax = 2000,
                speedMin = 2000,
                lifeSpanMax = 1f,
                angle = 270f,
                lifeSpanMin = 0.5f,
                rotationMin = 0,
                rotationMax = 0,
            };
            peBomb = new(staticEmitter, ped);
            ParticleManager.AddParticleEmitter(peBomb);
            activeEmitters.Add(peBomb);
            isBomb = true;
            Globals.Camera.Shake(0.5f, 15);

            for (int i = 0; i < Globals.EnemyManager.Count; i++)
            {
                if (collisionCooldown[i] <= 0)
                {
                    Globals.EnemyManager[i].TakeDamage(bombMultiplier, player, true, false, Color.Orange);
                    collisionCooldown[i] = 1;
                }
            }
        }

        public void DrawSlahCollision()
        {
            Globals.DrawLine(new Vector2(slashCollision.X, slashCollision.Y), new Vector2(slashCollision.X + slashCollision.Width, slashCollision.Y), Color.Purple, 4);
            Globals.DrawLine(new Vector2(slashCollision.X, slashCollision.Y + slashCollision.Height), new Vector2(slashCollision.X + slashCollision.Width, slashCollision.Y + slashCollision.Height), Color.Purple, 4);
        }
    }
}
