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
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Advencursor._Models.Enemy.Stage3
{
    public class Knife : _Enemy
    {
        public bool haveDamage = false;
        private float scale;
        private Sprite player;
        public bool isRotationComplete = false;
        public Knife(Texture2D texture, Vector2 position, int health, int attack, int row, int column,Sprite player) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Float", new(texture, row, column, 8, false) },
            };
            indicator = "Float";
            scale = 1.2f;
            movementAI = new FollowMovementAI();
            this.player = player;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var animation in animations.Values)
            {
                animation.Update();
                animation.rotation = rotation;
                animation.scale = scale;
            }
            collisionCooldown -= TimeManager.TimeGlobal;
            if (haveDamage)
            {
                UpdateCollision();
                if(collision.Intersects(player.collision) && collisionCooldown <= 0)
                {
                    player.Status.TakeDamage(500, this);
                    collisionCooldown = 1;
                }
            }

        }

        public override void Draw()
        {
            animations["Float"].Draw(position);
        }

        private void UpdateCollision()
        {
            if (rotation % 3 < 0.1) //Horizontal
            {
                collision = animations["Float"].GetCollision(position);
                collision = ChangeRectangleSize(collision,70,130,true);
            }

            if (rotation % 1.5 < 0.1) //Vertical
            {
                collision = animations["Float"].GetCollision(position);
                collision = ChangeRectangleSize(collision, 180, 0, true);
            }
        }
    }
}
