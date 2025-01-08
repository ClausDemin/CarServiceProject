using CarServiceProject.Model.Enums;
using CarServiceProject.Utils;

namespace CarServiceProject.Model.Infrastructure
{
    public class CarBuilder
    {
        private List<CarPart> _parts;
        private CarPartsFactory _partsFactory;

        public CarBuilder(CarPartsFactory partsFactory)
        {
            _parts = new List<CarPart>();
            _partsFactory = partsFactory;
        }

        public CarBuilder Reset()
        {
            _parts.Clear();

            return this;
        }

        public CarBuilder AddPart(PartType part)
        {
            _parts.Add(_partsFactory.Create(part));

            return this;
        }

        public CarBuilder BrokeRandomParts(int count)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0);

            if (count > _parts.Count)
            {
                foreach (var part in _parts)
                {
                    part.Broke();
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    BrokeRandomPart();
                }
            }

            return this;
        }

        public Car Build()
        {
            return new Car(_parts);
        }

        private void BrokeRandomPart()
        {
            bool isBroken = false;

            while (isBroken == false)
            {
                var index = UserUtils.Next(0, _parts.Count);
                var part = _parts[index];

                if (part.State == PartState.WorkingProperly)
                {
                    part.Broke();
                    isBroken = true;
                }
            }
        }
    }
}
