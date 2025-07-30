using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;


public class CoachDTOMapping
{


    public static Coach DTO_To_Coach(CoachDTO dto)
    {
        var newcoach = new Coach(dto.Name, dto.Email);
        newcoach.OverWriteCompetences(dto.Competencies);
        foreach (var bookingdto in dto.Bookings?.Distinct() ?? Enumerable.Empty<BookingDTO>())
        {
            newcoach.BookIn(BookingDTOMapping.DTO_To_Booking(bookingdto));
        }
        return newcoach;
    }

    public static CoachDTO Coach_To_DTO(Coach coach)
    {
        List<string> comp = new();
        List<BookingDTO> dtobookings = new();
        foreach (var skill in coach.competencies?.Distinct() ?? Enumerable.Empty<string>()) // done because competencies is a IReadOnlyList in Coach
        {
            comp.Add(skill);
        }
        foreach (var booking in coach.bookings?.Distinct() ?? Enumerable.Empty<Booking>()) // bookings is also an IReadOnlyList
        {
            dtobookings.Add(BookingDTOMapping.Booking_To_DTO(booking));
        }
        return new CoachDTO(coach.Name, coach.Email.Value, comp, dtobookings);
    }

    public static List<Coach> DTOList_To_CoachList(List<CoachDTO> dtolist)
    {
        List<Coach> coachlist = new();
        foreach (var dto in dtolist)
        {
            coachlist.Add(DTO_To_Coach(dto));
        }
        return coachlist;
    }

    public static List<CoachDTO> CoachList_To_DTOList(List<Coach> coachlist)
    {
        List<CoachDTO> dtolist = new();
        foreach (var coach in coachlist)
        {
            dtolist.Add(Coach_To_DTO(coach));
        }
        return dtolist;
    }


}