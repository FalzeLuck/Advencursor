using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy
{
    public class Boss1 : _Enemy
    {
        public bool dashed;
        public Rectangle checkRadius;

        public Boss1(Texture2D texture, Vector2 position, int health, int row, int column) : base(texture, position, health)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  1, true) },
                { "Attack", new(texture,row,column,2,1,false) },

            };
            indicator = "Idle";

            dashed = false;
            
        }

        public override void Update(GameTime gameTime)
        {
            movementAI.Move(this);
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update(gameTime);
                collision = animations[indicator].GetCollision(position);
            }

            //Update Radius
            checkRadius = collision;
            int increaseamount = 300;
            int newX = checkRadius.X - increaseamount / 2;
            int newY = checkRadius.Y - increaseamount / 2;
            int newWidth = checkRadius.Width + increaseamount;
            int newHeight = checkRadius.Height + increaseamount;
            checkRadius = new Rectangle(newX, newY, newWidth, newHeight);

            if (position.Y > 470 && position.Y < 1080-200)
            {
                velocity = new(50, 50);
            }
            else if (position.Y <= 470)
            {
                velocity = new(50,0);
                position = new(position.X,position.Y + 1);
            }
            else if (position.Y >= 1080 - 200)
            {
                velocity = new(50, 0);
                position = new(position.X, position.Y - 1);
            }

            
        }


    }
}
