using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Particles.Emitter
{
    public interface IEmitter
    {
        Vector2 EmitPosition { get; }
    }
}
