using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jump.Sprite
{
    public class Brick : Sprite
    {
        private static readonly Random Rand = new Random();
        private const int Speed = 40;

        private static int cnt;
        private readonly int id;

        private readonly BaseSprite powerUp;
        private bool showPowerUp;

        public Brick()
            : base("Brick", 7, Rand.Next(10, 250))
        {
            id = cnt++;
            powerUp = new BaseSprite("PowerUps", 20, 20);
        }

        public override void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager);
            Source = new Rectangle(0, 0, Width, (int)(MSpriteTexture.Height * Scale));
            Position = new Vector2(Rand.Next(0, 600 - Source.Width), id*80);

            powerUp.LoadContent(theContentManager);
            showPowerUp = Rand.Next(10) > 8;

            if(showPowerUp)
                powerUp.Position.X = Rand.Next((int)Position.X, (int)Position.X + Source.Width);
            else
            {
                powerUp.Position = new Vector2(-20, -20);
            }
        }

        public void Update(GameTime theGameTime)
        {
            if (Position.Y >= 800)
            {
                Source = new Rectangle(0, 0, Rand.Next(10, 250), (int)(MSpriteTexture.Height * Scale));
                Position = new Vector2(Rand.Next(0, 600 - Source.Width), 0);

                showPowerUp = Rand.Next(10) > 8;

                if(showPowerUp)
                    powerUp.Position.X = Rand.Next((int)Position.X, (int)Position.X + Source.Width - 20);
                else
                {
                    powerUp.Position = new Vector2(-20, -20);
                }
            }

            base.Update(theGameTime, new Vector2(0, Speed), new Vector2(0, 1));

            if(showPowerUp)
                powerUp.Position.Y = Position.Y - 20;
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            base.Draw(theSpriteBatch);
            powerUp.Draw(theSpriteBatch);
        }

        public bool PowerUp(Rectangle jumper)
        {
            if (!showPowerUp)
                return false;

            if(jumper.Intersects(new Rectangle((int)powerUp.Position.X, (int)powerUp.Position.Y, powerUp.Source.Width, powerUp.Source.Height)))
            {
                showPowerUp = false;
                powerUp.Position = new Vector2(-20, -20);
                return true;
            }

            return false;
        }
    }
}