using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Jump.Sprite
{
    public class Brick : Sprite
    {
        private static readonly Random Rand = new Random();
        private static int Speed = 40;

        private static int cnt = 0;
        private int id;

        public Brick() : base("Brick")
        {
            id = cnt++;
        }

        public override void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager);
            Source = new Rectangle(0, 0, Rand.Next(10, 250), (int)(mSpriteTexture.Height * Scale));
            Position = new Vector2(Rand.Next(0, 600 - Source.Width), id*80);
        }

        public void Update(GameTime theGameTime)
        {
            if (Position.Y >= 800)
            {
                Source = new Rectangle(0, 0, Rand.Next(10, 250), (int)(mSpriteTexture.Height * Scale));
                Position = new Vector2(Rand.Next(0, 600 - Source.Width), 0);
            }

            base.Update(theGameTime, new Vector2(0, Speed), new Vector2(0, 1));
        }
    }
}