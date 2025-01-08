using CarServiceProject.Model.Enums;

namespace CarServiceProject.Model.Interfaces
{
    public interface ICarService
    {
        public event Action? RepairCompleted;
        public event Action? RepairAborted;
        public event Action? QueueEmpty;

        public float Balance { get; }
        public int ClientsCount { get; }
        public int CurrentClientNumber { get; }

        public IReadOnlyDictionary<PartType, int> Storage { get; }

        public IEnumerable<ICarPartInfo>? InspectVehicle();

        public bool TryRepair(PartType type, int index);

        public void Abort();

        public bool TryOrderPars(Dictionary<PartType, int> parts);

        public float GetRepairCost(PartType type);

        public float GetCurrentRepairCost();

        public float GetPartPrice(PartType type);

        public float GetCurrentPenalty();

        public bool TryGetPartInfo(int index, out ICarPartInfo? info);

        public int GetStorageCount(PartType type);

        public bool TryServeNextClient();

        public int GetRepairedPartsCount();
    }
}
