using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy._CommonEnemy
{
    public class Kiki : CommonEnemy
    {
        public Kiki(Texture2D texture, Vector2 position, int health, int row, int column) : base(texture, position, health)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  1, true) },
                
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var animation in animations)
            {
                animation.Value.Update(gameTime);
                collision = animation.Value.GetCollision(position);
            }
            movementAI.Move(this);
            speed = 300;
        }

        public override void Draw()
        {
            foreach (var animation in animations)
            {
                animation.Value.Draw(position);
            }
        }
    }
}
