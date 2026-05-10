using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
    }
    public ApplicationUser(string id, string name, string surname, string userName, string deliveryAddress, string email)
    {
        Id = id;
        Name = name;
        Surname = surname;
        UserName = userName;
        DeliveryAddress = deliveryAddress;
        Email = email;
    }

    [Display(Name = "Name")]
    public string? Name
    {
        get;
        set;
    }

    [Required]
    [StringLength(100, ErrorMessage = "La direccion no puede ser superior a 100 carecteres")]
    public string DeliveryAddress { get; set; }

    [Display(Name = "Surname")]
    public string? Surname
    {
        get;
        set;
    }
}

