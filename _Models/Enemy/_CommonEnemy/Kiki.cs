using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy._CommonEnemy
{
    public class Kiki : _Enemy
    {
        private float minidash_timer;
        private bool dash;

        public Kiki(Texture2D texture, Vector2 position, int health, int row, int column) : base(texture, position, health)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  1, true) },
                { "Attack", new(texture,row,column,2,1,false) },
                
            };
            indicator = "Idle";
        }

        public override void Update(GameTime gameTime)
        {
            
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update(gameTime);
                collision = animations[indicator].GetCollision(position);
            }
            UpdateParryZone();

            movementAI.Move(this);
            recovery_time += TimeManager.TotalSeconds;

            minidash_timer += TimeManager.TotalSeconds;
            if (minidash_timer > 1f)
            {
                minidash_timer = 0f;
            }
            else if (minidash_timer > 0.5f)
            {
                dash = false;
                isAttacking = false;
                velocity = new Vector2(0, 0);
            }
            else if (minidash_timer > 0f)
            {
                dash = true;
                isAttacking = true;
                velocity = new Vector2(300, 300);
            }
            
            
        }

        /*public override void Draw()
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Draw(position);
            }
        }*/
    }
}
