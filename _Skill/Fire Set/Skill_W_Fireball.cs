using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Particles.Emitter;
using Advencursor._Particles;
using Advencursor._SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Advencursor._Models.Enemy;

namespace Advencursor._Skill.Fire_Set
{
    public class Skill_W_Fireball : Skill
    {
        private int ballAmount;
        private float ballSpeed;
        private float multiplier;


        private float duration;
        private SpriteEmitter spriteEmitter;
        private List<Vector2> litPoint;
        private List<Sprite> litSprite;
        private List<ParticleEmitter> activeEmitters = new List<ParticleEmitter>();
        private List<float> collisionCooldown = new List<float>();

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private List<Circle> circleCollision;
        public Skill_W_Fireball(string name, SkillData skillData) : base(name, skillData)
        {
            ballAmount = (int)skillData.GetMultiplierNumber(name, "Ball Amount");
            ballSpeed = skillData.GetMultiplierNumber(name, "Speed");
            multiplier = skillData.GetMultiplierNumber(name, "Damage Multiplier");
            circleCollision = new List<Circle>(new Circle[ballAmount]);
            litPoint = new List<Vector2>();
            litSprite = new List<Sprite>();
            rarity = 2;
            setSkill = "Fire";
            description = "Releases small fireballs that shoot out in a circle, like a meteor shower of fire falling from the sky, dealing Burning damage to enemies.";
        }

        public override void Use(Player player)
        {
            base.Use(player);
            Globals.soundManager.PlaySound("WFire");

            ped = new()
            {
                particleData = new ParticleData()
                {
                    sizeStart = 64f,
                    sizeEnd = 4f,
                    colorStart = Color.Orange,
                    colorEnd = Color.Red,
                },
                interval = 0.01f,
                emitCount = 10,
                angleVariance = 180f,
                speedMax = 100,
                speedMin = 10,
                lifeSpanMax = 1,
                lifeSpanMin = 0.5f,
            };

            Texture2D nullTexture = new Texture2D(Globals.graphicsDevice, 1, 1);
            litPoint = Globals.CreateCircleOutline(player.position, 2000, ballAmount);
            for (int i = 0; i < ballAmount; i++)
            {
                litSprite.Add(new Sprite(nullTexture, player.position));
                LitFire(litSprite[i]);
            }

            duration = 4f;
            collisionCooldown = new List<float>(new float[100]);
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            if(duration <= 0)
            {
                litSprite.Clear();
                RemoveFire();
                activeEmitters.Clear();
            }
            if (duration > 0)
            {
                duration -= TimeManager.TimeGlobal;

                for(int i = 0; i < ballAmount; i++)
                {
                    Vector2 dir = litPoint[i] - litSprite[i].position;
                    dir.Normalize();

                    litSprite[i].position += dir * ballSpeed * TimeManager.TimeGlobal;
                }

                for (int i = 0; i < ballAmount; i++)
                {
                    circleCollision[i] = new Circle(litSprite[i].position,75);
                }

                for (int i = 0; i < Globals.EnemyManager.Count; i++)
                {
                    if (collisionCooldown[i] <= 0)
                    {
                        if (circleCollision.Any(circle => circle.Intersects(Globals.EnemyManager[i].collision)))
                        {
                            Globals.EnemyManager[i].TakeDamage(multiplier,player,true,false, Color.Orange);
                            Globals.EnemyManager[i].burnDuration = 5f;
                            collisionCooldown[i] = 0.5f;
                        }
                    }
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
    }
}
