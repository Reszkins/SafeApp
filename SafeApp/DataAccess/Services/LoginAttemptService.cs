namespace SafeApp.DataAccess.Services
{
    public interface ILoginAttemptService
    {
        Task AddLoginAttempt(string userName);
        Task<bool> CheckIfLoginAttemptShouldBeBlocked(string userName);
    }
    public class LoginAttemptService : ILoginAttemptService
    {
        private readonly ISqlDataAccess _db;
        public LoginAttemptService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task AddLoginAttempt(string userName)
        {
            var sql = "INSERT INTO LoginAttempts (UserName, TimeStamp) VALUES (@UserName, @TimeStamp)";
            var parameters = new Dictionary<string, object> { 
                { "@UserName", userName },
                { "@TimeStamp", DateTime.Now }
            };

            await _db.SaveData(sql, parameters);
        }

        public async Task<bool> CheckIfLoginAttemptShouldBeBlocked(string userName)
        {
            var sql = "SELECT UserName FROM LoginAttempts WHERE TimeStamp > @TimeStamp";
            var parameters = new Dictionary<string, object> {
                { "@TimeStamp", DateTime.Now.AddMinutes(-5) }
            };

            var attempts = await _db.LoadData<string>(sql, parameters);

            if (attempts.Count >= 5) return true;
            return false;
        }
    }
}
