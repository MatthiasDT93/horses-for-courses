using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class TimeslotTest
{
    Timeslot Slot;

    public TimeslotTest()
    {
        Slot = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0));
    }

    [Fact]
    public void Creating_A_Timeslot()
    {
        Assert.Equal(DayOfWeek.Monday, Slot.Day);
        Assert.Equal(new TimeOnly(9, 0), Slot.Start);
        Assert.Equal(new TimeOnly(10, 0), Slot.End);
    }

    [Fact]
    public void Incorrect_Timeslot()
    {
        var exception1 = Assert.Throws<ArgumentException>(() => Timeslot.From(DayOfWeek.Saturday, new TimeOnly(9, 0), new TimeOnly(10, 0)));
        var exception2 = Assert.Throws<ArgumentException>(() => Timeslot.From(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(10, 0)));
        var exception3 = Assert.Throws<ArgumentException>(() => Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(9, 30)));

        Assert.Equal("Course cannot take place during the weekend.", exception1.Message);
        Assert.Equal("Course must be planned during working hours (9:00 - 17:00).", exception2.Message);
        Assert.Equal("Course must be one hour minimum.", exception3.Message);
    }

    [Fact]
    public void Overlap_Testing()
    {
        var slot1 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));
        var slot2 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(15, 0));
        var slot3 = Timeslot.From(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(10, 0));
        var slot4 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(11, 0));
        var slot5 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));
        var slot6 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(13, 0));

        Assert.True(slot1.Overlap(slot4));
        Assert.False(slot1.Overlap(slot2));
        Assert.False(slot1.Overlap(slot3));
        Assert.True(slot1.Overlap(slot5));
        Assert.True(slot1.Overlap(slot6));
    }

}