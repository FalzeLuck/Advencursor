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
using Advencursor._Models.Enemy.Stage3;

namespace Advencursor._Models.Enemy.Stage2
{
    public class Boss3 : _Enemy
    {
        private SpriteEmitter spriteEmitter;
        private StaticEmitter staticEmitter;
        private List<Sprite> waterSprite;

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private Texture2D warningTexture;
        private Vector2 warningDirection;
        private Sprite player;
        private bool warningTrigger;
        private float warningOpacity;
        private Vector2 screenCenter = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);

        private bool isStart;
        private Circle buffCollision;
        private float buffRadius = 600;
        private enum phase
        {
            Opening,
            SkillSurprise,
            SkillVaporBlast,
            SkillRest,
            Tomato,
            UnderControl,
        }
        private int phaseIndicator;

        private List<Knife> knives;
        private List<Vector2> knifeDestination;

        //Skill Surprise
        private bool isSmoke;
        private Circle damageCollision;
        private float damageRadius = 600;
        private float bombTimer;

        //Skill Vapor Blast
        private Texture2D blastTexture;
        private Animation blastAnim;
        private int blastSectionIndicator;
        private Vector2 blastPosition;
        private Rectangle blastCollision;
        private float blastWaitTime;
        private float blastSetTime = 1;
        Texture2D rayTexture;

