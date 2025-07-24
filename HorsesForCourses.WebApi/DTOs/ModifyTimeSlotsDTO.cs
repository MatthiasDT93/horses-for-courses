using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class ModifyTimeSlotsDTO
{
    public List<Timeslot> TimeSlotsToAdd { get; set; } = new();

    public List<Timeslot> TimeSlotsToRemove { get; set; } = new();
}