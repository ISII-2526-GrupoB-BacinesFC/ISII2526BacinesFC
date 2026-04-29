using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser() {
    }

    public ApplicationUser(string? name, string? surname, string? userName, string? street, string? city) {
        
        Name = name;
        Surname = surname;
        UserName = userName;
        Email = userName;
        City=city;
        Street=street;
    }

    public ApplicationUser(string id, string? name, string? surname, string? userName, string? street, string? city)

        //we call first the constructor without id
        :this(name,surname,userName,street, city)  {
        Id = id;
    }


    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Street { get; set; }
    public string? City { get; set; }


}



