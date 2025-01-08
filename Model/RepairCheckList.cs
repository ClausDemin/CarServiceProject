using CarServiceProject.Model.Enums;

namespace CarServiceProject.Model
{
    public class RepairCheckList
    {
        private Dictionary<PartType, float> _repairCost = new Dictionary<PartType, float>()
        {
            {PartType.Hull, 140000.00f },
            {PartType.Engine, 70000.00f },
            {PartType.Wheel, 1500.00f },
            {PartType.Transmission, 70000.00f },
            {PartType.Brakes, 30000.00f },
            {PartType.Controls, 45000.00f },
            {PartType.Exhaust, 15000.00f }
        };
        private Dictionary<PartType, float> _partsPenalty = new Dictionary<PartType, float>()
        {
            {PartType.Hull, 30000.00f },
            {PartType.Engine, 20000.00f },
            {PartType.Wheel, 3000.00f },
            {PartType.Transmission, 30000.00f},
            {PartType.Brakes, 10000.00f },
            {PartType.Controls, 15000.00f },
            {PartType.Exhaust, 5000.00f }
        };

        private float _repairAbortPenalty = 1500f;
        private int _replacedParts;

        private List<CarPartInfo> _brokenParts;
        private List<CarPartInfo> _repairedParts;

        public RepairCheckList(List<CarPartInfo> brokenParts)
        {
            _brokenParts = brokenParts;
            _repairedParts = new List<CarPartInfo>();
            _replacedParts = 0;
        }

        public event Action RepairCompleted = delegate { };
        public event Func<bool>? RepairAborted;

        public void Abort() 
        {
            RepairAborted?.Invoke();
        }

        public int RepairedParts => _repairedParts.Count;

        public float GetPayment()
        {
            float payment = 0;

            foreach (var part in _repairedParts)
            {
                payment += part.Price + _repairCost[part.Type];
            }

            return payment;
        }

        public float GetPenalty()
        {
            float penalty = 0;

            if (_replacedParts == 0)
            {
                penalty += _repairAbortPenalty;
            }
            else
            {
                foreach (var partInfo in _brokenParts)
                {
                    penalty += partInfo.Price;
                }
            }

            return penalty;
        }

        public float GetRepairCost(PartType type) 
        { 
            return _repairCost[type];
        }

        public float GetCurrentRepairCost() 
        {
            float cost = 0.0f;

            foreach (var part in _repairedParts) 
            { 
                cost += part.Price + _repairCost[part.Type];
            }

            return cost;
        }

        public void OnPartReplaced(CarPartInfo oldPart, CarPartInfo newPart) 
        {
            if (_brokenParts.Contains(oldPart))
            {
                _repairedParts.Add(newPart);
                _brokenParts.Remove(oldPart);
            }

            if (_brokenParts.Count == 0) 
            { 
                RepairCompleted.Invoke();
            }

            _replacedParts++;
        }
    }
}
