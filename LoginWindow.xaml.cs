using System.Windows;
using AddressBookApp.Data;

namespace AddressBookApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void UserLogin_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(isAdmin: false).Show();
            Close();
        }

        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем: установлен ли пароль администратора
            if (!DatabaseHelper.IsAdminPasswordSet())
            {
                // Пароль не установлен — сразу в админский режим
                new MainWindow(isAdmin: true).Show();
                Close();
                return;
            }

            // Пароль установлен — запрашиваем ввод
            var dialog = new PasswordDialog();
            if (dialog.ShowDialog() == true)
            {
                if (DatabaseHelper.ValidateAdminPassword(dialog.Password))
                {
                    new MainWindow(isAdmin: true).Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
