using Advencursor._Animation;
using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Models.Enemy._CommonEnemy;
using Advencursor._Models;
using Advencursor._SaveData;
using Advencursor._Skill;
using Advencursor._UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Advencursor._Particles;
using System.Diagnostics;

namespace Advencursor._Scene.Stage
{
    public abstract class StageAbstract : IScene
    {
        protected ContentManager contentManager;
        protected SceneManager sceneManager;
        protected AnimationManager animationManager = new AnimationManager();
        protected SoundManager soundManager = new SoundManager();
        protected DamageNumberManager damageNumberManager;

        protected SpriteFont font;


        protected Player player;
        protected Inventory inventory = new Inventory();
        protected GameData gameData = new GameData();

        protected List<Common1> commonEnemy;
        protected List<Item> items;


        //Pause Variable
        protected bool isPause;



        protected Timer timer;
        protected Texture2D background;

        protected readonly Random random = new Random();


        //Stage Timer & Controls
        protected float boss_spawn_time;
        protected float boss_dash_cooldown;
        protected bool boss_spawned;
        protected float enemy_spawn_time;
        protected float elite_spawn_time;
        protected int enemy_count = 0;
        protected int elite_count = 0;
        protected int enemy_max = 30;
        protected int elite_max = 2;
        protected int enemy_killed = 0;

        //UI
        protected UIManager uiManager = new UIManager();
        protected UIManager pauseUiManager = new UIManager();

        public virtual void Load()
        {
            Texture2D tempTexture = new Texture2D(Globals.graphicsDevice, 1, 1);

            //Player
            Texture2D playertexture = Globals.Content.Load<Texture2D>("playerTexture");
            player = new(playertexture, new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2), 1, 1, 1, 1);
            player.LoadPlayer(2, 1);
            damageNumberManager.SubscribeToTakeDamageEvent(player.Status, player);
            inventory.LoadInventory(tempTexture);
            gameData.LoadData();



            //Load Animation
            Animation slashAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/SlashTexture"), row: 1, column: 1, fps: 8, false, 1.5f);
            animationManager.AddAnimation("Slash", slashAnimation);

            //Load UI
            UIBackground uIBackground = new(Globals.Content.Load<Texture2D>("UI/SkillBackground"), new(Globals.Bounds.X / 2, 930));
            int startX = (int)uIBackground.position.X - (uIBackground.texture.Width / 2);
            int space = uIBackground.texture.Width / 10;
            int skillY = 950;


            UISkill skillUI_Q = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 2), skillY), player.Skills[Keys.Q]);
            UISkill skillUI_W = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 4), skillY), player.Skills[Keys.W]);
            UISkill skillUI_E = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 6), skillY), player.Skills[Keys.E]);
            UISkill skillUI_R = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 8), skillY), player.Skills[Keys.R]);
            UIPlayerCheckPanel uIPanel = new(Globals.Content.Load<Texture2D>("TestUI"), new(150, 100), player);

            Texture2D bg = Globals.Content.Load<Texture2D>("UI/HealthBarNone");
            Texture2D fg = Globals.Content.Load<Texture2D>("UI/HealthBarFull");
            ProgressBarAnimated playerHpBar = new ProgressBarAnimated(bg, fg, player.Status.MaxHP, new(Globals.Bounds.X / 2, uIBackground.position.Y - 75));


            uiManager.AddElement("uiBackground", uIBackground);
            uiManager.AddElement("playerBar", playerHpBar);
            uiManager.AddElement("skillUI_Q", skillUI_Q);
            uiManager.AddElement("skillUI_W", skillUI_W);
            uiManager.AddElement("skillUI_E", skillUI_E);
            uiManager.AddElement("skillUI_R", skillUI_R);


            Mouse.SetPosition(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
        }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        protected virtual void UpdatePlayer()
        {
            if (!player.isStun && !player.isStop)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    player.UseSkill(Keys.Q);
                    player.ChangeAnimation("Idle");
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    player.UseSkill(Keys.W);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    player.UseSkill(Keys.E);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    player.UseSkill(Keys.R);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    player.StartParry();
                }

                if (player.CanNormalAttack() || animationManager.animations["Slash"].IsComplete)
                {
                    foreach (var enemy in commonEnemy)
                    {
                        if (enemy.Status.immunity)
                        {
                            enemy.Status.immunity = false;
                        }
                    }
                    player.ChangeAnimation("Idle");
                    animationManager.Stop("Slash");
                }

                if (InputManager.MouseRightClicked && player.CanNormalAttack())
                {
                    animationManager.SetOffset("Slash", new Vector2(player.collision.Width / 2, 0));
                    player.ChangeAnimation("Attack", true);
                    animationManager.Flip("Slash", true);
                    animationManager.Play("Slash");
                    player.DoNormalAttack();
                }
                if (InputManager.MouseLeftClicked && player.CanNormalAttack())
                {
                    animationManager.SetOffset("Slash", new Vector2(-player.collision.Width / 2, 0));
                    player.ChangeAnimation("Attack", false);
                    animationManager.Flip("Slash", false);
                    animationManager.Play("Slash");
                    player.DoNormalAttack();
                }

            }



        }

        protected virtual void SceneManage()
        {
            if (!player.Status.IsAlive())
            {
                GotoSummary(false);
            }
        }
        protected virtual void UnloadScene()
        {
            damageNumberManager.UnSubscribeToTakeDamageEvent(player.Status, player);
            foreach (var enemy in Globals.EnemyManager)
            {
                damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
            }
            Globals.EnemyManager.Clear();
            inventory.Items.Clear();
            commonEnemy.Clear();
            TimeManager.ChangeGameSpeed(1);
            AllSkills.Reset();
            ParticleManager.RemoveAll();
        }

        protected void CheckPause(GameTime gameTime)
        {
            pauseUiManager.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                isPause = true;
                TimeManager.ChangeGameSpeed(0f);

                Vector2 screenOrigin = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
                Texture2D continueTexture = Globals.Content.Load<Texture2D>("UI/PauseButtonPlay");

                UIElement continueButton = new UIButton(continueTexture, screenOrigin, OnContinueClick);


                pauseUiManager.AddElement("continue", continueButton);
            }

        }

        protected void OnContinueClick()
        {
            isPause = false;
            Mouse.SetPosition((int)player.position.X, (int)player.position.Y);
            TimeManager.ChangeGameSpeed(1f);
        }

        protected void DrawPause()
        {
            Texture2D dimTexture = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Black);
            Texture2D pauseBackground = Globals.Content.Load<Texture2D>("UI/PauseBackground");
            Vector2 pauseBgOrigin = new Vector2(pauseBackground.Width / 2, pauseBackground.Height / 2);

            Globals.SpriteBatch.Draw(dimTexture, Vector2.Zero, Color.White * 0.8f);
            Globals.SpriteBatch.Draw(pauseBackground, new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2), null, Color.White, 0f, pauseBgOrigin, 1f, SpriteEffects.None, 0f);

            pauseUiManager.Draw(Globals.SpriteBatch);
            Globals.DrawCursor();
        }

        protected void GotoSummary(bool win)
        {
            timer.StartStop();
            float time = timer.timeLeft;
            UnloadScene();
            sceneManager.AddScene(new SummaryScene(contentManager, sceneManager, win, 100,gameData,time));
        }
    }
}
