namespace AddressBookApp.Models
{
    public class ContactItem
    {
        public string FullName => $"{Contact.FirstName} {Contact.LastName}";
        public Contact Contact { get; set; }
    }
}
