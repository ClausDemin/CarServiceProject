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
    public class RepairWindow
    {
        private CarServicePresenter _presenter;

        private SwitchableMenu _partChooseMenu;
        private TableComponent? _partsInfo;

        private Label _repairedParts;
        private Label _repairedPartsValue;

        private Label _evaluatedIncome;
        private Label _evaluatedIncomeValue;

        private Label _penaltyForRejection;
        private Label _penaltyForRejectionValue;

        private List<string> _tableHeader = new List<string>()
        {
            "Part",
            "State",
            "Part Cost",
            "Repair Cost",
            "At storage"
        };

        private bool _isRunning;

        public RepairWindow(Point position, CarServicePresenter presenter)
        {
            _presenter = presenter;
            Position = position;

            _partsInfo = CreatePartsInfo();

            if (_partsInfo != null)
            {
                _partChooseMenu = CreatePartChooseMenu(_partsInfo, new SwitchableMenuBuilder(_partsInfo.TablePosition));

                _partsInfo.Position = new Point(UIUtils.GetPositionAfter(_partChooseMenu).X, Position.Y);
            }

            _presenter.PartRepaired += OnPartRepaired;

            _repairedParts = new Label(UIUtils.GetPositionBelow(_partChooseMenu, 1), "Parts repaired: ");
            _repairedPartsValue = new Label(UIUtils.GetPositionAfter(_repairedParts, 1), _presenter.GetRepairedPartsCount().ToString());

            _evaluatedIncome = new Label(UIUtils.GetPositionBelow(_repairedParts), "Repair Cost: ");
            _evaluatedIncomeValue = new Label(UIUtils.GetPositionAfter(_evaluatedIncome), _presenter.GetCurrentRepairCost().ToString());

            _penaltyForRejection = new Label(UIUtils.GetPositionBelow(_evaluatedIncome), "Rejection penalty: ");
            _penaltyForRejectionValue = new Label(UIUtils.GetPositionAfter(_penaltyForRejection), _presenter.GetCurrentPenalty().ToString());
        }

        public event Action? WindowClosed;
        public event Action? WindowOpened;

        public Point Position { get; set; }

        public void Run()
        {
            _isRunning = true;

            WindowOpened?.Invoke();

            Show();

            while (_isRunning)
            {
                HandleUserInput();
            }

            Hide();
        }

        private void Show()
        {
            if (_partsInfo != null)
            {
                _partsInfo.Draw();
                _partChooseMenu.Draw();

                _repairedParts.Draw();
                _repairedPartsValue.Draw();

                _evaluatedIncome.Draw();
                _evaluatedIncomeValue.Draw();

                _penaltyForRejection.Draw();
                _penaltyForRejectionValue.Draw();

                UpdateStorageColumn(4);
            }
        }

        private void Hide()
        {
            _partChooseMenu.Clear();
            _partsInfo?.Clear();

            _repairedParts.Clear();
            _repairedPartsValue.Clear();

            _evaluatedIncome.Clear();
            _evaluatedIncomeValue.Clear();

            _penaltyForRejection.Clear();
            _penaltyForRejectionValue.Clear();

            WindowClosed?.Invoke();
        }

        private void HandleUserInput()
        {
            var input = Console.ReadKey(true).Key;

            switch (input)
            {
                case ConsoleKey.UpArrow:
                    _partChooseMenu.MoveCursor(CursorMovement.Up);
                    break;
                case ConsoleKey.DownArrow:
                    _partChooseMenu.MoveCursor(CursorMovement.Down);
                    break;
                case ConsoleKey.Enter:
                    _partChooseMenu.Click();
                    break;
                case ConsoleKey.Escape:
                    _isRunning = false;
                    break;
            }
        }

        private TableComponent? CreatePartsInfo()
        {
            if (_presenter.TryGetCarPartsInfo(out var partsInfo))
            {
                var partsData = new TableComponent(Position, partsInfo);
                var partsCosts = new List<string>();
                var partsRepairCosts = new List<string>();
                var countAtStorage = new List<string>();

                for (int i = 0; i < partsData.Rows; i++)
                {
                    if (_presenter.TryGetPartPrice(partsData[i, 0], out var price))
                    {
                        partsCosts.Add(price.ToString());
                    }

                    if (_presenter.TryGetPartRepairPrice(partsData[i, 0], out var repairPrice))
                    {
                        partsRepairCosts.Add(repairPrice.ToString());
                    }

                    countAtStorage.Add(_presenter.GetStorageCount(partsData[i, 0]).ToString());
                }

                partsData.AddColumn(partsCosts.ToArray());
                partsData.AddColumn(partsRepairCosts.ToArray());
                partsData.AddColumn(countAtStorage.ToArray());

                partsData.AddHeader(new TableHeader(Position, _tableHeader.ToArray()));

                return partsData;
            }

            return null;
        }

        private SwitchableMenu CreatePartChooseMenu(TableComponent info, SwitchableMenuBuilder builder)
        {
            for (int i = 0; i < info.Rows; i++)
            {
                var row = info.GetRow(i);
                var index = i;
                builder.AddItem(string.Empty, TextAlignment.Left, false, () => _presenter.TryRepair(index, row[0]));
            }

            return builder.Build();
        }

        private void OnPartRepaired(int rowIndex)
        {
            if (_partsInfo != null)
            {
                if (_presenter.TryGetPartInfo(rowIndex, out var info))
                {
                    if (_presenter.TryGetPartPrice(info[0], out var price))
                    {
                        if (_presenter.TryGetPartRepairPrice(info[0], out var repairPrice))
                        {
                            var row = new string[]
                            {
                            info[0],
                            info[1],
                            price.ToString(),
                            repairPrice.ToString(),
                            _presenter.GetStorageCount(info[0]).ToString()
                            };


                            _partsInfo.SetRow(rowIndex, row);

                            UpdateStorageColumn(_partsInfo.Columns - 1);
                            UpdateLabels();
                        }
                    }
                }
            }
        }

        private void UpdateStorageColumn(int column)
        {
            List<string> count = new List<string>();

            if (_partsInfo.HasHeader)
            {
                count.Add(_tableHeader.Last());
            }

            for (int i = 0; i < _partsInfo.Rows; i++)
            {
                var name = _partsInfo[i, 0];

                count.Add(_presenter.GetStorageCount(name).ToString());
            }

            _partsInfo.SetColumn(column, count.ToArray());
        }

        private void UpdateLabels() 
        {
            _repairedPartsValue.SetText(_presenter.GetRepairedPartsCount().ToString());
            _evaluatedIncomeValue.SetText(_presenter.GetCurrentRepairCost().ToString());
            _penaltyForRejectionValue.SetText(_presenter.GetCurrentPenalty().ToString());
        }

        public void OnRepairCompleted() 
        { 
            _isRunning = false;

            Hide();

            _partsInfo = CreatePartsInfo();

            if (_partsInfo != null) 
            {
                _partsInfo.Position = new Point(UIUtils.GetPositionAfter(_partChooseMenu).X, Position.Y);
            }
        }
    }
}
