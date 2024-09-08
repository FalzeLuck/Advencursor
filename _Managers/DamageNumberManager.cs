using Advencursor._Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Managers
{
    public class DamageNumberManager
    {
        private List<DamageNumber> damageNumbers = new List<DamageNumber>();
        private SpriteFont font;

        public DamageNumberManager(SpriteFont font)
        {
            this.font = font;
        }

        public void AddDamageNumber(string text, Vector2 position, Color color)
        {
            damageNumbers.Add(new DamageNumber(text, position, color));
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
