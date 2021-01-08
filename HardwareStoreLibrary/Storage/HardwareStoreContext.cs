using HardwareStoreLibrary.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace HardwareStoreLibrary.Storage
{
    public class HardwareStoreContext : DbContext
    {

        public HardwareStoreContext() : base("name=HardwareStoreConnectionString") { }

        public DbSet<Tool> Tools { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        /**
         * Get all available tools in the database.
         * If there are no bookings in the database
         * at all, then all tools are available.
         * If a tool has never been booked, 
         * then it is available.
         * Otherwise a tool is available, if the status
         * of its bookings are all "Tilbageleveret".
         */
         public List<Tool> GetAvailableTools()
        {
            if(Bookings == null)
            {
                return Tools.ToList();
            }
            else
            {
                List<Tool> AllAvailableTools = new List<Tool>();
                foreach(Tool t in Tools.Include(t => t.Bookings))
                {
                    if(t.Bookings == null)
                    {
                        AllAvailableTools.Add(t);
                    }
                }
                List<Tool> GetAvailableTools = Tools.Include(t => t.Bookings).Where(t => t.Bookings.All(b => b.Status == "Tilbageleveret")).ToList();
                AllAvailableTools.AddRange(GetAvailableTools);
                return AllAvailableTools;
            }
        }

        /**
         * Get a customer with the customer id.
         * Eager loading: Include bookings.
         */
        public Customer GetCustomer(int id)
        {
            return Customers.Where(c => c.Id == id).Include(c => c.Bookings).FirstOrDefault();
        }

        /**
         * Helper method takes a string value
         * and fetches customer by extracting 
         * an id from the string and calling 
         * the above method.
         * If no customer was found,
         * null is returned.
         */
        public Customer GetCustomerByString(string s)
        {
            // Parse string text to int value.
            if (int.TryParse(s, out int id))
            {
                Customer customer = GetCustomer(id);
                return customer;
            }
            return null;
        }

        /**
         * Get a booking with the booking id.
         * Eager loading: Include tools.
         */
        public Booking GetBooking(int id)
        {
            return Bookings.Where(b => b.Id == id).Include(b => b.Tools).FirstOrDefault();
        }

        /**
         * Sort bookings by most recent first.
         */
        public List<Booking> SortBookings(List<Booking> bookings)
        {
            var comparer = new BookingComparer();
            bookings.Sort(comparer);
            return bookings;
        }

        /**
         * A name cannot contain numbers or symbols, except for '-'.
         * Verify name by matching string with all letter characters, dashes 
         * and single whitespace characters after a word except for the last.
         * At least one case is not accounted for: a name is accepted even if 
         * it contains to dashes after one another. I let that one case slide 
         * because if-then-else conditionals in regex is too complicated.
         */
        public bool VerifyName(string name)
        {
            string pattern = @"^[æøåÆØÅa-zA-Z-]+(?: [æøåÆØÅa-zA-Z-]+)*$";
            Regex regx = new Regex(pattern);
            
            if (regx.IsMatch(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Verfiy email by matching all letters, numbers and specifick 
         * signs: '.', '_', '&', '/', '%', '+' and '-' one or more times
         * in a pattern that contains a single '@' and ends 
         * with a '.' followed by 2 or 3 of any letters.
         */
        public bool VerifyEmail(string email)
        {
            string pattern = @"^[æøåa-z0-9._&\/%+-]+@[æøåa-z0-9.-]+\.[æøåa-z]{2,3}?$";
            Regex regx = new Regex(pattern, RegexOptions.IgnoreCase);

            if (regx.IsMatch(email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Check if a customer has a login.
         */
        public bool HasLogin(Customer customer)
        {
            if (customer.Username != "abc" && customer.Password != "12345678")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * The username contains a total of maximum 25 signs.
         */
        public bool VerifyUsername(string username)
        {
            if (username.Length <= 25)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * The password contains betwen 8 and 20 signs.
         */
        public bool VerifyPassword(string password)
        {
            if (password.Length >= 8 && password.Length <= 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}