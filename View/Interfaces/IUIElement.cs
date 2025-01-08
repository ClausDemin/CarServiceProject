using System.Drawing;

namespace CarServiceProject.View.Interfaces
{
    public interface IUIElement
    {
        public Point Position { get; set; }
        public int Width { get; }
        public int Height { get; }
        public int ScreenWidth { get; }

        public void Draw();
    }
}
