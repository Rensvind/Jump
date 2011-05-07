using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jump.Sprite
{
    public class BaseSprite : Sprite
    {
        public BaseSprite(string theAssetName, int height, int width) : base(theAssetName, height, width)
        {
        }

        //Load the texture for the sprite using the Content Pipeline
        public override void LoadContent(ContentManager theContentManager)
        {
            MSpriteTexture = theContentManager.Load<Texture2D>(AssetName);
            Source = new Rectangle(0, 0, Width, Height);
            Size = new Rectangle(0, 0, (int)(Width * Scale), (int)(Height * Scale));
        }
    }
}
