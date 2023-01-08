using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeApp.DataAccess.Services;
using SafeApp.Dtos;
using SafeApp.Models;
using SafeApp.Utils;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SafeApp.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly INoteService _noteService;
        private readonly IStringEncrypter _stringEncrypter;  
        public NoteController(INoteService noteService, IAccountService accountService, IStringEncrypter stringEncrypter)
        {
            _noteService = noteService;
            _accountService = accountService;
            _stringEncrypter = stringEncrypter;
        }

        [HttpGet, Route("public")]
        [Authorize]
        public async Task<IActionResult> GetAllPublicNotes()
        {
            var notes = await _noteService.GetAllPublicNotes();

            return Ok(notes);
        }

        [HttpGet, Route("usernotes")]
        [Authorize]
        public async Task<IActionResult> GetUserNotes()
        {
            var user = await _accountService.GetUser(User.FindFirstValue("userName"));

            if (user is null) return BadRequest("Cannot get notes.");

            var notes = await _noteService.GetUserNotes(user.Id);

            return Ok(notes);
        }

        [HttpPost, Route("add")]
        [Authorize]
        public async Task<IActionResult> AddNote([FromBody] NewNoteDto note)
        {
            var user = await _accountService.GetUser(User.FindFirstValue("userName"));

            if (user is null) return BadRequest("Cannot add note.");

            if(note.Public is true && note.Encrypted is true) return BadRequest("Public note cannot be encrypted.");

            byte[] notePasswordHash = default;
            var noteContent = note.Content;

            if (note.Encrypted is true)
            {
                using var hmac = new HMACSHA512(user.PasswordSalt);
                notePasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(note.Password));
                noteContent = _stringEncrypter.Encrypt(note.Content, note.Password);
            }

            var noteToAdd = new NoteModel
            {
                Title = note.Title,
                Content = noteContent,
                Public = note.Public ? 1 : 0,
                OwnerId = user.Id,
                Encrypted = note.Encrypted ? 1 : 0,
                PasswordHash = notePasswordHash
            };

            await _noteService.AddNote(noteToAdd);

            return Ok();
        }

        [HttpGet, Route("decrypt/{id}/{password}")]
        [Authorize]
        public async Task<IActionResult> DecryptNote(int id, string password)
        {
            var user = await _accountService.GetUser(User.FindFirstValue("userName"));

            if (user is null) return BadRequest("Cannot get note.");

            var note = await _noteService.GetNote(id);

            if (note is null) return BadRequest("Cannot get note.");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var passwordHash = await _noteService.GetPasswordHash(note.Id);

            for (int i = 0; i < computedHash.Length; ++i)
            {
                if (computedHash[i] != passwordHash?[i])
                {
                    return BadRequest($"Cannot get note. ");
                }
            }

            note.Content = _stringEncrypter.Decrypt(note.Content, password);

            return Ok(note);
        }
    }
}
