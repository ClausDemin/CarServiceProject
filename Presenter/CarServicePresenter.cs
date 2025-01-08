using CarServiceProject.Model.Enums;
using CarServiceProject.Model.Interfaces;

namespace CarServiceProject.Presenter
{
    public class CarServicePresenter
    {
        private ICarService _carService;

        public CarServicePresenter(ICarService service)
        {
            _carService = service;
            _carService.RepairCompleted += OnRepairCompleted;
            _carService.QueueEmpty += OnQueueEmpty;
            _carService.RepairAborted += OnRepairAborted;
        }

        public event Action<int>? PartRepaired;
        public event Action? RepairCompleted;
        public event Action? RepairAborted;
        public event Action? GameOver;

        public void TryRepair(int rowIndex, string partName)
        {
            var type = GetPartType(partName);

            if (_carService.TryRepair(type, rowIndex))
            {
                PartRepaired?.Invoke(rowIndex);
            }
        }

        public void Abort()
        {
            _carService.Abort();
        }

        public bool TryGetPartInfo(int index, out string[] info)
        {
            if (_carService.TryGetPartInfo(index, out var partInfo))
            {
                if (partInfo != null)
                {
                    info = partInfo.GetInfo();

                    return true;
                }
            }

            info = Array.Empty<string>();
            return false;
        }

        public bool TryGetCarPartsInfo(out string[,] info)
        {
            var partsInfo = _carService.InspectVehicle();

            if (partsInfo == null)
            {
                info = new string[0, 0];

                return false;
            }

            var result = new List<string[]>();

            foreach (var part in partsInfo)
            {
                result.Add(part.GetInfo());
            }

            var rows = result.Count;
            var columns = result.Max(row => row.Length);

            info = new string[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    info[i, j] = result[i][j];
                }
            }

            return true;
        }

        public string[] GetPartsNames()
        {
            var names = Enum.GetNames<PartType>().ToList();

            names.RemoveAll(name => name == PartType.None.ToString());

            return names.ToArray();
        }

        public int GetCurrentClientNumber()
        {
            return _carService.CurrentClientNumber;
        }

        public int GetClientsCount() 
        { 
            return _carService.ClientsCount;
        }

        public float GetOrderCost(Dictionary<string, int> order) 
        { 
            float cost = 0f;

            foreach (var parts in order) 
            {
                if (TryGetPartPrice(parts.Key, out var price)) 
                { 
                    price *= parts.Value;
                    cost += price;
                }
            }

            return cost;
        }

        public float GetBalance() 
        { 
            return _carService.Balance;
        }

        public float GetCurrentPenalty() 
        { 
            return _carService.GetCurrentPenalty();
        }

        public float GetCurrentRepairCost() 
        { 
            return _carService.GetCurrentRepairCost();
        }

        public int GetRepairedPartsCount() 
        { 
            return _carService.GetRepairedPartsCount();
        }

        public bool TryGetPartRepairPrice(string partName, out float repairCosts)
        {
            var partType = GetPartType(partName);

            if (partType != PartType.None)
            {
                repairCosts = _carService.GetRepairCost(partType);
                return true;
            }

            repairCosts = 0;
            return false;
        }

        public bool TryGetPartPrice(string partName, out float partCost)
        {
            var partType = GetPartType(partName);

            if (partType != PartType.None)
            {
                partCost = _carService.GetPartPrice(partType);
                return true;
            }

            partCost = 0;
            return false;
        }

        public bool TryOrderParts(Dictionary<string, int> order)
        {
            var orderData = new Dictionary<PartType, int>();

            foreach (var part in order)
            {
                var type = GetPartType(part.Key);

                orderData[type] = part.Value;
            }

            return _carService.TryOrderPars(orderData);
        }

        public int GetStorageCount(string partName)
        {
            var partType = GetPartType(partName);

            return _carService.GetStorageCount(partType);
        }

        private PartType GetPartType(string name)
        {
            if (Enum.TryParse<PartType>(name, out var type))
            {
                return type;
            };

            return PartType.None;
        }

        private void OnRepairCompleted() 
        { 
            _carService.TryServeNextClient();

            RepairCompleted?.Invoke();
        }

        private void OnQueueEmpty() 
        { 
            GameOver?.Invoke();
        }

        private void OnRepairAborted() 
        { 
            RepairAborted?.Invoke();

            OnRepairCompleted();
        }
    }
}
