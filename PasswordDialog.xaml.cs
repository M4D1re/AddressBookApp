using System.Windows;

namespace AddressBookApp
{
    public partial class PasswordDialog : Window
    {
        public string Password => PasswordBox.Password;

        public PasswordDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e) => DialogResult = true;
        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
