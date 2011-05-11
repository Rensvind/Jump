﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jump.Sprite
{
    public abstract class Sprite
    {
        //The current position of the Sprite
        public Vector2 Position;

        //The texture object used when drawing the sprite
        protected Texture2D MSpriteTexture;

        //The Size of the Sprite (with scale applied)
        public Rectangle Size;

        //The amount to increase/decrease the size of the original sprite. 
        private float mScale = 1.0f;

        protected readonly string AssetName;

        protected readonly int Height;
        protected readonly int Width;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    Width,
                    Height);
            }
        }

        protected Sprite(string assetName, int height, int width)
        {
            AssetName = assetName;
            Height = height;
            Width = width;
        }

        //Load the texture for the sprite using the Content Pipeline
        public virtual void LoadContent(ContentManager theContentManager)
        {
            MSpriteTexture = theContentManager.Load<Texture2D>(AssetName);
            Source = new Rectangle(0, 0, Width, Height);
            Size = new Rectangle(0, 0, (int)(Width * Scale), (int)(Height * Scale));
        }

        Rectangle mSource;
        public Rectangle Source
        {
            get { return mSource; }
            set
            {
                mSource = value;
                Size = new Rectangle(0, 0, (int)(mSource.Width * Scale), (int)(mSource.Height * Scale));
            }
        }

        //When the scale is modified throught he property, the Size of the 
        //sprite is recalculated with the new scale applied.
        public float Scale
        {
            get { return mScale; }
            set
            {
                mScale = value;
                //Recalculate the Size of the Sprite with the new scale
                Size = new Rectangle(0, 0, (int)(Source.Width * Scale), (int)(Source.Height * Scale));
            }
        }

        

        //Draw the sprite to the screen
        public virtual void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(MSpriteTexture, Position, Source,
                Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
        {
            Position += theDirection * theSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}