using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class OrientedRectangle
    {
        public Vector2[] Corners;


        public OrientedRectangle(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            Corners = new[] { topLeft, topRight, bottomLeft, bottomRight };
        }


        public Vector2[] GetEdges()
        {
            return new[]
            {
            Corners[1] - Corners[0],
            Corners[3] - Corners[0],
            Corners[1] - Corners[2],
            Corners[3] - Corners[2]
        };
        }
    }
}
