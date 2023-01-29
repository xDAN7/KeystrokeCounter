using KeystrokeCounter.Collections;
using KeystrokeCounter.Intercepts;
using System;
using System.Windows.Input;

namespace KeystrokeCounter
{
    public class KeyCounter
    {

        private InterceptForeground _interceptForeground;
        private InterceptKeys _interceptKeys;
        private InterceptMouse _interceptMouse;

        public KeyCounter()
        {
            _interceptForeground = new InterceptForeground(this);
            _interceptKeys = new InterceptKeys(this);
            _interceptMouse = new InterceptMouse(this);
            Processes.Add("Global", _global);
        }

        public void Enable()
        {
            _interceptForeground.Enable();
            _interceptKeys.Enable();
            _interceptMouse.Enable();
        }

        public void Disable()
        {
            _interceptMouse.Disable();
            _interceptKeys.Disable();
            _interceptForeground.Disable();
        }

        private string _foreground = "Unknown";
        private ProgramCounter _global = new ProgramCounter();
        public ProgramCounter Global => _global;
        public ObservableConcurrentDictionary<string, ProgramCounter> Processes { get; private set; } = new ObservableConcurrentDictionary<string, ProgramCounter>();

        public void Record(Key key)
        {
            if (!Processes.TryGetValue(_foreground, out var programCounter))
            {
                programCounter = new ProgramCounter();
                Processes.Add(_foreground, programCounter);
            }
            programCounter.Record(key);
            _global.Record(key);
            Console.WriteLine(key);
        }

        public void Record(MouseButton key)
        {
            if (!Processes.TryGetValue(_foreground, out var programCounter))
            {
                programCounter = new ProgramCounter();
                Processes.Add(_foreground, programCounter);
            }
            programCounter.Record(key);
            _global.Record(key);
            Console.WriteLine(key);
        }

        public void SetForeground(string process)
        {
            _foreground = process;
            Console.WriteLine("Foreground: " + process);
        }

        public void Clear()
        {
            Processes.Clear();
            _global.Clear();
            Processes.Add("Global", _global);
        }

    }
}
