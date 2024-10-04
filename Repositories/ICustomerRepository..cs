namespace EAD.Repositories;
using EAD.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using static EAD.Repositories.CustomerRepository;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync();

    Task<Customer> GetCustomerByEmailAsync(string email);

    Task RegisterCustomerAsync(Customer customer);

    Task<Customer> UpdateCustomerAsync(string id, Customer customer);

    Task DeleteCustomerAsync(string email);

    Task ActivateCustomerAsync(string email);

    Task ReactivateCustomerAsync(string email);

    Task<LoginResult> LoginCustomerAsync(string email, string password);
}
