using HorsesForCourses.Core;

namespace HorsesForCourses.Service;


public class TimeslotDTOMapping
{

    public static Timeslot DTO_To_Timeslot(TimeSlotDTO dto)
    {
        return Timeslot.From(dto.Day, dto.Start, dto.End);
    }

    public static TimeSlotDTO Timeslot_To_DTO(Timeslot timeslot)
    {
        return new TimeSlotDTO(timeslot.Day, timeslot.Start, timeslot.End);
    }

    public static List<Timeslot> DTOList_To_TimeslotList(List<TimeSlotDTO> dtolist)
    {
        List<Timeslot> timeslotlist = new();
        foreach (var dto in dtolist)
        {
            timeslotlist.Add(DTO_To_Timeslot(dto));
        }
        return timeslotlist;
    }

    public static List<TimeSlotDTO> TimeslotList_To_DTOList(List<Timeslot> timeslotlist)
    {
        List<TimeSlotDTO> dtolist = new();
        foreach (var timeslot in timeslotlist)
        {
            dtolist.Add(Timeslot_To_DTO(timeslot));
        }
        return dtolist;
    }
}