using CarServiceProject.Model.Enums;
using CarServiceProject.Model.Interfaces;

namespace CarServiceProject.Model
{
    public class CarPartInfo: ICarPartInfo
    {
        public CarPartInfo(PartType type, float price, PartState state) 
        { 
            Type = type;
            Price = price;
            State = state;
        }

        public CarPartInfo(CarPart part)
        {
            Type = part.Type;
            Price = part.Price;
            State = part.State;
        }

        public PartType Type { get; }
        public float Price { get; }
        public PartState State { get; }

        public string[] GetInfo() 
        {
            var result = new List<string>()
            {
                GetPartType(),
                GetState()
            };

            return result.ToArray();
        }

        public string GetPartType() 
        { 
            return Type.ToString();
        }

        public string GetState() 
        { 
            return State.ToString();
        }

        public override string ToString()
        {
            string result = $"Type: {Type}, Price: {Price}, State: {State}";

            return result;
        }

        public override bool Equals(object? obj)
        {
            var info = obj as CarPartInfo;

            return (info.Type == Type && info.State == State);
        }

        public override int GetHashCode() 
        {
            return (int)Type;
        }
    }
}
