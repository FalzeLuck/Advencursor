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
        public float BaseHP { get; private set; }
        public float BaseAttack { get; private set; }
        public float Attack { get; private set; }
        public float Shield { get; private set; }

        public bool immunity;

        private float numberScale = 0.75f;

        //Crit
        public float CritRate { get; private set; }
        public float CritDam { get; private set; }

        //CC
        public float paralysisTimer;
        public bool isParalysis { get; private set; } = false;

        //Action
        public Action<string, Color, float> OnTakeDamage;

        public Status(float MaxHP, float Attack)
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

        public void TakeDamage(float damage, Sprite fromwho)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");
            float tempDamage;
            Color tempColor;
            float tempScale;
            string tempString;
            if (IsCrit(fromwho.Status.CritRate))
            {
                tempScale = numberScale * 1.75f;
                tempDamage = (damage * ((fromwho.Status.CritDam / 100) + 1));
                tempString = $"{tempDamage.ToString("F0")}!";
                tempColor = new Color(255, 16, 240);
            }
            else
            {
                if (fromwho is _Enemy)
                {
                    tempColor = Color.Red;
                }
                else
                {
                    tempColor = new Color(248, 228, 249, 1);
                }
                tempScale = numberScale;
                tempDamage = damage;
                tempString = tempDamage.ToString("F0");
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
                }
                else if (Shield == 0)
                {
                    CurrentHP -= tempDamage;
                }
                OnTakeDamage?.Invoke(tempString, tempColor, tempScale);
            }

            if (CurrentHP < 0) { CurrentHP = 0; }


        }
        public void TakeDamage(float damage, Sprite fromwho,float ampAmount)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");
            float tempDamage;
            Color tempColor;
            float tempScale;
            string tempString;
            if (IsCrit(fromwho.Status.CritRate))
            {
                tempScale = numberScale * 1.75f;
                tempDamage = (damage * ((fromwho.Status.CritDam / 100) + 1));
                tempString = $"{tempDamage.ToString("F0")}!";
                tempColor = new Color(255, 16, 240);
            }
            else
            {
                if (fromwho is _Enemy)
                {
                    tempColor = Color.Red;
                }
                else
                {
                    tempColor = new Color(248, 228, 249, 1);
                }
                tempScale = numberScale;
                tempDamage = damage * ampAmount;
                tempString = tempDamage.ToString("F0");
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
                }
                else if (Shield == 0)
                {
                    CurrentHP -= tempDamage;
                }
                OnTakeDamage?.Invoke(tempString, tempColor, tempScale);
            }

            if (CurrentHP < 0) { CurrentHP = 0; }


        }

        public void TakeDamageNoCrit(float damage, Sprite fromwho,Color color)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");
            float tempDamage = damage;
            Color tempColor = color;

            /*if (fromwho is _Enemy)
            {
                tempColor = Color.Red;
            }*/



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
                }
                else if (Shield == 0)
                {
                    CurrentHP -= tempDamage;
                }
                OnTakeDamage?.Invoke(tempDamage.ToString("F0"), tempColor, numberScale);
            }

            if (CurrentHP < 0) { CurrentHP = 0; }


        }

        public void TakeDamageNoImmune(float damage, Sprite fromwho,bool nocrit)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException("Damage can't be negative");
            float tempDamage;
            Color tempColor;
            float tempScale;
            string tempString;
            if (IsCrit(fromwho.Status.CritRate) && !nocrit)
            {
                tempScale = numberScale * 1.75f;
                tempDamage = (damage * ((fromwho.Status.CritDam / 100) + 1));
                tempString = $"{tempDamage.ToString("F0")}!";
                tempColor = new Color(255, 16, 240);
            }
            else
            {
                if (fromwho is _Enemy)
                {
                    tempColor = Color.Red;
                }
                else
                {
                    tempColor = new Color(248, 228, 249, 1);
                }
                tempScale = numberScale;
                tempDamage = damage;
                tempString = tempDamage.ToString("F0");
            }

            if (Shield >= tempDamage)
            {
                Shield -= tempDamage;
            }
            else if (Shield < tempDamage)
            {
                float remainDamage = (tempDamage - Shield);
                Shield = 0;
                CurrentHP -= remainDamage;
            }
            else if (Shield == 0)
            {
                CurrentHP -= tempDamage;
            }
            OnTakeDamage?.Invoke(tempString, tempColor, tempScale);



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
            OnTakeDamage?.Invoke(amount.ToString("F0"), Color.Green, numberScale);

            if (CurrentHP > MaxHP) { CurrentHP = MaxHP; }
        }

        public bool IsAlive()
        {
            return CurrentHP > 0;
        }

        public void Kill()
        {
            CurrentHP = 0;
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
                if (paralysisTimer < 0) { isParalysis = false; }
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
