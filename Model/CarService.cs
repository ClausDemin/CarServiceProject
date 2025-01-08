using CarServiceProject.Model.Enums;
using CarServiceProject.Model.Interfaces;

namespace CarServiceProject.Model
{
    public class CarService : ICarService
    {
        private Storage _storage;
        private Queue<Car> _clients;
        private Wallet _wallet;

        private RepairCheckList? _repairCheckList;
        private Car? _current;

        public CarService(Storage storage, Queue<Car> clients, float moneyAmount = 0)
        {
            _storage = storage;
            _clients = clients;

            ClientsCount = _clients.Count;

            _wallet = new Wallet(moneyAmount);
        }

        public event Action? RepairCompleted;
        public event Action? RepairAborted;
        public event Action? QueueEmpty;

        public float Balance => _wallet.Amount;
        public int ClientsCount { get; }
        public int CurrentClientNumber { get; private set; }

        public IReadOnlyDictionary<PartType, int> Storage => _storage.Parts;

        public IEnumerable<ICarPartInfo>? InspectVehicle()
        {
            if (_current == null)
            {
                if (TryServeNextClient())
                {
                    return _current?.PartsInfo;
                }
            }
            else
            {
                return _current.PartsInfo;
            }

            return default;
        }

        public bool TryRepair(PartType type, int index)
        {
            if (_current != null)
            {
                if (_storage.TryTake(type, out var part))
                {
                    return _current.TryReplacePart(part, index);
                }
            }

            return false;
        }

        public void Abort()
        {
            _repairCheckList?.Abort();

            RepairAborted?.Invoke();
        }

        public bool TryOrderPars(Dictionary<PartType, int> parts)
        {
            float price = 0;

            foreach (var part in parts)
            {
                price += _storage.GetPrice(part.Key) * part.Value;
            }

            if (_wallet.TryWithdrawMoney(price))
            {
                _storage.OrderParts(parts);

                return true;
            }

            return false;
        }

        public int GetRepairedPartsCount()
        {
            if (_repairCheckList != null)
            {
                return _repairCheckList.RepairedParts;
            }

            return 0;
        }

        public float GetRepairCost(PartType type)
        {
            if (_repairCheckList != null)
            {
                return _repairCheckList.GetRepairCost(type);
            }

            return 0;
        }

        public float GetPartPrice(PartType type)
        {
            return _storage.GetPrice(type);
        }

        public int GetStorageCount(PartType type)
        {
            return _storage.GetAmount(type);
        }

        public float GetCurrentRepairCost()
        {
            if (_repairCheckList != null)
            {
                return _repairCheckList.GetCurrentRepairCost();
            }

            return 0;
        }

        public float GetCurrentPenalty() 
        {
            if (_repairCheckList != null) 
            { 
                return _repairCheckList.GetPenalty();
            }

            return 0;
        }

        public bool TryGetPartInfo(int index, out ICarPartInfo? info)
        {
            if (_current != null)
            {
                if (_current.TryGetPartInfo(index, out var parInfo))
                {
                    if (parInfo != null)
                    {
                        info = parInfo;
                        return true;
                    }
                }
            }

            info = default;
            return false;
        }

        public bool TryServeNextClient()
        {
            if (_clients.Count > 0)
            {
                _current = _clients.Dequeue();
                _repairCheckList = new RepairCheckList(_current.PartsInfo.Where(part => part.State == PartState.Broken).ToList());
                _current.PartReplaced += _repairCheckList.OnPartReplaced;

                _repairCheckList.RepairCompleted += CompleteRepair;
                _repairCheckList.RepairAborted += TryPayPenalty;

                CurrentClientNumber++;

                return true;
            }

            QueueEmpty?.Invoke();

            return false;
        }

        private void CompleteRepair()
        {
            _wallet.IncreaseAmount(_repairCheckList.GetPayment());

            _repairCheckList.RepairCompleted -= CompleteRepair;

            _current.PartReplaced -= _repairCheckList.OnPartReplaced;

            _current = null;

            RepairCompleted?.Invoke();
        }

        private bool TryPayPenalty()
        {
            _wallet.IncreaseAmount(_repairCheckList.GetPayment());

            if (_wallet.TryWithdrawMoney(_repairCheckList.GetPenalty()))
            {
                return true;
            }

            _repairCheckList.RepairAborted -= TryPayPenalty;

            _repairCheckList.RepairCompleted -= CompleteRepair;

            _current.PartReplaced -= _repairCheckList.OnPartReplaced;

            return false;
        }
    }
}
