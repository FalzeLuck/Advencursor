using Advencursor._AI;
using Advencursor._Managers;
using Advencursor._Models;
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

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private Circle circleCollision;
        public Skill_Q_FireDomain(string name, SkillData skillData) : base(name, skillData)
        {
            radius = skillData.GetMultiplierNumber(name, "Radius");
            duration = skillData.GetMultiplierNumber(name, "Duration");
            multiplier = skillData.GetMultiplierNumber(name, "Damage Amplifier");
            healInterval = skillData.GetMultiplierNumber(name, "Heal Interval");
            healPercentage = skillData.GetMultiplierNumber(name, "Heal Percentage");
            circleCollision = new(Vector2.Zero, 0);
            rarity = 1;
            setSkill = "Fire";
            description = "Create a circle of fiery fire around you. Turns an area into a firestorm. Enemies that enter are bound by flames. Causes more damage to be received. When the duration ends, the bound fire is released.";
        }

        public override void Use(Player player)
        {
            base.Use(player);
            litPoint = Globals.CreateCircleOutline(player.position, radius, 150);
            circleCollision = new Circle(player.position, radius);
            skillDuration = duration;

            ped = new()
            {
                particleData = new ParticleData()
                {
                    sizeStart = 3f,
                    sizeEnd = 3f,
                    colorStart = Color.Red,
                    colorEnd = Color.Yellow,
                    opacityStart = 0.5f,
                },
                interval = 0.01f,
                emitCount = 5,
                angleVariance = 180f,
                speedMax = 100,
                speedMin = 10,
                lifeSpanMax = 1,
                lifeSpanMin = 0.5f,
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
                    enemy.ampMultiplier = 1;
                }
            }

            if (skillDuration > 0)
            {
                healTick -= TimeManager.TimeGlobal;
                if (circleCollision.Intersects(player.collision) && healTick <= 0)
                {
                    player.Status.Heal((healPercentage / 100) * player.Status.MaxHP);
                    healTick = healInterval;
                }
                foreach (var enemy in Globals.EnemyManager)
                {
                    if (circleCollision.Intersects(enemy.collision))
                    {
                        enemy.ampMultiplier = 1 + multiplier;
                    }
                }
            }

        }

        public override void Draw()
        {

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
