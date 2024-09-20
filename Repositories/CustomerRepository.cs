namespace EAD.Repositories
{
    using EAD.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using BCrypt.Net;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _customers;

        public CustomerRepository(IMongoDatabase database)
        {
            _customers = database.GetCollection<Customer>("Customers");
        }

        // Get all customers
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customers.Find(_ => true).ToListAsync();
        }

        // Get customer by email 
        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _customers.Find(c => c.Email == email).FirstOrDefaultAsync();
        }

        // Register a new customer 
        public async Task RegisterCustomerAsync(Customer customer) 
        {
            if (string.IsNullOrEmpty(customer.Id))
            {
                customer.Id = ObjectId.GenerateNewId().ToString(); 
            }

            // Hash the password before saving
            customer.Password = BCrypt.HashPassword(customer.Password);

            // Set IsActive to false by default
            customer.IsActive = false;

            await _customers.InsertOneAsync(customer);
        }

        // Update customer account 
        public async Task<Customer> UpdateCustomerAsync(string id, Customer customer)
        {
            // Ensure that the ID in the customer object is not null or altered
            if (customer.Id != id)
            {
                customer.Id = id; // Set the Id to ensure it remains the same
            }

            if (!string.IsNullOrEmpty(customer.Password))
            {
                customer.Password = BCrypt.HashPassword(customer.Password);
            }

            var result = await _customers.ReplaceOneAsync(c => c.Id == id, customer);
            if (result.MatchedCount == 0)
            {
                throw new Exception("Customer not found for update.");
            }

            return customer;
        }


        // Deactivate customer account
        public async Task DeactivateCustomerAsync(string email)
        {
            var update = Builders<Customer>.Update.Set(c => c.IsActive, false);
            await _customers.UpdateOneAsync(c => c.Email == email, update);
        }

        // Activate customer account (only CSR/Admin)
        public async Task ActivateCustomerAsync(string email)
        {
            var update = Builders<Customer>.Update.Set(c => c.IsActive, true);
            await _customers.UpdateOneAsync(c => c.Email == email, update);
        }

        // Reactivate customer account (only CSR/Admin)
        public async Task ReactivateCustomerAsync(string email)
        {
            var update = Builders<Customer>.Update.Set(c => c.IsActive, true);
            await _customers.UpdateOneAsync(c => c.Email == email, update);
        }

        public enum LoginResult
        {
            Success,
            InvalidEmail,
            IncorrectPassword,
            AccountDeactivated
        }

        public async Task<LoginResult> LoginCustomerAsync(string email, string password)
        {
            var customer = await _customers.Find(c => c.Email == email).FirstOrDefaultAsync();

            if (customer == null)
            {
                return LoginResult.InvalidEmail; // Email does not exist
            }

            if (!customer.IsActive)
            {
                return LoginResult.AccountDeactivated; // Account is deactivated
            }

            if (!BCrypt.Verify(password, customer.Password))
            {
                return LoginResult.IncorrectPassword; // Incorrect password
            }

            return LoginResult.Success; // Login successful
        }

    }
}
