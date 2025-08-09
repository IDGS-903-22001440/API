using Microsoft.AspNetCore.Identity;
using ProyectoFinal.Models;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiredTime { get; set; }
        [JsonIgnore]
        public ICollection<Sale>? Sales { get; set; }
    }
}
