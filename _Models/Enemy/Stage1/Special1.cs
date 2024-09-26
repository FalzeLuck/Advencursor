﻿using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy.Stage1
{
    public class Special1 : _Enemy
    {
        private float walkTimer;

        private float bombTimer = 0f;
        public bool isBombed => bombTimer > 3f;

        public Special1(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Right", new(texture, row, column,1,  4, true) },
                { "Left", new(texture,row,column,1,4,true) },
                { "Bomb", new(texture,row,column,1,30,true) }

            };
            animations["Left"].SetFlip(false);
            animations["Right"].SetFlip(true);

            indicator = "Left";
            walkTimer = 0;
            velocity = new Vector2(-50, 0);
        }

        public override void Update(GameTime gameTime)
        {
            collisionCooldown -= TimeManager.TimeGlobal;
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
                collision = animations[indicator].GetCollision(position);
            }
            UpdateParryZone();

            walkTimer += TimeManager.TimeGlobal;

            if (walkTimer > 1.5f)
            {
                walkTimer = 0f;
            }
            else if (walkTimer > 1f)
            {
                position = position;
            }
            else if (walkTimer > 0)
            {

                position += velocity * TimeManager.TimeGlobal;
            }


            if (collision.X + collision.Width > Globals.Bounds.X)
            {
                if (!isBombed) indicator = "Left";
                velocity = new Vector2(velocity.X * -1, 0);
                position += new Vector2(-10, 0);
            }

            if (collision.X < 0)
            {
                if (!isBombed) indicator = "Right";
                velocity = new Vector2(velocity.X * -1, 0);
                position += new Vector2(10, 0);
            }

        }

        public void Bomb()
        {
            indicator = "Bomb";
            bombTimer += TimeManager.TimeGlobal;
        }
    }
}