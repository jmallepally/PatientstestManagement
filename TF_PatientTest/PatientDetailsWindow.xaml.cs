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
using System.Windows.Shapes;

namespace TF_PatientTest {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PatientDetailsWindow : Window {
        private Patient _patient;
        public event EventHandler<SavePatientEventArgs> SavePatient;

        public PatientDetailsWindow (Patient p, bool editable) {
            InitializeComponent ();
            _patient = p;
            frmDetails.IsEnabled = editable;
            txtName.DataContext = p;
            dpDoB.DataContext = p;
            txtAddress.DataContext = p;

            dgTests.ItemsSource = p.Tests;

            if (editable) {
                btnEdit.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Visible;
            } else {
                btnSave.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Visible;
            }
        }

        private void btnEdit_Click (object sender, RoutedEventArgs e) {
            frmDetails.IsEnabled = true;
            btnEdit.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Visible;
        }

        private void btnSave_Click (object sender, RoutedEventArgs e) {
            //if (!Validation.GetHasError(txtName))
            //{
            //    System.Windows.MessageBox.Show("Patient name(" + txtName.Text + ") cannot be empty");
            //    txtName.Focus();
            //    return;
            //}
            _patient.Name = txtName.Text;
            _patient.Address = txtAddress.Text;
            _patient.DoB = dpDoB.SelectedDate.Value;

            SavePatientEventArgs args = new SavePatientEventArgs ();
            args.patient = _patient;
            SavePatient (this, args);
            this.Close ();
        }

        private void btnClose_Click (object sender, RoutedEventArgs e) {
            //FIXME: Show confirmation alert ?
            this.Close ();
        }
    }

    public class SavePatientEventArgs : EventArgs {
        public Patient patient { set; get; }
    }

    public class EmptyTextValidationRule : ValidationRule {
        public override ValidationResult Validate (object value, System.Globalization.CultureInfo cultureInfo) {
            if (value.ToString ().Trim () != "")
                return new ValidationResult (true, null);

            return new ValidationResult (false, "Field cannot be empty.");
        }
    }
}