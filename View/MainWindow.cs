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
    public class MainWindow
    {
        private RepairWindow _repairWindow;
        private StorageWindow _storageWindow;

        private SwitchableMenu _mainMenu;
        private TableComponent? _currenCarInfo;

        private CarServicePresenter _presenter;

        private List<string> _carInfoHeader = new List<string>()
        {
            "Part", "State"
        };

        private Label _repairedParts;
        private Label _repairedPartsValue;

        private Label _evaluatedIncome;
        private Label _evaluatedIncomeValue;

        private Label _penaltyForRejection;
        private Label _penaltyForRejectionValue;

        private Label _balance;
        private Label _balanceValue;

        private Label _currentClient;
        private Label _currentClientNumber;

        private Label _clientsCount;
        private Label _clientsCountValue;

        public MainWindow(Point position, CarServicePresenter presenter)
        {
            Position = position;

            _presenter = presenter;

            _repairWindow = new RepairWindow(position, presenter);
            _repairWindow.WindowClosed += Draw;
            _repairWindow.WindowOpened += Hide;

            _storageWindow = new StorageWindow(position, presenter);
            _storageWindow.WindowOpened += Hide;
            _storageWindow.WindowClosed += Draw;

            _mainMenu = CreateMainMenu(new SwitchableMenuBuilder(position));
            _currenCarInfo = CreateCarInfoTable(3);

            _presenter.PartRepaired += OnPartRepaired;
            _presenter.RepairCompleted += OnRepairCompleted;
            _presenter.RepairCompleted += _repairWindow.OnRepairCompleted;

            _presenter.GameOver += OnGameOver;
            _presenter.RepairAborted += OnRepairAborted;

            var labelsPosition = new Point(_mainMenu.Position.X, _currenCarInfo.Position.Y + _currenCarInfo.Height);

            _balance = new Label(labelsPosition, "Balance: ");
            _balanceValue = new Label(UIUtils.GetPositionAfter(_balance), _presenter.GetBalance().ToString());

            _repairedParts = new Label(UIUtils.GetPositionBelow(_balance), "Parts repaired: ");
            _repairedPartsValue = new Label(UIUtils.GetPositionAfter(_repairedParts, 1), _presenter.GetRepairedPartsCount().ToString());

            _evaluatedIncome = new Label(UIUtils.GetPositionBelow(_repairedParts), "Repair Cost: ");
            _evaluatedIncomeValue = new Label(UIUtils.GetPositionAfter(_evaluatedIncome), _presenter.GetCurrentRepairCost().ToString());

            _penaltyForRejection = new Label(UIUtils.GetPositionBelow(_evaluatedIncome), "Rejection penalty: ");
            _penaltyForRejectionValue = new Label(UIUtils.GetPositionAfter(_penaltyForRejection), _presenter.GetCurrentPenalty().ToString());

            _currentClient = new Label(UIUtils.GetPositionAfter(_currenCarInfo, 3), "Current Client: ");
            _currentClientNumber = new Label(UIUtils.GetPositionAfter(_currentClient), _presenter.GetCurrentClientNumber().ToString());

            _clientsCount = new Label(UIUtils.GetPositionBelow(_currentClient), "Clients Count: ");
            _clientsCountValue = new Label(UIUtils.GetPositionAfter(_clientsCount), _presenter.GetClientsCount().ToString());
        }

        public bool IsRunning { get; private set; }

        public Point Position { get; private set; }

        private SwitchableMenu CreateMainMenu(SwitchableMenuBuilder builder)
        {
            var menu = builder
                .AddItem("Починить", TextAlignment.Left, true, _repairWindow.Run)
                .AddItem("Осмотреть склад", TextAlignment.Left, true, _storageWindow.Run)
                .AddItem("Отказаться от ремонта", TextAlignment.Left, true, _presenter.Abort)
                .AddItem("Завершение работы", TextAlignment.Left, true, CloseApplication)
                .Build();

            return menu;
        }

        private TableComponent? CreateCarInfoTable(int offset)
        {
            if (_presenter.TryGetCarPartsInfo(out var info))
            {
                var position = UIUtils.GetPositionAfter(_mainMenu, offset);

                var table = new TableComponent(position, info);

                table.AddHeader(new TableHeader(position, _carInfoHeader.ToArray()));

                return table;
            }

            return null;
        }

        public void Run()
        {
            IsRunning = true;

            Draw();

            while (IsRunning)
            {
                HandleUserInput();
            }

            Hide();

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("YOU WON!");
        }

        private void Draw()
        {
            _mainMenu.Draw();
            _currenCarInfo?.Draw();

            _balance.Draw();
            _balanceValue.Draw();

            _repairedParts.Draw();
            _repairedPartsValue.Draw();

            _evaluatedIncome.Draw();
            _evaluatedIncomeValue.Draw();

            _penaltyForRejection.Draw();
            _penaltyForRejectionValue.Draw();

            _currentClient.Draw();
            _currentClientNumber.Draw();

            _clientsCount.Draw();
            _clientsCountValue.Draw();

            UpdateLabels();
        }

        private void Hide()
        {
            _mainMenu.Clear();
            _currenCarInfo?.Clear();

            _balance.Clear();
            _balanceValue.Clear();

            _repairedParts.Clear();
            _repairedPartsValue.Clear();

            _evaluatedIncome.Clear();
            _evaluatedIncomeValue.Clear();

            _penaltyForRejection.Clear();
            _penaltyForRejectionValue.Clear();

            _currentClient.Clear();
            _currentClientNumber.Clear();

            _clientsCount.Clear();
            _clientsCountValue.Clear();
        }

        private void HandleUserInput()
        {
            var input = Console.ReadKey(true).Key;

            switch (input)
            {
                case ConsoleKey.UpArrow:
                    _mainMenu.MoveCursor(CursorMovement.Up);
                    break;
                case ConsoleKey.DownArrow:
                    _mainMenu.MoveCursor(CursorMovement.Down);
                    break;
                case ConsoleKey.Enter:
                    _mainMenu.Click();
                    break;
            }
        }

        private void CloseApplication()
        {
            IsRunning = false;
            Console.Clear();
        }

        private void OnPartRepaired(int index)
        {
            if (_presenter.TryGetPartInfo(index, out var info))
            {
                _currenCarInfo.SetRow(index, info);
            }
        }

        private void OnRepairCompleted() 
        {
            _currenCarInfo?.Clear();

            _currenCarInfo = CreateCarInfoTable(3);

            UpdateLabels();
        }

        private void OnRepairAborted() 
        { 
            UpdateLabels();
        }

        private void OnGameOver() 
        { 
            IsRunning = false;
        }

        private void UpdateLabels()
        {
            _balanceValue.SetText(_presenter.GetBalance().ToString());
            _repairedPartsValue.SetText(_presenter.GetRepairedPartsCount().ToString());
            _evaluatedIncomeValue.SetText(_presenter.GetCurrentRepairCost().ToString());
            _penaltyForRejectionValue.SetText(_presenter.GetCurrentPenalty().ToString());
            _currentClientNumber.SetText(_presenter.GetCurrentClientNumber().ToString());
        }
    }
}
