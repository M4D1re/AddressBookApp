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

        // Флаги для сортировки
        private bool sortByFirstNameAsc = true;
        private bool sortByLastNameAsc = true;
        private enum SortType { None, FirstName, LastName }
        private SortType currentSort = SortType.FirstName;

        public MainWindow(bool isAdmin)
        {
            InitializeComponent();
            this.isAdmin = isAdmin;

            AddButton.IsEnabled = EditButton.IsEnabled = DeleteButton.IsEnabled = isAdmin;

            ContactsList.ItemsSource = allContacts;

            LoadContacts();
        }

        private void LoadContacts()
        {
            allContacts.Clear();
            var contactsFromDb = DatabaseHelper.GetContacts();
            foreach (var contact in contactsFromDb)
                allContacts.Add(contact);

            ApplyFilterAndSort();
        }

        //private void ApplyFilterAndSort()
        //{
        //    // Проверка на null для всех объектов
        //    if (SearchBox == null || allContacts == null)
        //        return;

        //    string text = SearchBox.Text?.Trim() ?? string.Empty;

        //    // Если текст пуст или это текст по умолчанию, фильтрацию не выполняем
        //    if (string.IsNullOrEmpty(text) || text == "Поиск...")
        //    {
        //        // Применяем сортировку
        //        ApplySort();
        //        return;
        //    }

        //    // Фильтрация контактов
        //    var filtered = allContacts.Where(c =>
        //        (!string.IsNullOrEmpty(c.FirstName) && c.FirstName.ToLower().Contains(text.ToLower())) ||
        //        (!string.IsNullOrEmpty(c.LastName) && c.LastName.ToLower().Contains(text.ToLower())) ||
        //        (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.ToLower().Contains(text.ToLower()))
        //    ).ToList();

        //    // Применяем сортировку
        //    ApplySort(filtered);
        //}

        //// Метод для сортировки
        //private void ApplySort(List<Contact> filtered = null)
        //{
        //    // Если фильтрация не применялась, работаем с исходным списком
        //    var contactsToSort = filtered ?? allContacts.ToList();

        //    switch (currentSort)
        //    {
        //        case SortType.FirstName:
        //            contactsToSort = sortByFirstNameAsc
        //                ? contactsToSort.OrderBy(c => c.FirstName).ToList()
        //                : contactsToSort.OrderByDescending(c => c.FirstName).ToList();
        //            break;
        //        case SortType.LastName:
        //            contactsToSort = sortByLastNameAsc
        //                ? contactsToSort.OrderBy(c => c.LastName).ToList()
        //                : contactsToSort.OrderByDescending(c => c.LastName).ToList();
        //            break;
        //    }

        //    // Обновляем ItemsSource
        //    ContactsList.ItemsSource = contactsToSort;
        //}

        private void ApplyFilterAndSort()
        {
            // Проверка на null для всех объектов
            if (SearchBox == null || allContacts == null)
            {
                MessageBox.Show("Поиск не может быть выполнен: объект SearchBox или все контакты не инициализированы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string text = SearchBox.Text?.Trim() ?? string.Empty;

            // Если текст пуст или это текст по умолчанию, фильтрацию не выполняем
            if (string.IsNullOrEmpty(text) || text == "Поиск...")
            {
                // Применяем сортировку
                ApplySort();
                return;
            }

            // Фильтрация контактов
            var filtered = allContacts.Where(c =>
                (!string.IsNullOrEmpty(c.FirstName) && c.FirstName.ToLower().Contains(text.ToLower())) ||
                (!string.IsNullOrEmpty(c.LastName) && c.LastName.ToLower().Contains(text.ToLower())) ||
                (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.ToLower().Contains(text.ToLower()))
            ).ToList();

            // Если после фильтрации список пуст
            if (filtered.Count == 0)
            {
                MessageBox.Show("Не найдено ни одного контакта, соответствующего запросу.", "Результаты поиска", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Применяем сортировку
            ApplySort(filtered);
        }

        // Метод для сортировки
        private void ApplySort(List<Contact> filtered = null)
        {
            // Если фильтрация не применялась, работаем с исходным списком
            var contactsToSort = filtered ?? allContacts.ToList();

            // Если contactsToSort пустой, выведем сообщение
            if (contactsToSort.Count == 0)
            {
                MessageBox.Show("Список контактов пуст. Необходимо добавить контакты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Включение отладочных сообщений для сортировки
           // MessageBox.Show($"Применяется сортировка по: {currentSort}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

            switch (currentSort)
            {
                case SortType.FirstName:
                    contactsToSort = sortByFirstNameAsc
                        ? contactsToSort.OrderBy(c => c.FirstName).ToList()
                        : contactsToSort.OrderByDescending(c => c.FirstName).ToList();
                    break;
                case SortType.LastName:
                    contactsToSort = sortByLastNameAsc
                        ? contactsToSort.OrderBy(c => c.LastName).ToList()
                        : contactsToSort.OrderByDescending(c => c.LastName).ToList();
                    break;
            }

            // Обновляем ItemsSource
            ContactsList.ItemsSource = contactsToSort;
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

        private void SortByFirstNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSort == SortType.FirstName)
                sortByFirstNameAsc = !sortByFirstNameAsc;
            else
            {
                currentSort = SortType.FirstName;
                sortByFirstNameAsc = true;
            }

            SortByFirstNameButton.Content = $"Сортировать по имени {(sortByFirstNameAsc ? "↑" : "↓")}";
            SortByLastNameButton.Content = "Сортировать по фамилии";

            ApplyFilterAndSort();
        }

        private void SortByLastNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSort == SortType.LastName)
                sortByLastNameAsc = !sortByLastNameAsc;
            else
            {
                currentSort = SortType.LastName;
                sortByLastNameAsc = true;
            }

            SortByLastNameButton.Content = $"Сортировать по фамилии {(sortByLastNameAsc ? "↑" : "↓")}";
            SortByFirstNameButton.Content = "Сортировать по имени";

            ApplyFilterAndSort();
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
