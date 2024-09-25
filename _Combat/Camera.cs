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


        private float shakeDuration = 0.0f;
        private float shakeMagnitude = 0f;



        public Camera()
        {
            position = Vector2.Zero;
        }

        public void Shake(float duration,float magnitude)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
        }
        public void Update()
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

            transform = Matrix.CreateTranslation(new Vector3(-position + shakeOffset, 0));
        }


    }
}
