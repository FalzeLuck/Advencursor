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

namespace Advencursor._Skill.Fire_Set
{
    public class Skill_R_FlameEmperor : Skill
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
        private RotatableLine fireLine;
        private List<ParticleEmitter> activeEmitters = new List<ParticleEmitter>();
        private List<float> collisionCooldown = new List<float>();
        private bool isBomb;

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private Circle circleCollision;

        private Vector2 previousPlayerPosition;
        private Player player;
        public Skill_R_FlameEmperor(string name, SkillData skillData) : base(name, skillData)
        {
            multiplier = skillData.GetMultiplierNumber(name, "Damage Multiplier");
            litPoint = new List<Vector2>();
            litSprite = new List<Sprite>();
            rarity = 4;
            setSkill = "Fire";
            description = "Accumulates fire energy for 5 seconds and then violently explodes, dealing damage around you. Enemies in range are knocked back and inflicted with the Burning effect.";
        }

        public override void Use(Player player)
        {
            base.Use(player);
            this.player = player;
            isBomb = false;
            countdownTick = 5;
            countdownTimer = 0;
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
                lifeSpanMax = 1,
                lifeSpanMin = 0.5f,
            };

            Texture2D nullTexture = new Texture2D(Globals.graphicsDevice, 1, 1);
            fireLine = new RotatableLine(Globals.graphicsDevice,100,player.position,2);
            fireLine.SetRotation(-45);
            litPoint = fireLine.GetPointList();
            for (int i = 0; i < litPoint.Count; i++)
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
                litSprite.Clear();
                RemoveFire();
                activeEmitters.Clear();
            }
            if (duration > 0)
            {
                duration -= TimeManager.TimeGlobal;

                for (int i = 0; i < litSprite.Count; i++)
                {
                    litSprite[i].position = fireLine.points[i];
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
            if(fireLine != null)
                fireLine.Draw(Globals.SpriteBatch);
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
                lifeSpanMax = 1f,
            };
            pe = new(staticEmitter, ped);
            ParticleManager.AddParticleEmitter(pe);
            activeEmitters.Add(pe);
            isBomb = true;
            duration = 0.5f;
            Globals.Camera.Shake(0.5f, 15);

            for (int i = 0; i < Globals.EnemyManager.Count; i++)
            {
                if (collisionCooldown[i] <= 0 && countdownTick <= 0)
                {
                    if (circleCollision.Intersects(Globals.EnemyManager[i].collision))
                    {
                        Globals.EnemyManager[i].TakeDamage(multiplier, player, true, false);
                        collisionCooldown[i] = 3f;
                    }
                }
            }
        }
    }
}
