using HardwareStoreLibrary.Models;
using HardwareStoreLibrary.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HardwareStoreWebApp.Models
{
    public class BookingViewModel
    {
        private readonly HardwareStoreContext db = new HardwareStoreContext();

        public BookingViewModel() { }

        [Required(ErrorMessage = "Vælg venligst en startdato for lejeperioden.")]
        [DisplayName("Startdato")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Vælg venligst en slutdato for lejeperioden.")]
        [DisplayName("Slutdato")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [DisplayName("Lejeperiode")]
        public int TimePeriod { get { return (int)(EndDate.Date - StartDate.Date).TotalDays; } }
        public int TotalPrice
        {
            get
            {
                int result = 0;
                if (SelectedTools != null)
                {
                    foreach (int i in SelectedTools)
                    {
                        var t = db.Tools.Find(i);
                        result += (t.SecurityDeposit + t.DailyPrice * TimePeriod);
                    }
                }
                return result;
            }
        }

        [DisplayName("Værktøjer")]
        public List<Tool> AvailableTools { get; set; }

        // Selected tools from SelectList.
        [Required(ErrorMessage = "Vælg venligst et værktøj.")]
        public int[] SelectedTools { get; set; }

        // SelectList for all available tools.
        public SelectList ToolsSelectList
        {
            get
            {
                return new SelectList(AvailableTools, nameof(Tool.Id), nameof(Tool.ToolToLongString));
            }
        }
    }
}