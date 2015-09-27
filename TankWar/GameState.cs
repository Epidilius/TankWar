using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using GraphicsSupport;

namespace TankWar
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameState
    {
        GraphicsSupport.Tank playerTank;
        GraphicsSupport.TexturedPrimitive m_Background;
        List<GraphicsSupport.Tank> enemyList = new List<GraphicsSupport.Tank>();
        List<GraphicsSupport.Bullet> bulletList = new List<GraphicsSupport.Bullet>();
        TimeSpan sLastShot;
        int enemyshooting;

        int bullets;
        int score;
        int lives;
        String liveString;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameState()
        {
            Audio.AudioSupport.PlayBackgroundAudio("Mind_Meld", 0.4f);

            playerTank = new GraphicsSupport.Tank("TANK_BASE_DARKGREY", "TANK_TURRET_DARKGREY", new Vector2(50,30), new Vector2(10, 10), true);

            enemyList.Add(new GraphicsSupport.Tank("TANK_BASE_BLUE", "TANK_TURRET_BLUE", new Vector2(20, 20), new Vector2(10, 10), false));
            enemyList.Add(new GraphicsSupport.Tank("TANK_BASE_GREEN", "TANK_TURRET_GREEN", new Vector2(30, 30), new Vector2(10, 10), false));
            enemyList.Add(new GraphicsSupport.Tank("TANK_BASE_RED", "TANK_TURRET_RED", new Vector2(40, 40), new Vector2(10, 10), false));
            enemyList.Add(new GraphicsSupport.Tank("TANK_BASE_YELLOW", "TANK_TURRET_YELLOW", new Vector2(50, 50), new Vector2(10, 10), false));

            m_Background = new GraphicsSupport.TexturedPrimitive("Ground_Tile", new Vector2(50, 50), new Vector2(100, 100));

            Reset();
        }

        /// <summary>
        /// Update the game state
        /// </summary>
        public void UpdateGame(GameTime gameTime)
        {
            // Update player tank and shoot if A is pressed
            if (playerTank.isEnabled())
            {
                playerTank.Update(gameTime);

                if (GraphicsSupport.InputWrapper.Buttons.A == ButtonState.Pressed)
                {
                    if ((gameTime.TotalGameTime.TotalMilliseconds - sLastShot.TotalMilliseconds) >= 500)
                    {
                        sLastShot = gameTime.TotalGameTime;
                        // Create a bullet if you press A/Space

                        bulletList.Add(new GraphicsSupport.Bullet("BULLET", playerTank.Position, new Vector2(3, 3), 3, 2, 0, playerTank.GetRotation()));
                        bulletList.Last().TimeWasShot = gameTime.TotalGameTime;
                        bullets++;

                        Audio.AudioSupport.PlayACue("Shoot");
                    }
                }

            }

            // Update all the enemy tank
            foreach (GraphicsSupport.Tank tank in enemyList)
            {
                tank.Update(gameTime);
            }

            // Update the bullets
            if (bullets > 0)
            {
                foreach (GraphicsSupport.Bullet bullet in bulletList)
                {
                    if (bullet.Enabled)
                    {
                        bullet.Update(gameTime);
                    }
                }
            }

            // Remove the bullet if necessary
            if (bullets > 0)
            {
                foreach (GraphicsSupport.Bullet bullet in bulletList)
                {
                    if (!bullet.Enabled)
                    {
                        bulletList.Remove(bullet);
                        bullets--;
                        break;
                    }
                }
            }

            // Ai to shoot bullets (not really good)
            if (gameTime.TotalGameTime.Milliseconds > 0 && gameTime.TotalGameTime.Milliseconds/100 % 33 == 0)
            {
                if (enemyList[enemyshooting % 4].isEnabled())
                {
                    bulletList.Add(new GraphicsSupport.Bullet("BULLET", enemyList[enemyshooting % 4].Position, new Vector2(3, 3), 3, 2, 0, enemyList[enemyshooting % 4].GetRotation(), 1 + (enemyshooting % 4)));
                    bullets++;
                }
                enemyshooting++;
            }

            CollisionDetection(gameTime);

            // Update the string for the amount of lives
            liveString = " ";
            for(int i = 0; i < lives; i++)
            {
                liveString += "<3 ";
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public void DrawGame()
        {
            m_Background.Draw();

            // Draw all bullets
            foreach (GraphicsSupport.Bullet bullet in bulletList)
            {
                bullet.Draw();                
            }

            // Draw if the played is enabled
            if (playerTank.isEnabled())
                playerTank.Draw();

            //Draw enabled enemy tanks
            foreach(GraphicsSupport.Tank tank in enemyList)
            {
                if (tank.isEnabled())
                    tank.Draw();
            }


            GraphicsSupport.FontSupport.PrintStatus("Score: " + score + "\nLives: " + liveString, Color.Azure);
        }

        public void Reset()
        {
            // Reset player
            playerTank.Reset();
            playerTank.Position = new Vector2(50, 30);

            // Reset all enemies
            foreach(GraphicsSupport.Tank enemy in enemyList)
            {
                enemy.Reset();
                enemy.Position = new Vector2(Game1.sRan.Next(10, 90), Game1.sRan.Next(10, 60));
            }

            //Clear the bullet list
            bulletList.Clear();

            // Reset all variables
            bullets = 0;
            score = 0;
            lives = 5;
            liveString = string.Empty;

            enemyshooting = 0;

            sLastShot = TimeSpan.Zero;
        }

        public void CollisionDetection(GameTime gameTime)
        {

            // Collide every bullet that isn't exploded
            foreach (GraphicsSupport.Bullet bullet in bulletList)
            {
                if (!bullet.Exploded)
                {
                    //Checked who shot
                    if (bullet.MyShooter != 0)
                    {
                        // With the player tank
                        if (playerTank.isEnabled() && playerTank.PrimitiveCollision(bullet))
                        {
                            bullet.Explode(gameTime);

                            Audio.AudioSupport.PlayACue("Explosion");

                            lives--;

                            if (lives <= 0)
                            {
                                playerTank.SetIsEnabled(false);
                            }
                        }
                    }
                    else
                    {
                        //With all enemies
                        for (int i = 1; i < 5; i++ )
                        {
                            if (enemyList[i - 1].isEnabled() && enemyList[i - 1].PrimitiveCollision(bullet) && bullet.MyShooter != i)
                            {
                                enemyList[i - 1].SetIsEnabled(false);
                                bullet.Explode(gameTime);
                                Audio.AudioSupport.PlayACue("Explosion");
                                score++;
                            }
                        }
                    }

                    //Collide with the window
                    if (!(GraphicsSupport.Camera.CollidedWithCameraWindow(bullet) == GraphicsSupport.Camera.CameraWindowCollisionStatus.InsideWindow))
                    {
                        bullet.Explode(gameTime);
                    }
                }
            }

        }
    }
}
