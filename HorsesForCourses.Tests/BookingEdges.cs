using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class BookingEdges
{
    [Fact(Skip = "No longer applies")]
    public void Overlaps_on_friday_but_we_only_teach_on_thursday()
    {
        // probably better guarded against/handled on Booking creation 
        //timeslot does not fall in booking start/end times
        var bookingOne = Booking.From([Timeslot.From(DayOfWeek.Friday, new TimeOnly(9, 0), new TimeOnly(10, 0))],
            new DateOnly(2025, 8, 21),
            new DateOnly(2025, 8, 21));

        var bookingTwo = Booking.From([Timeslot.From(DayOfWeek.Friday, new TimeOnly(9, 0), new TimeOnly(10, 0))],
            new DateOnly(2025, 8, 21),
            new DateOnly(2025, 8, 21));

        Assert.Equal(DayOfWeek.Thursday, new DateOnly(2025, 8, 21).DayOfWeek);
        Assert.False(bookingOne.BookingOverlap(bookingTwo), "Bookings should not overlap.");
    }

    [Fact]
    public void TimeSlot_should_fall_into_booking_duration()
    {
        var exception = Assert.Throws<Exception>(() => Booking.From([Timeslot.From(DayOfWeek.Friday, new TimeOnly(9, 0), new TimeOnly(10, 0))],
            new DateOnly(2025, 8, 21),
            new DateOnly(2025, 8, 21)));
        Assert.Equal("Day of timeslot should appear in duration of booking.", exception.Message);
    }

    [Fact]
    public void Overlaps_Edit_on_thursday_but_we_only_teach_on_thursday()
    {
        // probably better guarded against/handled on Booking creation 
        //timeslot does not fall in booking start/end times
        var bookingOne = Booking.From([Timeslot.From(DayOfWeek.Thursday, new TimeOnly(9, 0), new TimeOnly(10, 0))],
            new DateOnly(2025, 8, 21),
            new DateOnly(2025, 8, 21));

        var bookingTwo = Booking.From([Timeslot.From(DayOfWeek.Thursday, new TimeOnly(11, 0), new TimeOnly(12, 0))],
            new DateOnly(2025, 8, 21),
            new DateOnly(2025, 8, 21));

        Assert.Equal(DayOfWeek.Thursday, new DateOnly(2025, 8, 21).DayOfWeek);
        Assert.False(bookingOne.BookingOverlap(bookingTwo), "Bookings should not overlap.");
    }

    [Fact]
    public void DayNotInBothBookings()
    {
        var bookingOne = Booking.From([Timeslot.From(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(10, 0))],
            new DateOnly(2025, 1, 22),
            new DateOnly(2025, 1, 29));

        var bookingTwo = Booking.From([Timeslot.From(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(12, 0))],
            new DateOnly(2025, 1, 19),
            new DateOnly(2025, 1, 27));
        Assert.Equal(DayOfWeek.Wednesday, new DateOnly(2025, 1, 22).DayOfWeek);
        Assert.Equal(DayOfWeek.Monday, new DateOnly(2025, 1, 27).DayOfWeek);
        Assert.Equal(DayOfWeek.Tuesday, new DateOnly(2025, 1, 28).DayOfWeek);
        Assert.False(bookingOne.BookingOverlap(bookingTwo), "Bookings should not overlap.");
    }
}
