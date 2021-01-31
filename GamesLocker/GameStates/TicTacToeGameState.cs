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
    public class TicTacToeGameState : GameState
    {
        Texture2D background;
        Texture2D logo;
        Texture2D gameBoard;
        Texture2D[,] xMarkers;
        Texture2D[,] oMarkers;
        Texture2D[] RowWinTextures;
        Texture2D[] ColumnWinTextures;
        Texture2D[] DiagWinTextures;
        Texture2D player1sChip;
        Texture2D player2sChip;
        List<Button> boardPositions;
        SpriteFont spriteFont;
        Button backButton;
        Button resetButton;
        string messageText;
        string gameText, infoText, wayGameWon;
        bool drawReset, player1Go, gameWon;
        int[,] board;
        bool[] stillGo;
        List<int> squaresTaken;
        int spacesLeft;
        public TicTacToeGameState(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
            messageText = "Welcome to GameBox - Tic Tac Toe.";
            gameText = "Click to make your selection.";
            infoText = "Player 1, it's your turn!";
            drawReset = true;
            player1Go = true;
            stillGo = new bool[9];
            board = new int[3,3];
            xMarkers = new Texture2D[3, 3];
            oMarkers = new Texture2D[3, 3];
            RowWinTextures =    new Texture2D[3];
            ColumnWinTextures = new Texture2D[3];
            DiagWinTextures =   new Texture2D[2];
            squaresTaken = new List<int>();
            spacesLeft = 9;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i,j] = 0;
                }
            }
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Images/TicTacToe/TTTBackground");
            logo = content.Load<Texture2D>("Images/MainMenu/Logo");
            spriteFont = content.Load<SpriteFont>("Fonts/mavenPro");
            gameBoard = content.Load<Texture2D>("Images/TicTacToe/board");
            player1sChip = content.Load<Texture2D>("Images/TicTacToe/Player1_ChipInfo");
            player2sChip = content.Load<Texture2D>("Images/TicTacToe/Player2_ChipInfo");

            for (int i = 0; i < RowWinTextures.Length; i++)
            {
                RowWinTextures[i] = content.Load<Texture2D>("Images/TicTacToe/R"+(i+1).ToString());
            }

            for (int i = 0; i < RowWinTextures.Length; i++)
            {
                ColumnWinTextures[i] = content.Load<Texture2D>("Images/TicTacToe/C" + (i + 1).ToString());
            }

            DiagWinTextures[0] = content.Load<Texture2D>("Images/TicTacToe/D-R");
            DiagWinTextures[1] = content.Load<Texture2D>("Images/TicTacToe/D-L");

            var btnBoardMarkerSize = new Point(content.Load<Texture2D>("Images/TicTacToe/box").Width,
                                        content.Load<Texture2D>("Images/TicTacToe/box").Height);
            var btnId = 0;
            boardPositions = new List<Button>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var boardXPos = (j == 0) ? 657 : (j == 1) ? 770 : 887;
                    boardXPos -= (gameBoard.Width / 2);
                    var boardYPos = (i == 0) ? 488  : (i == 1) ? 611 : 726;
                    boardYPos -= (gameBoard.Height / 2);
                    boardPositions.Add(new Button(btnId, new RectangleX(new Point(boardXPos, boardYPos), btnBoardMarkerSize).ResultingRect,
                                                content.Load<Texture2D>("Images/TicTacToe/box"),
                                                content.Load<Texture2D>("Images/TicTacToe/boxH"),
                                                content.Load<Texture2D>("Images/TicTacToe/boxH")));

                    xMarkers[i, j] = content.Load<Texture2D>("Images/TicTacToe/cChip");
                    oMarkers[i, j] = content.Load<Texture2D>("Images/TicTacToe/pChip");

                    btnId++;
                }
            }

            var btnBackSize = new Point(content.Load<Texture2D>("Images/TicTacToe/Back").Width,
                                        content.Load<Texture2D>("Images/TicTacToe/Back").Height);
            backButton = new Button(new MainMenuGameState(_graphicsDevice, base.Points), new RectangleX(new Point(10, (Constants.WINDOW_HEIGHT - (btnBackSize.Y + 20))), btnBackSize).ResultingRect,
                                                content.Load<Texture2D>("Images/TicTacToe/Back"),
                                                content.Load<Texture2D>("Images/TicTacToe/BackH"),
                                                content.Load<Texture2D>("Images/TicTacToe/BackH"),
                                                "");

            var btnResetSize = new Point(content.Load<Texture2D>("Images/TicTacToe/reset").Width,
                                         content.Load<Texture2D>("Images/TicTacToe/reset").Height);
            resetButton = new Button(new RectangleX(new Point((Constants.WINDOW_WIDTH - (btnResetSize.X + 20)), (Constants.WINDOW_HEIGHT - (btnResetSize.Y + 20))), btnResetSize).ResultingRect,
                                                content.Load<Texture2D>("Images/TicTacToe/Reset"),
                                                content.Load<Texture2D>("Images/TicTacToe/ResetH"),
                                                content.Load<Texture2D>("Images/TicTacToe/ResetH"));
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {

            infoText = (player1Go) ? "Player 1, it's your turn!" : "Player 2, it's your turn!";


            var mouseState = Mouse.GetState();
            if (stillGo.ToList().Where(i => i == false).Count() < 9 && stillGo.ToList().Where(i => i == false).Count() > 0)
            {
                CheckWinPlayer();
                CheckWinPlayer2();
            }
            if (!gameWon)
            {
                foreach (var btnBoardPos in boardPositions)
                {
                    btnBoardPos.Update(mouseState);
                    if (!squaresTaken.Contains(btnBoardPos.ButtonId))
                    {
                        if (btnBoardPos.ButtonState == Button.State.Released)
                        {
                            squaresTaken.Add(btnBoardPos.ButtonId);

                            var pos = GetPositionFromId(btnBoardPos.ButtonId);
                            board[(int)pos.X, (int)pos.Y] = (player1Go) ? 1 : 2;
                            player1Go = !player1Go;
                            break;
                        }
                        CheckBoardWin();
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
            var boardX = ((Constants.WINDOW_WIDTH / 2) - gameBoard.Width / 2);
            var boardY = ((Constants.WINDOW_HEIGHT / 2) - gameBoard.Height / 2);
            spriteBatch.Draw(gameBoard, new Vector2(boardX, boardY), Color.White);
            spriteBatch.DrawString(spriteFont, messageText, new Vector2(Constants.WINDOW_WIDTH / 2, 225), Color.Red, 0, spriteFont.MeasureString(messageText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, infoText, new Vector2(Constants.WINDOW_WIDTH / 2, 275), Color.Red, 0, spriteFont.MeasureString(infoText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, gameText, new Vector2(Constants.WINDOW_WIDTH / 2, 715), Color.Red, 0, spriteFont.MeasureString(gameText) / 2, 1.3f, SpriteEffects.None, 0.5f);

            spriteBatch.Draw(player1sChip, new Vector2(140, 450), Color.White);
            spriteBatch.Draw(player2sChip, new Vector2(Constants.WINDOW_WIDTH - 140, 450), Color.White);

            foreach (var btnBoardPos in boardPositions)
            {
                if(!squaresTaken.Contains(btnBoardPos.ButtonId))
                    btnBoardPos.Draw(spriteBatch);
                else
                {
                    var pos = GetPositionFromId(btnBoardPos.ButtonId);
                    if (board[(int)pos.X, (int)pos.Y] == 1)
                    {
                        spriteBatch.Draw(xMarkers[(int)pos.X, (int)pos.Y], new Vector2(btnBoardPos._rectangle.X, btnBoardPos._rectangle.Y), Color.White);
                    }
                    else if (board[(int)pos.X, (int)pos.Y] == 2)
                    {
                        spriteBatch.Draw(oMarkers[(int)pos.X, (int)pos.Y], new Vector2(btnBoardPos._rectangle.X, btnBoardPos._rectangle.Y), Color.White);
                    }
                }
            }

            if (gameWon)
            {
                if (wayGameWon.Contains("ROW"))
                {

                    if (wayGameWon.Contains("1"))
                    {

                        spriteBatch.Draw(RowWinTextures[0], new Vector2(boardX, boardY - 30), Color.White);
                    }
                    else if (wayGameWon.Contains("2"))
                    {
                        spriteBatch.Draw(RowWinTextures[1], new Vector2(boardX, boardY), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(RowWinTextures[2], new Vector2(boardX, boardY + 30), Color.White);
                    }
                }
                else if (wayGameWon.Contains("COLUMN"))
                {
                    if (wayGameWon.Contains("1"))
                    {
                        spriteBatch.Draw(ColumnWinTextures[0], new Vector2(boardX - 30, boardY), Color.White);
                    }
                    else if (wayGameWon.Contains("2"))
                    {
                        spriteBatch.Draw(ColumnWinTextures[1], new Vector2(boardX, boardY), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(ColumnWinTextures[2], new Vector2(boardX + 30, boardY), Color.White);
                    }
                }
                else if (wayGameWon.Contains("Diag"))
                {
                    if (wayGameWon.Contains("left"))
                    {
                        spriteBatch.Draw(DiagWinTextures[1], new Vector2(boardX, boardY), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(DiagWinTextures[0], new Vector2(boardX, boardY), Color.White);
                    }
                }
            }
            backButton.Draw(spriteBatch);
            if (drawReset)
            {
                resetButton.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public void Reset()
        {
            messageText = "Welcome to GameBox - Tic Tac Toe.";
            gameText = "Click to make your selection.";
            wayGameWon = "";
            drawReset = true;
            player1Go = true;
            stillGo = new bool[9];
            board = new int[3, 3];
            gameWon = false;
            squaresTaken = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = 0;
                }
            }
        }

        public Vector2 GetPositionFromId(int btnId)
        {
            var xPosToMark = 0;
            var yPosToMark = 0;
            var currentIdx = 0;
            switch(btnId)
            {
                case 0:
                    xPosToMark = 0;
                    yPosToMark = 0;
                    break;
                case 1:
                    xPosToMark = 0;
                    yPosToMark = 1;
                    break;
                case 2:
                    xPosToMark = 0;
                    yPosToMark = 2;
                    break;
                case 3:
                    xPosToMark = 1;
                    yPosToMark = 0;
                    break;
                case 4:
                    xPosToMark = 1;
                    yPosToMark = 1;
                    break;
                case 5:
                    xPosToMark = 1;
                    yPosToMark = 2;
                    break;
                case 6:
                    xPosToMark = 2;
                    yPosToMark = 0;
                    break;
                case 7:
                    xPosToMark = 2;
                    yPosToMark = 1;
                    break;
                case 8:
                    xPosToMark = 2;
                    yPosToMark = 2;
                    break;
            }

            return new Vector2(xPosToMark, yPosToMark);
        }

        void CheckBoardWin()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i,j] == 0)
                    {
                        if (i == 0)
                        {
                            if (j == 0)
                                stillGo[0] = true;
                            if (j == 1)
                                stillGo[1] = true;
                            if (j == 2)
                                stillGo[2] = true;
                        }
                        if (i == 1)
                        {
                            if (j == 0)
                                stillGo[3] = true;
                            if (j == 1)
                                stillGo[4] = true;
                            if (j == 2)
                                stillGo[5] = true;
                        }
                        if (i == 2)
                        {
                            if (j == 0)
                                stillGo[6] = true;
                            if (j == 1)
                                stillGo[7] = true;
                            if (j == 2)
                                stillGo[8] = true;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            if (j == 0)
                                stillGo[0] = false;
                            if (j == 1)
                                stillGo[1] = false;
                            if (j == 2)
                                stillGo[2] = false;
                        }
                        if (i == 1)
                        {
                            if (j == 0)
                                stillGo[3] = false;
                            if (j == 1)
                                stillGo[4] = false;
                            if (j == 2)
                                stillGo[5] = false;
                        }
                        if (i == 2)
                        {
                            if (j == 0)
                                stillGo[6] = false;
                            if (j == 1)
                                stillGo[7] = false;
                            if (j == 2)
                                stillGo[8] = false;
                        }
                    }
                }
            }
        }

        void CheckWinPlayer()
        {
            if (board[0,0] == 1)
            {
                if (board[0,1] == 1)
                {
                    if (board[0,2] == 1)
                    {
                        gameText = "Player 1 WINS!";
                        wayGameWon = "ROW 1";
                        gameWon = true;
                    }
                }
            }
            if (board[1,0] == 1)
            {
                if (board[1,1] == 1)
                {
                    if (board[1,2] == 1)
                    {
                       gameText = "Player 1 WINS!";
                        wayGameWon = "ROW 2";
                        gameWon = true;
                    }
                }
            }
            if (board[2,0] == 1)
            {
                if (board[2,1] == 1)
                {
                    if (board[2,2] == 1)
                    {
                       gameText = "Player 1 WINS!";
                        wayGameWon = "ROW 3";
                        gameWon = true;
                    }
                }
            }
            if (board[0,0] == 1)
            {
                if (board[1,0] == 1)
                {
                    if (board[2,0] == 1)
                    {
                       gameText = "Player 1 WINS!";
                        wayGameWon = "COLUMN 1";
                        gameWon = true;
                    }
                }
            }
            if (board[0,1] == 1)
            {
                if (board[1,1] == 1)
                {
                    if (board[2,1] == 1)
                    {
                       gameText = "Player 1 WINS!";
                        wayGameWon = "COLUMN 2";
                        gameWon = true;
                    }
                }
            }
            if (board[0,2] == 1)
            {
                if (board[1,2] == 1)
                {
                    if (board[2,2] == 1)
                    {
                       gameText = "Player 1 WINS!";
                        wayGameWon = "COLUMN 3";
                        gameWon = true;
                    }
                }
            }
            if (board[0,0] == 1)
            {
                if (board[1,1] == 1)
                {
                    if (board[2,2] == 1)
                    {
                       gameText = "Player 1 WINS!";
                        wayGameWon = "Diag, Down right";
                        gameWon = true;
                    }
                }
            }
            if (board[2,0] == 1)
            {
                if (board[1,1] == 1)
                {

                    if (board[0,2] == 1)
                    {
                       gameText = "Player 1 WINS!";
                        wayGameWon = "Diag, Down left";
                        gameWon = true;
                    }
                }
            }
            if (gameWon)
            {
                infoText = "";
                drawReset = true;
            }
        }
        void CheckWinPlayer2()
        {
            if (board[0, 0] == 2)
            {
                if (board[0, 1] == 2)
                {
                    if (board[0, 2] == 2)
                    {
                        gameText = "Player 2 WINS!";
                        wayGameWon = "ROW 1";
                        gameWon = true;
                    }
                }
            }
            if (board[1, 0] == 2)
            {
                if (board[1, 1] == 2)
                {
                    if (board[1, 2] == 2)
                    {
                        gameText = "Player 2 WINS!";
                        wayGameWon = "ROW 2";
                        gameWon = true;
                    }
                }
            }
            if (board[2, 0] == 2)
            {
                if (board[2, 1] == 2)
                {
                    if (board[2, 2] == 2)
                    {
                        gameText = "Player 2 WINS!";
                        wayGameWon = "ROW 3";
                        gameWon = true;
                    }
                }
            }
            if (board[0, 0] == 2)
            {
                if (board[1, 0] == 2)
                {
                    if (board[2, 0] == 2)
                    {
                        gameText = "Player 2 WINS!";
                        wayGameWon = "COLUMN 1";
                        gameWon = true;
                    }
                }
            }
            if (board[0, 1] == 2)
            {
                if (board[1, 1] == 2)
                {
                    if (board[2, 1] == 2)
                    {
                        gameText = "Player 2 WINS!";
                        wayGameWon = "COLUMN 2";
                        gameWon = true;
                    }
                }
            }
            if (board[0, 2] == 2)
            {
                if (board[1, 2] == 2)
                {
                    if (board[2, 2] == 2)
                    {
                        gameText = "Player 2 WINS!";
                        wayGameWon = "COLUMN 3";
                        gameWon = true;
                    }
                }
            }
            if (board[0, 0] == 2)
            {
                if (board[1, 1] == 2)
                {
                    if (board[2, 2] == 2)
                    {
                        gameText = "Player 2 WINS!";
                        wayGameWon = "Diag, Down right";
                        gameWon = true;
                    }
                }
            }
            if (board[2, 0] == 2)
            {
                if (board[1, 1] == 2)
                {

                    if (board[0, 2] == 2)
                    {
                        gameText = "Player 2 WINS!";
                        wayGameWon = "Diag, Down left";
                        gameWon = true;
                    }
                }
            }
            if (gameWon)
            {
                infoText = "";
                drawReset = true;
            }
        }
    }
}
