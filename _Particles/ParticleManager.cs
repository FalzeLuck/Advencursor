using Advencursor._Particles.Emitter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Particles
{
    public static class ParticleManager
    {
        private static readonly List<Particle> particles = new();

        private static readonly List<ParticleEmitter> particleEmitters = new();
        public static void AddParticle(Particle particle)
        {
            particles.Add(particle);
        }

        public static void AddParticleEmitter(ParticleEmitter emitter)
        {
            particleEmitters.Add(emitter);
        }

        public static void UpdateParticles()
        {
            foreach (var particle in particles)
            {
                particle.Update();
            }

            particles.RemoveAll(p => p.IsFinished);
        }

        public static void UpdateEmitters()
        {
            foreach (var emitter in particleEmitters)
            {
                emitter.Update();
            }
        }

        public static void Update()
        {
            UpdateParticles();
            UpdateEmitters();
        }

        public static void Draw()
        {
            foreach (var particle in particles)
            {
                particle.Draw();
            }
        }
    }
}
