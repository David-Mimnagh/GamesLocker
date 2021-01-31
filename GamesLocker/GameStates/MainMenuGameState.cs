using GamesLocker.Extensions;
using GamesLocker.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GamesLocker.GameStates
{
    public class MainMenuGameState : GameState
    {

        Texture2D background;
        Texture2D logo;
        List<Button> buttons;
        Button btnGuessingGame;
        SpriteFont spriteFont;
        string welcomeText;
        string introLineOne;
        string introLineTwo;
        string introLineThree;
        string gameDescriptionText;
        string pointsText;
        GraphicsDevice _graphicsDevice;
        public MainMenuGameState(GraphicsDevice graphicsDevice, int currentPoints = 0)
        : base(graphicsDevice)
        {
            base.Points = (currentPoints > 0) ? currentPoints : 0;
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize()
        {

            welcomeText = "Welcome to GamesLocker!";
            introLineOne = "Within GamesLocker there are a number of different games...";
            introLineTwo = "For each win within a game a point will be gained.";
            introLineThree = "Click an option below to launch the game.";
            gameDescriptionText = "---";
            pointsText = "Your points: "+ base.Points.ToString();
            buttons = new List<Button>();
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Images/MainMenu/Background");
            logo = content.Load<Texture2D>("Images/MainMenu/Logo");
            spriteFont = content.Load<SpriteFont>("Fonts/mavenPro");

            var btnSize = new Point(content.Load<Texture2D>("Images/MainMenu/Buttons/button_guessing-game").Width, 
                                    content.Load<Texture2D>("Images/MainMenu/Buttons/button_guessing-game").Height);

            
            buttons.Add( new Button(new GuessingGameState(_graphicsDevice), new RectangleX(new Point(100, 500), btnSize).ResultingRect,
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_guessing-game"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_guessing-game_hover"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_guessing-game_hover"),
                                            "Try to guess the random number\nthat was selected by the computer."));
            buttons.Add(new Button(new HangmanGameState(_graphicsDevice), new RectangleX(new Point(400, 500), btnSize).ResultingRect,
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_hangman"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_hangman_hover"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_hangman_hover"),
                                            "Try to beat the computer in\na classic game of Hangman."));
            buttons.Add(new Button(new RockPaperScissorsGameState(_graphicsDevice), new RectangleX(new Point(665, 500), btnSize).ResultingRect,
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_rock-paper-scissors"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_rock-paper-scissors_hover"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_rock-paper-scissors_hover"),
                                            "Try to beat the computer in\na classic game of Rock-Paper-Scissors."));
            buttons.Add(new Button(new TicTacToeGameState(_graphicsDevice), new RectangleX(new Point(1010, 500), btnSize).ResultingRect,
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_tic-tac-toe"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_tic-tac-toe_hover"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_tic-tac-toe_hover"),
                                            "Go head to head with the computer in\na classic game of Tic-Tac-Toe."));
            buttons.Add(new Button(new BreakoutGameState(_graphicsDevice), new RectangleX(new Point(100, 700), btnSize).ResultingRect,
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_breakout"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_breakout_hover"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_breakout_hover"),
                                            "Go head to head with the computer in\na classic game of Breakout."));
            buttons.Add(new Button(new ConnectFourGameState(_graphicsDevice), new RectangleX(new Point(400, 700), btnSize).ResultingRect,
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_connect-four"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_connect-four_hover"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_connect-four_hover"),
                                            "Go head to head with your friend\nin a classic game of Connect Four."));
            buttons.Add(new Button(new MainMenuGameState(_graphicsDevice), new RectangleX(new Point(665, 700), btnSize).ResultingRect,
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_flappy-burd"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_flappy-burd_hover"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_flappy-burd_hover"),
                                            "Go against the pipes trying to slip through\nin a classic game of Flappy 'Burd'."));
            buttons.Add(new Button(new MainMenuGameState(_graphicsDevice), new RectangleX(new Point(1010, 700), btnSize).ResultingRect,
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_dungeon-escape"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_dungeon-escape_hover"),
                                            content.Load<Texture2D>("Images/MainMenu/Buttons/button_dungeon-escape_hover"),
                                            "Take control of the trapped bot\nand try to solve the puzzles to escape the dungeons."));
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            foreach (var btn in buttons)
            {
                btn.Update(mouseState);
                if(btn.ButtonState == Button.State.Hover)
                {
                    gameDescriptionText = btn.GameDescription;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //_graphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(logo, new Vector2(Constants.WINDOW_WIDTH / 2 - (logo.Width / 1.6f), 25 ), Color.White);

            spriteBatch.DrawString(spriteFont, welcomeText, new Vector2(Constants.WINDOW_WIDTH / 2, 200), Color.Cyan, 0, spriteFont.MeasureString(welcomeText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, introLineOne, new Vector2(Constants.WINDOW_WIDTH / 2, 300), Color.Cyan, 0, spriteFont.MeasureString(introLineOne) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, introLineTwo, new Vector2(Constants.WINDOW_WIDTH / 2, 325), Color.Cyan, 0, spriteFont.MeasureString(introLineTwo) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, introLineThree, new Vector2(Constants.WINDOW_WIDTH / 2, 450), Color.Cyan, 0, spriteFont.MeasureString(introLineThree) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, gameDescriptionText, new Vector2(Constants.WINDOW_WIDTH / 2, 600), Color.Cyan, 0, spriteFont.MeasureString(gameDescriptionText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, pointsText, new Vector2(Constants.WINDOW_WIDTH / 2, 380), Color.Cyan, 0, spriteFont.MeasureString(pointsText) / 2, 1.3f, SpriteEffects.None, 0.5f);

            foreach (var btn in buttons)
            {
                btn.Draw(spriteBatch);
            }

            // Draw sprites here
            spriteBatch.End();
        }
    }
}
