
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class BookingTests
{
    [Fact]
    public void Overlaps_on_friday()
    {
        var bookingOne = Booking.From([Timeslot.From(DayOfWeek.Friday, new TimeOnly(9, 0), new TimeOnly(17, 0))],
            DateOnly.FromDateTime(new DateTime(2025, 7, 21)),
            DateOnly.FromDateTime(new DateTime(2025, 7, 25)));

        var bookingTwo = Booking.From([Timeslot.From(DayOfWeek.Friday, new TimeOnly(9, 0), new TimeOnly(17, 0))],
            DateOnly.FromDateTime(new DateTime(2025, 7, 25)),
            DateOnly.FromDateTime(new DateTime(2025, 7, 31)));

        Assert.True(bookingOne.BookingOverlap(bookingTwo));
    }

    [Fact]
    public void Monday_no_overlap()
    {
        var bookingOne = Booking.From([Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0))],
            DateOnly.FromDateTime(new DateTime(2025, 7, 21)),
            DateOnly.FromDateTime(new DateTime(2025, 7, 25)));

        var bookingTwo = Booking.From([Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0))],
            DateOnly.FromDateTime(new DateTime(2025, 7, 25)),
            DateOnly.FromDateTime(new DateTime(2025, 7, 31)));

        Assert.False(bookingOne.BookingOverlap(bookingTwo));
    }
}