using CarServiceProject.Model.Interfaces;
using CarServiceProject.Utils;

namespace CarServiceProject.Model.Infrastructure
{
    public class CarServiceFactory : ICarServiceFactory
    {
        private CarPartsFactory _partsFactory;
        private CarFactory _carFactory;

        private int _minPartsAtStorage = 1;
        private int _maxPartsAtStorage = 10;

        private int _minClientsCount = 4;
        private int _maxClientsCount = 10;

        private float _initialMoneyAmount = 20000.00f;

        public CarServiceFactory() 
        { 
            _partsFactory = new CarPartsFactory();
            _carFactory = new CarFactory(_partsFactory);
        }

        public ICarService Create()
        {
            var storage = new Storage(_minPartsAtStorage, _maxPartsAtStorage, _partsFactory);

            var clients = CreateClients(_minClientsCount, _maxClientsCount);

            return new CarService(storage, clients, _initialMoneyAmount);
        }

        private Queue<Car> CreateClients(int minClientsCount, int maxClientsCount) 
        { 
            var clients = new Queue<Car>();

            var clientsCount = UserUtils.GetNextInt(minClientsCount, maxClientsCount);

            for (int i = 0; i < clientsCount; i++) 
            {
                var car = _carFactory.Create();
                clients.Enqueue(car);
            }

            return clients;
        }
    }
}
