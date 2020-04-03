using Microsoft.AspNetCore.Identity;

namespace TodoApi.Models
{
    public class WebApiUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}