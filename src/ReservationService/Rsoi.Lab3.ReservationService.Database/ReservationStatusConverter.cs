using DbReservationStatus = Rsoi.Lab3.ReservationService.Database.ReservationStatus;
using CoreReservationStatus = Rsoi.Lab3.ReservationService.Core.ReservationStatus;

namespace Rsoi.Lab3.ReservationService.Database;

public static class ReservationStatusConverter
{
    public static CoreReservationStatus Convert(ReservationStatus dbReservationStatus)
    {
        return dbReservationStatus switch
        {
            ReservationStatus.Expired => CoreReservationStatus.Expired,
            ReservationStatus.Rented => CoreReservationStatus.Rented,
            ReservationStatus.Returned => CoreReservationStatus.Returned,
            _ => throw new ArgumentOutOfRangeException(nameof(dbReservationStatus), dbReservationStatus, null)
        };
    }
    
    public static ReservationStatus Convert(CoreReservationStatus coreReservationStatus)
    {
        return coreReservationStatus switch
        {
            CoreReservationStatus.Expired => ReservationStatus.Expired,
            CoreReservationStatus.Rented => ReservationStatus.Rented,
            CoreReservationStatus.Returned => ReservationStatus.Returned,
            _ => throw new ArgumentOutOfRangeException(nameof(coreReservationStatus), coreReservationStatus, null)
        };
    }
}