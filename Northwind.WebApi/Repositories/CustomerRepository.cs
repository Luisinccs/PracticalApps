// 2023-03-12

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Packt.Shared;
using System.Collections.Concurrent;

namespace Northwind.WebApi.Repositories;

public class CustomerRepository : ICustomerRepository {

    // Use a static thread-safe dictionary field to cache the customers.
    private static ConcurrentDictionary<string, Customer>? customersCache;

    // Use an instance data context field because it should not be cached due to the data context having internal caching.
    private NorthwindContext db;

    private Customer UpdateCache(string id, Customer customer) {

        Customer? old;
        if(customersCache is not null) {
            if(customersCache.TryGetValue(id, out old)) {
                if(customersCache.TryUpdate(id, customer, old)) {
                    return customer;
                }
            }
        }
        return null!;
    }

    public CustomerRepository(NorthwindContext injectedContext){
        db = injectedContext;

        // Pre-locad customers from database as a normal Dictionary with CustomerID as the key, then convert to a thread-safe ConcurrentDictionary
        if(customersCache is null){
            customersCache = new ConcurrentDictionary<string, Customer>
            (db.Customers.ToDictionary(c => c.CustomerId));
        }
    }

    #region ICustomerRepository

    public async Task<Customer?> CreateAsync(Customer customer) {

        customer.CustomerId = customer.CustomerId.ToUpper();

        // Add to database using EF Core
        EntityEntry<Customer> added = await db.Customers.AddAsync(customer);
        int affected = await db.SaveChangesAsync();

        if (affected == 1) {
            // If the customer is new, add it to cache, else call UpdateCache method
            if (customersCache is null) return customer;
            return customersCache.AddOrUpdate(customer.CustomerId, customer, UpdateCache);
        } else {
            return null;
        }

    }

    public async Task<bool?> DeleteAsync(string id) {
        id = id.ToUpper();
        Customer? c = db.Customers.Find(id);
        if (c is null) return null;
        db.Customers.Remove(c);
        int affected = await db.SaveChangesAsync();
        if (affected == 1) {
            if (customersCache is null) return null;
            return customersCache.TryRemove(id, out c);
        } else {
            return null;
        }
    }

    public Task<IEnumerable<Customer>> RetrieveAllAsync() =>
        Task.FromResult(customersCache is null ? Enumerable.Empty<Customer>() : customersCache.Values);

    public Task<Customer?> RetrieveAsync(string id) {
        id = id.ToUpper();
        if (customersCache is null) return null!;
        customersCache.TryGetValue(id, out Customer? customer);
        return Task.FromResult(customer);
    }

    public async Task<Customer?> UpdateAsync(string id, Customer customer) {

        id = id.ToUpper();
        customer.CustomerId = customer.CustomerId.ToUpper();
        db.Customers.Update(customer);
        int affected = await db.SaveChangesAsync();
        if(affected == 1) {
            return UpdateCache(id, customer);
        }
        return null;

    }

    #endregion

}