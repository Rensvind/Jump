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

        private const int WalkingLeftFrameY = 0;
        private const int WalkingRightFrameY = 40;

        private const int FrameWidth = 20;

        private const int NrOfFrames = 3;
        private int currentFrame = 0;

        private float timeElapsed;
        private const float TimeToUpdate = 0.1f;

        private SoundEffect soundEffect;

        public Jumper(GraphicsDeviceManager graphicsDeviceManager) : base("MrJump", 40, 20)
        {
            windowWidth =  graphicsDeviceManager.PreferredBackBufferWidth;
            windowHeight = graphicsDeviceManager.PreferredBackBufferHeight;
        }

        KeyboardState mPreviousKeyboardState;

        enum State
        {
            Walking,
            Jumping,
            Falling
        }

        private bool parachute;

        State mCurrentState = State.Falling;

        Vector2 mDirection = Vector2.Zero;
        Vector2 mSpeed = Vector2.Zero;

        Vector2 mStartingPosition = Vector2.Zero;
        private Brick currentBrick;
        private const int FrameHeight = 40;

        public void Update(GameTime theGameTime, List<Brick> bricks)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();

            UpdateMovement(theGameTime, aCurrentKeyboardState);
            UpdateJump(aCurrentKeyboardState, bricks);
            UpdateFalling(aCurrentKeyboardState, bricks);

            mPreviousKeyboardState = aCurrentKeyboardState;

            base.Update(theGameTime, mSpeed, mDirection);
        }

        private void UpdateFalling(KeyboardState aCurrentKeyboardState, List<Brick> bricks)
        {
            if (mCurrentState != State.Falling) return;
            
            mDirection.Y = MoveDown;
            mSpeed.Y = JumperSpeed;

            if (aCurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                if (Position.X > 0)
                {
                    mSpeed.X = (float)JumperSpeed / 1.5f;
                    mDirection.X = MoveLeft;
                }

            }
            else if (aCurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                if (Position.X + Size.Width < windowWidth)
                {
                    mSpeed.X = (float)JumperSpeed / 1.5f;
                    mDirection.X = MoveRight;
                }
            }
            
            if(aCurrentKeyboardState.IsKeyDown(Keys.F) && !parachute)
            {
                parachute = true;
                Source = new Rectangle(FrameWidth * 3, FrameHeight * 2, FrameWidth, FrameHeight);
            }

            if (Position.X <= 0)
            {
                mDirection.X = MoveRight;
            }
            else if (Position.X + Size.Width > windowWidth)
            {
                mDirection.X = MoveLeft;
            }

            if (Position.Y >= windowHeight)
                Position.Y = 0;

            var brick = bricks.FirstOrDefault(
                x =>
                x.Position.X - 10 <= Position.X && x.Position.X + x.Source.Width - 10 >= Position.X &&
                x.Position.Y - 2 <= (int)(Position.Y + Source.Height) &&
                (int)(Position.Y + Source.Height) <= (x.Position.Y + x.Source.Height));


            if (brick == null)
            {
                mSpeed.Y = !parachute ? JumperSpeed * 2 : JumperSpeed;
                mDirection.Y = MoveDown;
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("WALKING");
                parachute = false;
                currentBrick = brick;
                mCurrentState = State.Walking;
                Source = new Rectangle(mDirection.X == 1 ? 40 : 0, WalkingRightFrameY, FrameWidth, FrameHeight);
                Position.Y = brick.Position.Y - Source.Height;
            }
        }

        private void UpdateMovement(GameTime theGameTime, KeyboardState aCurrentKeyboardState)
        {
            if (mCurrentState != State.Walking) return;


            mSpeed = Vector2.Zero;
            mDirection = Vector2.Zero;

            Position.Y = currentBrick.Position.Y - Source.Height;

            if (aCurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                timeElapsed += (float)theGameTime.ElapsedGameTime.TotalSeconds;

                if (timeElapsed > TimeToUpdate)
                {
                    timeElapsed -= TimeToUpdate;

                    Source = new Rectangle(currentFrame * 20, WalkingLeftFrameY, 20, FrameHeight);

                    if (++currentFrame == NrOfFrames)
                        currentFrame = 0;
                }

                if (Position.X > 0)
                {
                    mSpeed.X = JumperSpeed;
                    mDirection.X = MoveLeft;
                }
            }

            else if (aCurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                timeElapsed += (float)theGameTime.ElapsedGameTime.TotalSeconds;

                if (timeElapsed > TimeToUpdate)
                {
                    timeElapsed -= TimeToUpdate;

                    Source = new Rectangle(currentFrame * 20, WalkingRightFrameY, 20, FrameHeight);

                    if (++currentFrame == NrOfFrames)
                        currentFrame = 0;
                }

                if (Position.X + Size.Width < windowWidth)
                {
                    mSpeed.X = JumperSpeed;
                    mDirection.X = MoveRight;
                }
            }
            else
            {
                Source = new Rectangle(0, 80, 20, FrameHeight);
            }


            if (currentBrick.PowerUp(new Rectangle((int)Position.X, (int)Position.Y, FrameWidth, FrameHeight)))
            {
            }

            if(Position.X < currentBrick.Position.X - 10 || Position.X >= currentBrick.Position.X + currentBrick.Source.Width)
            {
                //System.Diagnostics.Debug.WriteLine("FALLING");
                mCurrentState = State.Falling;
                Source = new Rectangle(0, FrameHeight * 2, FrameWidth, FrameHeight);
                mSpeed.Y = JumperSpeed*2;
                mDirection.Y = MoveDown;
                currentBrick = null;
                parachute = false;
            }
        }

        private void UpdateJump(KeyboardState aCurrentKeyboardState, List<Brick> bricks)
        {
            if (mCurrentState == State.Walking)
            {
                if ((aCurrentKeyboardState.IsKeyDown(Keys.Space) || aCurrentKeyboardState.IsKeyDown(Keys.Up)) && (!mPreviousKeyboardState.IsKeyDown(Keys.Space) && !mPreviousKeyboardState.IsKeyDown(Keys.Up)))
                {
                    Jump();
                }
            }

            if (mCurrentState == State.Jumping)
            {
                if (mStartingPosition.Y - Position.Y > JumpHeight)
                {
                    //System.Diagnostics.Debug.WriteLine("FALLING2");
                    mDirection.Y = MoveDown;
                    mCurrentState = State.Falling;
                    Source = new Rectangle(0, FrameHeight * 2, FrameWidth, FrameHeight);
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
                        //System.Diagnostics.Debug.WriteLine("WALKING2");
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
                //System.Diagnostics.Debug.WriteLine("JUMPING");
                soundEffect.Play(0.3f, 0.0f, 0.0f);
                mCurrentState = State.Jumping;
                mStartingPosition = Position;
                mDirection.Y = MoveUp;
                mSpeed = new Vector2(320, 320);

                //System.Diagnostics.Debug.WriteLine("X: " + mDirection.X + ", previousRight: " + mPreviousKeyboardState.GetPressedKeys().Aggregate(string.Empty, (current, pressedKey) => current + pressedKey));
                Source = mDirection.X <= 0 ? new Rectangle(20, 80, 20, Source.Height) : new Rectangle(40, 80, 20, Source.Height);
            }
        }

        public void LoadContent(ContentManager theContentManager, int x, int y)
        {
            LoadContent(theContentManager);

            Position = new Vector2(x,y);
        }

        public override void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager);

            Source = new Rectangle(0, 80, 20, 40);

            soundEffect = theContentManager.Load<SoundEffect>("Untitled");
        }
    }
}