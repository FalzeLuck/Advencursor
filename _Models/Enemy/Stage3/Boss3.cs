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
using System.IO;

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
        public enum phase
        {
            Opening,
            SkillSurprise,
            SkillVaporBlast,
            SkillRest,
            Tomato,
            UnderControl,
        }
        public int phaseIndicator;

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
        private float blastCollideCooldown;
        private float blastWaitTime;
        private float blastSetTime = 1;
        Texture2D rayTexture;

        //Skill Rest
        private float reduceDamage = 20;
        private float healPercent = 5;
        private float healTimer;
        private float restTime;

        //Tomato
        private float tomatoTimer = 0;
        public bool isTomatoFinish => tomatoTimer <= 0;

        //Under Control
        private float openingTimer = 5f;
        private float grayScaleAmount;
        private Dictionary<int, Vector2> positionUnderControl;
        private List<int> positionIndexList;
        private int positionIndex;
        private RandomLoop<int> positionRandomLoop;
        private float knifeOpeningTimer;
        private bool isOpeningKnife;
        private bool isPrepareKnife;
        private bool isAttackKnife;
        private int knifeMethod;
        private float knifeAttackingTimer;
        private RandomPointGenerator randomPointGenerator;
        private Rectangle knifeStartArea1;
        private Rectangle knifeStartArea2;
        private Rectangle knifeEndArea1;
        private Rectangle knifeEndArea2;
        private Dictionary<int, Rectangle> knifeAreas;

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
            positionIndexList = new List<int>()
            {
                1,2,3,4
            };
            positionUnderControl = new Dictionary<int, Vector2>()
            {
                {1,new Vector2(Globals.Bounds.X / 4, Globals.Bounds.Y / 4)},
                {2,new Vector2(Globals.Bounds.X / 4 * 3, Globals.Bounds.Y / 4)},
                {3,new Vector2(Globals.Bounds.X / 4, Globals.Bounds.Y / 4 * 3)},
                {4,new Vector2(Globals.Bounds.X / 4 * 3, Globals.Bounds.Y / 4 * 3)},
            };
            positionRandomLoop = new RandomLoop<int>(positionIndexList);
            randomPointGenerator = new RandomPointGenerator();
            knifeAreas = new Dictionary<int, Rectangle>() //Start Topleft Rotate Clockwise
            {
                {1, new Rectangle(0,0- ((int)screenCenter.Y), ((int)screenCenter.X), ((int)screenCenter.Y))},
                {2, new Rectangle(((int)screenCenter.X),0- ((int)screenCenter.Y), ((int)screenCenter.X), ((int)screenCenter.Y))},
                {3, new Rectangle(Globals.Bounds.X,0, ((int)screenCenter.X), ((int)screenCenter.Y))},
                {4, new Rectangle(Globals.Bounds.X,(int)screenCenter.Y, ((int)screenCenter.X), ((int)screenCenter.Y))},
                {5, new Rectangle(((int)screenCenter.X),Globals.Bounds.Y, ((int)screenCenter.X), ((int)screenCenter.Y))},
                {6, new Rectangle(0,Globals.Bounds.Y, ((int)screenCenter.X), ((int)screenCenter.Y))},
                {7, new Rectangle(0-((int)screenCenter.X),((int)screenCenter.Y),((int)screenCenter.X),((int)screenCenter.Y))},
                {8, new Rectangle(0-((int)screenCenter.X),0, ((int)screenCenter.X), ((int)screenCenter.Y))},
            };
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
                if (Status.CurrentHP <= 0.3 * Status.MaxHP && phaseIndicator != (int)phase.UnderControl)
                {
                    position = screenCenter;
                    phaseIndicator = (int)phase.UnderControl;
                    tomatoTimer = 0;
                    collision = new Rectangle();
                    knives.Clear();
                    knifeDestination.Clear();
                    indicator = "WarpIn";
                    animations[indicator].Reset();
                    grayScaleAmount = 0;
                    Globals.SetGreyScale(grayScaleAmount);
                    isOpeningKnife = false;
                    openingTimer = 7;
                }


                if (phaseIndicator != (int)phase.UnderControl)
                {
                    if (!(phaseIndicator == (int)phase.Opening) && !(phaseIndicator == (int)phase.SkillSurprise))
                    {
                        if (animations.ContainsKey(indicator))
                        {
                            collision = animations[indicator].GetCollision(position);
                            collision = ChangeRectangleSize(collision, 320, 30, true);
                        }
                    }

                    if (phaseIndicator == (int)phase.Opening)
                    {
                        if (knives[0].position.Y < screenCenter.Y)
                        {
                            UpdateKnifeRotation(10);
                            position = new Vector2(0, -800);
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
                        UpdateKnifeRotation(10);
                        UpdateKnifePosition(3000);
                        knifeDestination[0] = screenCenter;
                        if (bombTimer <= 4f && !isSmoke)
                        {
                            SetBlackSmoke(screenCenter, damageRadius);
                            damageCollision = new Circle(screenCenter, damageRadius);
                            ParticleManager.AddParticleEmitter(pe);
                            isSmoke = true;
                            indicator = "WarpIn";
                            position = screenCenter;
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
                            if (animations.ContainsKey(indicator))
                            {
                                collision = animations[indicator].GetCollision(position);
                            }
                        }

                        if (bombTimer <= 0)
                        {
                            phaseIndicator = (int)phase.SkillVaporBlast;
                            ResetSkillSurprise();
                            blastWaitTime = blastSetTime;
                        }
                    }

                    if (phaseIndicator == (int)phase.SkillVaporBlast)
                    {
                        UpdateContainAnimation();
                        UpdateKnifePosition(2000);
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
                            blastCollideCooldown = 0;
                        }
                        else
                        {
                            if (blastSectionIndicator == 1)
                            {
                                blastPosition = new Vector2(Globals.Bounds.X / 6, 0);
                                blastCollision = new Rectangle((int)blastPosition.X - 640 / 2, (int)blastPosition.Y, 640, 1080);
                                knifeDestination[0] = new Vector2(blastPosition.X, blastPosition.Y);
                            }
                            else if (blastSectionIndicator == 2)
                            {
                                blastPosition = new Vector2(screenCenter.X, 0);
                                blastCollision = new Rectangle((int)blastPosition.X - 640 / 2, (int)blastPosition.Y, 640, 1080);
                                knifeDestination[0] = new Vector2(blastPosition.X, blastPosition.Y);
                            }
                            else if (blastSectionIndicator == 3)
                            {
                                blastPosition = new Vector2(5 * Globals.Bounds.X / 6, 0);
                                blastCollision = new Rectangle((int)blastPosition.X - 640 / 2, (int)blastPosition.Y, 640, 1080);
                                knifeDestination[0] = new Vector2(blastPosition.X, blastPosition.Y);
                            }
                            blastWaitTime -= TimeManager.TimeGlobal;
                            if (blastWaitTime <= 0)
                            {
                                UpdateKnifeRotation(20, new Vector2(Globals.Bounds.X, 2));
                                blastCollideCooldown -= TimeManager.TimeGlobal;
                                blastAnim.Update();
                                indicator = "Attack";
                                if (player.collision.Intersects(blastCollision) && blastCollideCooldown <= 0 && blastWaitTime > -0.5f)
                                {
                                    player.Status.TakeDamage(2000, this);
                                    blastCollideCooldown = 0.5f;
                                }
                            }
                            else
                            {
                                UpdateKnifeRotation(15);
                            }
                            if (blastWaitTime <= -1)
                            {
                                if (blastSectionIndicator == 3)
                                {
                                    phaseIndicator = (int)phase.SkillRest;
                                    restTime = 5f;
                                }
                                blastWaitTime = blastSetTime;
                                blastSectionIndicator++;
                                animations["Attack"].Reset();
                            }
                            if (animations["Attack"].IsComplete)
                            {
                                indicator = "Idle";
                            }

                        }
                    }

                    if (phaseIndicator == (int)phase.SkillRest)
                    {
                        indicator = "Sit";
                        UpdateContainAnimation();

                        restTime -= TimeManager.TimeGlobal;
                        healTimer -= TimeManager.TimeGlobal;
                        if (healTimer <= 0)
                        {
                            Status.Heal(((healPercent / 100) * Status.MaxHP) / 5);

                            healTimer = 1f;
                        }
                        if (restTime < 0)
                        {
                            tomatoTimer = 12;
                            phaseIndicator = (int)phase.Tomato;
                        }
                    }

                    if (phaseIndicator == (int)phase.Tomato)
                    {
                        Vector2 dir = new Vector2(0, -1);
                        if (position.Y > -1000)
                            position += dir * 1000 * TimeManager.TimeGlobal;
                        tomatoTimer -= TimeManager.TimeGlobal;
                        if (isTomatoFinish)
                        {
                            phaseIndicator = (int)phase.SkillSurprise;
                            ResetSkillSurprise();
                        }
                    }
                }
                else if (phaseIndicator == (int)phase.UnderControl)
                {
                    if (openingTimer >= 0)
                    {
                        openingTimer -= TimeManager.TimeGlobal;
                        UpdateContainAnimation();
                        if (grayScaleAmount < 1)
                        {
                            grayScaleAmount += 0.1f;
                            Globals.SetGreyScale(grayScaleAmount);
                        }
                        else Globals.SetGreyScale(1);
                        if (animations["WarpIn"].IsComplete && openingTimer > 5) indicator = "Idle";

                        if (openingTimer <= 5 && !isOpeningKnife)
                        {
                            indicator = "WarpOut";
                            isOpeningKnife = true;
                            AddKnife(30);
                            for (int i = 0; i < knifeDestination.Count; i++)
                            {
                                knives[i].position = screenCenter;
                            }
                        }

                        if (openingTimer <= 3)
                        {
                            knifeOpeningTimer -= TimeManager.TimeGlobal;
                            if (knifeOpeningTimer <= 0)
                            {
                                for (int i = 0; i < knifeDestination.Count; i++)
                                {
                                    int x = Globals.random.Next(-1920, 1920 + 1920);
                                    int y = Globals.random.Next(-1080, 1080 + 1080);
                                    knifeDestination[i] = new Vector2(x, y);
                                }
                                knifeOpeningTimer = 0.1f;
                            }
                            UpdateKnifePosition(10000);
                            UpdateKnifeRotation(100);
                        }

                        if (animations["WarpOut"].IsComplete)
                        {
                            positionIndex = positionRandomLoop.GetNext();
                            position = Vector2.Zero;

                        }
                        if (openingTimer <= 0)
                        {
                            animations["WarpIn"].Reset();
                            indicator = "WarpIn";
                            isPrepareKnife = false;
                            knifeAttackingTimer = 7f;
                        }
                    }
                    else
                    {
                        if (!isPrepareKnife)
                        {
                            isAttackKnife = false;
                            foreach (var knife in knives)
                            {
                                knife.position = new Vector2(screenCenter.X, -300);
                            }
                            switch (positionIndex)
                            {
                                case 1:
                                    position = positionUnderControl[positionIndex];
                                    knifeStartArea1 = knifeAreas[7];
                                    knifeStartArea2 = knifeAreas[2];
                                    knifeEndArea1 = knifeAreas[4];
                                    knifeEndArea2 = knifeAreas[5];
                                    knifeMethod = 1;
                                    isPrepareKnife = true;
                                    break;
                                case 2:
                                    position = positionUnderControl[positionIndex];
                                    knifeStartArea1 = knifeAreas[1];
                                    knifeStartArea2 = knifeAreas[7];
                                    knifeEndArea1 = knifeAreas[6];
                                    knifeEndArea2 = knifeAreas[4];
                                    knifeMethod = 2;
                                    isPrepareKnife = true;
                                    break;
                                case 3:
                                    position = positionUnderControl[positionIndex];
                                    knifeStartArea1 = knifeAreas[2];
                                    knifeStartArea2 = knifeAreas[7];
                                    knifeEndArea1 = knifeAreas[5];
                                    knifeEndArea2 = knifeAreas[4];
                                    knifeMethod = 1;
                                    isPrepareKnife = true;
                                    break;
                                case 4:
                                    position = positionUnderControl[positionIndex];
                                    knifeStartArea1 = knifeAreas[1];
                                    knifeStartArea2 = knifeAreas[6];
                                    knifeEndArea1 = knifeAreas[7];
                                    knifeEndArea2 = knifeAreas[4];
                                    knifeMethod = 2;
                                    isPrepareKnife = true;
                                    break;
                            }
                        }
                        else
                        {
                            if (!isAttackKnife)
                            {
                                var temp1 = randomPointGenerator.GenerateRandomPointsInRectangle(knifeStartArea1, knives.Count / 2);
                                var temp2 = randomPointGenerator.GenerateRandomPointsInRectangle(knifeStartArea2, knives.Count / 2);
                                var temp3 = randomPointGenerator.GenerateRandomPointsInRectangle(knifeEndArea1, knives.Count / 2);
                                var temp4 = randomPointGenerator.GenerateRandomPointsInRectangle(knifeEndArea2, knives.Count / 2);
                                for (int i = 0; i < knives.Count; i++)
                                {
                                    if (i < knives.Count / 2)
                                    {
                                        knives[i].position = temp1[i];
                                        knifeDestination[i] = temp3[i];
                                    }
                                    else
                                    {
                                        knives[i].position = temp2[i - knives.Count / 2];
                                        knifeDestination[i] = temp4[i - knives.Count / 2];
                                    }
                                }
                                isAttackKnife = true;
                                indicator = "WarpIn";
                                animations["WarpOut"].Reset();
                            }
                            else
                            {

                                UpdateContainAnimation();

                                if (animations["WarpIn"].IsComplete)
                                {
                                    indicator = "Attack";
                                }

                                if (animations["Attack"].IsComplete)
                                {
                                    knifeAttackingTimer -= TimeManager.TimeGlobal;
                                    indicator = "Idle";
                                    knifeAttackingTimer = 7f;
                                }

                                if (knifeMethod == 1)
                                {
                                    if (knifeAttackingTimer > 0)
                                    {
                                        UpdateKnifePosition(4000);
                                        UpdateKnifeRotation(10);
                                    }
                                }
                                else if (knifeMethod == 2)
                                {
                                    UpdateKnifeRotation(10);
                                    if (knifeAttackingTimer > 7 - 3.5f)
                                    {
                                        for (int i = 0; i < knives.Count/2; i++)
                                        {
                                            UpdateKnifePosition(4000,i);
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < knives.Count / 2; i++)
                                        {
                                            UpdateKnifePosition(4000, i + knives.Count / 2);
                                        }
                                    }
                                }

                                if (knifeAttackingTimer <= 0)
                                {
                                    indicator = "WarpOut";
                                }

                                if (animations["WarpOut"].IsComplete)
                                {
                                    isPrepareKnife = false;
                                    foreach(var anim in animations.Values)
                                    {
                                        anim.Reset();
                                    }
                                    positionIndex = positionRandomLoop.GetNext();
                                }
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
                        blastAnim.offset = new Vector2(0, blastAnim.GetCollision(blastPosition).Height / 2);
                        blastAnim.Draw(blastPosition);
                    }
                }
                else if (phaseIndicator == (int)phase.UnderControl)
                {

                }
            }
            base.Draw();
            foreach (var knife in knives)
            {
                knife.Draw();
            }

            DrawCollisionCheck();
        }

        private void UpdateContainAnimation()
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
            }
        }

        private void UpdateKnifeRotation(float rotationSpeed)
        {
            for (int i = 0; i < knives.Count; i++)
            {
                float targetAngle = (float)Math.Atan2(knives[i].position.Y - knifeDestination[i].Y, knives[i].position.X - knifeDestination[i].X);

                float angleDifference = MathHelper.WrapAngle(targetAngle - knives[i].rotation);

                if (Math.Abs(angleDifference) < 0.01f)
                {
                    knives[i].rotation = targetAngle;
                }
                else
                {
                    knives[i].rotation += MathHelper.Clamp(angleDifference, -rotationSpeed * TimeManager.TimeGlobal, rotationSpeed * TimeManager.TimeGlobal);
                }
            }
        }

        private void UpdateKnifeRotation(float rotationSpeed, Vector2 pointDirection)
        {
            for (int i = 0; i < knives.Count; i++)
            {
                float targetAngle = (float)Math.Atan2(knives[i].position.Y - pointDirection.Y, knives[i].position.X - pointDirection.X);

                float angleDifference = MathHelper.WrapAngle(targetAngle - knives[i].rotation);

                if (Math.Abs(angleDifference) < 0.01f)
                {
                    knives[i].rotation = targetAngle;
                }
                else
                {
                    knives[i].rotation += MathHelper.Clamp(angleDifference, -rotationSpeed * TimeManager.TimeGlobal, rotationSpeed * TimeManager.TimeGlobal);
                }
            }
        }

        private void UpdateKnifePosition(float speed)
        {
            for (int i = 0; i < knives.Count; ++i)
            {
                var dir = knifeDestination[i] - knives[i].position;
                float distance = dir.Length();
                if (distance < speed * TimeManager.TimeGlobal)
                {
                    knives[i].position = knifeDestination[i];
                }
                else
                {
                    dir.Normalize();
                    knives[i].position += dir * speed * TimeManager.TimeGlobal;
                }
            }
        }
        private void UpdateKnifePosition(float speed, int knifeIndex)
        {
            var dir = knifeDestination[knifeIndex] - knives[knifeIndex].position;
            float distance = dir.Length();
            if (distance < speed * TimeManager.TimeGlobal)
            {
                knives[knifeIndex].position = knifeDestination[knifeIndex];
            }
            else
            {
                dir.Normalize();
                knives[knifeIndex].position += dir * speed * TimeManager.TimeGlobal;
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
                knives.Add(new Knife(KnifeTexture, new Vector2(screenCenter.X, -300), 0, 150, 1, 4));
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
            animations["WarpIn"].Reset();
            bombTimer = 5f;
            isSmoke = false;
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

        public void DrawCollisionCheck()
        {
            Vector2 topLeft1 = new Vector2(collision.Left, collision.Top);
            Vector2 topRight1 = new Vector2(collision.Right, collision.Top);
            Vector2 bottomLeft1 = new Vector2(collision.Left, collision.Bottom);
            Vector2 bottomRight1 = new Vector2(collision.Right, collision.Bottom);

            Globals.DrawLine(topLeft1, topRight1, Color.Red, 1);
            Globals.DrawLine(bottomLeft1, bottomRight1, Color.Red, 1);
        }
    }
}
