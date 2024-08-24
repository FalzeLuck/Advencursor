using Advencursor._Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy
{
    public class Elite1 : _Enemy
    {
        public Elite1(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  1, true) },
                { "Attack", new(texture,row,column,2,1,true) },
                { "Charge",new(texture,row,column,3,1,true) }

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

            velocity = new(200, 200);
            movementAI.Move(this);


        }
    }
}
