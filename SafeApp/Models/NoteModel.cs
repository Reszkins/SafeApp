namespace SafeApp.Models
{
    public class NoteModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Public { get; set; }
        public int OwnerId { get; set; }
        public int Encrypted { get; set; }
        public byte[]? PasswordHash { get; set; } 
    }
}
