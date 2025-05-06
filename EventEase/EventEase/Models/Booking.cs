// Code Attribution:
// 1. ASP.NET Core MVC Foreign Key Relationships with Navigation Properties — rynop — https://stackoverflow.com/questions/43803833/asp-net-core-mvc-foreign-key-relationships-with-navigation-properties
// 2. Using Data Annotations for Email and Date Validation in ASP.NET Core — Fei Han — https://stackoverflow.com/questions/42083954/how-to-use-data-annotations-for-model-validation-in-asp-net-core


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;  // ✅ Needed for [ValidateNever]

namespace EventEase.Models
{
    public class Booking
    {
        [Key]
        [Column("BookingId")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Column("IsBooked")]
        public bool IsBooked { get; set; } = true;

        // Foreign keys
        [Required(ErrorMessage = "Venue ID is required.")]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Event ID is required.")]
        public int EventId { get; set; }

        // ✅ Navigation properties (excluded from model validation)
        [ForeignKey("VenueId")]
        [ValidateNever]  // Prevents "The Venue field is required" error
        public virtual Venue Venue { get; set; }

        [ForeignKey("EventId")]
        [ValidateNever]  // Prevents "The Event field is required" error
        public virtual Event Event { get; set; }

        // Read-only alias
        public int Id => BookingId;
    }
}
