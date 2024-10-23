using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene
{
    public class DialogueScene : IScene
    {
        protected ContentManager contentManager;
        protected SceneManager sceneManager;

        protected SpriteFont font;
        protected List<Dialogue> currentSceneDialogue;
        protected DialogueManager dialogueManager;
        protected int dialogueIndex = 0;

        protected ButtonState currentKeystate;
        protected ButtonState previousKeyState;

        protected float typingSpeed = 0.05f;
        protected float timeSinceLastChar = 0f;
        protected int currentCharIndex = 0;
        protected bool isTyping = true;
        protected bool canAdvance = false;

        public DialogueScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
        }

        public virtual void Load()
        {
            font = Globals.Content.Load<SpriteFont>("Font/TextFont");
            dialogueManager = new DialogueManager("Content/Dialogue.json");
            //currentSceneDialogue = dialogueManager.GetSceneDialogue("null");
        }

        public virtual void Update(GameTime gameTime)
        {
            currentKeystate = Mouse.GetState().LeftButton;

            if (currentSceneDialogue != null && dialogueIndex < currentSceneDialogue.Count)
            {
                Dialogue currentLine = currentSceneDialogue[dialogueIndex];

                if (isTyping)
                {
                    timeSinceLastChar += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timeSinceLastChar >= typingSpeed)
                    {
                        timeSinceLastChar = 0f;

                        currentCharIndex++;

                        if (currentCharIndex >= currentLine.text.Length)
                        {
                            isTyping = false;
                            canAdvance = true;
                        }
                    }
                }
                
                if (canAdvance && currentKeystate == ButtonState.Pressed && previousKeyState == ButtonState.Released)
                {
                    dialogueIndex++;
                    Globals.soundManager.PlaySound("ClickVisualNovel");
                    ResetTypingState();
                }

                if (isTyping && currentKeystate == ButtonState.Pressed && previousKeyState == ButtonState.Released && currentCharIndex >= 1)
                {
                    currentCharIndex = currentLine.text.Length;
                    isTyping = false;
                    canAdvance = true;
                }

            }

            previousKeyState = currentKeystate;

        }
        private void ResetTypingState()
        {
            currentCharIndex = 0;
            timeSinceLastChar = 0f;
            isTyping = true;
            canAdvance = false;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

            if (currentSceneDialogue != null && dialogueIndex < currentSceneDialogue.Count)
            {
                Dialogue currentLine = currentSceneDialogue[dialogueIndex];

                string displayedText = currentLine.text.Substring(0, currentCharIndex);
                spriteBatch.DrawString(font, $"{currentLine.speaker}: {displayedText}", new Vector2(100, 100), Color.White);
            }
        }
    }
}
