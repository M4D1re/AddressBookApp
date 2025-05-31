using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AddressBookApp.Data;
using AddressBookApp.Models;

namespace AddressBookApp
{
    public partial class MainWindow : Window
    {
        private bool isAdmin;
        private ObservableCollection<Contact> allContacts = new ObservableCollection<Contact>();

        public MainWindow(bool isAdmin)
        {
            InitializeComponent();
            this.isAdmin = isAdmin;

            AddButton.IsEnabled = EditButton.IsEnabled = DeleteButton.IsEnabled = isAdmin;
            ManagePasswordButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            ContactsList.ItemsSource = allContacts;

            LoadContacts();
        }

        private void LoadContacts()
        {
            allContacts.Clear();
            var contactsFromDb = DatabaseHelper.GetContacts();
            foreach (var contact in contactsFromDb)
                allContacts.Add(contact);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditContactWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadContacts();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Contact selected)
            {
                var dialog = new AddEditContactWindow(selected);
                if (dialog.ShowDialog() == true)
                {
                    LoadContacts();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Contact selected)
            {
                if (MessageBox.Show($"Удалить {selected.FirstName} {selected.LastName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DatabaseHelper.DeleteContact(selected.Id);
                    LoadContacts();
                }
            }
        }

        private void ManagePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SetAdminPasswordWindow();
            dialog.ShowDialog();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (SearchBox == null || allContacts == null)
            //    return;

            //string text = SearchBox.Text;

            //if (string.IsNullOrEmpty(text) || text.ToLower() == "поиск...")
            //{
            //    ContactsList.ItemsSource = allContacts;
            //    return;
            //}

            //string searchText = text.ToLower();

            //var filtered = allContacts.Where(c =>
            //{
            //    bool firstNameMatch = !string.IsNullOrEmpty(c.FirstName) && c.FirstName.ToLower().Contains(searchText);
            //    bool lastNameMatch = !string.IsNullOrEmpty(c.LastName) && c.LastName.ToLower().Contains(searchText);
            //    bool phoneMatch = !string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.ToLower().Contains(searchText);
            //    bool categoryMatch = !string.IsNullOrEmpty(c.Category) && c.Category.ToLower().Contains(searchText);
            //    return firstNameMatch || lastNameMatch || phoneMatch || categoryMatch;
            //}).ToList();

            //ContactsList.ItemsSource = filtered;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Поиск...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Поиск...";
                SearchBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
