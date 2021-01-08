using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HardwareStoreLibrary.Models
{
    public class Customer
    {
        public Customer(string name, string address, string email, string username = "abc", string password = "12345678")
        {
            Name = name;
            Address = address;
            Email = email;
            Username = username;
            Password = password;
            Bookings = new List<Booking>();
        }

        public Customer() { }

        public int Id { get; set; }
        [DisplayName("Navn")]
        public string Name { get; set; }
        [DisplayName("Adresse")]
        public string Address { get; set; }
        [DisplayName("E-mailadresse")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Brugernavn")]
        [Required(ErrorMessage = "Indtast venligst et brugernavn.", AllowEmptyStrings = false)]
        [StringLength(25, ErrorMessage = "Brugernavnet må højest indeholde 25 tegn.")]
        public string Username { get; set; }
        [DisplayName("Kodeord")]
        [Required(ErrorMessage = "Indtast venligst et kodeord.", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Kodeordet skal indeholde 8 - 20 tegn.", MinimumLength = 8)]
        public string Password { get; set; }
        public string LoginErrorMsg { get; set; }
        [DisplayName("Udlejninger")]
        public List<Booking> Bookings { get; set; }
    }
}