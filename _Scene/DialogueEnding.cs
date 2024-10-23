using Advencursor._Managers;
using Advencursor._Scene.Stage;
using Advencursor._Scene.Transition;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Animation;
using Advencursor._SaveData;
using Microsoft.Xna.Framework.Media;

namespace Advencursor._Scene
{
    public class DialogueEnding : DialogueScene
    {
        private Vector2 textPosition;

        Dialogue currentLine;
        private string currentScene;

        //Fade
        private float fadeTimeMax = 1f;
        private float fadeTime;
        private float fadeFactor;
        private float fadeOpacity;
        private Texture2D blackScreenTexture;

        //Texture
        private Texture2D bg;
        private Texture2D textUI;

        //Life
        private Texture2D textureLife;
        private Vector2 lifePos;
        private Vector2 lifeOrigin;
        private float lifeOpacity = 1;

        //Dead
        private Texture2D textureDead;
        private Animation deadDeadAnim;

        public DialogueEnding(ContentManager contentManager, SceneManager sceneManager) : base(contentManager, sceneManager)
        {
            
        }

        public override void Load()
        {
            base.Load();
            Globals.soundManager.StopAllSounds();
            Song bgsong = Globals.Content.Load<Song>("Sound/Song/Visual Novel Song");
            Globals.soundManager.PlaySong("Visual Novel Song", bgsong, true);
            currentSceneDialogue = dialogueManager.GetSceneDialogue("EndingSplit1");
            currentScene = "EndingSplit1";
            blackScreenTexture = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Black);
            fadeOpacity = 1;

            textureLife = Globals.Content.Load<Texture2D>("Story/L");
            lifePos = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
            lifeOrigin = new Vector2(textureLife.Width / 2, textureLife.Height / 2);

            textureDead = Globals.Content.Load<Texture2D>("Story/D");
            deadDeadAnim = new Animation(Globals.Content.Load<Texture2D>("Enemies/Boss3"),6,12,4,8,false,1.5f);

            Globals.SetGreyScale(0f);
            textUI = Globals.Content.Load<Texture2D>("UI/SkillBackground");
            bg = Globals.Content.Load<Texture2D>("Background/BG_Stage3");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(dialogueIndex < currentSceneDialogue.Count)
                currentLine = currentSceneDialogue[dialogueIndex];

            

