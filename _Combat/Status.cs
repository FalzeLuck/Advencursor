using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Combat
{
    public class Status
    {
        public int MaxHP { get; private set; }
        public int CurrentHP { get; private set; }
        public int Attack {  get; private set; }

        public bool immunity;
        public Status(int MaxHP) 
        {
            this.MaxHP = MaxHP;
            this.CurrentHP = MaxHP;
            immunity = false;
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");

            if (immunity == false)
            {
                CurrentHP -= damage;
            }

            if (CurrentHP < 0) {CurrentHP = 0; }
        }

        public void Heal(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException("Heal can't be negative");

            CurrentHP += amount;

            if (CurrentHP > MaxHP) { CurrentHP = MaxHP; }
        }

        public bool IsAlive()
        {
            return CurrentHP > 0;
        }
            

    }
}
