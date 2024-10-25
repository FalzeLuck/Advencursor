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
using System.Diagnostics.Contracts;

namespace Advencursor._Models.Enemy.Stage2
{
    public class Boss3 : _Enemy
    {
        private StaticEmitter staticEmitter;

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private Texture2D warningTexture;
        private Texture2D warningTexture2;
        private Vector2 warningDirection;
        private Sprite player;
        private bool warningTrigger;
        private float warningOpacity;
        private Vector2 screenCenter = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);

        private bool isStart;
        private float buffRadius = 600;
        public enum phase
        {
            Opening,
            SkillSurprise,
            SkillVaporBlast,
            SkillRest,
            Tomato,
            UnderControl,
            Die,
        }
        public int phaseIndicator;

        private List<Knife> knives;
        private List<Vector2> knifeDestination;

        //Skill Surprise
        public bool isOpening1 = false;
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

        //Skill Rest
        private float reduceDamage = 20;
        private float healPercent = 5;
        private float healTimer;
        private float restTime;

        //Tomato
        private float tomatoTimer = 0;
        public bool isTomatoFinish => tomatoTimer <= 0;

        //Under Control
        public float openingTimer = 5f;
        public bool isOpening2 = false;
        private float grayScaleAmount;
        private Dictionary<int, Vector2> positionUnderControl;
        private List<int> positionIndexList;
        private int positionIndex;
        private RandomLoop<int> positionRandomLoop;
        private float knifeOpeningTimer;
        private bool isOpeningKnife;
        private bool isPrepareKnife;
        private bool isAttackKnife;
        private bool isStartKnifeMove;
        private int knifeMethod;
        private float knifeAttackingTimer;
        private RandomPointGenerator randomPointGenerator;
        private Rectangle knifeStartArea1;
        private Rectangle knifeStartArea2;
        private Rectangle knifeEndArea1;
        private Rectangle knifeEndArea2;
        private Dictionary<int, Rectangle> knifeAreas;
        Texture2D KnifeTexture;
        private List<bool> isKnifeSound;
        private Vector2Pool vectorPool = new Vector2Pool();
        private bool isPhase2Warning = false;

