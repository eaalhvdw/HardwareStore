using HardwareStoreLibrary.Models;
using HardwareStoreLibrary.Storage;
using System.Linq;
using System.Web.Mvc;

namespace HardwareStoreWebApp.Controllers
{
    public class LoginController : Controller
    {
        private HardwareStoreContext db = new HardwareStoreContext();

        // GET: Login form
        public ActionResult Login()
        {
            return View();
        }

        // POST: Verify customer login
        [HttpPost]
        public ActionResult Login(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var customerUsername = db.Customers.Where(c => c.Username == customer.Username).FirstOrDefault();
                var resultCustomer = db.Customers.Where(c => c.Username == customer.Username && c.Password == customer.Password).FirstOrDefault();

                if(customerUsername != null && resultCustomer == null)
                {
                    customer.LoginErrorMsg = "Kodeordet er forkert.";
                    return View(customer);
                }

                if (resultCustomer != null)
                {
                    if(resultCustomer.Username == "abc" && resultCustomer.Password == "12345678")
                    {
                        customer.LoginErrorMsg = "Der er ikke oprettet en bruger til denne kunde.";
                        return View(customer);
                    }
                    else
                    {
                        Session["LoggedCustomerId"] = resultCustomer.Id.ToString();
                        Session["LoggedCustomerName"] = resultCustomer.Name.ToString();
                        return RedirectToAction("Index");
                    }
                }
                customer.LoginErrorMsg = "Brugernavnet eksisterer ikke i databasen.";
                return View(customer);
            }
            return View();
        }

        // GET: Frontpage
        public ActionResult Index()
        {
            if (Session["LoggedCustomerId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // Log out of webapp
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}