using HardwareStoreLibrary.Models;
using HardwareStoreLibrary.Storage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HardwareStoreDesktopApp
{
    public partial class AddCustomerWindow : Window
    {
        private readonly HardwareStoreContext _context;
        private readonly Window _mainWin;
        private readonly TextBox _mainWinTxtBox;
        private readonly Label _mainWinLbl;
        private readonly Grid _mainWinGrid;
        private readonly Button _mainWinEditButton;
        private readonly ListBox _mainWinBookingsList;

        public AddCustomerWindow(MainWindow mainWindow, TextBox TxtBoxCustomerId, Label searchErrorLabel, Grid customerGrid, Button editButton, ListBox bookingListBox)
        {
            InitializeComponent();

            _context = new HardwareStoreContext();
            _mainWin = mainWindow;
            _mainWinTxtBox = TxtBoxCustomerId;
            _mainWinLbl = searchErrorLabel;
            _mainWinGrid = customerGrid;
            _mainWinEditButton = editButton;
            _mainWinBookingsList = bookingListBox;
        }

        //------------------------------ EVENTHANDLERS --------------------------------------------------------

        /**
         * Creates a new customer.
         */
        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide all error labels.
            HideInteractionLabels();

            // Initialize new property variables.
            string name = "";
            string address = "";
            string email = "";
            string username = "abc"; // Default value.
            string password = "12345678"; // Default value.

            // Get input from TextBoxes.
            string Name = TxtBoxName.Text;
            string Address = TxtBoxAddress.Text;
            string Email = TxtBoxEmail.Text;
            string Username = TxtBoxUsername.Text;
            string Password = PasswordBox.Password;

            // Counts errors.
            int errorCounter = 0;

            // Check if name is set, validate and set new value or show error.
            if (NameIsSet(Name))
            {
                if (_context.VerifyName(Name))
                {
                    name = Name;
                }
                else
                {
                    // Name is not valid.
                    SetErrorLabel(NameErrorLabel, "Navnet er ugyldigt.");
                    errorCounter++;
                }
            }
            else
            {
                // Name is not set.
                SetErrorLabel(NameErrorLabel, "Indtast venligst et navn.");
                errorCounter++;
            }

            // See if address is set and set new value or show error.
            if (AddressIsSet(Address))
            {
                address = Address;
            }
            else
            {
                // Address is not set.
                SetErrorLabel(AddressErrorLabel, "Indtast venligst en adresse.");
                errorCounter++;
            }

            // See if email is set, validate and set new value or show error.
            if (EmailIsSet(Email))
            {
                if (_context.VerifyEmail(Email))
                {
                    email = Email;
                }
                else
                {
                    // Email is not valid.
                    SetErrorLabel(EmailErrorLabel, "E-mailadressen er ugyldig.");
                    errorCounter++;
                }
            }
            else
            {
                // Email is not set.
                SetErrorLabel(EmailErrorLabel, "Indtast venligst en e-mailadresse.");
                errorCounter++;
            }

            // If both the username and the password are set, validate the values
            // and set new values or show errors.
            if (UsernameIsSet(Username) && PasswordIsSet(Password))
            {
                if (_context.VerifyUsername(Username))
                {
                    username = Username;
                }
                else
                {
                    // The username is too long.
                    SetErrorLabel(UsernameErrorLabel, "Brugernavnet må højest indeholde 25 tegn.");
                    errorCounter++;
                }

                if (_context.VerifyPassword(Password))
                {
                    password = Password;
                }
                else
                {
                    // The password does not meet the length requirements.
                    SetErrorLabel(PasswordErrorLabel, "Kodeordet skal indeholde 8 - 20 tegn.");
                    errorCounter++;
                }
            }
            // One of the login informations is set, but not the other.
            else if (!UsernameIsSet(Username) && PasswordIsSet(Password) || UsernameIsSet(Username) && !PasswordIsSet(Password))
            {
                // Username is not set.
                if (!UsernameIsSet(Username))
                {
                    SetErrorLabel(UsernameErrorLabel, "Opret login ved at udfylde loginfelterne.");
                    errorCounter++;
                }
                // Password is not set.
                else if (!PasswordIsSet(Password))
                {
                    SetErrorLabel(PasswordErrorLabel, "Opret login ved at udfylde loginfelterne.");
                    errorCounter++;
                }
            }

            // If there are no errors, create a new customer, add
            // it to the database, save it and close the window.
            // If successful, show label with confirmation and 
            // the new customer on MainWindow.
            if (errorCounter == 0)
            {
                // Create and save new customer.
                Customer customer = new Customer(name, address, email, username, password);
                _context.Customers.Add(customer);
                _context.SaveChanges();

                // Reset search textbox in main.
                ResetSearchTxtBox();

                // Set and show SearchErrorLabel as InfoLabel in main.
                SetLabelGreenText(_mainWinLbl, "En ny kunde blev oprettet.");

                // Set datacontext in customergrid, reset listbox and show EditButton in main.
                _mainWinGrid.DataContext = customer;
                _mainWinBookingsList.ItemsSource = null;
                _mainWinEditButton.Visibility = Visibility.Visible;

                // Hide this window.
                this.Hide();
            }
        }

        /**
         * Closes the window without saving and setting a label on mainWindow.
         */
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset search textbox in main.
            ResetSearchTxtBox();

            // Set and show SearchErrorLabel as InfoLabel in main.
            SetLabelRedText(_mainWinLbl, "Oprettelsen af en ny kunde blev annulleret.");

            // Hide this window.
            this.Hide();
        }

        /**
         * Clears and changes styling of a textbox when clicked.
         */
        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Get TextBox.
            TextBox source = e.Source as TextBox;

            if (source != null)
            {
                // Clear TextBox.
                source.Clear();

                // Change styling of TextBoxes.
                source.Style = (Style)Resources["TxtBox"];
            }
        }

        //------------------------------ HELPER METHODS --------------------------------------------------------

        /**
         * The method takes a label and a string
         * text to set and show the label with the 
         * given text.
         */
         private void SetErrorLabel(Label lbl, string text)
        {
            lbl.Content = text;
            lbl.Visibility = Visibility.Visible;
        }

        /**
         * Helper method for elements from main window.
         * Set and show a label as infomessage.
         * Takes a label and a string text to
         * set as content to the label.
         * The label is styled with green text.
         */
        private void SetLabelGreenText(Label lbl, string text)
        {
            lbl.Style = (Style)_mainWin.Resources["InfoLabel"];
            lbl.Content = text;
            lbl.Visibility = Visibility.Visible;
        }

        /**
         * Helpter method for elements from main window.
         * Set and show a label as error message.
         * Takes a label and a string text to set
         * as content to the label.
         * The label is styled with red text.
         */
        private void SetLabelRedText(Label lbl, string text)
        {
            lbl.Style = (Style)_mainWin.Resources["ErrorLabel"];
            lbl.Content = text;
            lbl.Visibility = Visibility.Visible;
        }

        /**
         * Helper method for TextBox element from MainWindow.
         * The method styles the SearchTextbox as placeholder 
         * and sets the text to the default text.
         */
        private void ResetSearchTxtBox()
        {
            _mainWinTxtBox.Text = "Indtast kundenummer her";
            _mainWinTxtBox.Style = (Style)_mainWin.Resources["PlaceholderSearchTxtBox"];
        }

        /**
         * Hide all error labels.
         */
        private void HideInteractionLabels()
        {
            NameErrorLabel.Visibility = Visibility.Collapsed;
            AddressErrorLabel.Visibility = Visibility.Collapsed;
            EmailErrorLabel.Visibility = Visibility.Collapsed;
            UsernameErrorLabel.Visibility = Visibility.Collapsed;
            PasswordErrorLabel.Visibility = Visibility.Collapsed;
        }

        /**
         * Check that the name is set.
         */
        private bool NameIsSet(string name)
        {
            if (TxtBoxName != null && name != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Check that the address is set.
         */
        private bool AddressIsSet(string address)
        {
            if (TxtBoxAddress != null && address != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Check that email is set.
         */
        private bool EmailIsSet(string email)
        {
            if (TxtBoxEmail != null && email != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Check that username is set.
         */
        private bool UsernameIsSet(string username)
        {
            if (TxtBoxUsername != null && username != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Check that password is set.
         */
        private bool PasswordIsSet(string password)
        {
            if (PasswordBox != null && password != "")
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
