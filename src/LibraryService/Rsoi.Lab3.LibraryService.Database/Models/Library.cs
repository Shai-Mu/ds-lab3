﻿namespace Rsoi.Lab3.LibraryService.Database.Models;

public class Library
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string City { get; set; }
    
    public string Address { get; set; }
    
    
    public ICollection<LibraryBooks>? LibrariesBooks { get; set; }

    public Library(Guid id, 
        string name, 
        string city, 
        string address)
    {
        Id = id;
        Name = name;
        City = city;
        Address = address;
    }
}