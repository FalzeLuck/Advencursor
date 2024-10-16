using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class RandomPointGenerator
    {
        private Random random;

        public RandomPointGenerator()
        {
            random = new Random();
        }

        public List<Vector2> GenerateRandomPointsInRectangle(Rectangle area, int pointCount)
        {
            List<Vector2> points = new List<Vector2>();

            for (int i = 0; i < pointCount; i++)
            {
                float x = (float)random.Next(area.Left, area.Right);
                float y = (float)random.Next(area.Top, area.Bottom);
                points.Add(new Vector2(x, y));
            }

            Vector2 averagePoint = CalculateAveragePoint(points);

            Vector2 rectangleCenter = new Vector2(area.Center.X, area.Center.Y);

            Vector2 adjustment = rectangleCenter - averagePoint;
            AdjustPoints(points, adjustment);

            return points;
        }

        private Vector2 CalculateAveragePoint(List<Vector2> points)
        {
            Vector2 sum = Vector2.Zero;

            foreach (Vector2 point in points)
            {
                sum += point;
            }

            return sum / points.Count;
        }

        private void AdjustPoints(List<Vector2> points, Vector2 adjustment)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] += adjustment;
            }
        }
    }
}
