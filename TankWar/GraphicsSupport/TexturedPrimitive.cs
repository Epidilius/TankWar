using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankWar.GraphicsSupport
{
    /// <summary>
    /// TexturedPrimitive class
    /// </summary>
    public class TexturedPrimitive
    {
        //Enum for collision type
        public enum CollisionType
        {
            CollisionRectangle = 0,
            CollisionCircle
        }
        // Support for drawing the image
        protected Texture2D mImage;     // The UWB-JPG.jpg image to be loaded
        protected Vector2 mPosition;    // Center position of image
        protected Vector2 mSize;        // Size of the image to be drawn
        protected float mRotateAngle;   // In Radian, clockwise rotation
        protected String mLabelString;  // String to draw
        protected Color mLabelColor = Color.Black;
        CollisionType m_CollisionType;  // The collision type

        protected void InitPrimitive(String imageName, Vector2 position, Vector2 size, String label = null)
        {
            mImage = Game1.sContent.Load<Texture2D>(imageName);
            mPosition = position;
            mSize = size;
            mRotateAngle = 0f;
            mLabelString = label;
            m_CollisionType = 0; //Rectangle by default
        }

        /// <summary>
        /// Constructor of TexturePrimitive
        /// </summary>
        /// <param name="imageName">name of the image to be loaded as texture.</param>
        /// <param name="position">center position of the texture to be drawn</param>
        /// <param name="size">width/height of the texture to be drawn</param>
        public TexturedPrimitive(String imageName, Vector2 position, Vector2 size, String label = null)
        {
            InitPrimitive(imageName, position, size, label);
        }

        public TexturedPrimitive(String imageName)
        {
            InitPrimitive(imageName, Vector2.Zero, new Vector2(1f, 1f));
        }

        // Accessors
        public Vector2 Position { get { return mPosition; } set { mPosition = value; } }
        public float PositionX { get { return mPosition.X; } set { mPosition.X = value; } }
        public float PositionY { get { return mPosition.Y; } set { mPosition.Y = value; } }
        public Vector2 Size { get { return mSize; } set { mSize = value; } }
        public float Width { get { return mSize.X; } set { mSize.X = value; } }
        public float Height { get { return mSize.Y; } set { mSize.Y = value; } }
        public Vector2 MinBound { get { return mPosition - (0.5f * mSize); } }
        public Vector2 MaxBound { get { return mPosition + (0.5f * mSize); } }
        public float RotateAngleInRadian { get { return mRotateAngle; } set { mRotateAngle = value; } }
        public String Label { get { return mLabelString; } set { mLabelString = value; } }
        public Color LabelColor { get { return mLabelColor; } set { mLabelColor = value; } }

        /// <summary>
        /// Allows the primitive object to update its state
        /// </summary>
        /// <param name="deltaTranslate">Amount to change the position of the primitive. 
        ///                              Value of 0 says position is not changed.</param>
        /// <param name="deltaScale">Amount to change of the scale the primitive. 
        ///                          Value of 0 says size is not changed.</param>
        /// <param name="deltaAngleInRadian">Amount to rotate in clock-wise (in Radian) </param>
        public void Update(Vector2 deltaTranslate, Vector2 deltaScale, float deltaAngleInRadian)
        {
            mPosition += deltaTranslate;
            mSize += deltaScale;
            mRotateAngle += deltaAngleInRadian;
        }

        /// <summary>
        /// If two circular TexturedPrimitive collides (simple radius check)
        /// </summary>
        /// <param name="otherPrim">Primitive testing for collision</param>
        /// <returns>True: collides</returns>
        public bool PrimitivesTouches(TexturedPrimitive otherPrim)
        {
            Vector2 v = mPosition - otherPrim.Position;
            float dist = v.Length();
            return (dist < ((mSize.X / 2f) + (otherPrim.mSize.X / 2f)));
        }

        //Collision code
        public bool PrimitiveCollision(TexturedPrimitive aOtherPrimitive)
        {
            Vector2 vector = mPosition - aOtherPrimitive.Position;
            float dist = vector.Length();
            //TODO: Get this to work

            //Determines the collision shapes of the primitives
            switch(m_CollisionType)
            {
                case CollisionType.CollisionRectangle:
                    switch(aOtherPrimitive.MyCollisionType)
                    {
                        case CollisionType.CollisionRectangle:
                            return Collision2Rectangles(aOtherPrimitive);
                        case CollisionType.CollisionCircle:
                            return CollisionCircleRectangle(aOtherPrimitive);
                        default:
                            return Collision2Rectangles(aOtherPrimitive);    
                    }
                case CollisionType.CollisionCircle:
                    switch (aOtherPrimitive.MyCollisionType)
                    {
                        case CollisionType.CollisionRectangle:
                            return CollisionCircleRectangle(aOtherPrimitive);
                        case CollisionType.CollisionCircle:
                            return Collision2Circles(aOtherPrimitive);
                        default:
                            return CollisionCircleRectangle(aOtherPrimitive);
                    }  
                default:
                    switch (aOtherPrimitive.MyCollisionType)
                    {
                        case CollisionType.CollisionRectangle:
                            return Collision2Rectangles(aOtherPrimitive);
                        case CollisionType.CollisionCircle:
                            return CollisionCircleRectangle(aOtherPrimitive);
                        default:
                            return Collision2Rectangles(aOtherPrimitive);
                    }  
            }

        }

        public bool Collision2Rectangles(TexturedPrimitive aOtherPrimitive)
        {
            Vector2 distance = mPosition - aOtherPrimitive.mPosition;

            distance.X = Math.Abs(distance.X);
            distance.Y = Math.Abs(distance.Y);

            if (distance.X > (float)((mSize.X / 2) + (aOtherPrimitive.mSize.X / 2)) || distance.Y > (float)((mSize.Y / 2) + (aOtherPrimitive.mSize.Y / 2)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool Collision2Circles(TexturedPrimitive aOtherPrimitive)
        {
            //This function determines the distance between two(2) circles 
            //and uses their radii to determine if the circles are colliding

            //Distance holds the difference in position. Using .length given the actual distance
            Vector2 distance = mPosition - aOtherPrimitive.mPosition;

            //Compares the distance between the primitives to the sum of their radii
            if (distance.Length() < ((mSize.X / 2) + (aOtherPrimitive.mSize.X / 2)))
            {
                return true;
            }
            return false;
        }

        public bool CollisionCircleRectangle(TexturedPrimitive aOtherPrimitive)
        {
            //The distance between the two(2) primitives
            Vector2 distance = mPosition - aOtherPrimitive.mPosition;

            //Initial check to see if a collision is possible
            if (Math.Abs(distance.X) > (int)((aOtherPrimitive.mSize.X / 2) + (mSize.X / 2)) || Math.Abs(distance.Y) > (int)((aOtherPrimitive.mSize.Y / 2) + (mSize.Y / 2)))
            {
                //No collision
                return false;
            }

            //From here on is to check for a near miss around one of the corners of the rectangle

            //Circle's position and radius
            Vector2 circlePosition;
            float circleRadius;

            //The corners of the rectangle
            Vector2[] corners = new Vector2[4];

            //Determine which primitive is the circle 
            if (m_CollisionType == CollisionType.CollisionCircle)
            {
                //Get the circle's position and radius
                circlePosition = mPosition;
                circleRadius = (float)mSize.X / 2f;

                //Find the positions of the corners
                //Calculate the positions of the corners based on the image size and the origin

                //Bottom left is the simplest to find
                corners[0] = aOtherPrimitive.mPosition - new Vector2(aOtherPrimitive.mSize.X, aOtherPrimitive.mSize.Y);
                //Bottom right
                corners[1] = aOtherPrimitive.mPosition - new Vector2(0, aOtherPrimitive.mSize.Y);
                //Top left
                corners[2] = aOtherPrimitive.mPosition - new Vector2(aOtherPrimitive.mSize.X, 0);
                //Top right
                corners[3] = aOtherPrimitive.mPosition;

            }
            else
            {
                //Get the circle's position and radius
                circlePosition = aOtherPrimitive.mPosition;
                circleRadius = (float)aOtherPrimitive.mSize.X / 2f;

                //Find the positions of the corners
                //Calculate the positions of the corners based on the image size and the origin

                //Bottom left is the simplest to find
                corners[0] = mPosition - new Vector2(mSize.X, mSize.Y);
                //Bottom right
                corners[1] = mPosition - new Vector2(0, mSize.Y);
                //Top left
                corners[2] = mPosition - new Vector2(mSize.X, 0);
                //Top right
                corners[3] = mPosition;
            }


            //Check for collisions with the corners
            if ((corners[0] - circlePosition).Length() < circleRadius ||
                (corners[1] - circlePosition).Length() < circleRadius ||
                (corners[2] - circlePosition).Length() < circleRadius ||
                (corners[3] - circlePosition).Length() < circleRadius)
            {
                //The circle is colliding directly with a corner
                return true;
            }


            //To make things easier on my mind I'm going to sort the corners by distance from the circle
            //After this point assume that the corners got swapped around inside the array
            //Note to Joel: These braces are me attempting to manage memory in C# lol
            {
                Vector2 sorting;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if((corners[0] - circlePosition).Length() > (corners[1] - circlePosition).Length())
                        {
                            //This will sort the array so that the closest corner is corners[0] and the farthest is corners[3]
                            sorting = corners[0];
                            corners[0] = corners[1];
                            corners[1] = sorting;
                        }
                    }
                }
            }

            if (corners[0].X == corners[1].X)
            {
                //If the circle is farther from the 2nd and 3rd closest corners the the closest corner is 
                //then it is a near miss
                if ((corners[0] - (corners[1] + new Vector2(circleRadius, 0))).Length() < (circlePosition - corners[1]).Length() &&
                    (corners[0] - (corners[2] + new Vector2(0, circleRadius))).Length() < (circlePosition - corners[2]).Length())
                {
                    return false;
                }
            }
            else
            {
                //If the circle is farther from the 2nd and 3rd closest corners the the closest corner is 
                //then it is a near miss
                if ((corners[0] - (corners[1] + new Vector2(0, circleRadius))).Length() < (circlePosition - corners[1]).Length() &&
                    (corners[0] - (corners[2] + new Vector2(circleRadius, 0))).Length() < (circlePosition - corners[2]).Length())
                {
                    return false;
                }
            }

            
            //If it isn't a near miss then it is a hit
            return true;
        }




        /// <summary>
        /// Draw the primitive
        /// </summary>
        virtual public void Draw()
        {
            // Define location and size of the texture to show
            Rectangle destRect = Camera.ComputePixelRectangle(Position, Size);

            // Define the rotation origin
            Vector2 org = new Vector2(mImage.Width / 2, mImage.Height / 2);

            // Draw the texture
            Game1.sSpriteBatch.Draw(mImage,
                            destRect,           // Area to be drawn in pixel space
                            null,               // 
                            Color.White,        // 
                            mRotateAngle,       // Angle to rotate (clockwise)
                            org,                // Image reference position
                            SpriteEffects.None, 0f);

            if (null != Label)
                FontSupport.PrintStatusAt(Position, Label, LabelColor);
        }

        public CollisionType MyCollisionType
        {
            get { return m_CollisionType; }
            set { m_CollisionType = value; }
        }
    }
}
