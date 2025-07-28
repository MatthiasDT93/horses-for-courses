using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class ModifyTimeSlotsDTO
{
    public List<TimeSlotDTO> TimeSlotsToAdd { get; set; } = new();

    public List<TimeSlotDTO> TimeSlotsToRemove { get; set; } = new();
}