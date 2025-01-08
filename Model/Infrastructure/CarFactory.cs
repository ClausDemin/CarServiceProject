using CarServiceProject.Model.Enums;
using CarServiceProject.Utils;

namespace CarServiceProject.Model.Infrastructure
{
    public class CarFactory
    {
        private Dictionary<PartType, int> _carStructure = new Dictionary<PartType, int>() 
        {
            {PartType.Hull, 1 },
            {PartType.Engine, 1},
            {PartType.Wheel, 4 },
            {PartType.Transmission, 1 },
            {PartType.Brakes, 1},
            {PartType.Controls, 1 },
            {PartType.Exhaust, 1 },
        };

        private CarBuilder _builder;

        public CarFactory(CarPartsFactory partsFactory) 
        {
            _builder = new CarBuilder(partsFactory);
        }

        public Car Create(int minBrokenPartsCount = 1) 
        {
            var maxPartsCount = GetPartsCount();

            ArgumentOutOfRangeException.ThrowIfGreaterThan(minBrokenPartsCount, maxPartsCount);

            _builder.Reset();
            
            foreach (var part in _carStructure) 
            {
                for (int i = 0; i < part.Value; i++) 
                {
                    _builder.AddPart(part.Key);
                }
            }

            var brokenPartsCount = UserUtils.Next(minBrokenPartsCount, maxPartsCount);

            _builder.BrokeRandomParts(brokenPartsCount);

            return _builder.Build();
        }

        private int GetPartsCount() 
        {
            int count = 0;

            foreach (var part in _carStructure) 
            {
                count += part.Value;
            }

            return count;
        }
    }
}
