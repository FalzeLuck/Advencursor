using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Models.Enemy;
using Advencursor._Models.Enemy._CommonEnemy;
using Advencursor._Models.Enemy.Stage1;
using Advencursor._Particles;
using Advencursor._SaveData;
using Advencursor._Scene.Transition;
using Advencursor._Skill;
using Advencursor._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene.Stage
{
    public class StageTutorial : StageAbstract
    {
        private Common1 tutorialDummy;
        private string tutorialText;
        private Vector2 tutorialPosition;
        private Vector2 tutorialOrigin;
        private float tutorialTime;
        private bool tutorialFinished;

        private float afterKillDummy;

        public StageTutorial(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            Globals.Game.IsMouseVisible = false;
            font = Globals.Content.Load<SpriteFont>("basicFont");
            textFont = Globals.Content.Load<SpriteFont>("Font/TextFont");
            damageNumberManager = new(Globals.Content.Load<SpriteFont>("Font/DamageNumber"));
            isPause = false;
            boss_spawned = false;
        }


        public override void Load()
        {
            base.Load();
            Texture2D tempTexture = new Texture2D(Globals.graphicsDevice, 1, 1);

            //Load Background
            background = Globals.Content.Load<Texture2D>("Background/Stage0");

            //Load enemies
            tutorialDummy = new Common1(
                        Globals.Content.Load<Texture2D>("Enemies/Common1"),
                        new Vector2(Globals.Bounds.X - 500, Globals.Bounds.Y / 2),
                        health: 1000000,
                        attack: 100,
                        row: 3,
                        column: 8
                        )
            {
                movementAI = new FollowMovementAI()
            };
            damageNumberManager.SubscribeToTakeDamageEvent(tutorialDummy.Status, tutorialDummy);
            Globals.EnemyManager.Add(tutorialDummy);

            Item tempQ = new(AllSkills.itemNameViaSkillName["Thunder Core"], AllSkills.allSkills["Thunder Core"], Keys.Q);
            Item tempW = new(AllSkills.itemNameViaSkillName["Thunder Shuriken"], AllSkills.allSkills["Thunder Shuriken"], Keys.W);
            Item tempE = new(AllSkills.itemNameViaSkillName["Thunder Speed"], AllSkills.allSkills["Thunder Speed"], Keys.E);
            Item tempR = new(AllSkills.itemNameViaSkillName["I AM the Storm"], AllSkills.allSkills["I AM the Storm"], Keys.R);
            player.EquipItem(tempQ);
            player.EquipItem(tempW);
            player.EquipItem(tempE);
            player.EquipItem(tempR);

            uiManager.RemoveAll();

            //Load UI
            UIBackground uIBackground = new(Globals.Content.Load<Texture2D>("UI/SkillBackground"), new(Globals.Bounds.X / 2, 930));
            int startX = (int)uIBackground.position.X - (uIBackground.texture.Width / 2);
            int space = uIBackground.texture.Width / 12;
            int skillY = 980;


            UISkill skillUI_Q = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 3), skillY), player.Skills[Keys.Q]);
            UISkill skillUI_W = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 5), skillY), player.Skills[Keys.W]);
            UISkill skillUI_E = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 7), skillY), player.Skills[Keys.E]);
            UISkill skillUI_R = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 9), skillY), player.Skills[Keys.R]);
            UIPlayerCheckPanel uIPanel = new(Globals.Content.Load<Texture2D>("TestUI"), new(150, 100), player);

            Texture2D bg = Globals.Content.Load<Texture2D>("UI/HealthBarNone");
            Texture2D fg = Globals.Content.Load<Texture2D>("UI/HealthBarFull");
            ProgressBarAnimated playerHpBar = new ProgressBarAnimated(bg, fg, player.Status.MaxHP, new(Globals.Bounds.X / 2, uIBackground.position.Y - 75));


            //uiManager.AddElement("uiBackground", uIBackground);
            uiManager.AddElement("playerBar", playerHpBar);
            uiManager.AddElement("skillUI_Q", skillUI_Q);
            uiManager.AddElement("skillUI_W", skillUI_W);
            uiManager.AddElement("skillUI_E", skillUI_E);
            uiManager.AddElement("skillUI_R", skillUI_R);


            tutorialPosition = new Vector2(Globals.Bounds.X/2,300);
            tutorialText = "Move your mouse to control";
            tutorialTime = 9f;
            tutorialFinished = false;
        }

        public override void Update(GameTime gameTime)
        {
            CheckPause(gameTime);
            tutorialOrigin = textFont.MeasureString(tutorialText);
            tutorialOrigin = new Vector2(tutorialOrigin.X/2,tutorialOrigin.Y/2);
            if (!isPause)
            {
                if (!tutorialFinished)
                {
                    tutorialTime -= TimeManager.TimeGlobal;
                    if (tutorialTime <= 0)
                    {
                        tutorialFinished = true;
                        tutorialText = "Press Space to continue";
                    }
                    else if (tutorialTime <= 3)
                    {
                        tutorialText = "Press Q,W,E,R to use your skill";
                    }
                    else if (tutorialTime <= 6)
                    {
                        tutorialText = "Hold Left Click or Right Click to attack";
                    }
                }

                tutorialDummy.Update(gameTime);
                tutorialDummy.position = new Vector2(Globals.Bounds.X - 500, Globals.Bounds.Y / 2);
                UiManage(gameTime);
                CollisionManage(gameTime);
                UpdatePlayer();
                player.Update(gameTime);
                animationManager.Update(gameTime);
                animationManager.UpdatePosition("Slash", player.position);
                ParticleManager.Update();
                SceneManage();
                startWarning = false;
                if (player.CanNormalAttack())
                {
                        if (tutorialDummy.Status.immunity)
                        {
                            tutorialDummy.Status.immunity = false;
                        }
                }
            }
            if (isPause)
            {
                pauseUiManager.RemoveElement("restart");
                pauseUiManager.RemoveElement("exit");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(textFont, tutorialText, tutorialPosition, Color.Black, 0f, tutorialOrigin, 0.7f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(textFont, "Press Space to Skip Tutorial", new Vector2(30,Globals.Bounds.Y-50), Color.Black, 0f, Vector2.Zero, 0.2f, SpriteEffects.None, 0f);
            tutorialDummy.Draw();
            player.Draw();
            animationManager.Draw();
            ParticleManager.Draw();
            damageNumberManager.Draw();
            uiManager.Draw(spriteBatch);

            if (isPause)
            {
                DrawPause();
            }

        }



        private void UiManage(GameTime gameTime)
        {
            //UI Control
            if (uiManager.CheckCollide(player))
            {
                uiManager.SetAllOpacity(0.5f);
            }
            else { uiManager.SetAllOpacity(1f); }

            uiManager.Update(gameTime);
            uiManager.UpdateBarValue("playerBar", player.Status.CurrentHP);
        }
        protected override void CollisionManage(GameTime gameTime)
        {
            if (tutorialDummy.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
            {
                tutorialDummy.TakeDamage(1, player);
                tutorialDummy.Status.immunity = true;
            }

            damageNumberManager.Update();
        }

        protected override void SceneManage()
        {
            if (!player.Status.IsAlive())
            {
                player.Status.Heal(player.Status.MaxHP);
                tutorialText = "You died to it. Really?";
            }


            if (!tutorialDummy.Status.IsAlive())
            {
                tutorialText = "Woah! You kill it? Nice Patience. Now get out...";
                damageNumberManager.UnSubscribeToTakeDamageEvent(tutorialDummy.Status, tutorialDummy);
                Globals.EnemyManager.Remove(tutorialDummy);
                afterKillDummy = 60f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                gameData.stage = 1;
                gameData.isFirstTime = true;
                gameData.SaveData();
                AllSkills.Reset();
                sceneManager.AddScene(new StageSelectScene(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
            }
        }



    }
}
