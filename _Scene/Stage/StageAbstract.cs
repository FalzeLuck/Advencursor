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
using Advencursor._Scene.Transition;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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
        protected SpriteFont textFont;

        protected Player player;
        protected Inventory inventory = new Inventory();
        protected GameData gameData = new GameData();

        protected List<Common1> commonEnemy;
        protected List<Item> items;


        //Pause Variable
        protected bool isPause;
        protected bool isSoundPause;
        Texture2D dimTexture = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Black);
        Texture2D pauseBackground = Globals.Content.Load<Texture2D>("UI/PauseBackground");


        protected Timer timer;
        protected Texture2D background;

        protected bool startWarning = false;
        private bool warningTrigger;
        private float warningOpacity;
        Texture2D warningTexture;

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
        protected int elite_killed = 0;
        protected bool boss_killed = false;

        //UI
        protected UIManager uiManager = new UIManager();
        protected UIManager pauseUiManager = new UIManager();

        //Variable for reduce memory Load
        protected Texture2D playertexture;
        protected Texture2D Common1Texture;
        protected Texture2D Elite1Texture;
        protected Texture2D Boss1Texture;
        protected Texture2D Elite2Texture;
        protected Texture2D Boss2Texture;
        protected Texture2D Boss3Texture;
        protected Song bgsong;
        protected Animation slashAnimation;
        protected UIBackground uIBackground;
        protected UISkill skillUI_Q;
        protected UISkill skillUI_W;
        protected UISkill skillUI_E;
        protected UISkill skillUI_R;
        protected UIPlayerCheckPanel uIPanel;
        protected Texture2D bg;
        protected Texture2D fg;
        protected ProgressBarAnimated playerHpBar;

        public virtual void Load()
        {

            AllSkills.Reset();
            Texture2D tempTexture = new Texture2D(Globals.graphicsDevice, 1, 1);
            warningTexture = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Red);
            Globals.soundManager.StopAllSounds();
            ParticleManager.RemoveAll();

            //Player
            playertexture = Globals.Content.Load<Texture2D>("playerTexture");
            player = new(playertexture, new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2), 1, 1, 2, 4);
            player.LoadPlayer(2, 4);
            damageNumberManager.SubscribeToTakeDamageEvent(player.Status, player);
            inventory.LoadInventory(tempTexture);
            gameData.LoadData();
            player.LoadGameData(gameData);

            //Sound
            bgsong = Globals.Content.Load<Song>("Sound/Song/Fight Song");
            Globals.soundManager.SetSongVolume(gameData.volumeMusic);
            Globals.soundManager.PlaySong("Fight Song", bgsong, true);

            //Load Animation
            slashAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/SlashTexture"), row: 1, column: 4, fps: 30, false, 1.5f);
            animationManager.AddAnimation("Slash", slashAnimation);

            //Load UI
            uIBackground = new(Globals.Content.Load<Texture2D>("UI/SkillBackground"), new(Globals.Bounds.X / 2, 930));
            int startX = (int)uIBackground.position.X - (uIBackground.texture.Width / 2);
            int space = uIBackground.texture.Width / 12;
            int skillY = 980;


            skillUI_Q = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 3), skillY), player.Skills[Keys.Q]);
            skillUI_W = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 5), skillY), player.Skills[Keys.W]);
            skillUI_E = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 7), skillY), player.Skills[Keys.E]);
            skillUI_R = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 9), skillY), player.Skills[Keys.R]);
            uIPanel = new(Globals.Content.Load<Texture2D>("TestUI"), new(150, 100), player);

            bg = Globals.Content.Load<Texture2D>("UI/HealthBarNone");
            fg = Globals.Content.Load<Texture2D>("UI/HealthBarFull");
            playerHpBar = new ProgressBarAnimated(bg, fg, player.Status.MaxHP, new(Globals.Bounds.X / 2, uIBackground.position.Y - 75));


            //uiManager.AddElement("uiBackground", uIBackground);
            uiManager.AddElement("playerBar", playerHpBar);
            uiManager.AddElement("skillUI_Q", skillUI_Q);
            uiManager.AddElement("skillUI_W", skillUI_W);
            uiManager.AddElement("skillUI_E", skillUI_E);
            uiManager.AddElement("skillUI_R", skillUI_R);

            Common1Texture = Globals.Content.Load<Texture2D>("Enemies/Common1");
            Elite1Texture = Globals.Content.Load<Texture2D>("Enemies/Elite1");
            Boss1Texture = Globals.Content.Load<Texture2D>("Enemies/Boss1");
            Elite2Texture = Globals.Content.Load<Texture2D>("Enemies/Elite2");
            Boss2Texture = Globals.Content.Load<Texture2D>("Enemies/Boss2");
            Boss3Texture = Globals.Content.Load<Texture2D>("Enemies/Boss3");

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
                if (animationManager.animations["Slash"].IsComplete)
                {
                    animationManager.Stop("Slash");
                }
                if (player.CanNormalAttack())
                {
                    foreach (var enemy in Globals.EnemyManager)
                    {
                        if (enemy.Status.immunity)
                        {
                            enemy.Status.immunity = false;
                        }
                    }
                    player.ChangeAnimation("Idle");
                }

                if (InputManager.MouseRightClicked && player.CanNormalAttack())
                {
                    animationManager.SetOffset("Slash", new Vector2((player.collision.Width / 2) + 25, 0));
                    player.ChangeAnimation("Attack", true);
                    animationManager.Flip("Slash", true);
                    animationManager.Play("Slash");
                    player.DoNormalAttack();
                }
                if (InputManager.MouseLeftClicked && player.CanNormalAttack())
                {
                    animationManager.SetOffset("Slash", new Vector2((-player.collision.Width / 2) - 25, 0));
                    player.ChangeAnimation("Attack", false);
                    animationManager.Flip("Slash", false);
                    animationManager.Play("Slash");
                    player.DoNormalAttack();
                }

               SkipToBoss();
            }
        }
        protected virtual void UpdateSound()
        {
            if (startWarning)
            {
                Globals.soundManager.PlaySound("Warning", true);
            }
            else
            {
                Globals.soundManager.StopSound("Warning");
            }
        }
        protected virtual void CollisionManage(GameTime gameTime)
        {
            foreach (var enemy in Globals.EnemyManager)
            {
                if (enemy.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
                {
                    if (enemy is Common1)
                    {
                        Globals.soundManager.PlaySound("Common1Hit");
                    }
                    enemy.TakeDamage(1, player);
                    if (player.buffIndicator == "Thunder_")
                    {
                        enemy.Status.Paralysis(2f);
                    }
                    enemy.Status.immunity = true;
                }
            }
            damageNumberManager.Update();
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
                Mouse.SetPosition(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || !Globals.Game.IsActive)
            {
                if (!isPause)
                {
                    if (!isSoundPause)
                    {
                        Globals.soundManager.PauseAllSound();
                    }
                    Vector2 buttonPosition = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2 - 50);
                    Vector2 buttonIncrement = new Vector2(0, 150);
                    Texture2D continueTexture = Globals.Content.Load<Texture2D>("UI/PauseButtonPlay");
                    Texture2D restartTexture = Globals.Content.Load<Texture2D>("UI/PauseButtonRestart");
                    Texture2D exitTexture = Globals.Content.Load<Texture2D>("UI/PauseButtonExit");

                    UIElement continueButton = new UIButton(continueTexture, buttonPosition, OnContinueClick);
                    UIElement restartButton = new UIButton(restartTexture, buttonPosition + buttonIncrement * 1, OnRestartClick);
                    UIElement exitButton = new UIButton(exitTexture, buttonPosition + buttonIncrement * 2, OnExitClick);

                    pauseUiManager.AddElement("continue", continueButton);
                    pauseUiManager.AddElement("restart", restartButton);
                    pauseUiManager.AddElement("exit", exitButton);
                }

                isPause = true;
                isSoundPause = true;
                TimeManager.ChangeGameSpeed(0f);
            }
            if (!isPause)
            {
                if (isSoundPause)
                {
                    Globals.soundManager.ResumeAllSound();
                    isSoundPause = false;
                }
            }
        }

        protected void OnContinueClick()
        {
            isPause = false;
            pauseUiManager.RemoveAll();
            Mouse.SetPosition((int)player.position.X, (int)player.position.Y);
            TimeManager.ChangeGameSpeed(1f);
        }

        protected void OnRestartClick()
        {
            UnloadScene();
            switch (gameData.stage)
            {
                case 1:
                    sceneManager.AddScene(new Stage1(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
                    break;
                case 2:
                    sceneManager.AddScene(new Stage2(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
                    break;
                case 3:
                    sceneManager.AddScene(new Stage3(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
                    break;
            }
        }
        private void OnExitClick()
        {
            UnloadScene();
            sceneManager.AddScene(new MenuScene(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
        }

        protected void SkipToBoss()
        {

            if (Keyboard.GetState().IsKeyDown(Keys.F1) 
                && Keyboard.GetState().IsKeyDown(Keys.F10)
                && !boss_spawned)
            {
                boss_spawn_time = 115f;
                timer.TimeSet(115f);
            }
        }
        protected void DrawPause()
        {

            Vector2 pauseBgOrigin = new Vector2(pauseBackground.Width / 2, pauseBackground.Height / 2);

            Globals.SpriteBatch.Draw(dimTexture, Vector2.Zero, Color.White * 0.8f);
            Globals.SpriteBatch.Draw(pauseBackground, new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2), null, Color.White, 0f, pauseBgOrigin, 1f, SpriteEffects.None, 0f);

            pauseUiManager.Draw(Globals.SpriteBatch);
            Globals.DrawCursor();
        }

        protected void DrawWarning()
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

            Globals.SpriteBatch.Draw(warningTexture, Vector2.Zero, Color.Red * warningOpacity);
        }

        protected void GotoSummary(bool win)
        {
            timer.StartStop();
            float time = timer.timeLeft;
            int gems = CalculateReward();
            UnloadScene();
            sceneManager.AddScene(new SummaryScene(contentManager, sceneManager, win, gems, gameData, time));
        }
        protected void SongManage()
        {
            if (boss_spawn_time > 115f && !boss_spawned)
            {
                Song bgsong = Globals.Content.Load<Song>("Sound/Song/Boss Fight Song");
                Globals.soundManager.PlaySong("Boss Song", bgsong, true);
            }

        }
        protected int CalculateReward()
        {
            int gems = 0;
            float tempMultiplier = 0;

            if (timer.timeLeft > 120)
            {
                tempMultiplier = 0.3f;

            }
            else if (timer.timeLeft > 60)
            {
                tempMultiplier = 0.2f;
            }
            else if (timer.timeLeft > 0)
            {
                tempMultiplier = 0.1f;
            }

            gems += (int)(enemy_killed * tempMultiplier);

            switch (gameData.stage)
            {
                case 1:
                    gems += elite_killed * 2;
                    break;
                case 2:
                    gems += elite_killed * 4;
                    break;
                case 3:
                    gems += elite_killed * 6;
                    break;
            }

            if (boss_killed)
            {
                switch (gameData.stage)
                {
                    case 1:
                        gems += 100;
                        break;
                    case 2:
                        gems += 200;
                        break;
                    case 3:
                        gems += 300;
                        break;
                }
            }

            return gems;

        }
    }
}
