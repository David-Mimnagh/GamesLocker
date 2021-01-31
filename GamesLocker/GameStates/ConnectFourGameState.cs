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
    public class ConnectFourGameState : GameState
    {
        #region internalClasses
        class Player
        {
            public Texture2D chipTexture { get; set; }
            public bool won { get; set; }
        }
        #endregion
        Player player1;
        Player player2;
        Texture2D highlightTexture;
        Texture2D blankTexture;
        Texture2D background;
        Texture2D logo;
        SpriteFont spriteFont;
        string messageText, gameText;
        bool drawReset, player1Go, gameWon;
        Button backButton;
        Button resetButton;
        Vector2 chipPosition;
        Rectangle[,] boardCircles;
        int[,] board;
        bool[,] boardHighlight;


        public ConnectFourGameState(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            player1 = new Player();
            player2 = new Player();
        }

        public override void Initialize()
        {
            messageText = "Welcome to GameBox - Connect Four.";
            gameText = "Player 1, click to make your selection.";
            drawReset = true;
            player1Go = true;
            chipPosition = new Vector2(0, 0);
            boardCircles = new Rectangle[6, 7];
            for (int row = 0; row < 6; row++)
            {
                int additionBot = row * 80;
                for (int i = 0; i < 7; i++)
                {
                    int additionRight = i * 90;
                    boardCircles[row, i].X = 348 + additionRight;
                    boardCircles[row, i].Y = 90 + additionBot;
                    //circle[row][i].setFillColor(sf::Color(sf::Color::Transparent));
                    /*circle[row][i].setFillColor(sf::Color(255, 0, 0, 255));*/
                }
            }
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Images/ConnectFour/board");
            logo = content.Load<Texture2D>("Images/MainMenu/Logo");
            spriteFont = content.Load<SpriteFont>("Fonts/mavenPro");
            blankTexture = content.Load<Texture2D>("Images/ConnectFour/blank");
            player1.chipTexture = content.Load<Texture2D>("Images/ConnectFour/player1_small");
            player2.chipTexture = content.Load<Texture2D>("Images/ConnectFour/player2_small");

            var btnBackSize = new Point(content.Load<Texture2D>("Images/ConnectFour/Back").Width,
                                       content.Load<Texture2D>("Images/ConnectFour/Back").Height);
            backButton = new Button(new MainMenuGameState(_graphicsDevice, base.Points), new Rectangle(10, (Constants.WINDOW_HEIGHT - (btnBackSize.Y + 20)), btnBackSize.X, btnBackSize.Y),
                                                content.Load<Texture2D>("Images/ConnectFour/Back"),
                                                content.Load<Texture2D>("Images/ConnectFour/BackH"),
                                                content.Load<Texture2D>("Images/ConnectFour/BackH"),
                                                "");

            var btnResetSize = new Point(content.Load<Texture2D>("Images/ConnectFour/reset").Width,
                                         content.Load<Texture2D>("Images/ConnectFour/reset").Height);
            resetButton = new Button(new Rectangle((Constants.WINDOW_WIDTH - (btnResetSize.X + 20)), (Constants.WINDOW_HEIGHT - (btnResetSize.Y + 20)), btnResetSize.X, btnResetSize.Y),
                                                content.Load<Texture2D>("Images/ConnectFour/Reset"),
                                                content.Load<Texture2D>("Images/ConnectFour/ResetH"),
                                                content.Load<Texture2D>("Images/ConnectFour/ResetH"));
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {

            var mouseState = Mouse.GetState();
            chipPosition = new Vector2(mouseState.X, mouseState.Y);

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
            spriteBatch.DrawString(spriteFont, messageText, new Vector2(Constants.WINDOW_WIDTH / 2, 225), Color.Red, 0, spriteFont.MeasureString(messageText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, gameText, new Vector2(Constants.WINDOW_WIDTH / 2, 715), Color.Red, 0, spriteFont.MeasureString(gameText) / 2, 1.3f, SpriteEffects.None, 0.5f);

            var chipDrawPoisition = new Vector2(chipPosition.X - player1.chipTexture.Width / 2, chipPosition.Y - player1.chipTexture.Height / 2);
            if (player1Go)
            {
                spriteBatch.Draw(player1.chipTexture, new Vector2(chipDrawPoisition.X, chipDrawPoisition.Y), Color.White);
            }
            else
            {
                spriteBatch.Draw(player2.chipTexture, new Vector2(chipDrawPoisition.X, chipDrawPoisition.Y), Color.White);
            }
            
            backButton.Draw(spriteBatch);
            if (drawReset)
            {
                resetButton.Draw(spriteBatch);
            }

            spriteBatch.End();
        }
        void Reset()
        {
            player1Go = !player1Go;
                
        }
    }
}
