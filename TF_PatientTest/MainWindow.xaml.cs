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

namespace TF_PatientTest {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private PatientsDB backend = new PatientsDB ();
        private List<Patient> patients;

        public MainWindow () {
            InitializeComponent ();

            patients = backend.GetPatientsInfo ();

            lstPatients.ItemsSource = patients;
        }

        private void AddPatient (object sender, SavePatientEventArgs args) {
            /* Update Database */
            backend.SavePatient (args.patient);
            patients.Add (args.patient);
            lstPatients.Items.Refresh ();
        }

        private void SavePatient (object sender, SavePatientEventArgs args) {
            /* Update DataBase */
            backend.SavePatient (args.patient);
        }

        private void Button_AddPatient_Click (object sender, RoutedEventArgs e) {
            Patient p = new Patient ();
            PatientDetailsWindow winPatientDetails = new PatientDetailsWindow (p, true);
            winPatientDetails.SavePatient += AddPatient;
            winPatientDetails.Owner = Application.Current.MainWindow;
            winPatientDetails.Show ();
        }
        private void Button_DeletePatient_Click (object sender, RoutedEventArgs e) {
            Button self = sender as Button;
            Patient p = (Patient) self.DataContext;
            backend.DeletePatient (p);
            patients.Remove (p);
            lstPatients.Items.Refresh ();
        }

        private void lstPatients_MouseDoubleClick (object sender, MouseButtonEventArgs e) {
            if (lstPatients.SelectedIndex < 0)
                return;
            Patient p = patients.ElementAt (lstPatients.SelectedIndex);
            PatientDetailsWindow window = new TF_PatientTest.PatientDetailsWindow (p, false);
            window.SavePatient += SavePatient;
            window.Owner = Application.Current.MainWindow;
            window.Show ();
        }

        private void Button_Close_Click (object sender, RoutedEventArgs e) {
            System.Windows.Application.Current.Shutdown ();
        }

    }
}