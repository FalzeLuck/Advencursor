using Advencursor._Animation;
using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Skill;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class PoisonPool : Sprite
    {
        public float poolDuration = 0f;
        public float burnCooldown;
        private bool isPoisonSound = false;
        public bool canBurn => burnCooldown > 0.5f;
        public PoisonPool(Texture2D texture, Vector2 position,  int row, int column) : base(texture, position)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Normal", new(texture, row, column,1,  6, false) },
                { "Loop" , new(texture,row,column,2,column,1,6,true)}
            };
            indicator = "Normal";

        }

        public override void Update(GameTime gameTime)
        {
            poolDuration += TimeManager.TimeGlobal;
            burnCooldown += TimeManager.TimeGlobal;
            if (!isPoisonSound)
            {
                Globals.soundManager.PlaySoundCanStack("TomatoPoison");
                isPoisonSound = true;
            }
            if (animations["Normal"].IsComplete)
            {
                indicator = "Loop";
            }
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
                collision = animations[indicator].GetCollision(position);
            }

            
        }

        public void Burn()
        {
            burnCooldown = 0f;
        }
    }
}
