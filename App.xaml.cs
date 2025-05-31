using System.Windows;
using AddressBookApp.Data;
using SQLitePCL;


namespace AddressBookApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            Batteries.Init();
            DatabaseHelper.InitializeDatabase();



            if (!DatabaseHelper.IsAdminPasswordSet())
            {
                MainWindow mainWindow = new MainWindow(isAdmin: true);
                mainWindow.Show();
            }
            else
            {
                LoginWindow login = new LoginWindow();
                login.Show();
            }
        }
    }
}
