using Rsoi.Lab3.GatewayService.HttpApi.Models;
using Rsoi.Lab3.LibraryService.Dto.Models;
using Rsoi.Lab3.ReservationService.Core;

namespace Rsoi.Lab3.GatewayService.HttpApi.Converters;

public static class BookReservationConverter
{
    public static BookReservationResponse Convert(Reservation reservation,
        Books? books,
        Library? library,
        Guid bookUid,
        Guid libraryUid)
    {
        return new BookReservationResponse(reservation.Id,
            ReservationStatusConverter.Convert(reservation.ReservationStatus),
            reservation.StartDate.ToString("yyyy-MM-dd"),
            reservation.TillDate.ToString("yyyy-MM-dd"),
            bookUid,
            libraryUid,
            LibraryBookConverter.Convert(books),
            LibraryConverter.Convert(library));
    }
}