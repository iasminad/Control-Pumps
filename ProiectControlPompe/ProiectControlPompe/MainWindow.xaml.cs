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

namespace Simulator
{

    public partial class MainWindow : Window
    {
        public SistemControlViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new SistemControlViewModel();
            DataContext = ViewModel;
        }

        private void StopAll_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SistemControl.OpresteToatePompele();
            ViewModel.UpdateUI();
        }

        private void Pompa1_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SistemControl.Pompe[0].Porneste();
            ViewModel.UpdateUI();
        }

        private void Pompa2_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SistemControl.Pompe[1].Porneste();
            ViewModel.UpdateUI();
        }

        private void Pompa3_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SistemControl.Pompe[2].Porneste();
            ViewModel.UpdateUI();
        }

        private void Pompa4_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SistemControl.Pompe[3].Porneste();
            ViewModel.UpdateUI();
        }

        private void SenzorB2_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the TextBox that triggered the event
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            // Try to parse the float value from the textbox
            if (float.TryParse(textBox.Text, System.Globalization.NumberStyles.Float,
                               System.Globalization.CultureInfo.InvariantCulture, out float inputValue))
            {
                float rata = inputValue * 0.01f;

                // Subtract from SenzorB2 pressure
                ViewModel.SistemControl.SenzorB2.PresiuneCurenta -= rata;

                if (ViewModel.SistemControl.SenzorB2.PresiuneCurenta < 0)
                    ViewModel.SistemControl.SenzorB2.PresiuneCurenta = 0;

                // Update SenzorB1 to match SenzorB2
                ViewModel.SistemControl.SenzorB1.PresiuneCurenta = ViewModel.SistemControl.SenzorB2.PresiuneCurenta;

                ViewModel.StatusMesaj = $"Presiune scăzută manual cu rata {rata:F2}";
                ViewModel.UpdateUI();
            }
            else
            {
                // Optional: handle invalid input
                ViewModel.StatusMesaj = "Valoare invalidă pentru rată (introdu un număr valid).";
            }
        }
    }
}