using Microsoft.EntityFrameworkCore;

namespace Packt.Shared;

public class NorthwindService : INorthwindService {

    private readonly NorthwindContext db;

    public NorthwindService(NorthwindContext db) {
        this.db = db;
    }


    public Task<Customer> CreateCustomerAsync(Customer customer) {
        db.Customers.Add(customer);
        db.SaveChangesAsync();
        return Task.FromResult(customer);
    }

    public Task DeleteCustomerAsync(string id) {

        Customer? customer = db.Customers.FirstOrDefaultAsync
        (c => c.CustomerId == id).Result;
        if (customer == null) {
            return Task.CompletedTask;
        }
        else {
            db.Customers.Remove(customer);
            return db.SaveChangesAsync();
        }

    }

    public Task<Customer?> GetCustomerAsync(string id) =>
        db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);

    public Task<List<Customer>> GetCustomersAsync() => db.Customers.ToListAsync();

    public Task<List<Customer>> GetCustomersAsync(string country) =>
        db.Customers.Where(c => c.Country == country).ToListAsync();

    public Task<Customer> UpdateCustomerAsync(Customer customer) {
        db.Entry(customer).State = EntityState.Modified;
        db.SaveChangesAsync();
        return Task.FromResult(customer);
    }
}
