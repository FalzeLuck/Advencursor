using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class Vector2Pool
    {
        private Stack<Vector2> pool = new Stack<Vector2>();

        public Vector2 Get()
        {
            return pool.Count > 0 ? pool.Pop() : new Vector2();
        }

        public void Return(Vector2 vector)
        {
            pool.Push(vector);
        }
    }
}
