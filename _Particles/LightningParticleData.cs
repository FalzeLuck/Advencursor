using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Particles
{
    public class LightningParticleData : ParticleData
    {
        public LightningParticleData()
        {
            texture = Globals.Content.Load<Texture2D>("Particle/lightningTexture");
            lifespan = 0.2f;
            colorStart = Color.Cyan;
            colorEnd = Color.White;
            opacityStart = 1f;
            opacityEnd = 0f;
            sizeStart = 100f; 
            sizeEnd = 50f;
            speed = 200f;
            angle = 0f;
        }

    }
}
