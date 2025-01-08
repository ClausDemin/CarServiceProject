using CarServiceProject.Presenter;
using CarServiceProject.View.Infrastructure;
using CarServiceProject.View.SwitchableMenuView;
using CarServiceProject.View.SwitchableMenuView.Enums;
using CarServiceProject.View.TableView;
using CarServiceProject.View.TableView.Enums;
using CarServiceProject.View.Utils;
using System.Drawing;

namespace CarServiceProject.View
{
    public class StorageWindow
    {
        private TableComponent _storageInfo;
        private CarServicePresenter _presenter;
        private SwitchableMenu _orderMenu;

        private Label _balance;
        private Label _balanceValue;
        private Label _orderCost;
        private Label _orderCostValue;
        private Label _orderMessages;

        private Dictionary<string, int> _partsOrder = new Dictionary<string, int>();
        private bool _isRunning;

        private List<string> _storageHeader = new List<string>()
        {
            "Part", "Price", "Count", "To Order"
        };

        public StorageWindow(Point position, CarServicePresenter presenter)
        {
            Position = position;
            _presenter = presenter;
            _storageInfo = CreateTable(position);
            _orderMenu = CreateMenu(_storageInfo.TablePosition, _storageInfo);

            _storageInfo.Position = new Point(UIUtils.GetPositionAfter(_orderMenu).X, Position.Y);

            _balance = new Label(UIUtils.GetPositionBelow(_orderMenu, 2), "Balance:");
            _balanceValue = new Label(UIUtils.GetPositionAfter(_balance, 1), _presenter.GetBalance().ToString());

            _orderCost = new Label(UIUtils.GetPositionBelow(_balance, 1), "Order Cost:");
            _orderCostValue = new Label(UIUtils.GetPositionAfter(_orderCost, 1), _presenter.GetOrderCost(_partsOrder).ToString());

            _orderMessages = new Label(UIUtils.GetPositionBelow(_orderCost, 1), string.Empty);
        }

        public Point Position { get; set; }

        public event Action? WindowOpened;
        public event Action? WindowClosed;

        public void Run()
        {
            _isRunning = true;

            Show();

            while (_isRunning) 
            { 
                HandleUserInput();
            }

            Hide();
        }

        public void Clear()
        {
            _storageInfo?.Clear();
            _orderMenu?.Clear();
            _balance?.Clear();
            _balanceValue?.Clear();
            _orderCost?.Clear();
            _orderCostValue?.Clear();
            _orderMessages?.Clear();
        }

        public void ExternalInterrupt()
        {
            _isRunning = false;
        }

        private void Show()
        {
            WindowOpened?.Invoke();

            Update();

            _storageInfo?.Draw();
            _orderMenu?.Draw();
            _balance.Draw();
            _balanceValue.Draw();
            _orderCost.Draw();
            _orderCostValue.Draw();
            _orderMessages?.Draw();
        }

        private void Hide()
        {
            _partsOrder.Clear();
            UpdateOrderColumn();

            Clear();

            WindowClosed?.Invoke();
        }

        private void HandleUserInput()
        {
            var userInput = Console.ReadKey(true).Key;

            switch (userInput)
            {
                case ConsoleKey.UpArrow:
                    _orderMenu.MoveCursor(CursorMovement.Up);
                    break;
                case ConsoleKey.DownArrow:
                    _orderMenu.MoveCursor(CursorMovement.Down);
                    break;
                case ConsoleKey.LeftArrow:
                    RemoveFromOrder(GetCurrentPartName(_orderMenu.Current), 1);
                    break;
                case ConsoleKey.RightArrow:
                    AddToOrder(GetCurrentPartName(_orderMenu.Current), 1);
                    break;
                case ConsoleKey.Enter:
                    OrderParts();
                    break;
                case ConsoleKey.Escape:
                    _isRunning = false;
                    break;

            }
        }

