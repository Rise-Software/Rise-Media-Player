using Windows.UI.Core;

namespace Rise.App.Helpers
{
    public class KeyboardHelpers
    {
        public static bool IsCtrlPressed()
        {
            CoreVirtualKeyStates state = CoreWindow.GetForCurrentThread().GetKeyState(Windows.System.VirtualKey.Control);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}
