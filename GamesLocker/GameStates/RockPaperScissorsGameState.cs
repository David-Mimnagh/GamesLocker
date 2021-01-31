using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesLocker.Extensions;
using GamesLocker.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GamesLocker.GameStates
{
    public class RockPaperScissorsGameState : GameState
    {
        #region internalClasses
        public enum Choice
        {
            Rock = 1,
            Paper,
            Scissors
        }
        internal class Player
        {
            public Choice? choice;
        }
        #endregion

        Texture2D background;
        Texture2D logo;
        Dictionary<int, Texture2D> playerSelectedTextures;
        Dictionary<int, Texture2D> computerSelectedTextures;
        SpriteFont spriteFont;
        Button confirmSelectionButton;
        Button backButton;
        Button resetButton;
        Player computer;
        Player player;
        List<Button> Player1Buttons;
        List<Button> Player2Buttons;
        Texture2D[] CountDown;
        int choiceNumber;
        double totalTime;
        string messageText, gameText, selectionText, pointsText, winText, drawText, lossText;
        bool drawReset, gameWon, choosing, confirmSelection;
        public RockPaperScissorsGameState(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
            computer = new Player();
            Random rand = new Random();
            var choiceNum = rand.Next(1, 3);
            computer.choice = (Choice)choiceNum;
            playerSelectedTextures = new Dictionary<int, Texture2D>();
            computerSelectedTextures = new Dictionary<int, Texture2D>();
            player = new Player();
            player.choice = null;
            Player1Buttons = new List<Button>();
            Player2Buttons = new List<Button>();
            CountDown =new Texture2D[4];
            messageText = "Welcome to GameBox - Rock Paper Scissors.";
            gameText = "Make your selection then hit confirm to lock it in. - The AI has made up its mind.";
            winText = "Woohoo! Good choice, you won!";
            drawText = "Ouch! That's a draw.";
            lossText = "Well that's unfortunate. It looks like the AI won this one.";
            selectionText = "";
            drawReset = false;
            choosing = true;
            confirmSelection = false;
            totalTime = 180;
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Images/RockPaperScissors/RPSBackground");
            logo = content.Load<Texture2D>("Images/MainMenu/Logo");
            spriteFont = content.Load<SpriteFont>("Fonts/mavenPro");




            var btnOptionSize = new Point(content.Load<Texture2D>("Images/RockPaperScissors/P_Rock").Width,
                                            content.Load<Texture2D>("Images/RockPaperScissors/P_Rock").Height);

            Player1Buttons.Add(new Button(1, new RectangleX(new Point(Constants.WINDOW_WIDTH / 2 - 250, 350), btnOptionSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/P_Rock"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/PH_Rock"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/PH_Rock")));
            Player1Buttons.Add(new Button(2, new RectangleX(new Point(Constants.WINDOW_WIDTH / 2 - 250, 460), btnOptionSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/P_Paper"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/PH_Paper"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/PH_Paper")));
            Player1Buttons.Add(new Button(3, new RectangleX(new Point(Constants.WINDOW_WIDTH / 2 - 250, 570), btnOptionSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/P_Scissors"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/PH_Scissors"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/PH_Scissors")));

            Player2Buttons.Add(new Button(1, new RectangleX(new Point(Constants.WINDOW_WIDTH / 2 + 160, 350), btnOptionSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Rock"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Rock"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Rock")));
            Player2Buttons.Add(new Button(2, new RectangleX(new Point(Constants.WINDOW_WIDTH / 2 + 160, 460), btnOptionSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Paper"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Paper"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Paper")));
            Player2Buttons.Add(new Button(3, new RectangleX(new Point(Constants.WINDOW_WIDTH / 2 + 160, 570), btnOptionSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Scissors"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Scissors"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/C_Scissors")));


            for (int i = 1; i <= 3; i++)
            {
                CountDown[i] = content.Load<Texture2D>("Images/RockPaperScissors/"+i.ToString());
            }

            playerSelectedTextures = new Dictionary<int,Texture2D>();
            playerSelectedTextures.Add(1,content.Load<Texture2D>("Images/RockPaperScissors/PS_Rock"));
            playerSelectedTextures.Add(2,content.Load<Texture2D>("Images/RockPaperScissors/PS_Paper"));
            playerSelectedTextures.Add(3,content.Load<Texture2D>("Images/RockPaperScissors/PS_Scissors"));

           computerSelectedTextures = new Dictionary<int, Texture2D>();
           computerSelectedTextures.Add(1, content.Load<Texture2D>("Images/RockPaperScissors/CS_Rock"));
           computerSelectedTextures.Add(2, content.Load<Texture2D>("Images/RockPaperScissors/CS_Paper"));
           computerSelectedTextures.Add(3, content.Load<Texture2D>("Images/RockPaperScissors/CS_Scissors"));





            var btnConfirmSize = new Point(content.Load<Texture2D>("Images/RockPaperScissors/confirm").Width,
                                            content.Load<Texture2D>("Images/RockPaperScissors/confirm").Height);
            confirmSelectionButton = new Button(new RectangleX(new Point((Constants.WINDOW_WIDTH /2 - 60), 780), btnConfirmSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/confirm"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/confirmH"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/confirmH"));


            var btnBackSize = new Point(content.Load<Texture2D>("Images/RockPaperScissors/Back").Width,
                                            content.Load<Texture2D>("Images/RockPaperScissors/Back").Height);
            backButton = new Button(new MainMenuGameState(_graphicsDevice, base.Points), new RectangleX(new Point(10, (Constants.WINDOW_HEIGHT - (btnBackSize.Y + 20))), btnBackSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/Back"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/BackH"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/BackH"),
                                                "");

            var btnResetSize = new Point(content.Load<Texture2D>("Images/RockPaperScissors/reset").Width,
                                            content.Load<Texture2D>("Images/RockPaperScissors/reset").Height);
            resetButton = new Button(new RectangleX(new Point((Constants.WINDOW_WIDTH - (btnResetSize.X + 20)), (Constants.WINDOW_HEIGHT - (btnResetSize.Y + 20))), btnResetSize).ResultingRect,
                                                content.Load<Texture2D>("Images/RockPaperScissors/reset"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/resetH"),
                                                content.Load<Texture2D>("Images/RockPaperScissors/resetH"));
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (choosing)
            {
                foreach (var pButton in Player1Buttons)
                {
                    pButton.Update(mouseState);
                    if (pButton.ButtonState == Button.State.Hover)
                    {
                        selectionText = "Would you like to choose: " + Enum.GetName(typeof(Choice), pButton.ButtonId) + "?";
                    }
                    if (pButton.ButtonState == Button.State.Released)
                    {
                        player.choice = (Choice)pButton.ButtonId;
                    }

                }
                confirmSelectionButton.Update(mouseState);
                if(confirmSelectionButton.ButtonState == Button.State.Released)
                {
                    choosing = false;
                    confirmSelection = true;
                }
            }
            else
            {
                if (drawReset)
                {
                    /*ROCK > SCISSORS. PAPER > ROCK. SCISSORS > PAPER*/
                    switch (player.choice)
                    {
                        case Choice.Rock:
                            if(computer.choice == Choice.Rock)
                            {
                                selectionText = drawText;
                            }
                            else if (computer.choice == Choice.Paper)
                            {
                                selectionText = lossText;
                            }
                            else if (computer.choice == Choice.Scissors)
                            {
                                selectionText = winText;
                            }
                            break;
                        case Choice.Paper:
                            if (computer.choice == Choice.Rock)
                            {
                                selectionText = winText;
                            }
                            else if (computer.choice == Choice.Paper)
                            {
                                selectionText = drawText;
                            }
                            else if (computer.choice == Choice.Scissors)
                            {
                                selectionText = lossText;
                            }
                            break;
                        case Choice.Scissors:
                            if (computer.choice == Choice.Rock)
                            {
                                selectionText = lossText;
                            }
                            else if (computer.choice == Choice.Paper)
                            {
                                selectionText = winText;
                            }
                            else if (computer.choice == Choice.Scissors)
                            {
                                selectionText = drawText;
                            }
                            break;
                    }
                }
            }
            backButton.Update(mouseState);
            resetButton.Update(mouseState);
            if(resetButton.ButtonState == Button.State.Released)
            {
                Reset();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(logo, new Vector2(Constants.WINDOW_WIDTH / 2 - (logo.Width / 1.6f), 25), Color.White);
            spriteBatch.DrawString(spriteFont, messageText, new Vector2(Constants.WINDOW_WIDTH / 2, 225), Color.White, 0, spriteFont.MeasureString(messageText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, gameText, new Vector2(Constants.WINDOW_WIDTH / 2, 275), Color.White, 0, spriteFont.MeasureString(gameText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, selectionText, new Vector2(Constants.WINDOW_WIDTH / 2, 735), Color.White, 0, spriteFont.MeasureString(selectionText) / 2, 1.3f, SpriteEffects.None, 0.5f);

            if (confirmSelection)
            {
                if (totalTime > 0)
                {
                    totalTime -= 0.97;// / 60.0; //increase time
                    if (totalTime > 120)  // 3.
                        spriteBatch.Draw(CountDown[3], new Vector2(Constants.WINDOW_WIDTH / 2 - 40, 520), Color.White);
                    else if (totalTime > 60)  // 2.
                        spriteBatch.Draw(CountDown[2], new Vector2(Constants.WINDOW_WIDTH / 2 - 40, 520), Color.White);
                    else
                    {
                        spriteBatch.Draw(CountDown[1], new Vector2(Constants.WINDOW_WIDTH / 2 - 40, 520), Color.White);
                    }
                }
                else
                {
                    drawReset = true;
                }
            }

            foreach (var pButton in Player1Buttons)
            {
                pButton.Draw(spriteBatch);
            }
            if (player.choice != null)
            {
                switch (player.choice)
                {
                    case Choice.Rock:
                        spriteBatch.Draw(playerSelectedTextures[1], new Vector2(Constants.WINDOW_WIDTH / 2 - 250, 350), Color.White);
                        break;
                    case Choice.Paper:
                        spriteBatch.Draw(playerSelectedTextures[2], new Vector2(Constants.WINDOW_WIDTH / 2 - 250, 460), Color.White);
                        break;
                    case Choice.Scissors:
                        spriteBatch.Draw(playerSelectedTextures[3], new Vector2(Constants.WINDOW_WIDTH / 2 - 250, 570), Color.White);
                        break;
                }
            }
            foreach (var p2Button in Player2Buttons)
            {
                p2Button.Draw(spriteBatch);
            }

            if (drawReset)
            {
                switch (computer.choice)
                {
                    case Choice.Rock:
                        spriteBatch.Draw(computerSelectedTextures[1], new Vector2(Constants.WINDOW_WIDTH / 2 + 160, 350), Color.White);
                        break;
                    case Choice.Paper:
                        spriteBatch.Draw(computerSelectedTextures[2], new Vector2(Constants.WINDOW_WIDTH / 2 + 160, 460), Color.White);
                        break;
                    case Choice.Scissors:
                        spriteBatch.Draw(computerSelectedTextures[3], new Vector2(Constants.WINDOW_WIDTH / 2 + 160, 570), Color.White);
                        break;
                }
            }

            confirmSelectionButton.Draw(spriteBatch);
            backButton.Draw(spriteBatch);
            if (drawReset)
            {
                resetButton.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public void Reset()
        {
            Random rand = new Random();
            var choiceNum = rand.Next(1, 3);
            computer.choice = (Choice)choiceNum;
            player.choice = null;
            selectionText = "";
            drawReset = false;
            choosing = true;
            confirmSelection = false;
            totalTime = 180;
        }
    }
}
