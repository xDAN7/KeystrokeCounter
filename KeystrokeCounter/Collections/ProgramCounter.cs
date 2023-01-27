using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeystrokeCounter.Collections
{
    public partial class ProgramCounter : ObservableConcurrentDictionary<KeyStroke, uint>
    {

        public void Record(Key key)
        {
            //var exists = TryGetValue(KeyStroke.Global, out uint value);
            //this[KeyStroke.Global] = exists ? value + 1 : 1;
            //var stroke = KeyStroke.Of(key);
            //exists = TryGetValue(stroke, out value);
            //this[stroke] = exists ? value + 1 : 1;

            AddOrUpdate(KeyStroke.Global, 1, (k, v) => v + 1);
            AddOrUpdate(KeyStroke.Of(key), 1, (k, v) => v + 1);
        }

        public void Record(MouseButton key)
        {
            //var exists = TryGetValue(KeyStroke.Global, out uint value);
            //this[KeyStroke.Global] = exists ? value + 1 : 1;
            //var stroke = KeyStroke.Of(key);
            //exists = TryGetValue(stroke, out value);
            //this[stroke] = exists ? value + 1 : 1;

            AddOrUpdate(KeyStroke.Global, 1, (k, v) => v + 1);
            AddOrUpdate(KeyStroke.Of(key), 1, (k, v) => v + 1);
        }

    }
    
}
