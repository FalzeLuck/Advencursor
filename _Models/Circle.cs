using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class Circle
    {
        public Vector2 Center; 
        public float Radius;  

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        
        public bool Intersects(Rectangle rect)
        {
            
            float closestX = MathHelper.Clamp(Center.X, rect.Left, rect.Right);
            float closestY = MathHelper.Clamp(Center.Y, rect.Top, rect.Bottom);

            
            float distanceX = Center.X - closestX;
            float distanceY = Center.Y - closestY;

            
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared <= (Radius * Radius);
        }
    }

}
