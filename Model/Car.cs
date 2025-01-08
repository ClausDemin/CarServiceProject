using CarServiceProject.Model.Enums;

namespace CarServiceProject.Model
{
    public class Car
    {
        private List<CarPart> _parts;

        public Car(List<CarPart> parts) 
        { 
            _parts = new List<CarPart>(parts);
        }

        public event Action<CarPartInfo, CarPartInfo>? PartReplaced;

        public IEnumerable<CarPartInfo> PartsInfo => GetInfo();

        public bool TryReplacePart(CarPart part, int index) 
        {
            if (index >= 0 && index < _parts.Count)
            {
                var oldPartInfo = _parts[index].GetInfo();
                var partInfo = part.GetInfo();

                _parts[index] = part;

                PartReplaced?.Invoke(oldPartInfo, partInfo);

                return true;
            }

            return false;
        }

        public bool TryGetPartInfo(int index, out CarPartInfo? info) 
        {
            if (index >= 0 && index < _parts.Count) 
            {
                info = new CarPartInfo(_parts[index]);

                return true;
            }

            info = default;

            return false;
        }

        private IEnumerable<CarPartInfo> GetInfo() 
        {
            return _parts.Select(part => part.GetInfo());
        }
    }
}
