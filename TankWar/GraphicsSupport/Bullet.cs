using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankWar.GraphicsSupport
{
    class Bullet : SpritePrimitive
    {
        Vector2 m_direction;
        bool m_isExploded;
        bool m_isEnabled;
        int m_Shooter;
        TimeSpan m_TimeShot;

        public Bullet(String aImageName, Vector2 aPosition, Vector2 aSize, int aRowCount, int aColCount, int aPadding, float aAngle, int aShooter = 0)
            :base(aImageName, aPosition, aSize, aRowCount, aColCount, aPadding)
        {
            m_direction = new Vector2((float)Math.Sin(aAngle), (float)Math.Cos(aAngle));
            Position += m_direction * 7;
            
            SetSpriteAnimation(0, 0, 0, 2, 5);

            MyCollisionType = CollisionType.CollisionCircle;
            m_isEnabled = true;
            m_isExploded = false;

            m_Shooter = aShooter;
        }

        public void Update(GameTime gameTime)
        {
            // if it's been exploded for too long, disable it
            if (m_isExploded)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - m_TimeShot.TotalMilliseconds > 200)
                {
                    m_isEnabled = false;
                }
            }
            else // it's still active so move
            {
                Position += m_direction;

                base.Update();

                if (gameTime.TotalGameTime.TotalMilliseconds - m_TimeShot.TotalMilliseconds > 2000)
                {
                    m_isEnabled = false;
                }
            }


        }

        public void Explode(GameTime gameTime)
        {
            SetSpriteAnimation(1, 0, 1, 0, 10);
            m_TimeShot = gameTime.TotalGameTime;
            m_isExploded = true;
        }

        public bool Enabled { get { return m_isEnabled; } set { m_isEnabled = value; } }
        public bool Exploded { get { return m_isExploded; } set { m_isExploded = value; } }
        public int MyShooter { get { return m_Shooter; } set { m_Shooter = value; } }
        public TimeSpan TimeWasShot { get { return m_TimeShot; } set { m_TimeShot = value; } }
    }
}
