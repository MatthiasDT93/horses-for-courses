using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;


public class BookingDTOMapping
{

    public static Booking DTO_To_Booking(BookingDTO dto)
    {
        var newplanning = TimeslotDTOMapping.DTOList_To_TimeslotList(dto.Planning);
        return new Booking(newplanning, dto.Start, dto.End);
    }

    public static BookingDTO Booking_To_DTO(Booking booking)
    {
        var newplanning = TimeslotDTOMapping.TimeslotList_To_DTOList(booking.Planning);
        return new BookingDTO(newplanning, booking.StartDate, booking.EndDate);
    }

    public static List<Booking> DTOList_To_BookingList(List<BookingDTO> dtolist)
    {
        List<Booking> bookinglist = new();
        foreach (var dto in dtolist)
        {
            bookinglist.Add(DTO_To_Booking(dto));
        }
        return bookinglist;
    }

    public static List<BookingDTO> BookingList_To_DTOList(List<Booking> bookinglist)
    {
        List<BookingDTO> dtolist = new();
        foreach (var booking in bookinglist)
        {
            dtolist.Add(Booking_To_DTO(booking));
        }
        return dtolist;
    }
}