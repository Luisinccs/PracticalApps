using System.ComponentModel.DataAnnotations;

namespace Northwind.Mvc.Models;

public record Thing(
    // Decorate the Id Property with a validation attribure to limit the range of allowed 
    // numbers to 1 to 10
    [Range(1,10)] int? Id,      
    [Required] string? Color,
    [EmailAddress] string? Email);