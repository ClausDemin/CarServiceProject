using CarServiceProject.View.TableView.Enums;
using CarServiceProject.View.Utils;
using System.Drawing;

namespace CarServiceProject.View.TableView
{
    public class TableCell
    {
        private TableColumn? _parentColumn;
        private TableRow? _parentRow;

        private int _borderLength = 1;
        private int _textOffset = 1;

        public TableCell(Point position, string? text = null, TextAlignment alignment = TextAlignment.Left, bool isBordered = false, char border = '|')
        {
            Position = position;
            Text = text == null ? string.Empty : text;
            Alignment = alignment;
            IsBordered = isBordered;
            Border = border;
        }

        public event Action? ValueChanged;

        public Point Position { get; set; }
        public string Text { get; private set; }
        public int Width => GetWidth();
        public int ScreenWidth => GetScreenWidth();
        public bool IsBordered { get; set; }
        public char Border { get; set; }
        public TextAlignment Alignment { get; set; }

        public void SetValue(string? value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            Text = value;

            ValueChanged?.Invoke();
        }

        public void Clear()
        {
            Console.SetCursorPosition(Position.X, Position.Y);

            for (int i = 0; i < ScreenWidth; i++)
            {
                Console.Write(' ');
            }
        }

        public void Draw()
        {
            UIUtils.AlignText(Alignment, Text, Width, out var whiteSpacesBefore, out var whiteSpacesAfter);

            if (IsBordered)
            {
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.Write(Border);

                Console.SetCursorPosition(Console.CursorLeft + whiteSpacesBefore + _textOffset, Position.Y);
                Console.Write(Text);

                Console.SetCursorPosition(Console.CursorLeft + whiteSpacesAfter + _textOffset, Position.Y);
                Console.Write(Border);
            }
            else
            {
                Console.SetCursorPosition(Position.X + whiteSpacesBefore + _textOffset, Position.Y);
                Console.Write(Text);
            }
        }

        public void SetColumn(TableColumn parent)
        {
            _parentColumn = parent;
        }

        public void SetRow(TableRow parent)
        {
            _parentRow = parent;
        }

        private int GetWidth()
        {
            if (_parentColumn == null)
            {
                return Text.Length;
            }

            return _parentColumn.Width;
        }

        private int GetScreenWidth()
        {
            if (IsBordered && _parentColumn != null)
            {
                return _parentColumn.Width + 2 * (_borderLength + _textOffset);
            }
            else if (IsBordered)
            {
                return Width + 2 * (_borderLength + _textOffset);
            }

            return Width;
        }
    }
}
