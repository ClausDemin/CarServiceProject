using CarServiceProject.Model.Enums;

namespace CarServiceProject.Model
{
    public class CarPart
    {
        public CarPart(PartType type, float price)
        {
            Type = type;
            Price = price;
            State = PartState.WorkingProperly;
        }

        public PartType Type { get; }
        public float Price { get; }
        public PartState State { get; private set; }

        public void Broke() 
        { 
            State = PartState.Broken;
        }

        public CarPartInfo GetInfo() 
        {
            return new CarPartInfo(Type, Price, State);
        }

        public override int GetHashCode()
        {
            return (int) Type;
        }

        public override bool Equals(object? obj)
        {
            return (obj as CarPart)?.Type == Type;
        }
    }
}
