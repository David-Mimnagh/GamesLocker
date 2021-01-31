using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesLocker.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GamesLocker.GameStates
{
    public class BreakoutGameState : GameState
    {

        #region interanlClasses
        class Player
        {
            public int Score { get; set; }
            public int Lives { get; set; }
            public Texture2D PaddleTexture { get; set; }
            public float PositionX { get; set; }
            public float PositionY { get; set; }
        }
        class Block
        {
            public int BlockTypeId { get; set; }
            public int ScoreToGive { get; set; }
            public bool DrawBlock { get; set; }
            public Texture2D BlockTexture { get; set; }
            public Vector2 Position { get; set; }
        }
        class Ball
        {
            public float speedModifier { get; set; }
            public float currentSpeed { get; set; }
            public Texture2D BallTexture { get; set; }
            public float PositionX { get; set; }
            public float PositionY { get; set; }
            public Vector2 Direction { get; set; }
        }
        #endregion
        Player player;
        Ball ball;
        List<Block> blockList;
        Texture2D background;
        Texture2D logo;
        SpriteFont spriteFont;
        Button backButton;
        Button resetButton;
        bool playing, gameWon, drawReset, gameLost;
        string pointsText, livesText, winText;
        public BreakoutGameState(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }
        public override void Initialize()
        {
            drawReset = false;
            playing = false;
            gameWon = false;
            gameLost = false;
            player = new Player{ Lives = 5,
                                    Score = 0,
                                    PositionX = Constants.WINDOW_WIDTH / 2 - 80,
                                    PositionY = Constants.WINDOW_HEIGHT - 40
            };

            pointsText = String.Format("Your Points: {0} (Points from blocks Destroyed * Remaining Lives)", player.Score.ToString());
            livesText = String.Format("Lives Left: {0}", player.Lives.ToString());
            winText = "";
            ball = new Ball {
                PositionX = Constants.WINDOW_WIDTH / 2,
                PositionY = Constants.WINDOW_WIDTH / 2,
                currentSpeed = 3.0f,
                speedModifier = 2.0f,
                Direction = new Vector2(3.0f, 3.0f)
            };
            blockList = new List<Block>();

            for (int i = 0; i <= 48; i++)
            {
                if (i <= 11)
                {
                    blockList.Add(new Block
                    {
                        BlockTypeId = 1,
                        ScoreToGive = 1,
                        Position = new Vector2(90 + (i * 100), 400),
                        DrawBlock = true
                    });
                }
                else if (i > 11 && i <= 23)
                {
                    blockList.Add(new Block
                    {
                        BlockTypeId = 2,
                        ScoreToGive = 2,
                        Position = new Vector2(90 + ((i - 12) * 100), 350),
                        DrawBlock = true
                    });
                }
                else if (i > 23 && i <= 35)
                {
                    blockList.Add(new Block
                    {
                        BlockTypeId = 3,
                        ScoreToGive = 3,
                        Position = new Vector2(90 + ((i - 24) * 100), 300),
                        DrawBlock = true
                    });
                }
                else if (i > 35)
                {
                    blockList.Add(new Block
                    {
                        BlockTypeId = 4,
                        ScoreToGive = 4,
                        Position = new Vector2(90 + ((i - 36) * 100), 250),
                        DrawBlock = true
                    });
                }
            }
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Images/Breakout/BrBackground");
            logo = content.Load<Texture2D>("Images/MainMenu/Logo");
            spriteFont = content.Load<SpriteFont>("Fonts/mavenPro");
            player.PaddleTexture = content.Load<Texture2D>("Images/Breakout/PlayerPaddle");
            ball.BallTexture = content.Load<Texture2D>("Images/Breakout/Ball");


            for (int i = 0; i < blockList.Count; i++)
            {
                if (i <= 11)
                {
                    blockList[i].BlockTexture = content.Load<Texture2D>("Images/Breakout/Brick1");
                }
                else if (i > 11 && i <= 23)
                {
                    blockList[i].BlockTexture = content.Load<Texture2D>("Images/Breakout/Brick2");
                }
                else if (i > 23 && i <= 35)
                {
                    blockList[i].BlockTexture = content.Load<Texture2D>("Images/Breakout/Brick3");
                }
                else if (i > 35 && i <= 48)
                {
                    blockList[i].BlockTexture = content.Load<Texture2D>("Images/Breakout/Brick4");
                }
            }

            var btnBackSize = new Point(content.Load<Texture2D>("Images/Breakout/Back").Width,
                                            content.Load<Texture2D>("Images/Breakout/Back").Height);
            backButton = new Button(new MainMenuGameState(_graphicsDevice, base.Points), new Rectangle(10,120,btnBackSize.X, btnBackSize.Y),
                                                content.Load<Texture2D>("Images/Breakout/Back"),
                                                content.Load<Texture2D>("Images/Breakout/BackH"),
                                                content.Load<Texture2D>("Images/Breakout/BackH"),
                                                "");

            var btnResetSize = new Point(content.Load<Texture2D>("Images/Breakout/Reset").Width,
                                            content.Load<Texture2D>("Images/Breakout/Reset").Height);
            resetButton = new Button(new Rectangle((Constants.WINDOW_WIDTH - (btnResetSize.X + 20)), 120, btnResetSize.X, btnResetSize.Y),
                                                content.Load<Texture2D>("Images/Breakout/Reset"),
                                                content.Load<Texture2D>("Images/Breakout/ResetH"),
                                                content.Load<Texture2D>("Images/Breakout/ResetH"));
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            pointsText = String.Format("Your Points: {0}", (player.Score * player.Lives).ToString());
            livesText = String.Format("Lives Left: {0}", player.Lives.ToString());
            if(player.Lives == 0)
            {
                drawReset = true; 
                gameLost = true;
            }
            // Poll for current keyboard state
            KeyboardState state = Keyboard.GetState();

            if (playing)
            {
                var blocksLeft = blockList.Where(b => b.DrawBlock == true).Count();

                if (blocksLeft == 0)
                    gameWon = true;

                Rectangle ballRect = new Rectangle((int)ball.PositionX, (int)ball.PositionY, ball.BallTexture.Width, ball.BallTexture.Height);
                Rectangle paddleRect = new Rectangle((int)player.PositionX, (int)player.PositionY, player.PaddleTexture.Width, player.PaddleTexture.Height);

                if(player.PositionX <= 0)
                {
                    player.PositionX = 1;
                }
                else if(player.PositionX + player.PaddleTexture.Width >= Constants.WINDOW_WIDTH)
                {
                    player.PositionX = Constants.WINDOW_WIDTH - player.PaddleTexture.Width;
                }


                if (ballRect.Intersects(paddleRect))
                {
                    ball.Direction = new Vector2(ball.Direction.X, ball.Direction.Y * -1);
                }


                // LEFT WALL
                if (ballRect.X <= 0)
                {
                    ball.PositionX = 1;
                    ball.Direction = new Vector2(ball.Direction.X * -1, ball.Direction.Y);
                } //RIGHT WALL
                else if (ball.PositionX + ballRect.Width >= Constants.WINDOW_WIDTH)
                {
                    ball.PositionX = Constants.WINDOW_WIDTH - ball.BallTexture.Width;
                    ball.Direction = new Vector2(ball.Direction.X * -1, ball.Direction.Y);
                }//TOP
                else if (ball.PositionY <= 198)
                {
                    ball.PositionY = 199;
                    ball.Direction = new Vector2(ball.Direction.X, ball.Direction.Y * -1);
                }//BOTTOM
                else if (ball.PositionY >= Constants.WINDOW_HEIGHT - ball.BallTexture.Height)
                {
                    ball.PositionX = Constants.WINDOW_WIDTH / 2;
                    ball.PositionY = Constants.WINDOW_WIDTH / 2;
                    player.Lives--;
                    playing = false;
                }


                if (state.IsKeyDown(Keys.Right))
                    player.PositionX += 10.0f;

                if (state.IsKeyDown(Keys.Left))
                    player.PositionX -= 10.0f;


                foreach (var block in blockList)
                {
                    if (block.DrawBlock)
                    {
                        Rectangle blockRect = new Rectangle((int)block.Position.X, (int)block.Position.Y, block.BlockTexture.Width, block.BlockTexture.Height);
                        if (ballRect.Intersects(blockRect))
                        {
                            if (ball.Direction.Y > 0)
                            {
                                ball.Direction = new Vector2(ball.Direction.X * -1, ball.Direction.Y);
                            }
                            else
                            {
                                ball.Direction = new Vector2(ball.Direction.X, ball.Direction.Y * -1);
                            }
                            player.Score += block.ScoreToGive;
                            block.DrawBlock = false;
                            break;
                        }
                    }

                }

                ball.PositionX += ball.Direction.X;
                ball.PositionY += ball.Direction.Y;
            }
            else
            {
                if (state.IsKeyDown(Keys.Space)) {
                    if (!gameLost && !gameWon)
                    {
                        playing = true;
                        Random rand = new Random();
                        float xPos = (rand.Next(-5, 5) * 0.3f) + ball.currentSpeed;
                        if(xPos > 0)
                        {
                            rand = new Random();
                            int val = rand.Next(3);
                            if (val >= 1)
                                xPos *= -1;
                        }
                        float YPos = (rand.Next(-5, 5) * 0.3f) + ball.currentSpeed;
                        ball.Direction = new Vector2(xPos, YPos);
                    }
                }
            }


            backButton.Update(mouseState);
            resetButton.Update(mouseState);
            if (resetButton.ButtonState == Button.State.Released)
            {
                Reset();
            }


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(logo, new Vector2(Constants.WINDOW_WIDTH / 2 - (logo.Width / 1.6f), 25), Color.White);
            backButton.Draw(spriteBatch);
            spriteBatch.DrawString(spriteFont, pointsText, new Vector2(Constants.WINDOW_WIDTH / 2, 188), Color.White, 0, spriteFont.MeasureString(pointsText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, livesText, new Vector2(Constants.WINDOW_WIDTH / 2 - 376, 188), Color.White, 0, spriteFont.MeasureString(livesText) / 2, 1.3f, SpriteEffects.None, 0.5f);

            if (gameWon)
            {
                winText = String.Format("YOU WON! You gained {0} Points!", (player.Score * player.Lives).ToString());
                spriteBatch.DrawString(spriteFont, winText, new Vector2(Constants.WINDOW_WIDTH / 2, Constants.WINDOW_HEIGHT / 2), Color.Red, 0, spriteFont.MeasureString(winText) / 2, 3.3f, SpriteEffects.None, 0.5f);
            }
            else if (gameLost)
            {
                winText = "Ouch! No win this time.\nPress Reset to start again";
                spriteBatch.DrawString(spriteFont, winText, new Vector2(Constants.WINDOW_WIDTH / 2, Constants.WINDOW_HEIGHT / 2 + 100), Color.Red, 0, spriteFont.MeasureString(winText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            }

            foreach (var block in blockList)
            {
                if (block.DrawBlock)
                    spriteBatch.Draw(block.BlockTexture, block.Position, Color.White);
            }
            spriteBatch.Draw(ball.BallTexture, new Vector2(ball.PositionX, ball.PositionY), Color.White);
            spriteBatch.Draw(player.PaddleTexture, new Vector2(player.PositionX, player.PositionY), Color.White);

            if (drawReset)
            {
                resetButton.Draw(spriteBatch);
            }

            spriteBatch.End();
        }
        void Reset()
        {
            drawReset = true;
            playing = false;
            gameWon = false;
            gameLost = false;
            player.Lives = 5;
            player.Score = 0;
            player.PositionX = Constants.WINDOW_WIDTH / 2 - 80;
            player.PositionY = Constants.WINDOW_HEIGHT - 40;

            pointsText = String.Format("Your Points: {0} (Points from blocks Destroyed * Remaining Lives)", player.Score.ToString());
            livesText = String.Format("Lives Left: {0}", player.Lives.ToString());
            winText = "";
            ball.PositionX = Constants.WINDOW_WIDTH / 2;
            ball.PositionY = Constants.WINDOW_HEIGHT / 2;
            ball.Direction = new Vector2(3.0f, 3.0f);
            ball.currentSpeed = 3.0f;
            ball.speedModifier = 2.0f;
            foreach (var block in blockList)
            {
                block.DrawBlock = true;
            }
        }
    }
}
