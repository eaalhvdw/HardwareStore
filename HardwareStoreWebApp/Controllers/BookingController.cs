using HardwareStoreLibrary.Models;
using HardwareStoreLibrary.Storage;
using HardwareStoreWebApp.Models;
using System.Web.Mvc;
using System.Collections.Generic;

namespace HardwareStoreWebApp.Controllers
{
    public class BookingController : Controller
    {
        private HardwareStoreContext db = new HardwareStoreContext();

        // GET: Create form
        public ActionResult Create()
        {
            if (Session["LoggedCustomerId"] != null)
            {
                return View(new BookingViewModel() { AvailableTools = db.GetAvailableTools() });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Create Booking
        [HttpPost]
        public ActionResult Create(BookingViewModel bookingView)
        {
            if (Session["LoggedCustomerId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            
            if (ModelState.IsValid)
            {
                if (int.TryParse(Session["LoggedCustomerId"].ToString(), out int id))
                {
                    Customer customer = db.GetCustomer(id);
                    Booking newBooking = new Booking(bookingView.StartDate, bookingView.EndDate, new List<Tool>(), customer, "Reserveret", bookingView.TotalPrice);
                    foreach (int i in bookingView.SelectedTools)
                    {
                        var tool = db.Tools.Find(i);
                        db.Entry(tool).Collection(t => t.Bookings).Load();
                        newBooking.Tools.Add(tool);
                        tool.Bookings.Add(newBooking);
                    }
                    customer.Bookings.Add(newBooking);
                    db.Bookings.Add(newBooking);
                    db.SaveChanges();
                    return View("BookingInformation", newBooking);
                }
                else
                {
                    return View(bookingView);
                }
            }
            else
            {
                return View(bookingView);
            }
        }
    }
}