using Advencursor._Models;
using Advencursor._Models.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Combat
{
    public class DamageNumberManager
    {
        private List<DamageNumber> damageNumbers = new List<DamageNumber>();
        private SpriteFont font;


        public DamageNumberManager(SpriteFont font)
        {
            this.font = font;
        }

        public void AddDamageNumber(string text, Sprite whotake, Color color, float scale)
        {
            damageNumbers.Add(new DamageNumber(text, whotake.position, color, scale));
        }

        public void SubscribeToTakeDamageEvent(Status status, Sprite whotake)
        {
            status.OnTakeDamage += (text, color, scale) => AddDamageNumber(text, whotake, color, scale);
        }

        public void SubscribeToTakeDamageEvent(Status status, Sprite whotake, Color wantColor)
        {
            status.OnTakeDamage += (text, color, scale) => AddDamageNumber(text, whotake, wantColor, scale);
        }

        public void UnSubscribeToTakeDamageEvent(Status status, Sprite whotake)
        {
            status.OnTakeDamage -= (text, color, scale) => AddDamageNumber(text, whotake, color, scale);
        }

        public void Update()
        {
            for (int i = damageNumbers.Count - 1; i >= 0; i--)
            {
                damageNumbers[i].Update();
                if (damageNumbers[i].IsExpired())
                {
                    damageNumbers.RemoveAt(i);
                }
            }
        }

        public void Draw()
        {
            foreach (var damageNumber in damageNumbers)
            {
                damageNumber.Draw(font);
            }
        }
    }
}
