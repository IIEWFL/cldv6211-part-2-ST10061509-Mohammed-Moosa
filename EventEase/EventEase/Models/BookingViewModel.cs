// Code Attribution:
// 1. ASP.NET Core MVC Foreign Key Relationships with Navigation Properties — rynop — https://stackoverflow.com/questions/43803833/asp-net-core-mvc-foreign-key-relationships-with-navigation-properties
// 2. Using Data Annotations for Email and Date Validation in ASP.NET Core — Fei Han — https://stackoverflow.com/questions/42083954/how-to-use-data-annotations-for-model-validation-in-asp-net-core


using System;

namespace EventEase.Models
{
    public class BookingViewModel
    {
        // Booking basic info
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string ContactEmail { get; set; }
        public DateTime BookingDate { get; set; }
        public bool IsBooked { get; set; }

        // Foreign keys
        public int VenueId { get; set; }
        public int EventId { get; set; }

        // Display fields
        public string EventName { get; set; }
        public string VenueName { get; set; }

        // Computed field for display
        public string Status => IsBooked ? "Confirmed" : "Cancelled";
    }
}
