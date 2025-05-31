namespace AddressBookApp.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Category { get; set; }

        public string Display => $"{FirstName} {LastName} ({PhoneNumber})";
    }
}
