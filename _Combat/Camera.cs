using Advencursor._Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Combat
{
    public class Camera
    {
        public Matrix transform { get; private set; }
        public Vector2 position { get; set; }

        // Zoom
        private float zoom;
        private float targetZoom;
        private float zoomSpeed;

        //Shake
        private float shakeDuration = 0.0f;
        private float shakeMagnitude = 0f;

        // Screen dimensions (world size after zoom)
        public float ViewWidth { get; private set; }
        public float ViewHeight { get; private set; }

        public Camera()
        {
            position = Vector2.Zero;
            zoom = 1.0f;       
            targetZoom = 1.0f; 
            zoomSpeed = 1.0f;

            ViewWidth = Globals.Bounds.X;
            ViewHeight = Globals.Bounds.Y;
        }

        public void Shake(float duration,float magnitude)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
        }
        public void SmoothZoom(float newZoom, float speed)
        {
            targetZoom = newZoom;
            zoomSpeed = speed;
        }
        public void Update(int screenWidth, int screenHeight)
        {
            Vector2 shakeOffset = Vector2.Zero;
            if (shakeDuration > 0.0f)
            {
                shakeOffset = new Vector2(
                    (float)(Globals.random.NextDouble() * 2 - 1) * shakeMagnitude,
                    (float)(Globals.random.NextDouble() * 2 - 1) * shakeMagnitude
                );
                shakeDuration -= TimeManager.TotalSeconds;
            }

            if (zoom != targetZoom)
            {
                zoom = MathHelper.Lerp(zoom, targetZoom, zoomSpeed * TimeManager.TotalSeconds);
                if (Math.Abs(zoom - targetZoom) < 0.01f)
                {
                    zoom = targetZoom;
                }
            }

            ViewWidth = screenWidth / zoom;
            ViewHeight = screenHeight / zoom;

            transform =
                Matrix.CreateTranslation(new Vector3(-position + shakeOffset, 0)) *  
                Matrix.CreateTranslation(new Vector3(-screenWidth / 2f, -screenHeight / 2f, 0)) * 
                Matrix.CreateScale(zoom) *                                           
                Matrix.CreateTranslation(new Vector3(screenWidth / 2f, screenHeight / 2f, 0));
        }


    }
}
