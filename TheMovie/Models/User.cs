using Microsoft.AspNetCore.Identity;

namespace TheMovie.Models
{
    public class User : IdentityUser
    {
        public override string Email { get; set; }
    }
}
