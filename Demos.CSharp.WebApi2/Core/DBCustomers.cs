using Demos.CSharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demos.CSharp.WebApi2.Core
{
    public static class DBCustomers
    {
        public static IEnumerable<Customer> ToList(DBNorthwind db, string? company, string? city, string? country) 
        {      
            IQueryable<Customer> customers = db.Customers;

            if (!string.IsNullOrEmpty(company))
                customers = customers.Where(r => r.CompanyName.Contains(company));

            if (!string.IsNullOrEmpty(city))
                customers = customers.Where(r => r.City == city);

            if (!string.IsNullOrEmpty(country))
                customers = customers.Where(r => r.Country == country);

            return customers.ToList();     
        }

        public static Task<IEnumerable<Customer>> ToListAsync(DBNorthwind db, string? company, string? city, string? country)
        {
            return Task.Run<IEnumerable<Customer>>(async () => {
                IQueryable<Customer> customers = db.Customers;

                if (!string.IsNullOrEmpty(company))
                    customers = customers.Where(r => r.CompanyName.Contains(company));

                if (!string.IsNullOrEmpty(city))
                    customers = customers.Where(r => r.City == city);

                if (!string.IsNullOrEmpty(country))
                    customers = customers.Where(r => r.Country == country);

                return await customers.ToListAsync();
            });
        }
    }
}
