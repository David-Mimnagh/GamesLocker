using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GamesLocker.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GamesLocker.GameStates
{
    public class GuessingGameState : GameState
    {
        Texture2D background;
        Texture2D logo;
        SpriteFont spriteFont;
        string messageText, gameText, livesText;
        int numberToGuess, GuessedNumber, numberOfGuesses, guessesLeft;
        bool canGuess, gameWon, drawReset;
        List<Button> gameButtons;
        Button backButton;
        Button resetButton;
        List<KeyValuePair<int, Vector2>> buttonPositions;

        public GuessingGameState(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {

        }

        #region StateMethods
        public override void Initialize()
        {
            gameButtons = new List<Button>();
            buttonPositions = new List<KeyValuePair<int, Vector2>>();
            buttonPositions = GetButtonPositions();
            Random rand = new Random();
            numberToGuess = rand.Next(1, 11);
            GuessedNumber = 0;
            numberOfGuesses = 0;
            guessesLeft = 5;
            canGuess = true;
            gameWon = false;
            messageText = "Welcome to GameBox - Guessing Game.";
            gameText = "";
            livesText = String.Format("You have {0} lives left.", guessesLeft.ToString()); 
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Images/GuessingGame/GGBackground");
            logo = content.Load<Texture2D>("Images/MainMenu/Logo");
            spriteFont = content.Load<SpriteFont>("Fonts/mavenPro");

            var btnSize = new Point(content.Load<Texture2D>("Images/GuessingGame/Guess_1").Width,
                                    content.Load<Texture2D>("Images/GuessingGame/Guess_1").Height);

            foreach (var bP in buttonPositions)
            {
                gameButtons.Add(new Button(bP.Key, new Rectangle((int)bP.Value.X, (int)bP.Value.Y, btnSize.X, btnSize.Y),
                                                content.Load<Texture2D>("Images/GuessingGame/Guess_"+bP.Key.ToString()),
                                                content.Load<Texture2D>("Images/GuessingGame/Guess_" + bP.Key.ToString() + "H"),
                                                content.Load<Texture2D>("Images/GuessingGame/Guess_" + bP.Key.ToString() + "H")));
            }
            var btnBackSize = new Point(content.Load<Texture2D>("Images/GuessingGame/Back").Width,
                                    content.Load<Texture2D>("Images/GuessingGame/Back").Height);
            backButton = new Button(new MainMenuGameState(_graphicsDevice, base.Points), new Rectangle(10, (Constants.WINDOW_HEIGHT - (btnBackSize.Y + 20)), btnBackSize.X, btnBackSize.Y),
                                                content.Load<Texture2D>("Images/GuessingGame/Back"),
                                                content.Load<Texture2D>("Images/GuessingGame/BackH"),
                                                content.Load<Texture2D>("Images/GuessingGame/BackH"),
                                                "");

            var btnResetSize = new Point(content.Load<Texture2D>("Images/GuessingGame/Reset").Width,
                        content.Load<Texture2D>("Images/GuessingGame/Reset").Height);
            resetButton = new Button(new Rectangle((Constants.WINDOW_WIDTH - (btnResetSize.X + 20)), (Constants.WINDOW_HEIGHT - (btnResetSize.Y + 20)), btnResetSize.X, btnResetSize.Y),
                                                content.Load<Texture2D>("Images/GuessingGame/Reset"),
                                                content.Load<Texture2D>("Images/GuessingGame/ResetH"),
                                                content.Load<Texture2D>("Images/GuessingGame/ResetH"));
        }
        public override void UnloadContent()
        {

        }
        public override void Update(GameTime gameTime)
        {
            if (numberOfGuesses > 0)
                drawReset = true;

            var mouseState = Mouse.GetState();
            if (!gameWon)
            {
                foreach (var btn in gameButtons)
                {
                    btn.Update(mouseState);
                    if (btn.ButtonState == Button.State.Released)
                    {
                        if (guessesLeft > 0)
                        {
                            Thread.Sleep(300);
                            HandleGuess(btn.ButtonId);
                            break;
                        }
                    }
                }
            }
            backButton.Update(mouseState);
            resetButton.Update(mouseState);
            if(resetButton.ButtonState == Button.State.Pressed)
            {
                ResetGame();
                resetButton.ButtonState = Button.State.None;
                drawReset = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(logo, new Vector2(Constants.WINDOW_WIDTH / 2 - (logo.Width / 1.6f), 25), Color.White);
            spriteBatch.DrawString(spriteFont, messageText, new Vector2(Constants.WINDOW_WIDTH / 2, 225), Color.Red, 0, spriteFont.MeasureString(messageText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, gameText, new Vector2(Constants.WINDOW_WIDTH / 2, 275), Color.Red, 0, spriteFont.MeasureString(gameText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, livesText, new Vector2(Constants.WINDOW_WIDTH / 2, 685), Color.Red, 0, spriteFont.MeasureString(livesText) / 2, 1.3f, SpriteEffects.None, 0.5f);

            foreach (var btn in gameButtons)
            {
                btn.Draw(spriteBatch);
            }
            backButton.Draw(spriteBatch);
            if (drawReset)
            {
                resetButton.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        #endregion

        #region GameSpecificMethods
        void ResetGame()
        {
            Random rand = new Random();
            numberToGuess = rand.Next(1, 11);
            GuessedNumber = 0;
            numberOfGuesses = 0;
            guessesLeft = 5;
            canGuess = true;
            gameWon = false;
            messageText = "Welcome to GameBox - Guessing Game.";
            gameText = "";
            livesText = String.Format("You have {0} lives left.", guessesLeft.ToString());
        }
        void HandleGuess(int buttonId)
        {
            guessesLeft--;
            livesText = String.Format("You have {0} guesses left.", guessesLeft);
            numberOfGuesses++;

            if (buttonId == numberToGuess)
            {
                if (numberOfGuesses == 1)
                    gameText = "No Way!\nYou got it in 1 try. That's Impressive.";
                else
                    gameText = String.Format("No Way!\nYou got it in {0} tries. Good Job.",numberOfGuesses);

                canGuess = false;
                gameWon = true;
                base.Points += guessesLeft + 1;
                drawReset = true;
            }
            else
            {
                string helpText = (buttonId < numberToGuess) ? "higher" : "lower";
                gameText = String.Format("Incorrect. Maybe try a {0} guess!", helpText);
            }
            canGuess = true;
        }
        public List<KeyValuePair<int, Vector2>> GetButtonPositions()
        {
            var allButtonPositions = new List<KeyValuePair<int, Vector2>>();
            int startX = 540;
            int startY = 320;
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = new Vector2();
                if (i < 3)
                {
                    pos.X = startX + (i * 80);
                    pos.Y = startY;
                }
                else if (i > 2 && i < 6)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        pos.X = startX + ((i - 3) * 80);
                        pos.Y = startY + 80;
                        break;
                    }
                }
                else if (i > 5 && i < 9)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        pos.X = startX + ((i - 6) * 80);
                        pos.Y = startY + 160;
                        break;
                    }
                }
                else
                {
                    pos.X = startX + 80;
                    pos.Y = startY + 240;
                }

                var currentButtonPosition = new KeyValuePair<int, Vector2>(i+1,pos);
                allButtonPositions.Add(currentButtonPosition);
            }
            return allButtonPositions;
        }
        #endregion
    }
}
