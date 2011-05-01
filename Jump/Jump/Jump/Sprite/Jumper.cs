using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jump.Sprite
{
    public class Jumper : Sprite
    {
        private const int JumperSpeed = 160;
        private const int MoveUp = -1;
        private const int MoveDown = 1;
        private const int MoveLeft = -1;
        private const int MoveRight = 1;
        private const int JumpHeight = 150;

        private readonly int windowHeight;
        private readonly int windowWidth;

        private SoundEffect soundEffect;

        public Jumper(GraphicsDeviceManager graphicsDeviceManager) : base("MrJump")
        {
            windowWidth = graphicsDeviceManager.PreferredBackBufferWidth;
            windowHeight = graphicsDeviceManager.PreferredBackBufferHeight;
        }

        KeyboardState mPreviousKeyboardState;

        enum State
        {
            Walking,
            Jumping,
            Falling
        }

        State mCurrentState = State.Walking;

        Vector2 mDirection = Vector2.Zero;
        Vector2 mSpeed = Vector2.Zero;

        Vector2 mStartingPosition = Vector2.Zero;

        public void Update(GameTime theGameTime, List<Brick> bricks)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();

            UpdateMovement(aCurrentKeyboardState, bricks);
            UpdateJump(aCurrentKeyboardState, bricks);

            mPreviousKeyboardState = aCurrentKeyboardState;

            base.Update(theGameTime, mSpeed, mDirection);
        }

        private void UpdateMovement(KeyboardState aCurrentKeyboardState, List<Brick> bricks)
        {
            if (mCurrentState == State.Walking)
            {
                mSpeed = Vector2.Zero;
                mDirection = Vector2.Zero;

                var brick = bricks.FirstOrDefault(
                    x =>
                    x.Position.X -10 <= Position.X && x.Position.X + x.Source.Width - 10 >= Position.X &&
                    x.Position.Y - 2 <= (int) (Position.Y + Source.Height) &&
                    (int) (Position.Y + Source.Height) <= (x.Position.Y + x.Source.Height));


                if(brick == null)
                {
                    mSpeed.Y = JumperSpeed * 2;
                    mDirection.Y = MoveDown;
                }else
                {
                    Position.Y = brick.Position.Y - Source.Height;
                }

                if (aCurrentKeyboardState.IsKeyDown(Keys.Left) && Position.X > 0)
                {
                    mSpeed.X = JumperSpeed;
                    mDirection.X = MoveLeft;
                }

                else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) && Position.X + Size.Width < windowWidth)
                {
                    mSpeed.X = JumperSpeed;
                    mDirection.X = MoveRight;
                }

                if (Position.Y >= windowHeight)
                    Position.Y = 0;
            }
        }

        private void UpdateJump(KeyboardState aCurrentKeyboardState, List<Brick> bricks)
        {
            if (mCurrentState == State.Walking)
            {
                if (aCurrentKeyboardState.IsKeyDown(Keys.Space)&& mPreviousKeyboardState.IsKeyDown(Keys.Space) == false)
                {
                    Jump();
                }
            }

            if (mCurrentState == State.Jumping)
            {
                if (mStartingPosition.Y - Position.Y > JumpHeight)
                {
                    mDirection.Y = MoveDown;
                }

                if(Position.X <= 0 || Position.X + Source.Width > windowWidth)
                {
                    mDirection.X *= -1;
                }

                if (mDirection.Y == MoveDown)
                {
                    var brick = bricks.FirstOrDefault(
                    x =>
                    x.Position.X - 10 <= Position.X && x.Position.X + x.Source.Width - 10 >= Position.X &&
                    x.Position.Y - 2 <= (int)(Position.Y + Source.Height) &&
                    (int)(Position.Y + Source.Height) <= (x.Position.Y + x.Source.Height));


                    if (brick != null)
                    {
                        Position.Y = brick.Position.Y - Source.Height;
                        mCurrentState = State.Walking;
                        mDirection = Vector2.Zero;
                        Source = new Rectangle(0, 0, 20, Source.Height);
                    }
                    
                }

                if(Position.Y >= windowHeight)
                {
                    Position.Y = 0;
                }
            }
        }

        private void Jump()
        {

            if (mCurrentState != State.Jumping)
            {
                soundEffect.Play(0.3f, 0.0f, 0.0f);
                mCurrentState = State.Jumping;
                mStartingPosition = Position;
                mDirection.Y = MoveUp;
                mSpeed = new Vector2(320, 320);
                Source = new Rectangle(20, 0, 20, Source.Height);
            }
        }

        
        public override void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager);

            Source = new Rectangle(0, 0, 20, mSpriteTexture.Height);

            soundEffect = theContentManager.Load<SoundEffect>("Untitled");
        }
    }
}