using Microsoft.AspNetCore.Identity;

namespace Identity;

public class User : IdentityUser
{
    public string? Initials { get; set; }
}