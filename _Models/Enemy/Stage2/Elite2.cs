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
        private bool isMoveSoundPlayed = false;

        private int dashSpeed = 600;
        public Elite2(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Dash",new(texture,row,column,1,8,true) },
                { "Shake",new(texture,row,column,2,8,true) },
                { "Die",new(texture,row,column,3,8,false) }
            };
            indicator = "Dash";

            Status.immunity = true;
            isBomb = false;
            isDamage = false;
            warningOpacity = 0.8f;
            bombTextureWarning = Globals.CreateRectangleTexture(bombRadiusSize, bombRadiusSize, Color.Red);
            bombTextureAnimation = new Animation(Globals.Content.Load<Texture2D>("Enemies/Elite2_Effect"), 1, 8, 8, false);
            velocity = new Vector2(dashSpeed);
            shadowTexture = Globals.Content.Load<Texture2D>("Enemies/Shadow3");
            
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
                collision = animations[indicator].GetCollision(position);
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
                if (!isMoveSoundPlayed)
                {
                    Globals.soundManager.PlaySound("Elite2Moving", false);
                    isMoveSoundPlayed = true;
                }
            }

            if (isBomb)
            {
                bombTimer -= TimeManager.TimeGlobal;
                if (bombTimer > 0)
                {
                    indicator = "Shake";
                }
                if (bombTimer <= 0)
                {
                    Status.Kill();
                    bombTextureAnimation.Update();
                    if (!isDamage)
                    {
                        Globals.soundManager.PlaySound("Elite2Explode");
                        foreach (var enemy in Globals.EnemyManager)
                        {
                            if (enemy.collision.Intersects(bombRadius) && !(enemy is Boss3))
                            {
                                enemy.TakeDamage(2500, this, new(31, 81, 255));
                                isDamage = true;
                            }
                        }
                        if (movementAI.target is Player)
                        {
                            if (movementAI.target.collision.Intersects(bombRadius))
                            {
                                movementAI.target.Status.TakeDamageNoCrit(2500, this,new (31,81,255));
                                isDamage = true;
                            }
                        }
                    }
                }
            }

        }
        public override void Draw()
        {
            DrawShadow();
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
                if (bombTimer > 0)
                {
                    Vector2 warningOrigin = new Vector2((bombTextureWarning.Width / 2), bombTextureWarning.Height / 2);
                    Globals.SpriteBatch.Draw(bombTextureWarning, position, null, Color.White * warningOpacity, rotation, warningOrigin, 1f, spriteEffects, 0f);
                }
                else if(bombTimer <= 0f)
                {
                    bombTextureAnimation.Draw(position);
                }
            }

            base.Draw();
        }

        private void DrawShadow()
        {
            Vector2 shadowPosition = new Vector2(position.X, position.Y + 150 / 2);
            float shadowScale = 1f;

            Vector2 shadowOrigin = new Vector2(shadowTexture.Width / 2, shadowTexture.Height / 2);
            Globals.SpriteBatch.Draw(shadowTexture, shadowPosition, null, Color.White, rotation, shadowOrigin, shadowScale, spriteEffects, 0f);
        }
    }
}
