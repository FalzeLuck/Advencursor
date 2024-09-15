using Advencursor._Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Advencursor._Particles.Emitter
{
    public class ParticleEmitter
    {
        private ParticleEmitterData data;
        private float intervalLeft;
        private readonly IEmitter emitter;

        public ParticleEmitter(IEmitter emitter,ParticleEmitterData data)
        {
            this.emitter = emitter;
            this.data = data;
            intervalLeft = 0f;
        }

        private void Emit(Vector2 pos)
        {
            ParticleData d = data.particleData;
            d.lifespan = Globals.RandomFloat(data.lifeSpanMin, data.lifeSpanMax);
            d.speed = Globals.RandomFloat(data.speedMin, data.speedMax);
            float r = (float)(Globals.random.NextDouble() * 2) - 1;
            d.angle += data.angleVariance * r;

            d.rotation = Globals.RandomFloat(data.rotationMin,data.rotationMax);


            Particle p = new(pos,d);
            ParticleManager.AddParticle(p);
        }

        public void Update()
        {
            intervalLeft -= TimeManager.TimeGlobal;
            while (intervalLeft <= 0f)
            {
                intervalLeft += data.interval;
                var pos = emitter.EmitPosition;
                for (int i = 0; i < data.emitCount; i++)
                {
                    Emit(pos);
                }
            }
        }
    }
}
