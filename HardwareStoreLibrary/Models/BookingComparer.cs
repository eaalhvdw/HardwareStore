using System.Collections.Generic;

namespace HardwareStoreLibrary.Models
{
    /**
     * Comparer to compare bookings by startdate first and by enddate second.
     * Used in HardwareStoreContext.
     */
    public class BookingComparer : IComparer<Booking>
    {
        public int Compare(Booking b1, Booking b2)
        {
            int compareStartDate = b2.StartDate.CompareTo(b1.StartDate);
            if (compareStartDate == 0)
            {
                return b2.EndDate.CompareTo(b1.EndDate);
            }
            return compareStartDate;
        }
    }
}