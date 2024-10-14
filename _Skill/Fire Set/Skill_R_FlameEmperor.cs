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
        private ParticleEmitter peBomb;

        private Circle circleCollision;

        private Vector2 previousPlayerPosition;
        private Player player;
        public Skill_R_FlameEmperor(string name, SkillData skillData) : base(name, skillData)
        {
            multiplier = skillData.GetMultiplierNumber(name, "Damage Multiplier");
            radius = 600;
            litPoint = new List<Vector2>();
            litSprite = new List<Sprite>();
            rarity = 4;
            setSkill = "Fire";
            description = "";
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
                    lifespan = 0.5f,
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



            Texture2D nullTexture = new Texture2D(Globals.graphicsDevice, 1, 1);
            fireLine = new RotatableLine(Globals.graphicsDevice, 25, player.position, 20);
            fireLine.SetRotation(MathHelper.ToRadians(135));
            litPoint = fireLine.GetPointList();


            for (int i = 0; i < litPoint.Count; i++)
            {
                litSprite.Add(new Sprite(nullTexture, player.position));
                litSprite[i].position = litPoint[i];
            }

            foreach (Sprite sprite in litSprite)
            {
                LitFire(sprite);
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
                fireLine.Update(player);

                Vector2 movementDelta = player.position - previousPlayerPosition;

                previousPlayerPosition = player.position;

                for (int i = 0; i < litSprite.Count; i++)
                {
                    fireLine.points[i] += movementDelta;
                    litSprite[i].position = fireLine.GetPointList()[i];
                }

                if (!player.CanNormalAttack())
                {
                    fireLine.SetRotation(MathHelper.ToRadians(45));
                }
                else
                {
                    fireLine.SetRotation(MathHelper.ToRadians(135));
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
                    Globals.EnemyManager[i].TakeDamage(multiplier, player, true, false);
                }
            }
        }
    }
}
