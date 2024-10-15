using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._AI;

namespace Advencursor._Models.Enemy.Stage3
{
    public class Knife : _Enemy
    {
        public Knife(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Float", new(texture, row, column, 8, false) },
            };
            indicator = "Float";

            movementAI = new FollowMovementAI();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var animation in animations.Values)
            {
                animation.Update();
                animation.rotation = rotation;
                animation.scale = 1.2f;
            }

        }

        public override void TakeDamage(float multiplier, Player player, bool throughImmune = false, bool NoCrit = false)
        {
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
