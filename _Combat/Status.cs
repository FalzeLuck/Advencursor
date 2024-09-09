using Advencursor._Managers;
using Microsoft.Xna.Framework;
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
        public int BaseAttack { get; private set; }
        public int Attack {  get; private set; }
        public int Shield { get; private set; }

        public bool immunity;

        //Crit
        public float CritRate {  get; private set; }
        public float CritDam { get; private set; }
        public bool isCrit {  get; private set; }

        //CC
        public float paralysisTimer;
        public bool isParalysis { get; private set; } = false;

        //Action
        public Action<string,Color> OnTakeDamage;

        public Status(int MaxHP,int Attack) 
        {
            this.MaxHP = MaxHP;
            this.CurrentHP = MaxHP;
            this.BaseAttack = Attack;
            this.Attack = Attack;
            immunity = false;
            Shield = 0;
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");

            if (immunity == false)
            {
                if (Shield >= damage)
                {
                    Shield -= damage;
                }
                else if (Shield < damage)
                {
                    int remainDamage = (damage - Shield);
                    Shield = 0;
                    CurrentHP -= remainDamage;
                }else if (Shield == 0)
                {
                    CurrentHP -= damage;
                }
                OnTakeDamage?.Invoke(damage.ToString(),Color.HotPink);
            }

            if (CurrentHP < 0) {CurrentHP = 0; }

            
        }

        public void TakeDamageNoImmune(int damage)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");

            if (Shield >= damage)
            {
                Shield -= damage;
            }
            else if (Shield < damage)
            {
                int remainDamage = (damage - Shield);
                Shield = 0;
                CurrentHP -= remainDamage;
            }
            else if (Shield == 0)
            {
                CurrentHP -= damage;
            }

            if (CurrentHP < 0) { CurrentHP = 0; }
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
            
        public void AddShield(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException("Shield can't be negative");

            Shield += amount;
        }

        public void AddAttack(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException("Attack can't be negative");

            Attack += amount;
        }

        public void Paralysis(float time)
        {
            paralysisTimer = time;
            isParalysis = true;
        }

        public void Update()
        {
            if (isParalysis)
            {
                paralysisTimer -= TimeManager.TotalSeconds;
                if( paralysisTimer < 0 ) { isParalysis=false; }
            }
        }
    }
}
