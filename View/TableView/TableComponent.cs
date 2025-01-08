using CarServiceProject.View.Interfaces;
using CarServiceProject.View.TableView.Enums;
using CarServiceProject.View.Utils;
using System.Drawing;

namespace CarServiceProject.View.TableView
{
    public class TableComponent : IUIElement
    {
        private TableCell[,] _cells;

        private TableHeader? _header;

        private List<TableColumn> _columns = new List<TableColumn>();
        private List<TableRow> _rows = new List<TableRow>();

        private bool _isDisplaying;
        private Point _position;

        public TableComponent(int rows, int columns, Point position)
        {
            _position = position;
            _cells = new TableCell[rows, columns];
            Init();
        }

        public TableComponent(Point position, string[,] data)
        {
            _position = position;
            _cells = new TableCell[data.GetLength(0), data.GetLength(1)];
            Init(data);
        }

        public string this[int row, int column]
        {
            get { return _cells[row, column].Text; }
            set { _cells[row, column].SetValue(value); }
        }

        public int Columns => _cells.GetLength(1);
        public int Rows => _cells.GetLength(0);
        public Point Position 
        { 
            get 
            { 
                return _position; 
            } 
            set 
            { 
                Clear();
                _position = value;  
                OnPositionChanged(); 
            } 
        }
        public Point TablePosition => GetTablePosition();
        public int Width => GetWidth();
        public int ScreenWidth => GetScreenWidth();
        public int Height => GetHeight();
        public bool HasHeader => _header != null;

        public void Draw()
        {
            foreach (var column in _columns)
            {
                column.Draw();
            }

            _isDisplaying = true;
        }

        public void Clear() 
        {
            if (_isDisplaying) 
            {
                foreach (var column in _columns)
                {
                    column.Clear();
                }

                _isDisplaying = false;
            }
        }

        public void AddHeader(TableHeader header) 
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(header.Columns, Columns);

            if (_header == null)
            {
                for (int i = 0; i < Columns; i++)
                {
                    _columns[i].Add(header[i], 0);
                    header[i].SetColumn(_columns[i]);
                }

                header.BindWithTable(this);
            }
            else 
            {
                for (int i = 0; i < Columns; i++)
                {
                    _columns[i].SetValue(0, _header[i].Text);
                }
            }

            _header = header;

            UpdateRowsPosition();
        }

        public void AddColumn(string[]? data = null)
        {
            if (data != null)
            {
                ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, Height);
            }

            var lastColumn = _columns.Last();
            var positionX = lastColumn.Position.X + lastColumn.ScreenWidth;
            var positionY = lastColumn.Position.Y;

            var column = new TableColumn(new Point(positionX, positionY), Columns);

            for (int i = 0; i < Height; i++)
            {
                var cell = new TableCell(new Point(positionX, _rows[i].Position.Y), null, TextAlignment.Center, true);

                if (data != null)
                {
                    cell.SetValue(data[i]);
                }


                column.Add(cell);
                _rows[i].Add(cell);

                cell.SetColumn(column);
                cell.SetRow(_rows[i]);
            }

            column.WidthChanged += RedrawColumns;
            column.WidthChanged += UpdateColumnsPositions;

            _columns.Add(column);

            Expand();
        }

        public void AddRow(string[]? data = null)
        {
            if (data != null)
            {
                ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, _columns.Count);
            }

            var lastRow = _rows.Last();
            var positionX = lastRow.Position.X;
            var positionY = lastRow.Position.Y + lastRow.Height;

            var row = new TableRow(Position, _rows.Count);

            for (int i = 0; i < _columns.Count; i++)
            {
                var cell = new TableCell(new Point(_columns[i].Position.X, positionY), null, TextAlignment.Center, true);

                if (data != null)
                {
                    cell.SetValue(data[i]);
                }

                row.Add(cell);
                _columns[i].Add(cell);

                cell.SetColumn(_columns[i]);
                cell.SetRow(row);
            }

            Expand();
        }

        public string[] GetRow(int row) 
        { 
            return _rows[row].GetValues();
        }

        public void SetRow(int row, string[] values) 
        { 
            _rows[row].SetValues(values);
        }

        public void SetColumn(int column, string[] values) 
        { 
            _columns[column].SetValues(values);
        }

        private void Init(string[,]? data = null)
        {
            if (data != null)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        var position = new Point(Position.X +j, Position.Y + i);
                        _cells[i, j] = new TableCell(position, data[i, j], TextAlignment.Center, true);
                    }
                }
            }
            else
            {
                Init();
            }

            CreateColumns();
            CreateRows();
        }

        private void Init()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var position = new Point(i, j);
                    _cells[i, j] = new TableCell(position, string.Empty, TextAlignment.Center, true);
                }
            }

            CreateColumns();
            CreateRows();
        }

        private void CreateColumns()
        {
            var position = Position;

            for (int i = 0; i < Columns; i++)
            {
                var column = new TableColumn(position, i);

                for (int j = 0; j < Rows; j++)
                {
                    var cell = _cells[j, i];

                    column.Add(cell);
                    cell.SetColumn(column);
                }

                column.WidthChanged += RedrawColumns;
                column.WidthChanged += UpdateColumnsPositions;

                _columns.Add(column);

                position = new Point(column.Position.X + column.ScreenWidth, column.Position.Y);
            }
        }

        private void CreateRows()
        {
            var position = Position;

            for (int i = 0; i < Rows; i++)
            {
                var row = new TableRow(position, i);

                for (int j = 0; j < Columns; j++)
                {
                    var cell = _cells[i, j];

                    row.Add(cell);
                    cell.SetRow(row);
                }

                _rows.Add(row);

                position = new Point(Position.X, i);
            }
        }

        private void RedrawColumns(int from)
        {
            if (_isDisplaying) 
            {
                for (int i = from; i < _columns.Count; i++)
                {
                    _columns[i].Draw();
                }
            }
        }

        private int GetWidth()
        {
            var width = 0;

            foreach (var column in _columns)
            {
                width += column.Width;
            }

            return width;
        }

        private int GetScreenWidth()
        {
            var width = 0;

            foreach (var column in _columns)
            {
                width += column.ScreenWidth;
            }

            return width;
        }

        private int GetHeight()
        {
            var height = _columns.Max(column => column.Height);

            return height;
        }

        private void Expand()
        {
            var newCells = new TableCell[_rows.Count, _columns.Count];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    newCells[i, j] = _cells[i, j];
                }
            }

            _cells = newCells;
        }

        private void UpdateColumnsPositions(int from) 
        {
            var position = Position;

            for (int i = from; i < Columns; i++) 
            {
                _columns[i].Position = position;
                position = new Point(position.X + _columns[i].ScreenWidth, position.Y);
            }
        }

        private void UpdateRowsPosition() 
        {
            var position = Position;

            if (_header != null) 
            { 
                position = UIUtils.GetPositionBelow(_header);
            }

            for (int i = 0; i < Rows; i++) 
            {
                _rows[i].Position = position;

                position = new Point(position.X, position.Y + _rows[i].Height);
            }
        }

        private void OnPositionChanged() 
        { 
            UpdateColumnsPositions(0);
            UpdateRowsPosition();
        }

        private Point GetTablePosition() 
        {
            if (_header == null) 
            {
                return Position;
            }

            return new Point(Position.X, Position.Y + _header.Height);
        }
    }
}
