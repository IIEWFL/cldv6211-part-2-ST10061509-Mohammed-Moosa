// Code Attribution:
// 1. ASP.NET Core MVC DropDownList Selected Value Not Binding On Postback — rynop — https://stackoverflow.com/questions/43803833/asp-net-core-mvc-dropdownlist-selected-value-not-binding-on-postback
// 2. How to Bind Multiple Models in ASP.NET Core MVC Post Action — Fei Han — https://stackoverflow.com/questions/42083954/how-to-bind-multiple-models-in-asp-net-core-mvc-post-action


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> EnhancedIndex(string searchString)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    ContactEmail = b.ContactEmail,
                    BookingDate = b.BookingDate,
                    IsBooked = b.IsBooked,
                    VenueId = (int)b.VenueId,
                    EventId = (int)b.EventId,
                    EventName = b.Event.Name,
                    VenueName = b.Venue.Name
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.BookingId.ToString().Contains(searchString) ||
                    b.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    b.EventName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    b.VenueName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(bookings);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        public IActionResult Create()
        {
            PopulateVenueAndEventDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName,ContactEmail,BookingDate,IsBooked,VenueId,EventId")] Booking booking)
        {
            Console.WriteLine($"ModelState valid? {ModelState.IsValid}");

            foreach (var entry in ModelState)
            {
                if (entry.Value.Errors.Count > 0)
                {
                    Console.WriteLine($"Field '{entry.Key}' has errors: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            // Additional manual validation for dropdowns
            if (booking.VenueId <= 0)
                ModelState.AddModelError("VenueId", "Please select a valid venue.");

            if (booking.EventId <= 0)
                ModelState.AddModelError("EventId", "Please select a valid event.");

            if (ModelState.IsValid)
            {
                // Venue double booking check (no .Value needed)
                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.VenueId == booking.VenueId
                                              && b.BookingDate == booking.BookingDate
                                              && b.IsBooked == true);

                if (existingBooking != null)
                {
                    ModelState.AddModelError("BookingDate", "This venue is already booked for the selected date and time.");
                }
                else
                {
                    try
                    {
                        _context.Add(booking);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving booking: {ex.Message}");
                        ModelState.AddModelError("", "An error occurred while saving the booking. Please try again.");
                    }
                }
            }

            PopulateVenueAndEventDropDowns(booking.VenueId, booking.EventId);
            return View(booking);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            PopulateVenueAndEventDropDowns(booking.VenueId, booking.EventId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,CustomerName,ContactEmail,BookingDate,IsBooked,VenueId,EventId")] Booking booking)
        {
            if (id != booking.BookingId) return NotFound();

            if (ModelState.IsValid)
            {
                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.VenueId == booking.VenueId
                                              && b.BookingDate == booking.BookingDate
                                              && b.IsBooked == true
                                              && b.BookingId != booking.BookingId);

                if (existingBooking != null)
                {
                    ModelState.AddModelError("BookingDate", "This venue is already booked for the selected date and time.");
                }
                else
                {
                    try
                    {
                        _context.Update(booking);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BookingExists(booking.BookingId)) return NotFound();
                        else throw;
                    }
                }
            }

            PopulateVenueAndEventDropDowns(booking.VenueId, booking.EventId);
            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }

        // FIXED METHOD — this ensures the correct value fields are used for dropdowns
        private void PopulateVenueAndEventDropDowns(int? selectedVenueId = null, int? selectedEventId = null)
        {
            ViewBag.VenueList = new SelectList(_context.Venues.OrderBy(v => v.Name), "Id", "Name", selectedVenueId);
            ViewBag.EventList = new SelectList(_context.Events.OrderBy(e => e.Name), "EventId", "Name", selectedEventId);
        }
    }
}
