using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jump.Sprite
{
    public class BaseSprite : Sprite
    {
        public BaseSprite(string theAssetName) : base(theAssetName)
        {
        }

        //Load the texture for the sprite using the Content Pipeline
        public override void LoadContent(ContentManager theContentManager)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
            Source = new Rectangle(0, 0, 20, 20);
            Size = new Rectangle(0, 0, (int)(20 * Scale), (int)(20 * Scale));
        }
    }
}
