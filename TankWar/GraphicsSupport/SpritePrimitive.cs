using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankWar.GraphicsSupport
{
    class SpritePrimitive : GameObject
    {
        // Dimension of the spritesheet
        // num x of rows
        // Number of images, padding between images
        private int m_NumRow, m_NumCol, m_Padding;

        // Dimension of each image
        private int m_SpriteImageWidth, m_SpriteImageHeight;

        // Image animation
        private int m_UserSpecifiedTicks; // number of ticks before changing to next image
        private int m_CurrentTick;
        private int m_CurrentRow, m_CurrentCol;
        private int m_BeginRow, m_EndRow;
        private int m_BeginCol, m_EndCol;

        public SpritePrimitive(String aImageName, Vector2 aPosition, Vector2 aSize, int aRowCount, int aColCount, int aPadding)
            : base(aImageName, aPosition, aSize)
        {
            m_NumRow = aRowCount;
            m_NumCol = aColCount;
            m_Padding = aPadding;
            m_SpriteImageWidth = mImage.Width / m_NumRow;
            m_SpriteImageHeight = mImage.Height / m_NumCol;

            m_UserSpecifiedTicks = 1;
            m_CurrentTick = 0;
            m_CurrentRow = 0;
            m_CurrentCol = 0;
            m_BeginRow = m_BeginCol = m_EndRow = m_EndCol = 0;
        }

        public int SpriteBeginRow
        {
            get { return m_BeginRow; }
            set { m_BeginRow = value; m_CurrentRow = value; }
        }

        public int SpriteEndRow
        {
            get { return m_EndRow; }
            set { m_EndRow = value; }
        }

        public int SpriteBeginCol
        {
            get { return m_BeginCol; }
            set { m_BeginCol = value; m_CurrentCol = value; }
        }

        public int SpriteEndCol
        {
            get { return m_EndCol; }
            set { m_EndCol = value; }
        }

        public int SpriteAnimationTicks
        {
            get { return m_UserSpecifiedTicks; }
            set { m_UserSpecifiedTicks = value; }
        }

        public void SetSpriteAnimation(int aBeginRow, int aBeginCol, int aEndRow, int aEndCol, int aTickInterval)
        {
            m_UserSpecifiedTicks = aTickInterval;
            m_BeginRow = aBeginRow;
            m_BeginCol = aBeginCol;
            m_EndRow = aEndRow;
            m_EndCol = aEndCol;

            m_CurrentRow = m_BeginRow;
            m_CurrentCol = m_BeginCol;
            m_CurrentTick = 0;
        }

        public override void Update()
        {
            base.Update();

            m_CurrentTick++;

            if(m_CurrentTick == m_UserSpecifiedTicks)
            {
                m_CurrentTick = 0;

                m_CurrentCol++;
                if(m_CurrentCol > m_EndCol)
                {
                    m_CurrentCol = m_BeginCol;

                    m_CurrentRow++;
                    if (m_CurrentRow > m_EndRow)
                        m_CurrentRow = m_BeginRow;

                }
            }
        }

        public override void Draw()
        {
            Rectangle rect = Camera.ComputePixelRectangle(Position, Size);
            int imageTop = m_CurrentRow * m_SpriteImageWidth;
            int imageLeft = m_CurrentCol * m_SpriteImageHeight;

            Rectangle sourceRect = new Rectangle(imageLeft + m_Padding, imageTop + m_Padding, m_SpriteImageWidth, m_SpriteImageHeight);

            // Define the rotation origin
            Vector2 origin = new Vector2(m_SpriteImageWidth / 2, m_SpriteImageHeight / 2);
            TankWar.Game1.sSpriteBatch.Draw(mImage, rect, sourceRect, Color.White, mRotateAngle, origin, SpriteEffects.None, 0f);

        }

    }
}
