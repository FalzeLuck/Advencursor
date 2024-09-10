using Advencursor._Models;
using Advencursor._Particles;
using Advencursor._Particles.Emitter;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill.Thunder_Set
{
    public class Skill_E_ThunderSpeed : Skill
    {
        private SpriteEmitter spriteEmitter;

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private float bufftime;
        private bool isUsing = false;
        public Skill_E_ThunderSpeed(string name, float cooldown, Player player) : base(name, cooldown)
        {
            spriteEmitter = new SpriteEmitter(() => player.position);

            ped = new()
            {
                particleData = new LightningParticleData(),
                interval = 0.5f,
                emitCount = 150,
                angleVariance = 180f,
                speedMax = 100f,
                speedMin = 100f,
            };

            pe = new(spriteEmitter, ped);
        }

        public override void Use()
        {
            base.Use();
            ParticleManager.AddParticleEmitter(pe);

            isUsing = true;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);

        }
    }
}
