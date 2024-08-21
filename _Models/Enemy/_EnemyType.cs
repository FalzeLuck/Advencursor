using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy
{
    public abstract class CommonEnemy : _Enemy
    {
        public CommonEnemy(Texture2D texture, Vector2 position, int health) : base (texture, position,health)
        { 

        }
    }
    public abstract class Miniboss : _Enemy
    {
        public Miniboss(Texture2D texture, Vector2 position, int health) : base(texture, position, health)
        {

        }
    }
    public abstract class Boss : _Enemy
    {
        public Boss(Texture2D texture, Vector2 position, int health) : base(texture, position, health)
        {

        }
    }
}
