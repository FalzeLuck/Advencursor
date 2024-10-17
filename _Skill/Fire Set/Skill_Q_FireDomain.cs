using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Models.Enemy;
using Advencursor._Particles;
using Advencursor._Particles.Emitter;
using Advencursor._SaveData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill.Fire_Set
{
    public class Skill_Q_FireDomain : Skill
    {
        private float radius;
        private float duration;
        private float multiplier;
        private float healInterval;
        private float healPercentage;
        private float healTick = 0f;
        private float skillDuration;

        private StaticEmitter staticEmitter;
        private List<Vector2> litPoint;
        private List<ParticleEmitter> activeEmitters = new List<ParticleEmitter>();
        private Texture2D floorTexture;
        private Animation floorAnim;
        private Vector2 litPosition;
        private ParticleEmitterData ped;
        private ParticleEmitter pe;
        private float oldAmp;
        private float newAmp;

        private Circle circleCollision;
        public Skill_Q_FireDomain(string name, SkillData skillData) : base(name, skillData)
        {
            radius = skillData.GetMultiplierNumber(name, "Radius");
            duration = skillData.GetMultiplierNumber(name, "Duration");
            multiplier = skillData.GetMultiplierNumber(name, "Damage Amplifier");
            healInterval = skillData.GetMultiplierNumber(name, "Heal Interval");
            healPercentage = skillData.GetMultiplierNumber(name, "Heal Percentage");
            circleCollision = new(Vector2.Zero, 0);
            floorTexture = Globals.Content.Load<Texture2D>("Item/SetFire/Q_Effect");
            float scale = (radius * 2) / Math.Min(floorTexture.Width, floorTexture.Height);
            floorAnim = new Animation(floorTexture, 1, 8, 8, true,scale);
            rarity = 1;
            setSkill = "Fire";
            description = "Create a circle of fiery fire around you. Turns an area into a firestorm. Enemies that enter are bound by flames. Causes more damage to be received. When the duration ends, the bound fire is released.";
        }

        public override void Use(Player player)
        {
            base.Use(player);
            litPoint = Globals.CreateCircleOutline(player.position, radius, 16);
            circleCollision = new Circle(player.position, radius);
            litPosition = player.position;
            skillDuration = duration;
            
            ped = new()
            {
                particleData = new ParticleData()
                {
                    lifespan = 1f,
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
                angleVariance = 10,
                lifeSpanMin = 0.5f,
                lifeSpanMax = 1.0f,
                speedMin = 100f,
                speedMax = 150f,
                rotationMin = 0,
                rotationMax = 0,
                interval = 0.05f,
                emitCount = 10
            };
            for (int i = 0; i < litPoint.Count; i++)
            {
                LitFire(litPoint[i]);
            }
            
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            skillDuration -= deltaTime;
            if (skillDuration <= 0)
            {
                if (litPoint != null)
                {
                    RemoveFire();

                }
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.ampMultiplier = 0;
                }
                activeEmitters.Clear();
            }

            if (skillDuration > 0)
            {
                healTick -= TimeManager.TimeGlobal;
                floorAnim.Update();
                if (circleCollision.Intersects(player.collision) && healTick <= 0)
                {
                    player.Status.Heal((healPercentage / 100) * player.Status.MaxHP);
                    healTick = healInterval;
                }
                foreach (var enemy in Globals.EnemyManager)
                {
                    if (circleCollision.Intersects(enemy.collision))
                    {
                        enemy.ampMultiplier = multiplier;
                    }
                }
            }

        }

        public override void Draw()
        {
            if(skillDuration > 0)
            {
                floorAnim.Draw(litPosition);
            }
        }

        private void LitFire(Vector2 litPosition)
        {
            staticEmitter = new StaticEmitter(litPosition);
            pe = new(staticEmitter, ped);
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
