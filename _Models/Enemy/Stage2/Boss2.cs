using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Advencursor._Particles;
using Advencursor._Particles.Emitter;

namespace Advencursor._Models.Enemy.Stage2
{
    public class Boss2 : _Enemy
    {
        //Water
        private SpriteEmitter spriteEmitter;
        private List<Sprite> waterSprite;

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private Texture2D warningTexture;
        private Vector2 warningDirection;
        private Sprite player;
        private bool warningTrigger;
        private float warningOpacity;

        private Texture2D rayTexture;
        private Animation rayAnimation;
        private int rayWidth;
        private int rayHeight;

        private float rayDamageInterval = 0f;
        private float rayChargeTime;
        private float rayDuration;

        private bool isStart;

        public bool isCenter;
        private bool isStand;
        private bool isDashLeft;
        private bool isDashRight;
        private bool isRayAttack;
        private bool isRayUltimate;

        public float moveTimeToCenter = 3f;
        private float standTime;

        private Vector2 screenCenter;
        private bool isShake = false;


        public Boss2(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Open",new Animation(Globals.Content.Load<Texture2D>("Enemies/Boss2Open"),1,24,8,false)},
                { "Idle", new(texture, row, column,2,8, true) },
                { "Walk", new(texture, row, column,3,8, true) },
                { "WalkOpen", new(texture, row, column,1,8, true) },
                { "Attack", new(texture, row, column,4,8, true) },
                {"Die", new(texture,row,column,5,8,false) },

            };
            indicator = "WalkOpen";
            rayWidth = Globals.Bounds.X + 500;
            rayHeight = 300;
            warningTexture = Globals.CreateRectangleTexture(rayWidth, rayHeight, Color.Red);
            rayTexture = Globals.Content.Load<Texture2D>("Enemies/Boss2Ray");
            rayAnimation = new Animation(rayTexture,1,8,8,false);
            screenCenter = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
            isStart = false;
            isCenter = true;
            isStand = false;
            isDashLeft = false;
            isDashRight = false;
            isRayAttack = false;
            isRayUltimate = false;

            shadowTexture = Globals.Content.Load<Texture2D>("Enemies/Shadow2");

        }