        private TableComponent CreateTable(Point position)
        {
            var info = _presenter.GetPartsNames();

            var countAtStorage = new List<string>();
            var partPrices = new List<string>();
            var toOrderCount = new List<string>();

            for (int i = 0; i < info.Length; i++)
            {
                countAtStorage.Add(_presenter.GetStorageCount(info[i]).ToString());

                _presenter.TryGetPartPrice(info[i], out var price);
                partPrices.Add(price.ToString());

                toOrderCount.Add("0");
            }

            var table = new TableComponent(info.Length, 1, position);

            table.SetColumn(0, info);

            table.AddColumn(partPrices.ToArray());

            table.AddColumn(countAtStorage.ToArray());

            table.AddColumn(toOrderCount.ToArray());

            table.AddHeader(new TableHeader(position, _storageHeader.ToArray()));

            return table;
        }

        private SwitchableMenu CreateMenu(Point position, TableComponent orderTable)
        {
            var builder = new SwitchableMenuBuilder(position);

            for (int i = 0; i < orderTable.Rows; i++)
            {
                builder.AddItem(string.Empty, TextAlignment.Left, false, OrderParts);
            }

            return builder.Build();
        }

        private void AddToOrder(string name, int count)
        {
            if (HasEnoughMoney())
            {
                if (_partsOrder.ContainsKey(name))
                {
                    _partsOrder[name] += count;
                }
                else
                {
                    _partsOrder[name] = count;
                }

                UpdateOrderColumn();
                UpdateOrderCost();
            }
            else 
            {
                SendMessage("Not enough funds");
            }
        }

        private void RemoveFromOrder(string name, int count)
        {
            if (_partsOrder.ContainsKey(name))
            {
                if (count < _partsOrder[name])
                {
                    _partsOrder[name] -= count;
                }
                else
                {
                    _partsOrder[name] = 0;
                }

                if (_partsOrder[name] == 0)
                {
                    _partsOrder.Remove(name);
                }

                UpdateOrderColumn();
                UpdateOrderCost();
            }

            if (HasEnoughMoney())
            {
                SendMessage(string.Empty);
            }
            else 
            {
                SendMessage("Not enough funds");
            }
        }

        private void OrderParts()
        {
            if (_partsOrder.Count > 0) 
            {
                if (_presenter.TryOrderParts(_partsOrder))
                {
                    UpdateCountAtStorageColumn();
                    _partsOrder.Clear();
                    UpdateOrderColumn();
                    UpdateOrderCost();
                    UpdateBalance();

                    SendMessage("Payment Successful");
                }
                else
                {
                    SendMessage("Not enough funds");
                }
            }
        }

        private string GetCurrentPartName(int index)
        {
            return _storageInfo[index, 0];
        }

        private void Update() 
        {
            UpdateCountAtStorageColumn();
            UpdateOrderColumn();
            UpdateOrderCost();
            UpdateBalance();
        }

        private void UpdateOrderColumn() 
        {
            var values = new List<string>();

            if (_storageInfo.HasHeader) 
            {
                values.Add(_storageHeader[3]);
            }

            for (int i = 0; i < _storageInfo.Rows; i++) 
            {
                if (_partsOrder.TryGetValue(_storageInfo[i, 0], out var value))
                {
                    values.Add(value.ToString());
                }
                else 
                {
                    values.Add("0");
                }
            }

            _storageInfo?.SetColumn(3, values.ToArray());
        }

        private void UpdateCountAtStorageColumn() 
        {
            var countAtStorage = new List<string>();

            if (_storageInfo.HasHeader)
            {
                countAtStorage.Add(_storageHeader[2]);
            }

            var info = _presenter.GetPartsNames();

            for (int i = 0; i < info.Length; i++)
            {
                countAtStorage.Add(_presenter.GetStorageCount(info[i]).ToString());
            }

            _storageInfo.SetColumn(2, countAtStorage.ToArray());
        }

        private void UpdateOrderCost() 
        { 
            var cost = _presenter.GetOrderCost(_partsOrder);

            _orderCostValue.SetText(cost.ToString());
        }

        private void UpdateBalance() 
        {
            _balanceValue.SetText(_presenter.GetBalance().ToString());
        }

        private bool HasEnoughMoney() 
        { 
            var balance = _presenter.GetBalance();
            var orderCost = _presenter.GetOrderCost(_partsOrder);

            return balance >= orderCost;
        }

        private void SendMessage(string message) 
        { 
            _orderMessages.SetText(message);
        }
    }
}
