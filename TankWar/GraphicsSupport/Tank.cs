using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankWar.GraphicsSupport
{
    class Tank : GameObject
    {
        enum TankDirection
        {
            Up = 0,
            Right,
            Down,
            Left,
            None
        }

        private Texture2D m_turret;
        protected float m_turretRotation;
        protected bool m_isEnabled;
        protected bool m_isPlayer;
        TankDirection m_RotationDirection;
        TankDirection m_MovementDirection;

        TimeSpan m_TimeSpan;
        TimeSpan m_TimeRespawn;


        public Tank(String aTankName, String aTurretName, Vector2 aPosition, Vector2 aSize, bool aIsPlayer)
            : base(aTankName, aPosition, aSize)
        {
            //turret = new TexturedPrimitive(aTurretName, aPosition, aSize);
            m_turret = Game1.sContent.Load<Texture2D>(aTurretName);
            m_turretRotation = 0f;

            m_isEnabled = true;
            m_isPlayer = aIsPlayer;

            m_RotationDirection = TankDirection.Up;
            m_MovementDirection = TankDirection.None;

            m_TimeSpan = new TimeSpan();
            m_TimeRespawn = new TimeSpan();

        }

        public void Update(GameTime gameTime)
        {
            // Is this tank is the player, update with the controller
            if (m_isPlayer)
            {
                // Get the direction of the tank
                Vector2 input = InputWrapper.ThumbSticks.Left;

                if (input.X != 0)
                {
                    if (input.X > 0)
                        m_RotationDirection = TankDirection.Right;
                    else
                        m_RotationDirection = TankDirection.Left;
                }
                else if (input.Y != 0)
                {
                    if (input.Y > 0)
                        m_RotationDirection = TankDirection.Up;
                    else
                        m_RotationDirection = TankDirection.Down;
                }

                // Change the movement position
                if (input == Vector2.Zero)
                    m_MovementDirection = TankDirection.None;
                else
                    m_MovementDirection = m_RotationDirection;

                // Change the turret rotation
                m_turretRotation += GraphicsSupport.InputWrapper.ThumbSticks.Right.X / 5;
            }



            else // Is it's an enemy tank: AI movement
            {
                m_TimeSpan += gameTime.ElapsedGameTime;

                if (m_TimeSpan.Milliseconds % 21 == 0)
                {
                    m_RotationDirection = (TankDirection)Game1.sRan.Next(5);
                    m_MovementDirection = m_RotationDirection;

                    m_turretRotation = (float)(Game1.sRan.NextDouble() * 2 * 3.14159265359);
                }
            }
            

            // Rotate the tank
                if (m_RotationDirection == TankDirection.Left || m_RotationDirection == TankDirection.Right)
                    RotateAngleInRadian = (float)(3.14159265359 / 2);
                else
                    RotateAngleInRadian = 0;

            // Move if inside the window
                if (GraphicsSupport.Camera.CollidedWithCameraWindow(this) == Camera.CameraWindowCollisionStatus.InsideWindow)
                {
                    switch (m_MovementDirection)
                    {
                        case TankDirection.None:
                            break;

                        case TankDirection.Up:
                            mPosition += new Vector2(0, 1);
                            break;

                        case TankDirection.Right:
                            mPosition += new Vector2(1, 0);
                            break;

                        case TankDirection.Down:
                            mPosition += new Vector2(0, -1);
                            break;

                        case TankDirection.Left:
                            mPosition += new Vector2(-1, 0);
                            break;
                    }
                }
                else // move reverse if on the edge
                {
                    switch (m_MovementDirection)
                    {
                        case TankDirection.None:
                            break;

                        case TankDirection.Up:
                            mPosition += new Vector2(0, -2);
                            break;

                        case TankDirection.Right:
                            mPosition += new Vector2(-2, 0);
                            break;

                        case TankDirection.Down:
                            mPosition += new Vector2(0, 2);
                            break;

                        case TankDirection.Left:
                            mPosition += new Vector2(2, 0);
                            break;
                    }

                    m_MovementDirection = TankDirection.None;
                }

            // Check if need to be re-enabled - to respwan
            m_TimeRespawn += gameTime.ElapsedGameTime;
            if(!m_isEnabled && m_TimeRespawn.Seconds >= 5)
            {
                SetIsEnabled(true);
                Reset();
                m_TimeRespawn = TimeSpan.Zero;
            }

        }

        public void Draw()
        {
            base.Draw();

            // Draw the turret on top on the tank
            // Define location and size of the texture to show
            Rectangle destRect = Camera.ComputePixelRectangle(Position, Size);

            // Define the rotation origin
            Vector2 org = new Vector2(m_turret.Width / 2, m_turret.Height / 2);

            // Draw the texture
            Game1.sSpriteBatch.Draw(m_turret, destRect, null, Color.White, m_turretRotation, org, SpriteEffects.None, 0f);
        }

        public void Reset()
        {
            m_isEnabled = true;
            m_turretRotation = 0;
            m_RotationDirection = TankDirection.Up;
            m_MovementDirection = TankDirection.None;
        }

        public bool isEnabled()
        {
            return m_isEnabled;
        }

        public void SetIsEnabled(bool aIsEnabled)
        {
            m_isEnabled = aIsEnabled;

            // If is't disabled, start the timespan for respawning
            if(m_isEnabled == false)
            {
                m_TimeRespawn = TimeSpan.Zero;
            }
            else  // if it's enabled, reset
            {
                Reset();
            }
        }

        public float GetRotation()
        {
            return m_turretRotation;
        }
    }
}
