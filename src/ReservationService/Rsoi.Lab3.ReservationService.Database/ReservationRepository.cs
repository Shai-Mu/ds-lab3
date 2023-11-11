using Microsoft.EntityFrameworkCore;
using Rsoi.Lab3.ReservationService.Core;

namespace Rsoi.Lab3.ReservationService.Database;

public class ReservationRepository : IReservationRepository
{
    private readonly ReservationContext _reservationContext;

    public ReservationRepository(ReservationContext reservationContext)
    {
        _reservationContext = reservationContext;
    }

    public async Task<Lab3.ReservationService.Core.Reservation> CreateReservationAsync(string username, Guid booksId, Guid libraryId, DateOnly tillDate)
    {
        var reservation = new Reservation(Guid.NewGuid(), username, booksId, libraryId, ReservationStatus.Rented,
            DateOnly.FromDateTime(DateTime.Now), tillDate);

        _reservationContext.Reservations.Add(reservation);

        await _reservationContext.SaveChangesAsync();

        var reservationToReturn = await _reservationContext.Reservations.FirstAsync(r => r.Id == reservation.Id);

        return ReservationConverter.Convert(reservationToReturn);
    }

    public async Task<List<Lab3.ReservationService.Core.Reservation>> GetReservationsForUserAsync(string username)
    {
        var reservations = await _reservationContext.Reservations
            .Where(r => r.Username == username)
            .AsNoTracking()
            .ToListAsync();

        return reservations
            .Select(ReservationConverter.Convert)
            .ToList()!;
    }

    public async Task<Lab3.ReservationService.Core.Reservation> GetReservationForUserBookAndLibraryAsync(string username, Guid booksId, Guid libraryId)
    {
        var reservation = await _reservationContext.Reservations
            .OrderByDescending(r => r.StartDate)
            .AsNoTracking()
            .FirstAsync(r => r.Username == username 
                             && r.BooksId == booksId 
                             && r.LibraryId == libraryId);

        return ReservationConverter.Convert(reservation);
    }

    public async Task UpdateReservationAsync(Guid id, Lab3.ReservationService.Core.ReservationStatus status)
    {
        var reservation = await _reservationContext.Reservations
            .FirstAsync(r => r.Id == id);

        reservation.ReservationStatus = ReservationStatusConverter.Convert(status);

        await _reservationContext.SaveChangesAsync();
    }

    public async Task<Lab3.ReservationService.Core.Reservation?> FindReservationAsync(Guid id)
    {
        var reservation = await _reservationContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == id);

        return ReservationConverter.Convert(reservation);
    }

    public async Task DeleteReservationAsync(Guid id)
    {
        var reservation = await _reservationContext.Reservations.FirstAsync(r => r.Id == id);

        _reservationContext.Reservations.Remove(reservation);

        await _reservationContext.SaveChangesAsync();
    }
}