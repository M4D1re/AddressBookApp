using System;
using System.Collections.Generic;
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
        private readonly bool isAdmin;
        private readonly ObservableCollection<Contact> allContacts;

        private enum SortType { None, FirstName, LastName }
        private SortType currentSort = SortType.None;
        private bool sortByFirstNameAsc = true;
        private bool sortByLastNameAsc = true;

        public MainWindow(bool isAdmin)
        {
            InitializeComponent();
            this.isAdmin = isAdmin;
            allContacts = new ObservableCollection<Contact>();

            // Настройка доступности кнопок
            AddButton.IsEnabled = isAdmin;
            EditButton.IsEnabled = isAdmin;
            DeleteButton.IsEnabled = isAdmin;
            AdminSettingsButton.IsEnabled = isAdmin;

            LoadContacts();
        }

        private void LoadContacts()
        {
            try
            {
                allContacts.Clear();
                var contacts = DatabaseHelper.GetContacts() ?? Enumerable.Empty<Contact>();
                foreach (var contact in contacts)
                {
                    allContacts.Add(contact);
                }
                ApplyFilterAndSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке контактов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilterAndSort()
        {
            if (SearchBox == null || allContacts == null) return;

            string searchText = SearchBox.Text?.Trim() ?? string.Empty;
            bool isDefaultSearchText = searchText == "Поиск..." || string.IsNullOrEmpty(searchText);

            // Фильтрация
            IEnumerable<Contact> filteredContacts = allContacts;

            if (!isDefaultSearchText)
            {
                filteredContacts = filteredContacts.Where(c =>
                    (c.FirstName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.LastName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.PhoneNumber?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            // Сортировка
            IOrderedEnumerable<Contact> sortedContacts = null;
            switch (currentSort)
            {
                case SortType.FirstName:
                    sortedContacts = sortByFirstNameAsc
                        ? filteredContacts.OrderBy(c => c.FirstName)
                        : filteredContacts.OrderByDescending(c => c.FirstName);
                    break;
                case SortType.LastName:
                    sortedContacts = sortByLastNameAsc
                        ? filteredContacts.OrderBy(c => c.LastName)
                        : filteredContacts.OrderByDescending(c => c.LastName);
                    break;
                default:
                    sortedContacts = filteredContacts.OrderBy(c => 0); // Без сортировки, но сохраняем тип IOrderedEnumerable
                    break;
            }

            // Обновление UI
            ContactsList.ItemsSource = sortedContacts.ToList();

            // Сообщение о пустом результате поиска
            if (!isDefaultSearchText && !filteredContacts.Any())
            {
                MessageBox.Show("Контакты не найдены", "Результаты поиска",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
            else
            {
                MessageBox.Show("Выберите контакт для редактирования", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Contact selected)
            {
                if (MessageBox.Show($"Удалить контакт {selected.FirstName} {selected.LastName}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DatabaseHelper.DeleteContact(selected.Id);
                    LoadContacts();
                }
            }
            else
            {
                MessageBox.Show("Выберите контакт для удаления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SortByFirstNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSort == SortType.FirstName)
            {
                sortByFirstNameAsc = !sortByFirstNameAsc;
            }
            else
            {
                currentSort = SortType.FirstName;
                sortByFirstNameAsc = true;
            }

            SortByFirstNameButton.Content = $"Имя {(sortByFirstNameAsc ? "↑" : "↓")}";
            SortByLastNameButton.Content = "Фамилия";
            ApplyFilterAndSort();
        }

        private void SortByLastNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSort == SortType.LastName)
            {
                sortByLastNameAsc = !sortByLastNameAsc;
            }
            else
            {
                currentSort = SortType.LastName;
                sortByLastNameAsc = true;
            }

            SortByLastNameButton.Content = $"Фамилия {(sortByLastNameAsc ? "↑" : "↓")}";
            SortByFirstNameButton.Content = "Имя";
            ApplyFilterAndSort();
        }

        private void AdminSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var passwordWindow = new SetAdminPasswordWindow();
            passwordWindow.Owner = this;
            passwordWindow.ShowDialog();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilterAndSort();
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