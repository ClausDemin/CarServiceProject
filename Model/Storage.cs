using CarServiceProject.Model.Enums;
using CarServiceProject.Model.Infrastructure;
using CarServiceProject.Utils;

namespace CarServiceProject.Model
{
    public class Storage
    {
        private Dictionary<CarPart, int> _parts;
        private CarPartsFactory _partsFactory;

        public Storage(int minPartsCount, int maxPartsCount, CarPartsFactory partsFactory)
        {
            _parts = new Dictionary<CarPart, int>();
            _partsFactory = partsFactory;

            Init(minPartsCount, maxPartsCount);
        }

        public event Action PartsCountChanged = delegate { };

        public IReadOnlyDictionary<PartType, int> Parts => GetParts();

        public bool TryTake(PartType type, out CarPart? part)
        {
            part = Find(type);
            Remove(part);

            return part != null;
        }

        public void OrderParts(Dictionary<PartType, int> parts)
        {
            foreach (var part in parts)
            {
                Add(part.Key, part.Value);
            }

            PartsCountChanged.Invoke();
        }

        public float GetPrice(PartType type) 
        { 
            return _partsFactory.GetPrice(type);
        }

        public int GetAmount(PartType type) 
        { 
            var part = _partsFactory.Create(type);

            if (_parts.TryGetValue(part, out var count)) 
            { 
                return count;
            }

            return 0;
        }

        private void Add(PartType type, int count)
        {
            var part = _partsFactory.Create(type);

            if (_parts.ContainsKey(part))
            {
                _parts[part] += count;
            }
            else
            {
                _parts[part] = count;
            }
        }

        private void Remove(CarPart? part)
        {
            if (part != null)
            {
                if (_parts.ContainsKey(part))
                {
                    _parts[part]--;

                    if (_parts[part] <= 0)
                    {
                        _parts.Remove(part);
                    }
                }

                PartsCountChanged.Invoke();
            }
        }

        private void Init(int minPartsCount, int maxPartsCount)
        {
            foreach (var partType in GetPartTypes())
            {
                var count = UserUtils.Next(minPartsCount, maxPartsCount);
                var part = _partsFactory.Create(partType);

                _parts[part] = count;
            }
        }

        private CarPart? Find(PartType type)
        {
            var part = _parts.Keys.Where(part => part.Type == type).FirstOrDefault();

            return part;
        }

        private IReadOnlyDictionary<PartType, int> GetParts()
        {
            return _parts
                .Select(part => KeyValuePair.Create(part.Key.Type, part.Value))
                .ToDictionary();
        }

        private List<PartType> GetPartTypes()
        {
            var types = Enum.GetValues<PartType>().Where(type => type != PartType.None).ToList();

            return types;
        }
    }
}
