using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HardwareStoreLibrary.Models
{
    public class Tool
    {
        public Tool(string name, string description, int securityDeposit, int dailyPrice)
        {
            Name = name;
            Description = description;
            SecurityDeposit = securityDeposit;
            DailyPrice = dailyPrice;
            Bookings = new List<Booking>();
        }

        public Tool() { }

        public int Id { get; set; }
        [DisplayName("Navn")]
        public string Name { get; set; }
        [DisplayName("Beskrivelse")]
        public string Description { get; set; }
        [DisplayName("Depositum")]
        public int SecurityDeposit { get; set; }
        [DisplayName("Døgnpris")]
        public int DailyPrice { get; set; }
        [DisplayName("Udlejninger")]
        public List<Booking> Bookings { get; set; }

        /*
         *  ToString method to show tool in listbox in WebApp.
         */
        public string ToolToLongString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("| Navn: {0,-15}", Name));
                sb.Append(string.Format(" | Depositum: {0,-6} kr.", SecurityDeposit));
                sb.Append(string.Format(" | Pris pr. døgn: {0,-4} kr. |", DailyPrice));
                return sb.ToString();   
            }
        }

        /*
         * ToString method to show tool in disabled listbox in DesktopApp.
         */
        public string ToolToShortString
        {
            get
            {
                return Name;
            }
        }
    }
}