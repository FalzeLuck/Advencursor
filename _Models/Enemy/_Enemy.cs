using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Combat;
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
    public abstract class _Enemy : Sprite
    {
        public Status Status { get; set; }
        public MovementAI movementAI { get; set; }
        public Rectangle collision;
        public bool isAttacking;

        public _Enemy(Texture2D texture, Vector2 position, int health) : base(texture, position)
        {
            Status = new(health);
            animations = new Dictionary<string, Animation>();
        }



    }
}
