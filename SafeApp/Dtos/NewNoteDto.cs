namespace SafeApp.Dtos
{
    public class NewNoteDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
       // public string OwnerUserName { get; set; } = string.Empty;
        public bool Public { get; set; }
        public bool Encrypted { get; set; }
        public string? Password { get; set; }
    }
}
