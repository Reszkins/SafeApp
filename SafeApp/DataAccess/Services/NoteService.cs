using SafeApp.Dtos;
using SafeApp.Models;

namespace SafeApp.DataAccess.Services
{
    public interface INoteService
    {
        Task<List<NoteDto>> GetUserNotes(int userId);
        Task<List<NoteDto>> GetAllPublicNotes();
        Task AddNote(NoteModel newNote);
        Task<NoteDto?> GetNote(int id);
        Task<byte[]?> GetPasswordHash(int id);
    }

    public class NoteService : INoteService
    {
        private readonly ISqlDataAccess _db;
        public NoteService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<List<NoteDto>> GetUserNotes(int userId)
        {
            var sql = "SELECT * FROM Notes WHERE OwnerId = @UserId";
            var parameters = new Dictionary<string, object> { { "@UserId", userId } };

            var notes = await _db.LoadData<NoteModel>(sql, parameters);
            List<NoteDto> result = new List<NoteDto>();

            foreach (var note in notes)
            {
                result.Add(new NoteDto 
                { 
                    Id = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    Public = note.Public,
                    Encrypted = note.Encrypted,
                });
            }

            return result;
        }

        public async Task<List<NoteDto>> GetAllPublicNotes()
        {
            var sql = "SELECT * FROM Notes WHERE Public = 1";
            var parameters = new Dictionary<string, object>();

            var notes = await _db.LoadData<NoteModel>(sql, parameters);
            List<NoteDto> result = new List<NoteDto>();

            foreach (var note in notes)
            {
                result.Add(new NoteDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    Public = note.Public,
                    Encrypted = note.Encrypted,
                });
            }

            return result;
        }

        public async Task AddNote(NoteModel newNote)
        {
            var sql = "INSERT INTO Notes (Title, Content, Public, OwnerId, Encrypted, PasswordHash)" +
                "VALUES (@Title, @Content, @Public, @OwnerId, @Encrypted, @PasswordHash)";

            var parameters = new Dictionary<string, object> {
                { "@Title", newNote.Title },
                { "@Content", newNote.Content },
                { "@Public", newNote.Public },
                { "@OwnerId", newNote.OwnerId },
                { "@Encrypted", newNote.Encrypted },
                { "@PasswordHash", newNote.PasswordHash },
            };

            await _db.SaveData(sql, parameters);
        }

        public async Task<NoteDto?> GetNote(int id)
        {
            var sql = "SELECT * FROM Notes WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id } };

            var notes = await _db.LoadData<NoteModel>(sql, parameters);
            var note = notes.FirstOrDefault();

            if (note == null) return null;

            var result = new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                Public = note.Public,
                Encrypted = note.Encrypted,
            };

            return result;
        }

        public async Task<byte[]?> GetPasswordHash(int id)
        {
            var sql = "SELECT PasswordHash FROM Notes WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id } };

            var result = await _db.LoadData<byte[]>(sql, parameters);

            return result.FirstOrDefault();
        }
    }
}
