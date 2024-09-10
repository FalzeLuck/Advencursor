using Advencursor._Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Particles.Emitter
{
    public class SpriteEmitter : IEmitter
    {
        private readonly Func<Vector2> getPlayerPosition;
        public Vector2 EmitPosition => getPlayerPosition();
        public SpriteEmitter(Func<Vector2> getPlayerPosition)
        {
            this.getPlayerPosition = getPlayerPosition;
        }
    }
}
