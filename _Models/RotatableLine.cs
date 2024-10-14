using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class RotatableLine
{
    public List<Vector2> points; // List of points for the line
    private Vector2 origin;       // Origin point for rotation
    private float rotationAngle;  // Rotation angle in radians
    private Texture2D pointTexture;  // Texture for rendering a point
    private int pointSize = 5;    // Size of the point to be drawn

    public RotatableLine(GraphicsDevice graphicsDevice, int lineHeight, Vector2 startPosition, float pointSpacing)
    {
        points = new List<Vector2>();
        origin = startPosition;
        rotationAngle = 0f;

        // Create a basic 1x1 texture for drawing points
        pointTexture = new Texture2D(graphicsDevice, 1, 1);
        pointTexture.SetData(new[] { Color.White });

        // Initialize the vertical line points
        for (int i = 0; i < lineHeight; i++)
        {
            points.Add(new Vector2(startPosition.X, startPosition.Y + i * pointSpacing));
        }
    }

    // Method to set rotation angle (in radians)
    public void SetRotation(float angle)
    {
        rotationAngle = angle;
    }

    // Method to draw the line
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var point in points)
        {
            // Rotate each point around the origin
            Vector2 rotatedPoint = RotatePoint(point, origin, rotationAngle);

            // Draw each point as a small rectangle
            spriteBatch.Draw(pointTexture, new Rectangle((int)rotatedPoint.X, (int)rotatedPoint.Y, pointSize, pointSize), Color.White);
        }
    }

    // Helper method to rotate a point around an origin
    private Vector2 RotatePoint(Vector2 point, Vector2 origin, float angle)
    {
        float cosTheta = (float)Math.Cos(angle);
        float sinTheta = (float)Math.Sin(angle);

        Vector2 translatedPoint = point - origin;

        float rotatedX = translatedPoint.X * cosTheta - translatedPoint.Y * sinTheta;
        float rotatedY = translatedPoint.X * sinTheta + translatedPoint.Y * cosTheta;

        return new Vector2(rotatedX, rotatedY) + origin;
    }

    public List<Vector2> GetPointList()
    {
        return points;
    }
}
