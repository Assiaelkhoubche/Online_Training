using Microsoft.AspNetCore.Identity;

namespace Online_training.Server.Models
{
    public class User:IdentityUser
    {
        public string? PictureUrl { get; set; }
        public string? Bio {  get; set; }

    }
}