        //Die
        private float dieTimer;
        private bool isDieSound = false;
        public Boss3(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,4,1,8, true) },
                { "Sit", new(texture, row, column,4,2,8, true) },
                { "Attack", new(texture, row, column,4,3,8, false) },
                { "Die", new(texture, row, column,8,4,8, false) },
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
            isKnifeSound = new List<bool> { false };
            KnifeTexture = Globals.Content.Load<Texture2D>("Enemies/Boss3Knife");
            positionIndexList = new List<int>()
            {
                1,2,3,4
            };
            positionUnderControl = new Dictionary<int, Vector2>()
            {
                {1,new Vector2(Globals.Bounds.X / 4, Globals.Bounds.Y / 4)},
                {2,new Vector2(Globals.Bounds.X / 4 * 3, Globals.Bounds.Y / 4)},
                {3,new Vector2(Globals.Bounds.X / 4 * 3, Globals.Bounds.Y / 4 * 3)},
                {4,new Vector2(Globals.Bounds.X / 4, Globals.Bounds.Y / 4 * 3)},
            };
            positionRandomLoop = new RandomLoop<int>(positionIndexList);
            randomPointGenerator = new RandomPointGenerator();
            knifeAreas = new Dictionary<int, Rectangle>() //Start Topleft Rotate Clockwise
            {
                {1, new Rectangle(0,-Globals.Bounds.Y, (int)screenCenter.X, Globals.Bounds.Y - 200)},
                {2, new Rectangle((int)screenCenter.X,-Globals.Bounds.Y, (int)screenCenter.X, Globals.Bounds.Y - 200)},
                {3, new Rectangle(Globals.Bounds.X*2,0, Globals.Bounds.X - 200, (int)screenCenter.Y)},
                {4, new Rectangle(Globals.Bounds.X*2,(int)screenCenter.Y, Globals.Bounds.X - 200, (int)screenCenter.Y)},
                {5, new Rectangle((int)screenCenter.X,Globals.Bounds.Y*2, (int)screenCenter.X, Globals.Bounds.Y - 200)},
                {6, new Rectangle(0,Globals.Bounds.Y * 2, (int)screenCenter.X, Globals.Bounds.Y - 200)},
                {7, new Rectangle(-Globals.Bounds.X,((int)screenCenter.Y),Globals.Bounds.X - 200, (int)screenCenter.Y)},
                {8, new Rectangle(-Globals.Bounds.X,0, Globals.Bounds.X - 200, (int)screenCenter.Y)},
            };


            warningTexture = new Texture2D(Globals.graphicsDevice, 640, 1080);
            warningTexture = Globals.CreateRectangleTexture(640, 1080, Color.Red);
            SetBlackSmoke(screenCenter, damageRadius);
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
            foreach (Knife knife in knives)
            {
                knife.Update(gameTime);
            }

            if (isStart)
            {
                if (animations["WarpIn"].currentFrame == 50 || animations["WarpOut"].currentFrame == 62)
                {
                    Globals.soundManager.PlaySound("Boss3WarpEffect");
                }
                if (phaseIndicator != (int)phase.UnderControl && phaseIndicator != (int)phase.Die)
                {
                    baseAmp = 1f;
                    if (!(phaseIndicator == (int)phase.Opening) && !(phaseIndicator == (int)phase.SkillSurprise))
                    {
                        UpdateCollision();
                    }

                    if (phaseIndicator == (int)phase.Opening)
                    {
                        isOpening1 = true;
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
                        UpdateKnifeRotation(10);
                        UpdateKnifePosition(5000);
                        knifeDestination[0] = screenCenter;
                        if (bombTimer <= 4f && !isSmoke)
                        {
                            Globals.soundManager.PlaySound("Boss3Surprise");
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
                            isOpening1 = false;
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
                            if(blastWaitTime > 0)
                                UpdateKnifePosition(2000);
                            if (blastWaitTime <= 0)
                            {
                                UpdateKnifeRotation(20, new Vector2(Globals.Bounds.X, 2));
                                blastCollideCooldown -= TimeManager.TimeGlobal;
                                blastAnim.Update();
                                Globals.soundManager.PlaySound("Boss3Laser");
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

                        knives[0].position = new Vector2(screenCenter.X, -100);
                        restTime -= TimeManager.TimeGlobal;
                        healTimer -= TimeManager.TimeGlobal;
                        if (healTimer <= 0)
                        {
                            Status.Heal(((healPercent / 100) * Status.MaxHP) / 5);
                            Globals.soundManager.PlaySound("Boss3Healing");
                            healTimer = 1f;
                        }
                        if (restTime < 0)
                        {
                            Globals.soundManager.PlaySound("Boss3Warp");
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
                        if (!isPhase2Warning)
                        {
                            warningTexture = new Texture2D(Globals.graphicsDevice, (int)screenCenter.X, Globals.Bounds.Y);
                            warningTexture = Globals.CreateRectangleTexture((int)screenCenter.X, Globals.Bounds.Y, Color.Red);
                            warningTexture2 = new Texture2D(Globals.graphicsDevice, Globals.Bounds.X, (int)screenCenter.Y);
                            warningTexture2 = Globals.CreateRectangleTexture(Globals.Bounds.X, (int)screenCenter.Y, Color.Red);
                            isPhase2Warning = true;
                            GC.Collect();
                        }
                        isOpening2 = true;
                        openingTimer -= TimeManager.TimeGlobal;
                        UpdateContainAnimation();
                        if (grayScaleAmount < 0.5f)
                        {
                            grayScaleAmount += 0.05f;
                            Globals.SetGreyScale(grayScaleAmount);
                        }
                        else Globals.SetGreyScale(0.5f);
                        if (animations["WarpIn"].IsComplete && openingTimer > 5) indicator = "Idle";

                        if (openingTimer <= 5 && !isOpeningKnife)
                        {
                            Globals.soundManager.PlaySound("Boss3Warp");
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
                        isOpening2 = false;
                        baseAmp = 0.4f;
                        if (!isPrepareKnife)
                        {
                            foreach (var knife in knives)
                            {
                                knife.haveDamage = true;
                            }
                            indicator = "WarpIn";
                            isAttackKnife = false;
                            isStartKnifeMove = false;
                            foreach (var knife in knives)
                            {
                                knife.position = new Vector2(screenCenter.X, -300);
                            }
                            switch (positionIndex)
                            {
                                case 1:
                                    position = positionUnderControl[positionIndex];
                                    knifeStartArea1 = knifeAreas[2];
                                    knifeStartArea2 = knifeAreas[7];
                                    knifeEndArea1 = knifeAreas[5];
                                    knifeEndArea2 = knifeAreas[4];
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
                                    knifeStartArea2 = knifeAreas[7];
                                    knifeEndArea1 = knifeAreas[6];
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
                                indicator = "WarpIn";
                                animations["WarpOut"].Reset();
                                if (knifeMethod == 1)
                                {
                                    knifeAttackingTimer = 2f;
                                }
                                else if (knifeMethod == 2)
                                {
                                    knifeAttackingTimer = 5f;
                                }
                                isAttackKnife = true;
                            }
                            else
                            {
                                UpdateContainAnimation();
                                if (animations["WarpIn"].IsComplete)
                                {
                                    indicator = "Attack";
                                    UpdateCollision();
                                }

                                if (animations["Attack"].IsComplete)
                                {
                                    isStartKnifeMove = true;
                                    indicator = "Idle";
                                }
                                if (isStartKnifeMove)
                                {
                                    knifeAttackingTimer -= TimeManager.TimeGlobal;
                                    if (knifeMethod == 1)
                                    {
                                        UpdateKnifePosition(5000);
                                        UpdateKnifeRotation(100);

                                    }
                                    else if (knifeMethod == 2)
                                    {
                                        UpdateKnifeRotation(100);
                                        if (knifeAttackingTimer > 5 - 3.5f)
                                        {
                                            for (int i = 0; i < knives.Count / 2; i++)
                                            {
                                                UpdateKnifePosition(5000, i);
                                            }
                                        }
                                        if (knifeAttackingTimer > 5 - 3.1f && knifeAttackingTimer < 5 - 3f)
                                        {
                                            animations["Attack"].Reset();
                                            indicator = "Attack";
                                        }
                                        if (knifeAttackingTimer < 5 - 3.5f)
                                        {
                                            for (int i = 0; i < knives.Count / 2; i++)
                                            {
                                                UpdateKnifePosition(5000, i + knives.Count / 2);
                                            }
                                        }
                                    }
                                }

                                if (knifeAttackingTimer <= 0)
                                {
                                    indicator = "WarpOut";
                                }

                                if (animations["WarpOut"].IsComplete)
                                {
                                    Globals.soundManager.PlaySound("Boss3Warp");
                                    collision = new Rectangle();
                                    position = new Vector2(-600);
                                    isPrepareKnife = false;
                                    foreach (var anim in animations.Values)
                                    {
                                        anim.Reset();
                                    }
                                    positionIndex = positionRandomLoop.GetNext();
                                    GC.Collect();
                                }
                            }
                        }
                    }
                }
                else if (phaseIndicator == (int)phase.Die)
                {
                    UpdateContainAnimation();
                    dieTimer -= TimeManager.TimeGlobal;
                    if (dieTimer <= 4)
                    {
                        indicator = "Die";
                    }

                    if (!isDieSound)
                    {
                        Globals.soundManager.PlaySound("Boss3Die");
                        isDieSound = true;
                    }
                }

                if (Status.CurrentHP <= 0.3 * Status.MaxHP && phaseIndicator != (int)phase.UnderControl && phaseIndicator != (int)phase.Die)
                {
                    position = screenCenter;
                    phaseIndicator = (int)phase.UnderControl;
                    tomatoTimer = 0;
                    collision = new Rectangle();
                    knives.Clear();
                    knifeDestination.Clear();
                    isKnifeSound.Clear();
                    indicator = "WarpIn";
                    animations[indicator].Reset();
                    grayScaleAmount = 0;
                    Globals.SetGreyScale(grayScaleAmount);
                    isOpeningKnife = false;
                    openingTimer = 7;
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
                    if (isAttackKnife)
                    {
                        if (!animations["Attack"].IsComplete)
                        {
                            if (warningOpacity <= 0.3f)
                            {
                                warningTrigger = true;
                            }
                            else if (warningOpacity >= 0.5f)
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
                            Globals.EndDrawGrayScale();
                            Globals.SpriteBatch.Draw(warningTexture, new Vector2(knifeStartArea1.X, 0), null, Color.White * warningOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            Globals.SpriteBatch.Draw(warningTexture2, new Vector2(0, screenCenter.Y), null, Color.White * warningOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            Globals.BeginDrawGrayScale();
                        }
                        else
                        {
                            isPhase2Warning = false;
                        }
                    }
                }
            }
            base.Draw();

            DrawShadow();
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

        private void UpdateCollision()
        {
            collision = animations[indicator].GetCollision(position);
            collision = ChangeRectangleSize(collision, 320, 30, true);
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
                Vector2 dir = vectorPool.Get();
                dir = knifeDestination[i] - knives[i].position;
                float distance = dir.Length();
                if (distance == 0)
                {
                    knives[i].position = knifeDestination[i];
                    isKnifeSound[i] = false;
                }
                else if (distance < speed * TimeManager.TimeGlobal)
                {
                    knives[i].position = knifeDestination[i];
                }
                else
                {
                    if (!isKnifeSound[i])
                    {
                        Globals.soundManager.PlaySoundCanStack("Boss3Knife");
                        isKnifeSound[i] = true;
                    }
                    dir.Normalize();
                    knives[i].position += dir * speed * TimeManager.TimeGlobal;
                }
                vectorPool.Return(dir);
            }
        }
        private void UpdateKnifePosition(float speed, int knifeIndex)
        {
            Vector2 dir = vectorPool.Get();
            dir = knifeDestination[knifeIndex] - knives[knifeIndex].position;
            float distance = dir.Length();
            if (distance == 0)
            {
                knives[knifeIndex].position = knifeDestination[knifeIndex];
                isKnifeSound[knifeIndex] = false;
            }
            else if (distance < speed * TimeManager.TimeGlobal)
            {
                knives[knifeIndex].position = knifeDestination[knifeIndex];
            }
            else
            {
                if (!isKnifeSound[knifeIndex])
                {
                    Globals.soundManager.PlaySoundCanStack("Boss3Knife");
                    isKnifeSound[knifeIndex] = true;
                }
                dir.Normalize();
                knives[knifeIndex].position += dir * speed * TimeManager.TimeGlobal;
            }
            vectorPool.Return(dir);
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
            for (int i = 0; i < amount; i++)
            {
                knives.Add(new Knife(KnifeTexture, new Vector2(screenCenter.X, -300), 0, 150, 1, 4, player));
                isKnifeSound.Add(false);
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

        private void DrawShadow()
        {
            Vector2 shadowPosition = new Vector2(position.X, position.Y + 380);
            float shadowScale = 1.3f;
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

        public override void Die()
        {
            foreach (var anim in animations.Values)
            {
                anim.Reset();
            }
            isAttackKnife = false;
            tomatoTimer = 0;
            collision = new Rectangle();
            knives.Clear();
            knifeDestination.Clear();
            isKnifeSound.Clear();
            indicator = "WarpIn";
            animations[indicator].Reset();
            dieTimer = 5f;
            phaseIndicator = (int)phase.Die;
        }
    }
}
