using Advencursor._Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._AI
{
    public abstract class MovementAI
    {
        public bool stop { get; set; } = false;
        public abstract void Move(Sprite sprite);
        public abstract void Stop();
        public abstract void Start();
    }
}
