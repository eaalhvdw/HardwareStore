using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HardwareStoreLibrary.Models
{
    public class Booking
    {
        public Booking(DateTime startdate, DateTime enddate, List<Tool> tools, Customer customer, string status, int totalPrice)
        {
            StartDate = startdate.Date;
            EndDate = enddate.Date;
            TimePeriod = (int)(EndDate - StartDate).TotalDays;
            Tools = new List<Tool>(tools);
            Customer = customer;
            Status = status;
            TotalPrice = totalPrice;
        }

        public Booking() { }

        public int Id { get; set; }
        [DisplayName("Startdato")]
        public DateTime StartDate { get; set; }
        public string StartDateToString => StartDate.ToShortDateString();
        [DisplayName("Slutdato")]
        public DateTime EndDate { get; set; }
        public string EndDateToString => EndDate.ToShortDateString();
        [DisplayName("Lejeperiode")]
        public int TimePeriod { get; set; }
        [DisplayName("Værktøjer")]
        public List<Tool> Tools { get; set; }
        public Customer Customer { get; set; }
        public string Status { get; set; } // "Reserveret", "Udleveret" or "Tilbageleveret".
        [DisplayName("Totalpris")]
        public int TotalPrice { get; set; }
        
        /*
         * ToString method to show booking in listbox in DesktopApp.
         */
        public string BookingToString
        {
            get
            {
                return
                    "| Startdato: " + StartDate.ToShortDateString() +
                    " | Slutdato: " + EndDate.ToShortDateString() +
                    " | Status: " + Status +
                    " |";
            }
        }
    }
}