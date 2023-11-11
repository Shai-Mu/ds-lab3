﻿using System.Runtime.Serialization;

namespace Rsoi.Lab3.RatingService.Dto.Models;

[DataContract]
public class CreateRatingRequest
{
    [DataMember]
    public string Username { get; set; }
    
    [DataMember]
    public int Stars { get; set; }
}