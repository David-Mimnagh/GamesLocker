using GamesLocker.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLocker.Resources
{
    public class Button
    {

        public enum State
        {
            None,
            Pressed,
            Hover,
            Released
        }

        public Rectangle _rectangle;
        private GameStates.GameState _gameState;
        private State _buttonState;
        public State ButtonState
        {
            get { return _buttonState; }
            set { _buttonState = value; } // you can throw some events here if you'd like
        }
        private int _buttonId;
        public int ButtonId { get { return _buttonId; } }
        string _gameDescription;
        public string GameDescription { get { return _gameDescription; }}
        private Dictionary<State, Texture2D> _textures;

        public Button(GameStates.GameState gameState, Rectangle rectangle, Texture2D noneTexture, Texture2D hoverTexture, Texture2D pressedTexture, string gameDescription)
        {
            _gameState = gameState;
            _rectangle = rectangle;
            _gameDescription = gameDescription;
            _textures = new Dictionary<State, Texture2D>
                        {
                            { State.None, noneTexture },
                            { State.Released, noneTexture },
                            { State.Hover, hoverTexture },
                            { State.Pressed, pressedTexture }
                        };
        }

        public Button(int buttonId, Rectangle rectangle, Texture2D noneTexture, Texture2D hoverTexture, Texture2D pressedTexture)
        {
            _buttonId = buttonId;
            _rectangle = rectangle;
            _textures = new Dictionary<State, Texture2D>
                        {
                            { State.None, noneTexture },
                            { State.Released, noneTexture },
                            { State.Hover, hoverTexture },
                            { State.Pressed, pressedTexture }
                        };
        }

        public Button(Rectangle rectangle, Texture2D noneTexture, Texture2D hoverTexture, Texture2D pressedTexture)
        {
            _rectangle = rectangle;
            _textures = new Dictionary<State, Texture2D>
                        {
                            { State.None, noneTexture },
                            { State.Released, noneTexture },
                            { State.Hover, hoverTexture },
                            { State.Pressed, pressedTexture }
                        };
        }

        public void Update(MouseState mouseState)
        {
            if (_rectangle.Contains(mouseState.X, mouseState.Y))
            {
                if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    ButtonState = State.Pressed;

                    if (_gameState != null)
                    {
                        GameStateManager.Instance.ClearScreens();
                        GameStateManager.Instance.AddScreen(_gameState);
                    }
                }
                else
                    ButtonState = (ButtonState == State.Pressed) ? State.Released : State.Hover;
            }
            else
            {
                ButtonState = State.None;
            }
        }

        // Make sure Begin is called on s before you call this function
        public void Draw(SpriteBatch s)
        {
           s.Draw(_textures[ButtonState], _rectangle, Color.White);
        }

    }
}
