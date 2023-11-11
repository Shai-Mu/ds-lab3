using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Rsoi.Lab3.GatewayService.HttpApi.Models.Enum;
using Rsoi.Lab3.GatewayService.Services.Exceptions;
using Rsoi.Lab3.GatewayService.HttpApi.Converters;
using Rsoi.Lab3.GatewayService.HttpApi.Models;
using Rsoi.Lab3.GatewayService.Services.BackgroundServices;
using Rsoi.Lab3.GatewayService.Services.Clients;
using Rsoi.Lab3.LibraryService.Dto.Models;
using Rsoi.Lab3.ReservationService.Core;
using Swashbuckle.AspNetCore.Annotations;

namespace Rsoi.Lab3.GatewayService.HttpApi.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class LibrarySystemController : ControllerBase
    {
        private readonly LibraryServiceClient _libraryServiceClient;
        private readonly RatingServiceClient _ratingServiceClient;
        private readonly ReservationServiceClient _reservationServiceClient;
        private readonly UndoneRequestsQueue _undoneRequestsQueue;

        public LibrarySystemController(LibraryServiceClient libraryServiceClient, 
            RatingServiceClient ratingServiceClient, 
            ReservationServiceClient reservationServiceClient, 
            UndoneRequestsQueue undoneRequestsQueue)
        {
            _libraryServiceClient = libraryServiceClient;
            _ratingServiceClient = ratingServiceClient;
            _reservationServiceClient = reservationServiceClient;
            _undoneRequestsQueue = undoneRequestsQueue;
        }

        /// <summary>
        /// Получить список библиотек в городе
        /// </summary>
        /// <param name="city">Город</param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <response code="200">Список библиотек в городе</response>
        [HttpGet]
        [Route("/api/v1/libraries")]
        [SwaggerOperation("ApiV1LibrariesGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(LibraryPaginationResponse), description: "Список библиотек в городе")]
        public async Task<IActionResult> ApiV1LibrariesGet([FromQuery][Required()]string city, [FromQuery]decimal? page, [FromQuery][Range(1, 100)]decimal? size)
        {
            try
            {
                var response = await _libraryServiceClient.GetCityLibrariesAsync(city, (int?)page, (int?)size);
                return Ok(LibraryPageConverter.Convert(response, (int?)page, (int?)size));
            }
            catch (InternalServiceException e)
            {
                return StatusCode(e.ErrorCode ?? 500, new ErrorResponse($"{e.ServiceName}:{e.Description}"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse(e.ToString()));
            }
        }

        /// <summary>
        /// Получить список книг в выбранной библиотеке
        /// </summary>
        /// <param name="libraryUid">UUID библиотеки</param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="showAll"></param>
        /// <response code="200">Список книг библиотеке</response>
        [HttpGet]
        [Route("/api/v1/libraries/{libraryUid}/books")]
        [SwaggerOperation("ApiV1LibrariesLibraryUidBooksGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(LibraryBookPaginationResponse), description: "Список книг библиотеке")]
        public async Task<IActionResult> ApiV1LibrariesLibraryUidBooksGet([FromRoute][Required]Guid libraryUid, [FromQuery]decimal? page, [FromQuery][Range(1, 100)]decimal? size, [FromQuery]bool? showAll)
        { 
            try
            {
                var response = await _libraryServiceClient.GetBooksForLibraryIdAsync(libraryUid,
                    (int?)page,
                    (int?)size,
                    showAll);
                return Ok(LibraryBookPageConverter.Convert(response, page, size));
            }
            catch (InternalServiceException e)
            {
                return StatusCode(e.ErrorCode ?? 500, new ErrorResponse($"{e.ServiceName}:{e.Description}"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse(e.ToString()));
            }
        }

        /// <summary>
        /// Получить рейтинг пользователя
        /// </summary>
        /// <param name="xUserName">Имя пользователя</param>
        /// <response code="200">Рейтинг пользователя</response>
        [HttpGet]
        [Route("/api/v1/rating")]
        [SwaggerOperation("ApiV1RatingGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserRatingResponse), description: "Рейтинг пользователя")]
        public async Task<IActionResult> ApiV1RatingGet([FromHeader(Name = "X-User-Name")][Required]string xUserName)
        {
            try
            {
                var response = await _ratingServiceClient.GetRatingForUserAsync(xUserName);
                return Ok(new UserRatingResponse(response?.Rating?.Stars ?? 0));
            }
            catch (InternalServiceException e) when (e.ErrorCode is null)
            {
                return StatusCode(503, new {Message = "Bonus Service unavailable"});
            }
            catch (InternalServiceException e)
            {
                return StatusCode(e.ErrorCode!.Value, new ErrorResponse($"{e.ServiceName}:{e.Description}"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse(e.ToString()));
            }
        }

        /// <summary>
        /// Получить информацию по всем взятым в прокат книгам пользователя
        /// </summary>
        /// <param name="xUserName">Имя пользователя</param>
        /// <response code="200">Информация по всем взятым в прокат книгам</response>
        [HttpGet]
        [Route("/api/v1/reservations")]
        [SwaggerOperation("ApiV1ReservationsGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<BookReservationResponse>), description: "Информация по всем взятым в прокат книгам")]
        public async Task<IActionResult> ApiV1ReservationsGet([FromHeader(Name = "X-User-Name")][Required()]string xUserName)
        { 
            try
            {
                var response = await _reservationServiceClient.GetReservationsForUserAsync(xUserName);

                var reservations = new List<BookReservationResponse>();
                foreach (var reservation in response!)
                {
                    Library? library = null;
                    Books? books = null;

                    try
                    {
                        library = await _libraryServiceClient.GetLibraryAsync(reservation.LibraryId);
                        books = await _libraryServiceClient.GetBookAsync(reservation.BooksId);
                    }
                    catch
                    {
                        // ignored
                    }

                    reservations.Add(BookReservationConverter.Convert(reservation, books, library, reservation.BooksId, reservation.LibraryId));
                }
                
                return Ok(reservations);
            }
            catch (InternalServiceException e)
            {
                return StatusCode(e.ErrorCode ?? 500, new ErrorResponse($"{e.ServiceName}:{e.Description}"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse(e.ToString()));
            }
        }

        /// <summary>
        /// Взять книгу в библиотеке
        /// </summary>
        /// <param name="xUserName">Имя пользователя</param>
        /// <param name="takeBookRequest"></param>
        /// <response code="200">Информация о бронировании</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPost]
        [Route("/api/v1/reservations")]
        [SwaggerOperation("ApiV1ReservationsPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(TakeBookResponse), description: "Информация о бронировании")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationErrorResponse), description: "Ошибка валидации данных")]
        public async Task<IActionResult> ApiV1ReservationsPost([FromHeader(Name = "X-User-Name")][Required]string xUserName, [FromBody]TakeBookRequest takeBookRequest)
        {
            try
            {
                var reservations = await _reservationServiceClient.GetReservationsForUserAsync(xUserName);
                
                var rating = await _ratingServiceClient.GetRatingForUserAsync(xUserName);

                if (rating!.Rating is null || reservations!.Count >= rating.Rating.Stars)
                {
                    var modelStateDictionary = new ModelStateDictionary();
                    modelStateDictionary.AddModelError(nameof(xUserName), 
                        $"User {xUserName} has no enough rating for this action or hasn't been added to rating system.");
                    return ValidationProblem(modelStateDictionary);
                }
                
                var reservation = await _reservationServiceClient.CreateReservationAsync(xUserName,
                    takeBookRequest.BookUid,
                    takeBookRequest.LibraryUid,
                    DateOnly.Parse(takeBookRequest.TillDate).ToDateTime(TimeOnly.MaxValue));
                
                try
                {
                    await _libraryServiceClient.TakeBookAsync(takeBookRequest.BookUid, takeBookRequest.LibraryUid);
                }
                catch
                {
                    await _reservationServiceClient.DeleteReservationAsync(reservation.Id);
                    throw;
                }

                Books? book = null;
                Library? library = null;
                
                try
                {
                    book = await _libraryServiceClient.GetBookAsync(takeBookRequest.BookUid);
                    library = await _libraryServiceClient.GetLibraryAsync(takeBookRequest.LibraryUid);
                }
                catch (InternalServiceException e)
                {
                    if (e.ErrorCode is not null)
                        throw;
                }
                

                return Ok(new TakeBookResponse(reservation.Id, 
                    ReservationStatusConverter.Convert(reservation.ReservationStatus), 
                    reservation.StartDate.ToString("yyyy-MM-dd"), 
                    reservation.TillDate.ToString("yyyy-MM-dd"),
                    takeBookRequest.BookUid,
                    takeBookRequest.LibraryUid,
                    LibraryBookConverter.Convert(book),
                    LibraryConverter.Convert(library),
                    new UserRatingResponse(rating.Rating.Stars)));
            }
            catch (InternalServiceException e) when(e.ErrorCode is null)
            {
                return StatusCode(503, new {Message = "Bonus Service unavailable"});
            }
            catch (InternalServiceException e) when (e.ErrorCode == 404)
            {
                var modelStateDictionary = new ModelStateDictionary();
                modelStateDictionary.AddModelError(nameof(takeBookRequest.BookUid), "Library doesn't exist or book doesn't exist in this library");

                return ValidationProblem(modelStateDictionary);
            }
            catch (InternalServiceException e)
            {
                return StatusCode(e.ErrorCode ?? 500, new ErrorResponse($"{e.ServiceName}:{e.Description}"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse(e.ToString()));
            }
        }

        /// <summary>
        /// Вернуть книгу
        /// </summary>
        /// <param name="reservationUid">UUID бронирования</param>
        /// <param name="xUserName">Имя пользователя</param>
        /// <param name="returnBookRequest"></param>
        /// <response code="204">Книга успешно возвращена</response>
        /// <response code="404">Бронирование не найдено</response>
        [HttpPost]
        [Route("/api/v1/reservations/{reservationUid}/return")]
        [SwaggerOperation("ApiV1ReservationsReservationUidReturnPost")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "Бронирование не найдено")]
        public async Task<IActionResult> ApiV1ReservationsReservationUidReturnPost([FromRoute][Required]Guid reservationUid, [FromHeader(Name = "X-User-Name")][Required]string xUserName, [FromBody]ReturnBookRequest returnBookRequest)
        { 
            try
            {
                var reservation = await _reservationServiceClient.CloseReservationAsync(reservationUid, DateTimeOffset.Parse(returnBookRequest.Date));


                try
                {
                    await _libraryServiceClient.ReturnBookAsync(reservation.Reservation.BooksId, reservation.Reservation.LibraryId, (int)BookConditionConverter.Convert(returnBookRequest.Condition));
                }
                catch (InternalServiceException e) when (e.ErrorCode is null)
                {
                    _undoneRequestsQueue.AddTaskToQueue(new UndoneRequest(e.Request, _libraryServiceClient.ServiceAddress));
                }
                
                var book = await _libraryServiceClient.GetBookAsync(reservation.Reservation.BooksId);
                
                int ratingModifier = 0;

                if (book?.Condition != BookConditionConverter.Convert(returnBookRequest.Condition))
                    ratingModifier -= 10;

                if (reservation.Reservation.ReservationStatus is ReservationStatus.Expired)
                    ratingModifier -= 10;

                if (ratingModifier == 0)
                    ratingModifier = 1;

                try
                {
                    await _ratingServiceClient.ModifyRatingForUserAsync(xUserName, ratingModifier);
                }
                catch (InternalServiceException e) when (e.ErrorCode is null)
                {
                    _undoneRequestsQueue.AddTaskToQueue(new UndoneRequest(e.Request, _ratingServiceClient.ServiceAddress));
                }

                return NoContent();
            }
            catch (InternalServiceException e) when (e is { ErrorCode: 404, ServiceName: "Reservation service" })
            {
                return NotFound(new ErrorResponse($"Reservation with id {reservationUid} doesn't exist."));
            }
            catch (InternalServiceException e)
            {
                return StatusCode(e.ErrorCode ?? 500, new ErrorResponse($"{e.ServiceName}:{e.Description}"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse(e.ToString()));
            }
        }
    }
}
