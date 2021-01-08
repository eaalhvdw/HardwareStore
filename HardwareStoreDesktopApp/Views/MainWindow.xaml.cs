using HardwareStoreLibrary.Models;
using HardwareStoreLibrary.Storage;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace HardwareStoreDesktopApp
{
    public partial class MainWindow : Window
    {
        private readonly HardwareStoreContext _context;
        private readonly List<TextBox> _editTextBoxes;

        public MainWindow()
        {
            InitializeComponent();

            _context = new HardwareStoreContext();

            // Set datacontext of this window.
            DataContext = _context;

            // Collection of textboxes to edit a customer.
            _editTextBoxes = new List<TextBox>()
            {
                TxtBoxCustomerName,
                TxtBoxCustomerAddress,
                TxtBoxCustomerEmail,
                TxtBoxCustomerUsername
            };
        }

        //------------------------------ EVENTHANDLERS --------------------------------------------------------

        /**
         * Opens a new window with a form to create a new customer.
         * The AddCustomerWindow takes arguments from this window to
         * be able to show the new customer in this window, using
         * these elements.
         */
        private void CreateCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            AddCustomerWindow addCustomerWindow = new AddCustomerWindow(this, TxtBoxCustomerId, SearchErrorLabel, CustomerGrid, EditButton, BookingListBox);
            addCustomerWindow.Show();

            HideEditCustomerElements();
            HideInteractionLabels();
        }

        /**
         * Opens a new window with the selected bookings info.
         */
        private void GetBooking_DoubleClick(object sender, RoutedEventArgs e)
        {
            Booking booking = BookingListBox.SelectedItem as Booking;

            if (booking != null)
            {
                BookingViewWindow bookingWindow = new BookingViewWindow(_context, booking, BookingListBox);
                bookingWindow.Show();
            }
        }

        /**
         * Search for customer with an id.
         * If found show the data on the customer in CustomerGrid.
         * If not found set ErrorLabels text and show error message.
         */
        private void FindCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide labels with errors and other info for the user.
            HideInteractionLabels();

            // Hide edit customer elements if need be.
            if (EditButton.Content.ToString() == "Gem Ændringer")
            {
                HideEditCustomerElements();

                // Set InfoLabels text to inform the user.
                SetLabelGreenText(InfoLabel, "Der blev ikke gemt nogen ændringer.");
            }

            // Get text from search textbox.
            string result = TxtBoxCustomerId.Text;

            // Parse string text to int value.
            if (int.TryParse(result, out int id))
            {
                // Search for customer in database.
                Customer customer = _context.GetCustomer(id);

                // If customer was found.
                if (customer != null)
                {
                    // Show customer info in customerGrid.
                    CustomerGrid.DataContext = customer;

                    // Sort and show customer bookings in listbox.
                    _context.SortBookings(customer.Bookings);
                    BookingListBox.ItemsSource = customer.Bookings;

                    EditButton.Visibility = Visibility.Visible;
                    ResetSearchTxtBox();
                    SetLabelGreenText(SearchErrorLabel, "Succes! Kunden blev fundet.");
                }
                else
                {
                    SetLabelRedText(SearchErrorLabel, "Beklager, en kunde med det angivne id blev ikke fundet.");
                    ResetSearchTxtBox();
                }
            }
            else
            {
                SetLabelRedText(SearchErrorLabel, "Indtast venligst et id bestående af tal.");
                ResetSearchTxtBox();
            }
        }

        /**
         * If button text is "Rediger Kunde", show edit customer elements.
         * If button text is "Gem ændringer", get the customer and explore 
         * edit textboxes and passwordbox for relevant changes. If relevant 
         * changes have been made, the changes are saved in the database
         * and CustomerInfoGrid is updated to show changes.
         * Otherwise sets InfoLabels text and shows error message.
         */
        private void EditCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            // Set EditErrorLabel to default error content.
            EditErrorLabel.Content = "Fejl.";

            // Hide all error/confirmation labels.
            HideInteractionLabels();

            // Get button text.
            // Checking whether it is the edit or the save 
            // functionality that needs to be executed.
            string text = EditButton.Content.ToString();

            // Edit functionality executes.
            if (text == "Rediger Kunde")
            {
                ShowEditCustomerElements();
            }

            // Save functionality executes.
            if (text == "Gem Ændringer")
            {
                // Get id fom binded label.
                string id = CustomerId.Content.ToString();

                // Get customer from database.
                Customer customer = _context.GetCustomerByString(id);

                // Count number of changes.
                int changesCounter = 0;

                // Property changes booleans.
                bool nameBool = false;
                bool addressBool = false;
                bool emailBool = false;
                bool usernameBool = false;
                bool passwordBool = false;

                // Current values.
                string currentName = CustomerName.Content.ToString();
                string currentAddress = CustomerAddress.Content.ToString();
                string currentEmail = CustomerEmail.Content.ToString();
                string currentUsername = CustomerUsername.Content.ToString();
                string currentPassword = CustomerPassword.Content.ToString();

                // Textbox text values.
                string nameValue = TxtBoxCustomerName.Text;
                string addressValue = TxtBoxCustomerAddress.Text;
                string emailValue = TxtBoxCustomerEmail.Text;
                string usernameValue = TxtBoxCustomerUsername.Text;
                string passwordValue = CustomerPassBox.Password;

                /**
                 * Examining textboxes and the passwordbox to see if text has changed
                 * to something useful. If there are changes, the property boolean
                 * is set to true and the changescounter is incremented.
                 */
                if (NameHasChanged(currentName, nameValue))
                {
                    nameBool = true;
                    changesCounter++;
                }

                if (AddressHasChanged(currentAddress, addressValue))
                {
                    addressBool = true;
                    changesCounter++;
                }

                if (EmailHasChanged(currentEmail, emailValue))
                {
                    emailBool = true;
                    changesCounter++;

                }

                if (UsernameHasChanged(currentUsername, usernameValue))
                {
                    usernameBool = true;
                    changesCounter++;
                }

                if (PasswordHasChanged(currentPassword, passwordValue))
                {
                    passwordBool = true;
                    changesCounter++;
                }

                // For each of the properties, if there is a
                // change, verify new value and set customer
                // property if validation was succesful.
                if (changesCounter > 0)
                {
                    // Counts errors.
                    int errorCounter = 0;

                    // If name has changed, verify new name 
                    // and set new value or set error.
                    if (nameBool)
                    {
                        if (_context.VerifyName(nameValue))
                        {
                            customer.Name = nameValue;
                        }
                        else
                        {
                            EditErrorLabel.Content = "Navnet er ugyldigt.";
                            errorCounter++;
                        }
                    }

                    // If address has changed, set new value.
                    if (addressBool)
                    {
                        customer.Address = addressValue;
                    }

                    // If email has changed, verify new email
                    // and set new value or set error.
                    if (emailBool)
                    {
                        if (_context.VerifyEmail(emailValue))
                        {
                            customer.Email = emailValue;
                        }
                        else
                        {
                            // If there already is an error above, don't show this one yet.
                            // If there are no errors above this one, show this error.
                            if (IsDefaultError(EditErrorLabel))
                            {
                                EditErrorLabel.Content = "E-mailaddressen er ugyldig.";
                                errorCounter++;
                            }
                        }
                    }

                    // If the customer does not have a login.
                    if (!_context.HasLogin(customer))
                    {
                        // Both username and password have changes.
                        if (usernameBool && passwordBool)
                        {
                            // Verify username and set new value or set error.
                            if (_context.VerifyUsername(usernameValue))
                            {
                                customer.Username = usernameValue;
                            }
                            else
                            {
                                // If there already is an error above, don't show this one yet.
                                // If there are no errors above this one, show this error.
                                if (IsDefaultError(EditErrorLabel))
                                {
                                    EditErrorLabel.Content = "Brugernavnet må højest indeholde 25 tegn.";
                                    errorCounter++;
                                }
                            }
                            
                            // Verify Password and set new value or set error.
                            if (_context.VerifyPassword(passwordValue))
                            {
                                customer.Password = passwordValue;
                            }
                            else
                            {
                                // If there already is an error above, don't show this one yet.
                                // If there are no errors above this one, show this error.
                                if (IsDefaultError(EditErrorLabel))
                                {
                                    EditErrorLabel.Content = "Kodeordet skal indeholde 8 - 20 tegn.";
                                    errorCounter++;
                                }
                            }
                        }
                        
                        // Set errror, if customer does not have a login 
                        // and only one of the login fields are filled.
                        if (usernameBool && !passwordBool || !usernameBool && passwordBool)
                        {
                            // If there already is an error above, don't show this one yet.
                            // If there are no errors above this one, show this error.
                            if (IsDefaultError(EditErrorLabel))
                            {
                                EditErrorLabel.Content = "Opret login ved at udfylde loginfelterne.";
                                errorCounter++;
                            }
                        }
                    }

                    // If the customer does have a login.
                    if (_context.HasLogin(customer))
                    {
                        // If username has changed, verify new 
                        // username and set new value or set error.
                        if (usernameBool)
                        {
                            if (_context.VerifyUsername(usernameValue))
                            {
                                customer.Username = usernameValue;
                            }
                            else
                            {
                                // If there already is an error above, don't show this one yet.
                                // If there are no errors above this one, show this error.
                                if (IsDefaultError(EditErrorLabel))
                                {
                                    EditErrorLabel.Content = "Brugernavnet må højest indeholde 25 tegn.";
                                    errorCounter++;
                                }
                            }
                        }

                        // If password has changed, verify new password
                        // and set new value or set error.
                        if (passwordBool)
                        {
                            if (_context.VerifyPassword(passwordValue))
                            {
                                customer.Password = passwordValue;
                            }
                            else
                            {
                                // If there already is an error above, don't show this one yet.
                                // If there are no errors above this one, show this error.
                                if (IsDefaultError(EditErrorLabel))
                                {
                                    EditErrorLabel.Content = "Kodeordet skal indeholde 8 - 20 tegn.";
                                    errorCounter++;
                                }
                            }
                        }
                    }

                    // If there are any errors, reset properties and show error.
                    if (errorCounter != 0)
                    {
                        EditErrorLabel.Visibility = Visibility.Visible;
                        SetLabelRedText(InfoLabel, "Ingen ændringer blev gemt.");
                        ShowEditCustomerElements();

                        // Reset customer properties.
                        customer.Name = currentName;
                        customer.Address = currentAddress;
                        customer.Email = currentEmail;
                        customer.Username = currentUsername;
                        customer.Password = currentPassword;
                    }

                    // If there are no errors, save the changes.
                    if (errorCounter == 0)
                    {
                        EditErrorLabel.Visibility = Visibility.Collapsed;
                        HideEditCustomerElements();

                        if (changesCounter == 1)
                        {
                            SetLabelGreenText(InfoLabel, "Ændringen blev gemt.");
                        }
                        else
                        {
                            SetLabelGreenText(InfoLabel, "Ændringerne er blevet gemt.");
                        }

                        // SAVE changes to database.
                        _context.SaveChanges();
                    }

                    // Update CustomerGrid to show new values.
                    CustomerGrid.DataContext = null;
                    CustomerGrid.DataContext = customer;
                }
                else
                {
                    SetLabelRedText(InfoLabel, "Der blev ikke gemt nogen ændringer.");
                    HideEditCustomerElements();
                }
            }
        }


        /**
         * Cancels customer editing.
         */
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            HideEditCustomerElements();
            EditErrorLabel.Visibility = Visibility.Collapsed;
            SetLabelRedText(InfoLabel, "Redigeringen blev annulleret.");
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
                // Clear TextBox
                source.Clear();

                // Change styling of TextBox.
                if (source == TxtBoxCustomerId)
                {
                    source.Style = (Style)Resources["SearchTxtBox"];
                }
                else
                {
                    source.Style = (Style)Resources["CustomerTxtBox"];
                }
            }
        }

        //------------------------------ HELPER METHODS --------------------------------------------------------

        /**
         * Checks whether the labels content is
         * set to a default error text, which
         * is "Fejl.".
         */
         private bool IsDefaultError(Label lbl)
        {
            if(lbl.Content.ToString() == "Fejl.")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Set and show a label as infomessage.
         * Takes a label and a string text to
         * set as content to the label.
         * The label is styled with green text.
         */
         private void SetLabelGreenText(Label lbl, string text)
        {
            lbl.Style = (Style)Resources["InfoLabel"];
            lbl.Content = text;
            lbl.Visibility = Visibility.Visible;
        }

        /**
         * Set and show a label as error message.
         * Takes a label and a string text to set
         * as content to the label.
         * The label is styled with red text.
         */
         private void SetLabelRedText(Label lbl, string text)
        {
            lbl.Style = (Style)Resources["ErrorLabel"];
            lbl.Content = text;
            lbl.Visibility = Visibility.Visible;
        }

        /**
         * Style SearchTextbox as placeholder and set default text.
         */
        private void ResetSearchTxtBox()
        {
            TxtBoxCustomerId.Text = "Indtast kundenummer her";
            TxtBoxCustomerId.Style = (Style)Resources["PlaceholderSearchTxtBox"];
        }

        /**
         * Hides the labels used for interaction with users.
         */
        private void HideInteractionLabels()
        {
            InfoLabel.Visibility = Visibility.Collapsed;
            EditErrorLabel.Visibility = Visibility.Collapsed;
            SearchErrorLabel.Visibility = Visibility.Collapsed;
        }

        /**
         * Hides elements for editing a customer and resets editbutton.
         */
        private void HideEditCustomerElements()
        {
            foreach (var txt in _editTextBoxes)
            {
                txt.Visibility = Visibility.Collapsed;

            }
            CustomerPassBox.Visibility = Visibility.Collapsed;

            CancelButton.Visibility = Visibility.Collapsed;
            EditButton.Content = "Rediger Kunde";
        }

        /**
         * Shows the elements required for editing a 
         * customer and changes editbuttons text.
         */
        private void ShowEditCustomerElements()
        {
            // Reset textboxes.
            TxtBoxCustomerName.Text = "Navn";
            TxtBoxCustomerAddress.Text = "Adresse";
            TxtBoxCustomerEmail.Text = "E-mail";
            TxtBoxCustomerUsername.Text = "Brugernavn";

            // Show textboxes as placeholders.
            foreach (TextBox txt in _editTextBoxes)
            {
                txt.Visibility = Visibility.Visible;
                txt.Style = (Style)Resources["PlaceholderCustomerTxtBox"];
            }

            // Reset and show Passwordbox.
            CustomerPassBox.Clear();
            CustomerPassBox.Visibility = Visibility.Visible;

            // Show cancel button.
            CancelButton.Visibility = Visibility.Visible;

            // Change EditButtons text.
            EditButton.Content = "Gem Ændringer";
        }

        /**
         * This method explores wheter or not the 
         * new name is a change to the current one.
         * It takes two strings, one with the current 
         * value and one, with the new value.
         * The new value is tested for a few
         * simple conditions to tell, if this
         * value could be an intended change.
         */
        private bool NameHasChanged(string current, string name)
        {
            if (name != "Navn" && name != "" && name != current)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * This method does the same as the 
         * above one, except that this is 
         * testing for another property.
         * This tests if an address 
         * has changed.
         */
        private bool AddressHasChanged(string current, string address)
        {
            if (address != "Adresse" && address != "" && address != current)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * This one tests if an email has changed.
         * Otherwise does the same as the above
         * methods.
         */
        private bool EmailHasChanged(string current, string email)
        {
            if (email != "E-mail" && email != "" && email != current)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * This method tests the username.
         */
        private bool UsernameHasChanged(string current, string username)
        {
            if (username != current && username != "Brugernavn" && username != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * This method tests the password.
         */
        private bool PasswordHasChanged(string current, string password)
        {
            if (password != "" && password != current)
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
