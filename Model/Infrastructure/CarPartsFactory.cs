using CarServiceProject.Model.Enums;

namespace CarServiceProject.Model.Infrastructure
{
    public class CarPartsFactory
    {
        private Dictionary<PartType, float> _partCatalogue = new Dictionary<PartType, float>()
        {
            {PartType.Hull, 250000.00f },
            {PartType.Engine, 150000.00f},
            {PartType.Wheel, 15000.00f },
            {PartType.Transmission, 120000.00f },
            {PartType.Brakes,  100000.00f},
            {PartType.Controls, 75000.00f},
            {PartType.Exhaust, 50000.00f }
        };

        public CarPart Create(PartType type)
        {
            return new CarPart(type, _partCatalogue[type]);
        }

        public float GetPrice(PartType type) 
        {
            return _partCatalogue[type];
        }
    }
}
