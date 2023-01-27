using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeystrokeCounter.Collections
{

    public enum KeyStrokeType
    {
        Global,
        Keyboard,
        Mouse
    }

    public struct KeyStroke
    {

        public KeyStrokeType Type { get; private set; }
        public ushort Value { get; private set; }
        public string Name { get; private set; }

        private KeyStroke(KeyStrokeType type, ushort value, string name)
        {
            Type = type;
            Value = value;
            Name = name;
        }

        public static KeyStroke Of(MouseButton button)
        {
            return new KeyStroke(KeyStrokeType.Mouse, (ushort)button, button.ToString());
        }

        public static KeyStroke Of(Key key)
        {
            return new KeyStroke(KeyStrokeType.Keyboard, (ushort)key, key.ToString());
        }

        private static KeyStroke _global = new KeyStroke(KeyStrokeType.Global, 0, "All Keys");
        public static KeyStroke Global => _global;

    }

}
