namespace CarServiceProject.Model.Interfaces
{
    public interface ICarPartInfo
    {
        public string[] GetInfo();

        public string GetPartType();

        public string GetState();
    }
}
