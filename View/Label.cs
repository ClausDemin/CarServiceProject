using CarServiceProject.View.Interfaces;
using System.Drawing;

namespace CarServiceProject.View
{
    public class Label : IUIElement
    {
        private string _text;
        private bool _isDisplaying;

        private int _maxLength;

        public Label(Point position, string text)
        {
            Position = position;
            _text = text;
            _maxLength = text.Length;
        }

        public Point Position { get; set; }
        public int Width => _text.Length;
        public int Height => 1;
        public int ScreenWidth => Width;

        public void SetText(string text)
        {
            _text = text;

            if (_text.Length > _maxLength) 
            { 
                _maxLength = _text.Length;
            }

            if (_isDisplaying)
            {
                Draw();
            }
        }

        public void Draw()
        {
            Clear();

            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write(_text);

            _isDisplaying = true;
        }

        public void Clear()
        {
            Console.SetCursorPosition(Position.X, Position.Y);

            for (int i = 0; i < _maxLength; i++)
            {
                Console.Write(' ');
            }

            _isDisplaying = false;
        }
    }
}
