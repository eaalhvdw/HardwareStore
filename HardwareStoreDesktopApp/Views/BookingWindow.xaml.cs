using HardwareStoreLibrary.Models;
using HardwareStoreLibrary.Storage;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HardwareStoreDesktopApp
{
    public partial class BookingViewWindow : Window
    {
        private readonly HardwareStoreContext _context;
        private readonly Booking _booking;
        private readonly ListBox _bookingListBox;

        public BookingViewWindow(HardwareStoreContext context, Booking booking, ListBox bookingListBox)
        {
            InitializeComponent();

            _context = context;
            _booking = booking;
            _bookingListBox = bookingListBox;

            // Set datacontext of this window.
            DataContext = booking;

            // Adds text to the timeperiod binded label.
            // The text depends on the number of days.
            if (booking.TimePeriod == 1)
            {
                TimePeriod.Content = booking.TimePeriod + " dag";
            }
            else
            {
                TimePeriod.Content = booking.TimePeriod + " dage";
            }

            // Fill the listbox with the tools of this booking.
            ToolsListBox.ItemsSource = _context.GetBooking(booking.Id).Tools;
        }

        //------------------------------ EVENTHANDLERS --------------------------------------------------------

        /**
         * This method has two functions, either it shows the elements needed to
         * change the status or it changes and saves the status of the booking.
         */
        private void EditStatusButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide InfoLabel.
            InfoLabel.Visibility = Visibility.Collapsed;

            // Get EditButtons text to see, if it is the edit or
            // the save functionality that should be executed.
            string text = EditButton.Content.ToString();

            // The edit functionality executes.
            if (text == "Rediger Status")
            {
                ShowEditStatusElements();
            }

            // The save functionality executes.
            if (text == "Gem Status")
            {
                // Get current status.
                string current = Status.Content.ToString();

                // Get selected item from ComboBox. Probably not the prettiest solution, but it works.
                string selectedStatus = Combo.SelectedItem.ToString(); // System.Windows.Controls.ComboBoxItem: Udleveret
                string status = selectedStatus.Split()[1]; // Udleveret

                // Only change and save if there are any changes to make.
                if(status != current)
                {
                    // If a user tries to change a status from "Udleveret" 
                    // to "Reserveret", an error is returned.
                    if(current == "Udleveret" && status == "Reserveret")
                    {
                        // Set and show InfoLabel as Error.
                        SetLabelRedText(InfoLabel, "Udlejningen er udleveret og kan derfor ikke reserveres.");
                    }
                    // If status is not the same as current and current
                    // is not "Udleveret", the status can be set.
                    else if (status == "Reserveret" || status == "Udleveret" || status == "Tilbageleveret")
                    {
                        // Save changes to database.
                        _booking.Status = status;
                        _context.SaveChanges();

                        // Update this datacontext.
                        DataContext = null;
                        DataContext = _booking;

                        // Update BookingsListBox in MainWindow.
                        List<Booking> bookings = new List<Booking>(_booking.Customer.Bookings);
                        _context.SortBookings(bookings);
                        _bookingListBox.ItemsSource = null;
                        _bookingListBox.ItemsSource = bookings;

                        SetLabelGreenText(InfoLabel, "Ændringen blev gemt.");
                    }
                }
                else
                {
                    SetLabelRedText(InfoLabel, "Der blev ikke gemt nogen ændringer.");
                }

                HideEditStatusElements();
            }
        }

        /**
         * Hides the elements used for editing the status 
         * of a booking and shows an info message.
         */
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            HideEditStatusElements();
            SetLabelRedText(InfoLabel, "Redigering af status blev annulleret.");
        }

        //------------------------------ HELPER METHODS --------------------------------------------------------

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
         * Hides the elements used for editing the 
         * status and resets EditButtons content.
         */
        private void HideEditStatusElements()
        {
            Combo.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            EditButton.Content = "Rediger Status";
        }

        /**
         * Show the elements used to change the status of
         * a booking and changes EditButtons content.
         */
        private void ShowEditStatusElements()
        {
            Combo.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
            EditButton.Content = "Gem Status";
        }
    }
}
