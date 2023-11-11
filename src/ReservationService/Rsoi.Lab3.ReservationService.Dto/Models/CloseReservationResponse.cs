﻿using System.Runtime.Serialization;
using Rsoi.Lab3.ReservationService.Core;

namespace Rsoi.Lab3.ReservationService.Dto.Models;

[DataContract]
public class CloseReservationResponse
{
    [DataMember]
    public DateOnly CloseDate { get; set; }
    
    [DataMember]
    public Reservation Reservation { get; set; }

    public CloseReservationResponse(Reservation reservation, DateOnly closeDate)
    {
        Reservation = reservation;
        CloseDate = closeDate;
    }
}