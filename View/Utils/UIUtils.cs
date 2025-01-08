using CarServiceProject.View.Interfaces;
using CarServiceProject.View.TableView.Enums;
using System.Drawing;

namespace CarServiceProject.View.Utils
{
    public static class UIUtils
    {
        private delegate void AlignmentHandler(string text, int length, out int before, out int after);

        private static Dictionary<TextAlignment, AlignmentHandler> _alignmentHandlers =
            new Dictionary<TextAlignment, AlignmentHandler>()
            {
                { TextAlignment.Left, AlignAtLeft },
                { TextAlignment.Right, AlignAtRight },
                { TextAlignment.Center, AlignAtCenter }
            };

        public static Point GetPositionAfter(IUIElement element, int offset = 0)
        {
            var position = new Point(element.Position.X + element.ScreenWidth + offset, element.Position.Y);

            return position;
        }

        public static Point GetPositionAbove(IUIElement below, IUIElement above, int offset = 0)
        {
            var position = new Point(below.Position.X, below.Position.Y - above.Height - offset);

            return position;
        }

        public static Point GetPositionBelow(IUIElement parent, int offset = 0)
        {
            var position = new Point(parent.Position.X, parent.Position.Y + parent.Height + offset);

            return position;
        }

        public static void AlignText
        (
                TextAlignment alignmentOption,
                string text,
                int length,
                out int whiteSpacesBefore,
                out int whiteSpacesAfter
        )
        {
            _alignmentHandlers[alignmentOption].Invoke(text, length, out whiteSpacesBefore, out whiteSpacesAfter);
        }

        private static void AlignAtLeft(string text, int length, out int whiteSpacesBefore, out int whiteSpacesAfter)
        {
            whiteSpacesBefore = 0;

            whiteSpacesAfter = length - text.Length;
        }

        private static void AlignAtRight(string text, int length, out int whiteSpacesBefore, out int whiteSpacesAfter)
        {
            whiteSpacesBefore = length - text.Length; ;

            whiteSpacesAfter = 0;
        }

        private static void AlignAtCenter(string text, int length, out int whiteSpacesBefore, out int whiteSpacesAfter)
        {
            int whiteSpacesCount = length - text.Length;

            if (IsEven(whiteSpacesCount))
            {
                whiteSpacesBefore = whiteSpacesCount / 2;
                whiteSpacesAfter = whiteSpacesBefore;
            }
            else
            {
                whiteSpacesBefore = whiteSpacesCount / 2;
                whiteSpacesAfter = whiteSpacesCount / 2 + 1;
            }
        }

        private static bool IsEven(int value)
        {
            return value % 2 == 0;
        }
    }
}
