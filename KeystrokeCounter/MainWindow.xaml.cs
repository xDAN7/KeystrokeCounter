using KeystrokeCounter.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KeystrokeCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public KeyCounter Counter { get; private set; } = new KeyCounter();
        public KeyValuePair<string, ProgramCounter> SelectedCounter { get; set; }

        public MainWindow()
        {
            // Initialize Counter
            Counter = new KeyCounter();
            Resources["AllProcesses"] = Counter.Processes;
            Counter.Enable();
            Counter.Global.Record(MouseButton.Left);

            // Inititalize WPF
            InitializeComponent();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Counter.Disable();
        }

        private void OnSelectedProgramChanged(object sender, SelectionChangedEventArgs e)
        {
            Resources["SelectedCounter"] = (sender as ComboBox).SelectedValue;
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            Counter.Clear();
            SelectedProgram.SelectedIndex = 0;
        }
    }
}
