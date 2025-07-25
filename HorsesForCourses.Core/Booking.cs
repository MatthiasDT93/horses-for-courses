namespace HorsesForCourses.Core;

public record Booking
{
    public List<Timeslot> Planning { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }


    public Booking(List<Timeslot> planning, DateOnly startdate, DateOnly enddate) { Planning = planning; StartDate = startdate; EndDate = enddate; }


    public static Booking From(List<Timeslot> planning, DateOnly startdate, DateOnly enddate)
    {
        if (startdate > enddate) throw new Exception("start date must be before end date");
        return new Booking(planning, startdate, enddate);
    }


    private bool PeriodOverlap(Booking booking)
    {
        return (booking.StartDate <= StartDate && booking.EndDate <= EndDate)
            || (booking.StartDate >= StartDate && booking.EndDate <= EndDate)
            || (booking.StartDate >= StartDate && booking.EndDate <= EndDate)
            || (booking.StartDate <= StartDate && booking.EndDate >= EndDate);
    }

    private bool PlanningOverlap(Booking booking)
    {
        return Planning.Any(slot => booking.Planning.Any(newslot => slot.Overlap(newslot)));
    }

    public bool BookingOverlap(Booking booking)
    {
        if (PeriodOverlap(booking))
        {
            return PlanningOverlap(booking);
        }
        else
        {
            return false;
        }
    }

}