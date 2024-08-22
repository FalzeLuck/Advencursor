using Advencursor._Animation;
using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Skill;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class Zone : Sprite
    {
        public Rectangle collision {  get; private set; }
        public Zone(Texture2D texture, Vector2 position, int health, int row, int column) : base(texture, position)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Normal", new(texture, row, column,1,  TimeManager.framerate, true) },
            };
            indicator = "Normal";
        }

        public override void Update(GameTime gameTime)
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update(gameTime);
                collision = animations[indicator].GetCollision(position);
            }
        }
    }
}
