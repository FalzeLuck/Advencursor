using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy.Stage2
{
    public class Elite2 : _Enemy
    {
        private Texture2D bombTextureWarning;
        private bool warningTrigger;
        private float warningOpacity;
        private Animation bombTextureAnimation;
        private int bombRadiusSize = 300;
        private Rectangle bombRadius;
        private float bombTimer;
        private bool isBomb;
        private bool isDamage;


        private int dashSpeed = 600;
        public Elite2(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Dash",new(texture,row,column,1,8,true) },
                { "Die",new(texture,row,column,2,8,false) }
            };
            indicator = "Dash";

            Status.immunity = true;
            isBomb = false;
            isDamage = false;
            warningOpacity = 0.8f;
            bombTextureWarning = Globals.CreateRectangleTexture(bombRadiusSize, bombRadiusSize, Color.Red);
            velocity = new Vector2(dashSpeed);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
            }
            bombRadius = new Rectangle((int)(position.X - bombRadiusSize / 2), (int)(position.Y - bombRadiusSize / 2), bombRadiusSize, bombRadiusSize);


            if (!isBomb && bombRadius.Contains(movementAI.target.position))
            {
                velocity = Vector2.Zero;
                isBomb = true;
                bombTimer = 0.5f;
            }
            if (!isBomb)
            {
                movementAI.Move(this);
            }

            if (isBomb)
            {
                bombTimer -= TimeManager.TimeGlobal;

                if (bombTimer <= 0)
                {
                    Status.Kill();
                    if (!isDamage)
                    {
                        foreach (var enemy in Globals.EnemyManager)
                        {
                            if (enemy.collision.Intersects(bombRadius))
                            {
                                enemy.TakeDamage(5000, this);
                                isDamage = true;
                            }
                        }
                        if (movementAI.target is Player)
                        {
                            if (movementAI.target.collision.Intersects(bombRadius))
                            {
                                movementAI.target.Status.TakeDamageNoCrit(5000, this);
                                isDamage = true;
                            }
                        }

                    }
                }
            }

        }
        public override void Draw()
        {
            if (isBomb)
            {
                if (warningOpacity <= 0.3f)
                {
                    warningTrigger = true;
                }
                else if (warningOpacity >= 0.8f)
                {
                    warningTrigger = false;
                }
                if (!warningTrigger)
                {
                    warningOpacity -= 1 * TimeManager.TimeGlobal;
                }
                else
                {
                    warningOpacity += 1 * TimeManager.TimeGlobal;
                }
                Vector2 warningOrigin = new Vector2((bombTextureWarning.Width / 2), bombTextureWarning.Height / 2);
                Globals.SpriteBatch.Draw(bombTextureWarning, position, null, Color.White * warningOpacity, rotation, warningOrigin, 1f, spriteEffects, 0f);
            }

            base.Draw();
        }
    }
}