        public override void Update(GameTime gameTime)
        {
            if (movementAI.target is Player)
            {
                player = movementAI.target;
            }
            if (Status.IsAlive())
            {
                DrawBurn();
            }
            else
            {
                RemoveBurn();
            }
            if (isStart)
            {
                collisionCooldown -= TimeManager.TimeGlobal;
                burnDuration -= TimeManager.TimeGlobal;
                Vector2 playerPosition = new(InputManager._mousePosition.X, InputManager._mousePosition.Y);



                if (animations.ContainsKey(indicator))
                {
                    animations[indicator].Update();
                }



                if (Status.IsAlive())
                {
                    if (!animations["Open"].IsComplete && animations["Open"].currentFrame == 1) Globals.soundManager.PlaySound("Boss2Opening");
                    movementAI.Stop();
                    if (isCenter && animations["Open"].IsComplete || isDashLeft || isDashRight)
                    {
                        collision = animations[indicator].GetCollision(position);
                        collision = ChangeRectangleSize(collision,100,true);
                    }
                    if (isCenter)
                    {
                        Globals.soundManager.StopSound("Boss2Attack");
                        if (!animations["Open"].IsComplete)
                        {
                            indicator = "WalkOpen";
                        }
                        else
                        {
                            indicator = "Walk";
                        }
                        moveTimeToCenter -= TimeManager.TimeGlobal;
                        Vector2 direction = screenCenter - position;
                        direction.Normalize();
                        velocity = new Vector2(300, 300);
                        position += direction * 200 * TimeManager.TimeGlobal;
                        if (moveTimeToCenter <= 0 || position == screenCenter)
                        {
                            if (!animations["Open"].IsComplete)
                            {
                                indicator = "Open";
                            }
                            isCenter = false;
                            isStand = true;
                            standTime = 5f;
                        }
                    }

                    if (isStand)
                    {
                        standTime -= TimeManager.TimeGlobal;
                        velocity = Vector2.Zero;
                        if (animations["Open"].IsComplete)
                        {
                            indicator = "Idle";
                        }
                        if (animations["Open"].IsComplete && standTime <= 0f)
                        {
                            isStand = false;
                            isDashLeft = true;
                        }
                    }

                    if (isDashLeft)
                    {
                        Globals.soundManager.StopSound("Boss2Attack");
                        indicator = "Walk";
                        RotateTo(90);
                        velocity = new Vector2(-2000, 0);
                        position += velocity * TimeManager.TimeGlobal;
                        if (position.X <= 0)
                        {
                            isDashLeft = false;
                            isRayAttack = true;
                            rayChargeTime = 1f;
                            rayDuration = 6f;
                            rayAnimation.Reset();
                        }
                    }

                    if (isDashRight)
                    {
                        Globals.soundManager.StopSound("Boss2Attack");
                        indicator = "Walk";
                        RotateTo(-90);
                        velocity = new Vector2(2000, 0);
                        position += velocity * TimeManager.TimeGlobal;
                        if (position.X >= Globals.Bounds.X)
                        {
                            isDashRight = false;
                            isRayAttack = true;
                            rayChargeTime = 1f;
                            rayDuration = 6f;
                            rayAnimation.Reset();
                        }
                    }
                    
                    if (isRayAttack)
                    {
                        indicator = "WalkOpen";
                        Rectangle rayCollision = new(
                            (int)position.X + (collision.Width / 2),
                            (int)position.Y - (collision.Height / 2),
                            rayWidth,
                            rayHeight);

                        if (position.X >= Globals.Bounds.X)//Rotate rayCollision
                        {
                            Vector2 pivot = new Vector2(rayCollision.X, rayCollision.Y + rayCollision.Height / 2f);
                            rayCollision = RotateRayCollision(rayCollision, 180f,pivot);
                        }
                        rayDamageInterval -= TimeManager.TimeGlobal;
                        rayChargeTime -= TimeManager.TimeGlobal;
                        if (rayChargeTime <= 0f)
                        {
                            Globals.soundManager.PlaySound("Boss2Attack");
                            rayAnimation.Update();
                            rayDuration -= TimeManager.TimeGlobal;
                            if(!isShake)
                                Globals.Camera.Shake(0.2f,3);
                            isShake = true;

                        }
                        if (rayDuration > 0.4f)
                        {
                            if (rayAnimation.currentFrame == 5)
                            {
                                rayAnimation.currentFrame = 3;
                            }
                        }
                        if (rayDamageInterval <= 0 && rayChargeTime <= 0)
                        {
                            
                            if (rayCollision.Intersects(player.collision))
                            {
                                player.Status.TakeDamage(1000, this);
                                rayDamageInterval = 1f;
                            }
                            
                            if (rayDuration <= 0f)
                            {
                                isRayAttack = false;
                                isShake = false;
                                if (position.X <= 0)
                                {
                                    isDashRight = true;
                                }
                                else
                                {
                                    isRayUltimate = true;
                                    rayChargeTime = 2f;
                                    rayDuration = 5f;
                                    rayAnimation.Reset();
                                }
                            }
                        }
                    }

                    if (isRayUltimate)
                    {
                        collision = new Rectangle();

                        Rectangle temp = player.collision;
                        OrientedRectangle playerBox = RectangleToOrientedRectangle(temp);
                        Vector2 pivot1 = Vector2.Zero;
                        Vector2 pivot2 = new(Globals.Bounds.X, 0);
                        Vector2 dir1 = screenCenter - pivot1;
                        Vector2 dir2 = screenCenter - pivot2;
                        dir1.Normalize();
                        dir2.Normalize();
                        float angle1 = (float)Math.Atan2(dir1.Y,dir1.X);
                        float angle2 = (float)Math.Atan2(dir2.Y, dir2.X);

                        Vector2 topLeft1 = Globals.RotatePoint(new Vector2(0, 0 - (rayHeight / 2)), pivot1, angle1);
                        Vector2 topRight1 = Globals.RotatePoint(new Vector2(rayWidth, 0 - (rayHeight / 2)), pivot1, angle1);
                        Vector2 bottomLeft1 = Globals.RotatePoint(new Vector2(0, 0 + (rayHeight / 2)), pivot1, angle1);
                        Vector2 bottomRight1 = Globals.RotatePoint(new Vector2(rayWidth, 0 + (rayHeight / 2)), pivot1, angle1);

                        Vector2 topLeft2 = Globals.RotatePoint(new Vector2(Globals.Bounds.X, 0 - (rayHeight / 2)), pivot2, angle2);
                        Vector2 topRight2 = Globals.RotatePoint(new Vector2(Globals.Bounds.X + rayWidth, 0 - (rayHeight / 2)), pivot2, angle2);
                        Vector2 bottomLeft2 = Globals.RotatePoint(new Vector2(Globals.Bounds.X, 0 + (rayHeight / 2)), pivot2, MathHelper.ToRadians(angle2));
                        Vector2 bottomRight2 = Globals.RotatePoint(new Vector2(Globals.Bounds.X + rayWidth, 0 + (rayHeight / 2)), pivot2, angle2);

                        OrientedRectangle rayBox1 = new OrientedRectangle(topLeft1, topRight1, bottomLeft1, bottomRight1);
                        OrientedRectangle rayBox2 = new OrientedRectangle(topLeft2, topRight2, bottomLeft2, bottomRight2);

                        

                        RotateTo(0);
                        position += new Vector2(-10);
                        rayDamageInterval -= TimeManager.TimeGlobal;
                        rayChargeTime -= TimeManager.TimeGlobal;
                        if (rayChargeTime <= 0f)
                        {
                            Globals.soundManager.PlaySound("Boss2Attack");
                            rayAnimation.Update();
                            rayDuration -= TimeManager.TimeGlobal;
                            if (!isShake)
                                Globals.Camera.Shake(0.2f, 3);
                        }
                        if (rayDuration > 0.4f)
                        {
                            if (rayAnimation.currentFrame == 5)
                            {
                                rayAnimation.currentFrame = 3;
                            }
                        }
                        if (rayDamageInterval <= 0 && rayChargeTime <= 0)
                        {
                            if (SATCollision(rayBox1,playerBox) || SATCollision(rayBox2, playerBox))
                            {
                                player.Status.TakeDamage(2000, this);
                                rayDamageInterval = 1f;
                            }
                            
                            if (rayDuration <= 0f)
                            {
                                isRayUltimate = false;
                                isShake = false;
                                isCenter = true;
                                moveTimeToCenter = 5f;
                                position = new Vector2(screenCenter.X, -100);
                            }
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            if (Status.CurrentHP > 0)
            {
                DrawRayAttack();
                if (isRayUltimate)
                {
                    if (rayChargeTime > 0 && isRayUltimate)
                    {
                        Vector2 origin = new Vector2(0, warningTexture.Height / 2);
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
                        Vector2 pivot1 = Vector2.Zero;
                        Vector2 pivot2 = new(Globals.Bounds.X, 0);
                        Vector2 dir1 = screenCenter - pivot1;
                        Vector2 dir2 = screenCenter - pivot2;
                        dir1.Normalize();
                        dir2.Normalize();
                        float angle1 = (float)Math.Atan2(dir1.Y, dir1.X);
                        float angle2 = (float)Math.Atan2(dir2.Y, dir2.X);
                        Globals.SpriteBatch.Draw(warningTexture, Vector2.Zero, null, Color.White * warningOpacity, angle1, origin, 1f, SpriteEffects.None, 0f);
                        Globals.SpriteBatch.Draw(warningTexture, new Vector2(Globals.Bounds.X, 0), null, Color.White * warningOpacity, angle2, origin, 1f, SpriteEffects.None, 0f);
                    }
                    if (rayChargeTime <= 0 && isRayUltimate)
                    {
                        Vector2 rayOffset = new Vector2(rayWidth / 2, 0);
                        Vector2 pivot1 = Vector2.Zero;
                        Vector2 pivot2 = new(Globals.Bounds.X, 0);
                        Vector2 dir1 = screenCenter - pivot1;
                        Vector2 dir2 = screenCenter - pivot2;
                        dir1.Normalize();
                        dir2.Normalize();
                        float angle1 = (float)Math.Atan2(dir1.Y, dir1.X);
                        float angle2 = (float)Math.Atan2(dir2.Y, dir2.X);
                        rayAnimation.scale = 1.3f;
                        rayAnimation.offset = rayOffset;
                        animations["WalkOpen"].rotation = angle1 + MathHelper.ToRadians(90);
                        animations["WalkOpen"].Draw(pivot1);
                        rayAnimation.rotation = angle1;
                        rayAnimation.Draw(Globals.MoveVector(pivot1, -300, angle1));
                        //rayAnimation.Draw(pivot1);
                        animations["WalkOpen"].rotation = angle2 + MathHelper.ToRadians(90);
                        animations["WalkOpen"].Draw(pivot2);
                        rayAnimation.rotation = angle2;
                        rayAnimation.Draw(Globals.MoveVector(pivot2, -300, angle2));
                        //rayAnimation.Draw(pivot2);
                    }
                }
            }
            DrawShadow();
            base.Draw();

            
            //DrawCollisionCheck();
        }

        public void Start()
        {
            isStart = true;
        }

        public void RotateTo(float degree)
        {
            if (MathHelper.ToDegrees(rotation) < degree)
            {
                rotation += 0.05f;
            }else
            {
                rotation -= 0.05f;
            }
            foreach (var anim in animations.Values)
            {
                anim.rotation = rotation;
            }
        }
        private void DrawRayAttack()
        {
            if (rayChargeTime > 0 && isRayAttack)
            {
                Vector2 origin = new Vector2(0, warningTexture.Height / 2);
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
                float rotation;
                if (position.X >= Globals.Bounds.X)
                {
                    rotation = 180f;
                }
                else
                {
                    rotation = 0f;
                }
                rotation = MathHelper.ToRadians(rotation);
                Globals.SpriteBatch.Draw(warningTexture, position, null, Color.White * warningOpacity, rotation, origin, 1f, SpriteEffects.None, 0f);
            }
            if (rayChargeTime <= 0 && isRayAttack)
            {
                Vector2 rayOrigin = new Vector2(rayAnimation.GetCollision(Vector2.Zero).Width/2, 0);
                float rotation;
                if (position.X >= Globals.Bounds.X)
                {
                    rotation = 180f;
                }
                else
                {
                    rotation = 0f;
                }
                rotation = MathHelper.ToRadians(rotation);
                rayAnimation.scale = 1f;
                rayAnimation.rotation = rotation;
                rayAnimation.offset = rayOrigin;
                rayAnimation.Draw(position);
            }
        }
        private Rectangle RotateRayCollision(Rectangle rayCollision, float angle,Vector2 pivot)
        {
            Vector2 topLeft = new Vector2(rayCollision.Left, rayCollision.Top);
            Vector2 topRight = new Vector2(rayCollision.Right, rayCollision.Top);
            Vector2 bottomLeft = new Vector2(rayCollision.Left, rayCollision.Bottom);
            Vector2 bottomRight = new Vector2(rayCollision.Right, rayCollision.Bottom);


            float changeAngle = MathHelper.ToRadians(angle);

            topLeft = Globals.RotatePoint(topLeft, pivot, changeAngle);
            topRight = Globals.RotatePoint(topRight, pivot, changeAngle);
            bottomLeft = Globals.RotatePoint(bottomLeft, pivot, changeAngle);
            bottomRight = Globals.RotatePoint(bottomRight, pivot, changeAngle);

            return Globals.CreateBoundingRectangle(topLeft, topRight, bottomLeft, bottomRight);
        }

        private OrientedRectangle RectangleToOrientedRectangle(Rectangle rectangle)
        {
            Vector2 topLeft = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 topRight = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 bottomLeft = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 bottomRight = new Vector2(rectangle.Right, rectangle.Bottom);

            return new OrientedRectangle(topLeft, topRight, bottomLeft, bottomRight);
        }

        private bool SATCollision(OrientedRectangle rect1, OrientedRectangle rect2)
        {
            var edges1 = rect1.GetEdges();
            var edges2 = rect2.GetEdges();
            var axes = edges1.Concat(edges2).Select(edge => new Vector2(-edge.Y, edge.X));

            
            foreach (var axis in axes)
            {
                
                var projection1 = ProjectRectangle(rect1, axis);
                var projection2 = ProjectRectangle(rect2, axis);

                
                if (!ProjectionsOverlap(projection1, projection2))
                {
                    return false;
                }
            }

            return true;
        }

        public (float min, float max) ProjectRectangle(OrientedRectangle rect, Vector2 axis)
        {
            float min = Vector2.Dot(rect.Corners[0], axis);
            float max = min;

            foreach (var corner in rect.Corners)
            {
                float projection = Vector2.Dot(corner, axis);
                min = Math.Min(min, projection);
                max = Math.Max(max, projection);
            }

            return (min, max);
        }

        public bool ProjectionsOverlap((float min, float max) proj1, (float min, float max) proj2)
        {
            return proj1.max >= proj2.min && proj2.max >= proj1.min;
        }

        private void LitWater(Sprite sprite)
        {
            spriteEmitter = new SpriteEmitter(() => sprite.position);
            ped = new()
            {
                
                particleData = new ParticleData()
                {
                    sizeStart = 18f,
                    sizeEnd = 18f,
                    colorStart = Color.Purple,
                    colorEnd = Color.Blue,
                },
                interval = 0.07f,
                emitCount = 32,
                angleVariance = 180f,
                speedMax = 0.5f,
                speedMin = 0.5f,
                lifeSpanMax = 1,
                lifeSpanMin = 1,
                rotationMax = 180,
            };

            pe = new(spriteEmitter, ped);
        }

        public void DrawCollisionCheck()
        {
            Rectangle temp = player.collision;
            OrientedRectangle playerBox = RectangleToOrientedRectangle(temp);
            Vector2 pivot1 = Vector2.Zero;
            Vector2 pivot2 = new(Globals.Bounds.X, 0);
            Vector2 dir1 = screenCenter - pivot1;
            Vector2 dir2 = screenCenter - pivot2;
            dir1.Normalize();
            dir2.Normalize();
            float angle1 = (float)Math.Atan2(dir1.Y, dir1.X);
            float angle2 = (float)Math.Atan2(dir2.Y, dir2.X);

            Vector2 topLeft1 = Globals.RotatePoint(new Vector2(0, 0 - (rayHeight / 2)), pivot1, angle1);
            Vector2 topRight1 = Globals.RotatePoint(new Vector2(rayWidth, 0 - (rayHeight / 2)), pivot1, angle1);
            Vector2 bottomLeft1 = Globals.RotatePoint(new Vector2(0, 0 + (rayHeight / 2)), pivot1, angle1);
            Vector2 bottomRight1 = Globals.RotatePoint(new Vector2(rayWidth, 0 + (rayHeight / 2)), pivot1, angle1);

            Vector2 topLeft2 = Globals.RotatePoint(new Vector2(Globals.Bounds.X, 0 - (rayHeight / 2)), pivot2, angle2);
            Vector2 topRight2 = Globals.RotatePoint(new Vector2(Globals.Bounds.X + rayWidth, 0 - (rayHeight / 2)), pivot2, angle2);
            Vector2 bottomLeft2 = Globals.RotatePoint(new Vector2(Globals.Bounds.X, 0 + (rayHeight / 2)), pivot2, angle2);
            Vector2 bottomRight2 = Globals.RotatePoint(new Vector2(Globals.Bounds.X + rayWidth, 0 + (rayHeight / 2)), pivot2, angle2);

            OrientedRectangle rayBox1 = new OrientedRectangle(topLeft1, topRight1, bottomLeft1, bottomRight1);
            OrientedRectangle rayBox2 = new OrientedRectangle(topLeft2, topRight2, bottomLeft2, bottomRight2);

            Globals.DrawLine(topLeft1,topRight1,Color.Red,1);
            Globals.DrawLine(bottomLeft1, bottomRight1, Color.Red, 1);
            Globals.DrawLine(topLeft2, topRight2, Color.Red, 1);
            Globals.DrawLine(bottomLeft2, bottomRight2, Color.Red, 1);
            Globals.DrawLine(new Vector2(temp.X,temp.Y), new Vector2(temp.X + temp.Width, temp.Y), Color.Green, 1);
            Globals.DrawLine(new Vector2(temp.X, temp.Y+temp.Height), new Vector2(temp.X + temp.Width, temp.Y+temp.Height), Color.Green, 1);
        }
        private void DrawShadow()
        {
            Vector2 shadowPosition = new Vector2(position.X, position.Y + 200);
            float shadowScale = 1.5f;

            Vector2 shadowOrigin = new Vector2(shadowTexture.Width / 2, shadowTexture.Height / 2);
            Globals.SpriteBatch.Draw(shadowTexture, shadowPosition, null, Color.White * 0.6f, 0f, shadowOrigin, shadowScale, spriteEffects, 0f);
        }
    }
}
