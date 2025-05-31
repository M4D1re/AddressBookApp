using System.Windows;
using AddressBookApp.Data;
using AddressBookApp.Models;

namespace AddressBookApp
{
    public partial class AddEditContactWindow : Window
    {
        public Contact Contact { get; private set; }

        public AddEditContactWindow(Contact contact = null)
        {
            InitializeComponent();
            if (contact != null)
            {
                Contact = contact;
                FirstNameBox.Text = contact.FirstName;
                LastNameBox.Text = contact.LastName;
                PhoneBox.Text = contact.PhoneNumber;
                CategoryBox.Text = contact.Category;
            }
            else
            {
                Contact = new Contact();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Contact.FirstName = FirstNameBox.Text.Trim();
            Contact.LastName = LastNameBox.Text.Trim();
            Contact.PhoneNumber = PhoneBox.Text.Trim();
            Contact.Category = CategoryBox.Text.Trim();

            if (string.IsNullOrEmpty(Contact.FirstName) || string.IsNullOrEmpty(Contact.LastName))
            {
                MessageBox.Show("Имя и фамилия обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Contact.Id == 0) // новый контакт
            {
                DatabaseHelper.AddContact(Contact);
            }
            else
            {
                DatabaseHelper.UpdateContact(Contact);
            }

            DialogResult = true;
        }
    }
}
