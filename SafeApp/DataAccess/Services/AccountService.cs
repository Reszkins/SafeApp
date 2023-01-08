using Dapper;
using SafeApp.Models;

namespace SafeApp.DataAccess.Services
{
    public interface IAccountService 
    {
        Task<UserModel?> GetUser(string userName);
        Task AddAccount(UserModel newAccount);
    }

    public class AccountService : IAccountService
    {
        private readonly ISqlDataAccess _db;
        public AccountService(ISqlDataAccess db) 
        {
            _db = db;
        }

        public async Task<UserModel?> GetUser(string userName)
        {
            var sql = "SELECT Id, UserName, PasswordHash, PasswordSalt FROM Accounts WHERE UserName = @UserName";
            var parameters = new Dictionary<string, object> { { "@UserName", userName } };

            var users = await _db.LoadData<UserModel>(sql, parameters);

            return users.FirstOrDefault();
        }

        public async Task AddAccount(UserModel newAccount)
        {
            var sql = "INSERT INTO Accounts (UserName, PasswordHash, PasswordSalt) VALUES (@UserName, @PasswordHash, @PasswordSalt)";
            var parameters = new Dictionary<string, object> { 
                { "@UserName", newAccount.UserName },
                { "@PasswordHash", newAccount.PasswordHash },
                { "@PasswordSalt", newAccount.PasswordSalt }
            };

            await _db.SaveData(sql, parameters);
        }
    }
}
