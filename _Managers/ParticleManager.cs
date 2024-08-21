using Advencursor._Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Managers
{
    public static class ParticleManager
    {
        private static readonly List<Particle> particles = new();

        public static void AddParticle(Particle particle)
        {
            particles.Add(particle);
        }

        public static void UpdateParticles()
        {
            foreach (var particle in particles)
            {
                particle.Update();
            }

            particles.RemoveAll(p => p.IsFinished);
        }

        public static void Update()
        {
            UpdateParticles();
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
