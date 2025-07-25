using System.Linq.Expressions;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class BookingTest
{
    List<Timeslot> planning = new();
    Booking booking;
    public BookingTest()
    {
        var slot1 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0));
        var slot2 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(15, 0));
        var slot3 = Timeslot.From(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(10, 0));
        var slot4 = Timeslot.From(DayOfWeek.Thursday, new TimeOnly(9, 0), new TimeOnly(10, 0));

        var startdate = new DateOnly(2025, 8, 8);
        var enddate = new DateOnly(2025, 9, 9);

        planning.Add(slot1);
        planning.Add(slot2);
        planning.Add(slot3);
        planning.Add(slot4);

        booking = Booking.From(planning, startdate, enddate);
    }

    [Fact]
    public void BookingWrongDate()
    {
        var start = new DateOnly(2026, 8, 8);
        var end = new DateOnly(2025, 8, 8);

        var exception = Assert.Throws<Exception>(() => Booking.From(planning, start, end));
        Assert.Equal("start date must be before end date", exception.Message);
    }

    [Fact]
    public void PeriodOverlapTest()
    {
        var start = new DateOnly(2020, 10, 20);
        var end = new DateOnly(2020, 11, 20);

        var start1 = new DateOnly(2025, 7, 7);
        var end1 = new DateOnly(2025, 8, 15);

        var start2 = new DateOnly(2025, 8, 20);
        var end2 = new DateOnly(2025, 10, 10);

        var start3 = new DateOnly(2025, 8, 11);
        var end3 = new DateOnly(2025, 9, 5);

        var start4 = new DateOnly(2024, 1, 1);
        var end4 = new DateOnly(2026, 1, 1);

        var book = Booking.From(planning, start, end);
        var book1 = Booking.From(planning, start1, end1);
        var book2 = Booking.From(planning, start2, end2);
        var book3 = Booking.From(planning, start3, end3);
        var book4 = Booking.From(planning, start4, end4);

        Assert.False(booking.BookingOverlap(book)); //no overlap
        Assert.True(booking.BookingOverlap(book1)); //early overlap
        Assert.True(booking.BookingOverlap(book2)); //late overlap
        Assert.True(booking.BookingOverlap(book3)); //inside
        Assert.True(booking.BookingOverlap(book4)); //outside
        Assert.True(booking.BookingOverlap(booking)); //equal
    }

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