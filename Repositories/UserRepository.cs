namespace EAD.Repositories
{
    using MongoDB.Driver;

    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _users.Find(user => user.Role == role).ToListAsync();
        }

        public async Task<List<User>> GetAllAdminsAsync()
        {
            return await GetUsersByRoleAsync("Admin");
        }

        public async Task<List<User>> GetAllVendorsAsync()
        {
            return await GetUsersByRoleAsync("Vendor");
        }

        public async Task<List<User>> GetAllCsrsAsync()
        {
            return await GetUsersByRoleAsync("CSR");
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        // Activate a user account
        public async Task<bool> ActivateUserAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.IsActive, true);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Deactivate a user account
        public async Task<bool> DeactivateUserAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.IsActive, false);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

    }
}
