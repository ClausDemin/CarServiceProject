using CarServiceProject.View.Interfaces;
using CarServiceProject.View.TableView.Enums;
using CarServiceProject.View.Utils;
using System.Drawing;

namespace CarServiceProject.View.TableView
{
    public class TableHeader :IUIElement
    {
        private TableCell[] _headerCells;
        private TableComponent? _parent;

        public TableHeader(Point position, params string[] names) 
        { 
            Position = position;
            _headerCells = CreateHeader(names);
        }

        public TableCell this[int index]
        {
            get { return _headerCells[index]; }
            set { _headerCells[index] = value; }
        }

        public Point Position { get; set; }
        public IEnumerable<TableCell> Names => _headerCells;
        public int Columns => _headerCells.Length;
        public int Width => GetWidth();
        public int Height => 1;
        public int ScreenWidth => GetScreenWidth();

        public void BindWithTable(TableComponent parent) 
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(_headerCells.Length, parent.Columns);
       
            _parent = parent;

            UpdateCellsPositions();
        }

        public void Draw()
        {
            foreach (var cell in _headerCells) 
            { 
                cell.Draw();
            }
        }

        private TableCell[] CreateHeader(string[] names)
        {
            var result = new TableCell[names.Length];
            var position = Position;

            for (int i = 0; i < names.Length; i++)
            {
                var cell = new TableCell(position, names[i], TextAlignment.Center, true);
                result[i] = cell;

                position = new Point(position.X + cell.ScreenWidth, position.Y);
            }

            return result;
        }

        private int GetWidth() 
        { 
            int width = 0;

            if (_parent == null)
            {
                foreach (var cell in _headerCells)
                {
                    width += cell.Width;
                }
            }
            else 
            {
                width = _parent.Width;            
            }

            return width;
        }

        private int GetScreenWidth() 
        {
            int width = 0;

            if (_parent == null)
            {
                foreach (var cell in _headerCells)
                {
                    width += cell.ScreenWidth;
                }
            }
            else
            {
                width = _parent.ScreenWidth;
            }

            return width;
        }

        private void UpdateCellsPositions() 
        {
            var position = Position;

            for (int i = 0; i < _headerCells.Length; i++) 
            {
                _headerCells[i].Position = position;

                position = new Point(position.X + _headerCells[i].ScreenWidth, position.Y);
            }
        }
    }
}