        public Boss3(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,4,1,8, true) },
                { "Sit", new(texture, row, column,4,2,8, true) },
                { "Attack", new(texture, row, column,4,3,8, false) },
                { "Die", new(texture, row, column,8,3,8, true) },
                { "WarpIn", new(texture, row, column,5,8, false) },
                { "WarpOut", new(texture, row, column,6,8, false) },

            };
            indicator = "WarpIn";
            phaseIndicator = (int)phase.Opening;
            isStart = false;
            shadowTexture = Globals.Content.Load<Texture2D>("Enemies/Shadow2");
            warningTexture = new Texture2D(Globals.graphicsDevice, 1, 1);
            blastTexture = Globals.Content.Load<Texture2D>("Enemies/Boss3Ray");
            blastAnim = new Animation(blastTexture, 1, 8, 8, false);
            knives = new List<Knife>();
            knifeDestination = new List<Vector2>();
        }

        public override void Update(GameTime gameTime)
        {
            if (movementAI.target is Player)
            {
                player = movementAI.target;
            }

            foreach (Knife knife in knives)
            {
                knife.Update(gameTime);
            }
            if (isStart)
            {

                if (phaseIndicator == (int)phase.Opening)
                {
                    if (knives[0].position.Y < screenCenter.Y)
                    {
                        UpdateKnifeRotation();
                        float knifeSpeed = 300f;
                        var dir = screenCenter - knives[0].position;
                        dir.Normalize();
                        knifeDestination[0] = new Vector2(screenCenter.X, screenCenter.Y - 200);
                        knives[0].position += dir * knifeSpeed * TimeManager.TimeGlobal;
                    }
                    else
                    {
                        phaseIndicator = (int)phase.SkillSurprise;
                        ResetSkillSurprise();
                    }
                }

                if (phaseIndicator == (int)phase.SkillSurprise)
                {
                    bombTimer -= TimeManager.TimeGlobal;
                    buffRadius = 1800;
                    buffCollision = new Circle(screenCenter, buffRadius);
                    if (bombTimer <= 4f && !isSmoke)
                    {
                        SetBlackSmoke(screenCenter, damageRadius);
                        damageCollision = new Circle(screenCenter, damageRadius);
                        ParticleManager.AddParticleEmitter(pe);
                        isSmoke = true;
                        indicator = "WarpIn";
                        Globals.Camera.Shake(0.5f, 10);
                        if (damageCollision.Intersects(player.collision))
                        {
                            player.Status.TakeDamage(2500, this);
                        }
                    }
                    if (bombTimer < 3f)
                    {
                        ParticleManager.RemoveParticleEmitter(pe);
                        UpdateContainAnimation();
                    }

                    if (animations["WarpIn"].IsComplete)
                    {
                        indicator = "Idle";
                    }

                    if (bombTimer <= 0)
                    {
                        ResetSkillSurprise();
                        phaseIndicator = (int)phase.SkillVaporBlast;
                        blastWaitTime = blastSetTime;
                    }
                }

                if (phaseIndicator == (int)phase.SkillVaporBlast)
                {
                    UpdateContainAnimation();
                    indicator = "Idle";
                    Vector2 pos = new Vector2(screenCenter.X, screenCenter.Y / 2);
                    float walkSpeed = 1000;
                    if (position.Y > pos.Y)
                    {
                        var dir = pos - position;
                        dir.Normalize();
                        position += dir * walkSpeed * TimeManager.TimeGlobal;
                        blastSectionIndicator = 1;
                        blastCollision = new Rectangle(0, 0, 1, 1);
                    }
                    else
                    {
                        if (blastSectionIndicator == 1)
                        {
                            blastPosition = new Vector2(Globals.Bounds.X / 6, 0);
                            blastCollision = new Rectangle((int)blastPosition.X, (int)blastPosition.Y, 640, 1080);
                        }
                        else if (blastSectionIndicator == 2)
                        {
                            blastPosition = new Vector2(screenCenter.X, 0);
                            blastCollision = new Rectangle((int)blastPosition.X, (int)blastPosition.Y, 640, 1080);
                        }
                        else if (blastSectionIndicator == 3)
                        {
                            blastPosition = new Vector2(5 * Globals.Bounds.X / 6, 0);
                            blastCollision = new Rectangle((int)blastPosition.X, (int)blastPosition.Y, 640, 1080);
                        }
                        blastWaitTime -= TimeManager.TimeGlobal;
                        if(blastWaitTime <= 0)
                        {
                            blastAnim.Update();
                        }if (blastWaitTime <= -1)
                        {
                            blastWaitTime = blastSetTime;
                            blastSectionIndicator++;
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            if (Status.CurrentHP > 0)
            {
                if (phaseIndicator == (int)phase.SkillVaporBlast)
                {
                    if (blastWaitTime > 0)
                    {
                        blastAnim.Reset();
                        warningTexture = new Texture2D(Globals.graphicsDevice, blastCollision.Width, blastCollision.Height);
                        warningTexture = Globals.CreateRectangleTexture(blastCollision.Width, blastCollision.Height, Color.Red);
                        Vector2 origin = new Vector2(warningTexture.Width / 2, 0);
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
                        Globals.SpriteBatch.Draw(warningTexture, blastPosition, null, Color.White * warningOpacity, 0f, origin, 1f, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        blastAnim.offset = new Vector2(0, blastAnim.GetCollision(blastPosition).Height/2);
                        blastAnim.Draw(blastPosition);
                    }
                }
            }
            base.Draw();
            foreach (var knife in knives)
            {
                knife.Draw();
            }
        }

        private void UpdateContainAnimation()
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
            }
        }

        private void UpdateKnifeRotation()
        {
            for (int i = 0; i < knives.Count; i++)
            {
                float angle = (float)Math.Atan2(knifeDestination[i].Y - knives[i].position.Y, knifeDestination[i].X - knives[i].position.X);
                knives[i].rotation = angle;
            }
        }
        public void Start()
        {
            indicator = "WarpIn";
            isStart = true;
            position = screenCenter;
            foreach (var anim in animations.Values)
            {
                anim.Reset();
            }
            phaseIndicator = (int)phase.Opening;
            AddKnife(1);
        }

        private void AddKnife(int amount)
        {
            Texture2D KnifeTexture = Globals.Content.Load<Texture2D>("Enemies/Boss3Knife");
            for (int i = 0; i < amount; i++)
            {
                knives.Add(new Knife(KnifeTexture, new Vector2(screenCenter.X / 2, -300), 0, 150, 1, 4));
                knifeDestination.Add(knives[knives.Count - 1].position);
                knifeDestination[knives.Count - 1] = knives[knives.Count - 1].position;
                knives[knives.Count - 1].rotation = MathHelper.ToRadians(90f);
            }
        }

        private void SetBlackSmoke(Vector2 position, float radius)
        {
            staticEmitter = new StaticEmitter(position);
            ped = new ParticleEmitterData()
            {
                particleData = new ParticleData()
                {
                    sizeStart = 30f,
                    sizeEnd = 60f,
                    colorStart = new Color(50, 0, 50),
                    colorEnd = Color.Black,
                    opacityStart = 1f,
                    opacityEnd = 0f,
                    rotation = 0f,
                    rangeMax = 600f,
                    speed = 20f,
                    lifespan = 3f
                },
                interval = 0.02f,
                emitCount = 50,
                angleVariance = 360f,
                speedMax = 2000,
                speedMin = 2000,
                lifeSpanMax = 2.5f,
                lifeSpanMin = 1.5f,
            };
            pe = new ParticleEmitter(staticEmitter, ped);
        }

        private void ResetSkillSurprise()
        {
            bombTimer = 5f;
        }
        private Rectangle RotateRayCollision(Rectangle rayCollision, float angle, Vector2 pivot)
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

        private void DrawShadow()
        {
            Vector2 shadowPosition = new Vector2(position.X, position.Y + 250);
            float shadowScale = 1.5f;

            Vector2 shadowOrigin = new Vector2(shadowTexture.Width / 2, shadowTexture.Height / 2);
            Globals.SpriteBatch.Draw(shadowTexture, shadowPosition, null, Color.White * 0.6f, 0f, shadowOrigin, shadowScale, spriteEffects, 0f);
        }
    }
}
