using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Services
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _cxt;
        public CustomerService(ApplicationDbContext cxt)
        {
            _cxt = cxt;
        }

        public async Task<Customer> UpsertCustomerAsync(Customer data, bool updateInfo, bool updateIdentity)
        {
            var customer = await _cxt.Customers.FirstOrDefaultAsync(c => c.CustEmail == data.CustEmail);

            //if no existing record, create new customer
            if (customer == null)
            {
                customer = new Customer
                {
                    CustFName = data.CustFName,
                    CustLName = data.CustLName,
                    CustEmail = data.CustEmail,
                    CustPhone = data.CustPhone,
                    IdentityUserId=data.IdentityUserId,

                };
                _cxt.Customers.Add(customer);
            }
            else
            {
                //info of registered but not logged in customer / unregistered customer with existing booking records will be updated
                if (updateInfo)
                {
                    customer.CustFName = data.CustFName;
                    customer.CustLName = data.CustLName;
                    customer.CustPhone = data.CustPhone;
                }
                if (updateIdentity)
                {
                    customer.IdentityUserId = data.IdentityUserId;
                }
            }
           
            return customer;

        }
    }
}
