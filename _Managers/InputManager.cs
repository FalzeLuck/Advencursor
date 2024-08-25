using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Managers
{
    public static class InputManager
    {
        public static MouseState MouseState;
        private static MouseState _lastmouseState;

        public static Vector2 _mousePosition => Mouse.GetState().Position.ToVector2();
        public static bool MouseLeftClicked { get; private set; }
        public static bool MouseRightClicked { get; private set; }
        public static Rectangle MouseCursor { get; set; }
        public static void Update()
        {
            var keyboardState = Keyboard.GetState();
            _lastmouseState = MouseState;
            MouseState = Mouse.GetState();

            MouseLeftClicked = Mouse.GetState().LeftButton == ButtonState.Pressed
                                && _lastmouseState.LeftButton == ButtonState.Released;

            MouseRightClicked = Mouse.GetState().RightButton == ButtonState.Pressed
                && _lastmouseState.RightButton == ButtonState.Released;


            MouseCursor = new((int)_mousePosition.X, (int)_mousePosition.Y, 1, 1);
        }

    }
}
