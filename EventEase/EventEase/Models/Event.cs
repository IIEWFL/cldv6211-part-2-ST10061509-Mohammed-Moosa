// Code Attribution:
// 1. ASP.NET Core Model Validation with Data Annotations — Fei Han — https://stackoverflow.com/questions/42083954/how-to-use-data-annotations-for-model-validation-in-asp-net-core
// 2. EF Core One-to-Many Relationship Example — Smit Patel — https://stackoverflow.com/questions/39422550/ef-core-one-to-many-relationship-example


using System;
using System.Collections.Generic; // Add this for the List<T> type
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        public string? ImageURL { get; set; }

        // Navigation property to represent the relationship with Booking
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

