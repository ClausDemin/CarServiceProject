using CarServiceProject.Model.Infrastructure;
using CarServiceProject.Presenter;
using CarServiceProject.View;
using System.Drawing;

namespace CarServiceProject
{
    public class Program
    {
        public static void Main()
        {
            Console.CursorVisible = false;

            var factory = new CarServiceFactory();

            var service = factory.Create();

            var presenter = new CarServicePresenter(service);

            var mainWindow = new MainWindow(new Point(0, 0), presenter);

            mainWindow.Run();
        }
    }
}
