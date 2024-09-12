using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Particles.Emitter
{
    public struct ParticleEmitterData
    {
        public ParticleData particleData = new();
        public float angle = 0f;
        public float angleVariance = 45f;
        public float lifeSpanMin = 0.1f;
        public float lifeSpanMax = 2.0f;
        public float speedMin = 10f;
        public float speedMax = 100f;
        public float rotationMin = 0f;
        public float rotationMax = 0f;
        public float interval = 1f;
        public int emitCount = 1;

        public ParticleEmitterData()
        {

        }
    }
}
