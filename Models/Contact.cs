namespace DormAPI.Models
{
    public class Contact
    {
        public int contactId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public int? RelationshipTypeID { get; set; }
        public string? RelationshipType { get; set; }
    }
}
// contact
