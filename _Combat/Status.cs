using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Models.Enemy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Combat
{
    public class Status
    {
        public float MaxHP { get; private set; }
        public float CurrentHP { get; private set; }
        public float BaseHP {  get; private set; }
        public float BaseAttack { get; private set; }
        public float Attack {  get; private set; }
        public float Shield { get; private set; }

        public bool immunity;

        //Crit
        public float CritRate {  get; private set; }
        public float CritDam { get; private set; }

        //CC
        public float paralysisTimer;
        public bool isParalysis { get; private set; } = false;

        //Action
        public Action<string,Color> OnTakeDamage;

        public Status(float MaxHP,float Attack) 
        {
            this.MaxHP = MaxHP;
            this.CurrentHP = MaxHP;
            this.BaseHP = MaxHP;
            this.BaseAttack = Attack;
            this.Attack = Attack;
            CritRate = 0;
            CritDam = 0;
            immunity = false;
            Shield = 0;
        }

        public void TakeDamage(float damage,Sprite fromwho)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");
            float tempDamage;
            Color tempColor;
            if (IsCrit(fromwho.Status.CritRate))
            {
                
                tempDamage = (damage * ((fromwho.Status.CritDam / 100) + 1));
                tempColor = Color.Gold;
            }
            else
            {
                if (fromwho is _Enemy)
                {
                    tempColor = Color.Red;
                }
                else
                {
                    tempColor = Color.White;
                }
                tempDamage = damage;
            }


            if (immunity == false)
            {
                if (Shield >= tempDamage)
                {
                    Shield -= tempDamage;
                }
                else if (Shield < tempDamage)
                {
                    float remainDamage = (tempDamage - Shield);
                    Shield = 0;
                    CurrentHP -= remainDamage;
                }else if (Shield == 0)
                {
                    CurrentHP -= tempDamage;
                }
                OnTakeDamage?.Invoke(tempDamage.ToString("F0"),tempColor);
            }

            if (CurrentHP < 0) {CurrentHP = 0; }

            
        }

        public void TakeDamageNoImmune(float damage, Sprite who)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");

            if (Shield >= damage)
            {
                Shield -= damage;
            }
            else if (Shield < damage)
            {
                float remainDamage = (damage - Shield);
                Shield = 0;
                CurrentHP -= remainDamage;
            }
            else if (Shield == 0)
            {
                CurrentHP -= damage;
            }

            if (CurrentHP < 0) { CurrentHP = 0; }
        }

        private bool IsCrit(float CritRate)
        {
            float critTrigger = Globals.RandomFloat(0, 100);
            if (critTrigger <= CritRate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Heal(float amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException("Heal can't be negative");

            CurrentHP += amount;
            OnTakeDamage?.Invoke(amount.ToString("F0"), Color.Green);

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
                paralysisTimer -= TimeManager.TimeGlobal;
                if( paralysisTimer < 0 ) { isParalysis=false; }
            }
        }

        public void SetHP(float value)
        {
            CurrentHP = value;
            MaxHP = value;
        }
        public void SetAttack(float value)
        {
            Attack = value;
        }
        public void SetCritRate(float value)
        {
            CritRate = value;
        }
        public void SetCritDamage(float value)
        {
            CritDam = value;
        }
    }
}
