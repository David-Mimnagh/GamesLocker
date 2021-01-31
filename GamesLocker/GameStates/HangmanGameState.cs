using GamesLocker.Extensions;
using GamesLocker.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLocker.GameStates
{
    public class HangmanGameState : GameState
    {
        #region internalClasses
        internal class Player
        {
            public int TryCount { get; set; }
            public int NumberOfLives { get; set; }
            public string Guess { get; set; }
        }

        internal class Category
        {
            public string Type { get; set; }
            public List<string> Titles { get; set; }
        }

        #endregion

        Texture2D background;
        Texture2D logo;
        Dictionary<int, Texture2D> hangmanPicture;
        SpriteFont spriteFont;
        Dictionary<int, char> alphabet;
        string messageText, gameText, livesText, categoryChoice;
        int categoryID;
        List<Category> categories;
        Player player;
        string word, wordToGuess, wordToGuessCryptic, currentProgressWord;
        int randomWordID;
        bool choosingCategory, drawReset, gameWon;
        List<char> guessedLetters;
        List<Button> CategoryButtons;
        List<Button> AlphabetButtons;
        Button backButton;
        Button resetButton;

        public HangmanGameState(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {

        }

        public override void Initialize()
        {
            messageText = "Welcome to GameBox - Hangman.";
            gameText = "Playing this game is quite simple.\nYou start by selecting a catagory on the left,\na word will then be randomly generated from that catagory.\nYou then take guesses, one letter at a time to complete the word.\nAre you ready?\n\nSelect a catagory to begin.";
            livesText = "";
            wordToGuess = "";
            wordToGuessCryptic = "";
            choosingCategory = true;
            drawReset = false;
            hangmanPicture = new Dictionary<int, Texture2D>();
            alphabet = new Dictionary<int, char>();
            alphabet = PrepAlphabet();
            player = new Player();
            player.NumberOfLives = 5;
            guessedLetters = new List<char>();
            CategoryButtons = new List<Button>();
            AlphabetButtons = new List<Button>();
            categories = new List<Category>();

        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Images/Hangman/HMBackground");
            logo = content.Load<Texture2D>("Images/MainMenu/Logo");
            spriteFont = content.Load<SpriteFont>("Fonts/mavenPro");

            var btnCategorySize = new Point(content.Load<Texture2D>("Images/Hangman/CelebCat").Width,
                                            content.Load<Texture2D>("Images/Hangman/CelebCat").Height);
            int categoryXPos = (int)(btnCategorySize.X * 2.5f);
            CategoryButtons.Add(new Button(100, new Rectangle(categoryXPos, 290, btnCategorySize.X, btnCategorySize.Y),
                                                content.Load<Texture2D>("Images/Hangman/TVCat"),
                                                content.Load<Texture2D>("Images/Hangman/TVCatH"),
                                                content.Load<Texture2D>("Images/Hangman/TVCatH")));
            CategoryButtons.Add(new Button(200, new Rectangle(categoryXPos, 360, btnCategorySize.X, btnCategorySize.Y),
                                                content.Load<Texture2D>("Images/Hangman/FilmCat"),
                                                content.Load<Texture2D>("Images/Hangman/FilmCatH"),
                                                content.Load<Texture2D>("Images/Hangman/FilmCatH")));
            CategoryButtons.Add(new Button(300, new Rectangle(categoryXPos, 430, btnCategorySize.X, btnCategorySize.Y),
                                                content.Load<Texture2D>("Images/Hangman/CelebCat"),
                                                content.Load<Texture2D>("Images/Hangman/CelebCatH"),
                                                content.Load<Texture2D>("Images/Hangman/CelebCatH")));

            var btnBackSize = new Point(content.Load<Texture2D>("Images/Hangman/Back").Width,
                                            content.Load<Texture2D>("Images/Hangman/Back").Height);
            backButton = new Button(new MainMenuGameState(_graphicsDevice, base.Points), new RectangleX(new Point(10, (Constants.WINDOW_HEIGHT - (btnBackSize.Y + 20))), btnBackSize).ResultingRect,
                                                content.Load<Texture2D>("Images/Hangman/Back"),
                                                content.Load<Texture2D>("Images/Hangman/BackH"),
                                                content.Load<Texture2D>("Images/Hangman/BackH"),
                                                "");

            var btnResetSize = new Point(content.Load<Texture2D>("Images/Hangman/Reset").Width,
                                            content.Load<Texture2D>("Images/Hangman/Reset").Height);
            resetButton = new Button(new RectangleX(new Point((Constants.WINDOW_WIDTH - (btnResetSize.X + 20)), (Constants.WINDOW_HEIGHT - (btnResetSize.Y + 20))), btnResetSize).ResultingRect,
                                                content.Load<Texture2D>("Images/Hangman/Reset"),
                                                content.Load<Texture2D>("Images/Hangman/ResetH"),
                                                content.Load<Texture2D>("Images/Hangman/ResetH"));

            hangmanPicture.Add(5, content.Load<Texture2D>("Images/Hangman/Stand-noose"));
            hangmanPicture.Add(4, content.Load<Texture2D>("Images/Hangman/Stand-HB"));
            hangmanPicture.Add(3, content.Load<Texture2D>("Images/Hangman/Stand-HBA1"));
            hangmanPicture.Add(2, content.Load<Texture2D>("Images/Hangman/Stand-HBA2"));
            hangmanPicture.Add(1, content.Load<Texture2D>("Images/Hangman/Stand-HBAL1"));
            hangmanPicture.Add(0, content.Load<Texture2D>("Images/Hangman/Stand-HBAL2"));

            var alphabetLength = 26;
            var btnAlphabetSize = new Point(content.Load<Texture2D>("Images/Hangman/1").Width,
                                            content.Load<Texture2D>("Images/Hangman/1").Height);

            var startXPos = Constants.WINDOW_WIDTH / 2 + 20;
            for (int i = 0; i < alphabetLength; i++)
            {
                var pos = new Point();
                var rowIndex = 0;
                if (i <= 4)
                {
                    rowIndex = i;
                    pos.Y = 250;
                }
                else if (i >= 5 && i <= 9)
                {
                    rowIndex = (i - 5);
                    pos.Y = 320;
                }
                else if (i >= 10 && i <= 14)
                {
                    rowIndex = (i - 10);
                    pos.Y = 390;
                }
                else if (i >= 15 && i <= 19)
                {
                    rowIndex = (i - 15);
                    pos.Y = 460;
                }
                else if (i >= 20 && i <= 24)
                {
                    rowIndex = (i - 20);
                    pos.Y = 530;
                }
                else if (i >= 25)
                {
                    rowIndex = (i - 25);
                    pos.Y = 600;
                }

                
                var newPos = (rowIndex * (btnAlphabetSize.X + 45));
                pos.X = (startXPos + newPos);
                  

                AlphabetButtons.Add(new Button(i,new RectangleX(pos, btnResetSize).ResultingRect,
                                                content.Load<Texture2D>("Images/Hangman/" + i.ToString()),
                                                content.Load<Texture2D>("Images/Hangman/" + i.ToString() + "H"),
                                                content.Load<Texture2D>("Images/Hangman/" + i.ToString() + "H")));
            }
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (!gameWon)
            {
                if (choosingCategory)
                {
                    foreach (var catBtn in CategoryButtons)
                    {
                        catBtn.Update(mouseState);
                        if (catBtn.ButtonState == Button.State.Released)
                        {
                            choosingCategory = false;
                            wordToGuess = GetRandomWordFromCategory(catBtn.ButtonId);
                            wordToGuessCryptic = GetCrypticVersionOfWord(wordToGuess);
                        }
                    }
                }
                else
                {
                    foreach (var alphButtons in AlphabetButtons)
                    {
                        alphButtons.Update(mouseState);
                        if (alphButtons.ButtonState == Button.State.Released)
                        {
                            var guessedChar = GetCharFromId(alphButtons.ButtonId);
                            if (!guessedLetters.Contains(guessedChar))
                            {
                                drawReset = true;
                                guessedLetters.Add(guessedChar);

                                var wordToGuessCrypticBefore = wordToGuessCryptic;
                                wordToGuessCryptic = GetCrypticVersionOfWord(wordToGuess);

                                if (!String.IsNullOrEmpty(wordToGuessCryptic) && wordToGuessCryptic == wordToGuessCrypticBefore)
                                    player.NumberOfLives--;

                                if (player.NumberOfLives > 1)
                                {
                                    livesText = String.Format("You have {0} lives remaining", player.NumberOfLives);
                                }
                                else if (player.NumberOfLives == 1)
                                {
                                    livesText = "You have 1 life remaining";
                                }
                                else
                                {
                                    livesText = String.Format("You lost! Unfortunately you weren't able to guess to '{0}' in the given lives.",wordToGuess);
                                }

                                if (wordToGuessCryptic.Trim().Replace(" ", "") == wordToGuess.Trim().Replace(" ", ""))
                                {
                                    gameWon = true;
                                    livesText = "Congratulations! You have correctly guessed the random phrase.";
                                }
                            }
                        }
                    }
                }

            }

            backButton.Update(mouseState);

            if (drawReset)
            {
                resetButton.Update(mouseState);
                if (resetButton.ButtonState == Button.State.Released)
                {
                    Reset();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(logo, new Vector2(Constants.WINDOW_WIDTH / 2 - (logo.Width / 1.6f), 25), Color.White);


            spriteBatch.DrawString(spriteFont, messageText, new Vector2(Constants.WINDOW_WIDTH / 2, 225), Color.Blue, 0, spriteFont.MeasureString(messageText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, livesText, new Vector2(Constants.WINDOW_WIDTH / 2, 715), Color.Blue, 0, spriteFont.MeasureString(livesText) / 2, 1.3f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(spriteFont, wordToGuessCryptic, new Vector2(Constants.WINDOW_WIDTH / 2, 750), Color.Blue, 0, spriteFont.MeasureString(wordToGuessCryptic) / 2, 1.3f, SpriteEffects.None, 0.5f);

            if (choosingCategory)
            {

                spriteBatch.DrawString(spriteFont, gameText, new Vector2(Constants.WINDOW_WIDTH - 350, 375), Color.Blue, 0, spriteFont.MeasureString(gameText) / 2, 1.3f, SpriteEffects.None, 0.5f);
                foreach (var catBtn in CategoryButtons)
                {
                    catBtn.Draw(spriteBatch);
                }
            }
            else
            {
                var currentHangmanPicture = hangmanPicture.Where(hP => hP.Key == player.NumberOfLives).First().Value;
                spriteBatch.Draw(currentHangmanPicture, new Vector2(Constants.WINDOW_WIDTH / 2 - 465, 250), Color.White);
                foreach (var alphButtons in AlphabetButtons)
                {
                    alphButtons.Draw(spriteBatch);
                }
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
            messageText = "Welcome to GameBox - Hangman.";
            gameText = "Playing this game is quite simple.\nYou start by selecting a catagory on the left,\na word will then be randomly generated from that catagory.\nYou then take guesses, one letter at a time to complete the word.\nAre you ready?\n\nSelect a catagory to begin.";
            livesText = "";
            wordToGuess = "";
            wordToGuessCryptic = "";
            choosingCategory = true;
            drawReset = false;
            player.NumberOfLives = 5;
            guessedLetters = new List<char>();
        }

        char GetCharFromId(int buttonId)
        {
            var character = alphabet.Where(a => a.Key == buttonId).Select(x => x.Value).First();
            return character;
        }

        string GetRandomWordFromCategory(int categoryId)
        {
            string chosenWord = "";
            string categoryChosen = "";
            string filename = "";
            switch (categoryId)
            {
                case 100://tv
                    categoryChosen = "TV";
                    break;
                case 200://film
                    categoryChosen = "Film";
                    break;
                case 300://celeb
                    categoryChosen = "Celebrity";
                    break;
            }
            filename = String.Format("Category_{0}_Options",categoryChosen);
            string[] lines = System.IO.File.ReadAllLines(String.Format(@"../../../../Content/Files/Hangman/{0}.txt", filename));
            Random rand = new Random();
            int randomArrPos = rand.Next(lines.Length);
            chosenWord = lines[randomArrPos];
            return chosenWord;
        }

        string GetCrypticVersionOfWord(string word)
        {
            string crypticWord = "";

            for (int i = 0; i < word.Length; i++)
            {
                if(word[i] != ' ')
                {
                    if (!guessedLetters.Contains(word[i]))
                    {
                        crypticWord += " ";
                        crypticWord += "_";
                    }
                    else
                    {
                        crypticWord += " ";
                        crypticWord += word[i];
                    }
                }
                else
                {
                    crypticWord += "  ";
                }
            }
            return crypticWord;
        }

        Dictionary<int,char> PrepAlphabet()
        {
            Dictionary<int, char> alphabet = new Dictionary<int, char>();
            alphabet.Add(0, 'a');
            alphabet.Add(1, 'b');
            alphabet.Add(2, 'c');
            alphabet.Add(3, 'd');
            alphabet.Add(4, 'e');
            alphabet.Add(5, 'f');
            alphabet.Add(6, 'g');
            alphabet.Add(7, 'h');
            alphabet.Add(8, 'i');
            alphabet.Add(9, 'j');
            alphabet.Add(10, 'k');
            alphabet.Add(11, 'l');
            alphabet.Add(12, 'm');
            alphabet.Add(13, 'n');
            alphabet.Add(14, 'o');
            alphabet.Add(15, 'p');
            alphabet.Add(16, 'q');
            alphabet.Add(17, 'r');
            alphabet.Add(18, 's');
            alphabet.Add(19, 't');
            alphabet.Add(20, 'u');
            alphabet.Add(21, 'v');
            alphabet.Add(22, 'w');
            alphabet.Add(23, 'x');
            alphabet.Add(24, 'y');
            alphabet.Add(25, 'z');

            return alphabet;
        }
    }
}
