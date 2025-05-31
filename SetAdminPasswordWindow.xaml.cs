using System.Windows;
using AddressBookApp.Data;

namespace AddressBookApp
{
    public partial class SetAdminPasswordWindow : Window
    {
        public SetAdminPasswordWindow()
        {
            InitializeComponent();
        }

        private void SetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (RemovePasswordCheckBox.IsChecked == true)
            {
                DatabaseHelper.SetAdminPassword(string.Empty);
                MessageBox.Show("Пароль администратора удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
                return;
            }

            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пароль не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DatabaseHelper.SetAdminPassword(password);
            MessageBox.Show("Пароль успешно установлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
