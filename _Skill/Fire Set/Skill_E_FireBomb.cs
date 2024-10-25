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
using Advencursor._Animation;

namespace Advencursor._Skill.Fire_Set
{
    public class Skill_E_FireBomb : Skill
    {
        private float multiplier;
        private int countdownTick;
        private float countdownInterval;
        private float radius;

        private float countdownTimer;
        private float duration;
        private SpriteEmitter spriteEmitter;
        private StaticEmitter staticEmitter;
        private List<Vector2> litPoint;
        private List<Sprite> litSprite;
        private Animation timer;
        private Texture2D bombTexture;
        private Animation bomb;
        private List<ParticleEmitter> activeEmitters = new List<ParticleEmitter>();
        private List<float> collisionCooldown = new List<float>();
        private bool isBomb;

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private Circle circleCollision;

        private Vector2 previousPlayerPosition;
        public Skill_E_FireBomb(string name, SkillData skillData) : base(name, skillData)
        {
            multiplier = skillData.GetMultiplierNumber(name, "Damage Multiplier");
            countdownInterval = skillData.GetMultiplierNumber(name, "Countdown Interval");
            radius = skillData.GetMultiplierNumber(name, "Radius");
            litPoint = new List<Vector2>();
            litSprite = new List<Sprite>();
            bombTexture = Globals.Content.Load<Texture2D>("Item/SetFire/E_Effect2");
            timer = new Animation(Globals.Content.Load<Texture2D>("Item/SetFire/E_Effect1"), 1, 5, 0, false);
            float scale = (radius * 2) / Math.Min(bombTexture.Width, bombTexture.Height);
            bomb = new Animation(bombTexture, 1, 8, 16, false, scale);
            previousPlayerPosition = new Vector2(-300);
            rarity = 3;
            setSkill = "Fire";
            description = "Accumulates fire energy for 5 seconds and then violently explodes, dealing damage around you. Enemies in range are knocked back and inflicted with the Burning effect.";
        }

        public override void Use(Player player)
        {
            base.Use(player);
            Globals.soundManager.PlaySound("EFire");
            isBomb = false;
            countdownTick = 5;
            countdownTimer = 0;
            timer.offset = new Vector2(-100, 0);
            timer.Reset();
            bomb.Reset();
            ped = new()
            {
                particleData = new ParticleData()
                {
                    sizeStart = 12f,
                    sizeEnd = 10f,
                    colorStart = Color.Red,
                    colorEnd = Color.Yellow,
                },
                interval = 0.01f,
                emitCount = 5,
                angleVariance = 90f,
                speedMax = 100,
                speedMin = 10,
                lifeSpanMax = 1f,
                lifeSpanMin = 0.75f,
            };

            Texture2D nullTexture = new Texture2D(Globals.graphicsDevice, 1, 1);
            litPoint = Globals.CreateCircleOutline(player.position, 150, countdownTick);
            for (int i = 0; i < countdownTick; i++)
            {
                litSprite.Add(new Sprite(nullTexture, player.position));
                litSprite[i].position = litPoint[i];
            }


            previousPlayerPosition = player.position;

            duration = 10f;
            collisionCooldown = new List<float>(new float[100]);
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);



            if (duration <= 0)
            {
                isBomb = false;
                litSprite.Clear();
                RemoveFire();
                activeEmitters.Clear();
            }
            if (duration > 0)
            {
                duration -= TimeManager.TimeGlobal;
                countdownTimer -= TimeManager.TimeGlobal;

                Vector2 movementDelta = player.position - previousPlayerPosition;

                previousPlayerPosition = player.position;

                for (int i = 0; i < litSprite.Count; i++)
                {
                    litSprite[i].position += movementDelta;
                }


                if (countdownTimer <= 0 && countdownTick > 0)
                {
                    timer.currentFrame++;
                    countdownTimer = countdownInterval;
                    LitFire(litSprite[countdownTick - 1]);
                    countdownTick -= 1;
                }
                if(countdownTick <= 0 && !isBomb)
                {
                    circleCollision = new Circle(player.position,radius);
                    staticEmitter = new StaticEmitter(player.position);
                    ped = new()
                    {
                        particleData = new ParticleData()
                        {
                            sizeStart = 64f,
                            sizeEnd = 0f,
                            colorStart = Color.OrangeRed,
                            colorEnd = Color.Yellow,
                            rangeMax = radius,
                        },
                        interval = 0.01f,
                        emitCount = 100,
                        angleVariance = 180f,
                        speedMax = 2000,
                        speedMin = 2000,
                        lifeSpanMax = 0.5f,
                    };
                    pe = new(staticEmitter, ped);
                    ParticleManager.AddParticleEmitter(pe);
                    activeEmitters.Add(pe);
                    isBomb = true;
                    duration = 0.5f;
                    Globals.Camera.Shake(0.5f, 15);
                }
                if (isBomb)
                {
                    bomb.Update();
                }

                for (int i = 0; i < Globals.EnemyManager.Count; i++)
                {
                    if (collisionCooldown[i] <= 0 && countdownTick <= 0)
                    {
                        if (circleCollision.Intersects(Globals.EnemyManager[i].collision))
                        {
                            Globals.EnemyManager[i].TakeDamage(multiplier, player, true, false, Color.Orange);
                            collisionCooldown[i] = 3f;
                        }
                    }
                }

                for (int i = 0; i < Globals.EnemyManager.Count; i++)
                {
                    if (countdownTick <= 0)
                    {
                        if (circleCollision.Intersects(Globals.EnemyManager[i].collision))
                        {
                            if (!(Globals.EnemyManager[i] is Boss1) && !(Globals.EnemyManager[i] is Boss2) && !(Globals.EnemyManager[i] is Boss3))
                            {
                                var dir = Globals.EnemyManager[i].position - player.position;
                                dir.Normalize();
                                Globals.EnemyManager[i].position += dir * 1000 * TimeManager.TimeGlobal;
                            }
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            timer.Draw(previousPlayerPosition);

            if (isBomb)
            {
                bomb.Draw(previousPlayerPosition);
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
    }
}