            if (currentLine != null && currentScene  == "EndingSplit1")
            {
                if (dialogueIndex >= currentSceneDialogue.Count)
                {
                    deadDeadAnim.Update();
                }

                if (deadDeadAnim.IsComplete)
                {
                    currentSceneDialogue = dialogueManager.GetSceneDialogue("EndingSplit2");
                    currentScene = "EndingSplit2";
                    dialogueIndex = 0;
                }
            }else if (currentLine != null && currentScene == "EndingSplit2")
            {
                if (currentLine.speaker == "[L.I.F.E.]")
                {
                    lifeOpacity = 0f;
                    textureLife = Globals.Content.Load<Texture2D>("Story/L_Smile");
                }
                else
                {
                    textureLife = Globals.Content.Load<Texture2D>("Story/L");
                    lifeOpacity = 0.5f;
                }
                if (dialogueIndex >= currentSceneDialogue.Count)
                {
                    currentSceneDialogue = dialogueManager.GetSceneDialogue("EndingSplit3");
                    currentScene = "EndingSplit3";
                    dialogueIndex = 0;
                    textureLife = Globals.Content.Load<Texture2D>("Story/L_END");
                    Globals.Camera.Shake(0.5f, 5);
                    Globals.soundManager.PlaySound("Stab");
                    Globals.SetGreyScale(1f);
                }
            }
            else if (currentLine != null && currentScene == "EndingSplit3")
            {
                if (currentLine.speaker == "[L.I.F.E.]")
                {
                    lifeOpacity = 0f;
                }
                else
                {
                    lifeOpacity = 0.5f;
                }
                if (dialogueIndex >= currentSceneDialogue.Count)
                {
                    currentSceneDialogue = dialogueManager.GetSceneDialogue("EndingSplit4");
                    currentScene = "EndingSplit4";
                    dialogueIndex = 0;
                }
            }
            else if (currentLine != null && currentScene == "EndingSplit4")
            {
                if (dialogueIndex >= currentSceneDialogue.Count)
                {
                    EndScene();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Globals.BeginDrawGrayScale();
            Globals.SpriteBatch.Draw(bg, Vector2.Zero, Color.White);
            if (currentLine != null && currentScene == "EndingSplit1")
            {
                if (dialogueIndex >= currentSceneDialogue.Count)
                {
                    deadDeadAnim.Draw(lifePos);
                }
                else
                {
                    Globals.SpriteBatch.Draw(textureDead, lifePos, null, Color.White, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                }
                DrawTextUI();
                DrawCurrentDialogue(new Vector2((Globals.Bounds.X / 2) - 500, 930), 0.2f, Color.Black);
                DrawCurrentSpeaker(new Vector2(Globals.Bounds.X / 2, 860), 0.4f, Color.Black);
            }else if (currentLine != null && currentScene == "EndingSplit2")
            {
                Globals.SpriteBatch.Draw(textureLife, lifePos, null, Color.White, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                Globals.SpriteBatch.Draw(textureLife, lifePos, null, Color.Black * lifeOpacity, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                DrawTextUI();
                DrawCurrentDialogue(new Vector2((Globals.Bounds.X / 2) - 500, 930), 0.2f, Color.Black);
                DrawCurrentSpeaker(new Vector2(Globals.Bounds.X / 2, 860), 0.4f, Color.Black);
            }
            else if (currentLine != null && currentScene == "EndingSplit3")
            {
                Globals.SpriteBatch.Draw(textureLife, lifePos, null, Color.White, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                Globals.SpriteBatch.Draw(textureLife, lifePos, null, Color.Black * lifeOpacity, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                DrawTextUI();
                DrawCurrentDialogue(new Vector2((Globals.Bounds.X / 2) - 500, 930), 0.2f, Color.Black);
                DrawCurrentSpeaker(new Vector2(Globals.Bounds.X / 2, 860), 0.4f, Color.Black);
            }
            else if (currentLine != null && currentScene == "EndingSplit4")
            {
                Globals.EndDrawGrayScale();
                Globals.SpriteBatch.Draw(textureLife, lifePos, null, Color.White, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                Globals.SpriteBatch.Draw(textureLife, lifePos, null, Color.Black * lifeOpacity, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                Globals.BeginDrawGrayScale();
                DrawTextUI();
                Globals.EndDrawGrayScale();
                DrawCurrentDialogue(new Vector2((Globals.Bounds.X / 2) - 500, 930), 0.2f, Color.Red);
                DrawCurrentSpeaker(new Vector2(Globals.Bounds.X / 2, 860), 0.4f, Color.Red);
                Globals.BeginDrawGrayScale();
            }



            Globals.DrawCursor();
            Globals.EndDrawGrayScale();
        }

        private void DrawCurrentDialogue(Vector2 position, float scale, Color color)
        {
            if (currentSceneDialogue != null && dialogueIndex < currentSceneDialogue.Count)
            {
                
                Dialogue currentLine = currentSceneDialogue[dialogueIndex];
                string displayedText = currentLine.text.Substring(0, currentCharIndex);
                Globals.SpriteBatch.DrawString(font, $"{displayedText}", position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawTextUI()
        {
            Vector2 uiOrigin = new Vector2(textUI.Width / 2, textUI.Height / 2);
            Vector2 uiPos = new Vector2(Globals.Bounds.X / 2, 930);
            Globals.SpriteBatch.Draw(textUI, uiPos, null, Color.White, 0f, uiOrigin, 1f, SpriteEffects.None, 0f);
        }
        private void DrawCurrentSpeaker(Vector2 position, float scale, Color color)
        {
            if (currentSceneDialogue != null && dialogueIndex < currentSceneDialogue.Count)
            {
                Dialogue currentLine = currentSceneDialogue[dialogueIndex];
                Vector2 origin = font.MeasureString(currentLine.speaker);
                origin = new Vector2(origin.X / 2, origin.Y / 2);
                Globals.SpriteBatch.DrawString(font, $"{currentLine.speaker}", position, color, 0, origin, scale, SpriteEffects.None, 0f);
            }
        }

        private void EndScene()
        {
            GameData gameData = new GameData();
            gameData.LoadData();
            gameData.stage3Clear = true;
            gameData.SaveData();
            sceneManager.AddScene(new MenuScene(contentManager, sceneManager));
        }
    }
}
