using Advencursor._AI;
using Advencursor._Models;
using Advencursor._Particles;
using Advencursor._Particles.Emitter;
using Advencursor._SaveData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        private StaticEmitter staticEmitter;
        private List<Vector2> litPoint;
        private ParticleEmitterData ped;
        private ParticleEmitter pe;
        public Skill_Q_FireDomain(string name, SkillData skillData) : base(name, skillData)
        {
            duration = skillData.GetMultiplierNumber(name, "Duration");
            rarity = 1;
            setSkill = "Fire";
            description = "Create a circle of fiery fire around you. Turns an area into a firestorm. Enemies that enter are bound by flames. Causes more damage to be received. When the duration ends, the bound fire is released.";
        }

        public override void Use(Player player)
        {
            base.Use(player);

            litPoint = Globals.CreateCircleOutline(player.position, radius,200);

            for (int i = 0; i < litPoint.Count; i++)
            {
                LitFire(litPoint[i]);
            }
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);

        }

        public override void Draw()
        {
        }

        private void LitFire(Vector2 litPosition)
        {
            staticEmitter = new StaticEmitter(litPosition);

            ped = new()
            {
                particleData = new ParticleData()
                {
                    sizeStart = 18f,
                    sizeEnd = 18f,
                    colorStart = Color.Red,
                    colorEnd = Color.Yellow,
                },
                interval = 0.07f,
                emitCount = 32,
                angleVariance = 180f,
                speedMax = 0.5f,
                speedMin = 0.5f,
                lifeSpanMax = duration,
                lifeSpanMin = duration,
                rotationMax = 180,
            };

            pe = new(staticEmitter, ped);
        }
    }
}
