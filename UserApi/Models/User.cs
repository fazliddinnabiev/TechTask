using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace UserApi.Models;

public class User
{
    [Key] 
    [Required] 
    [Name("username")] 
    public string UserName { get; set; } = null!;
    
    [Required] [Name("useridentifier")] 
    public string UserIdentifier { get; set; } = null!;
    
    [Name("age")] 
    public int Age { get; set; }
    
    [Name("city")] 
    public string City { get; set; } = null!;
    
    [Name("phonenumber")] 
    public string PhoneNumber { get; set; } = null!;
    
    [Name("email")] 
    public string Email { get; set; } = null!;
}