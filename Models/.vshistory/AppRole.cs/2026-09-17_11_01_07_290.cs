using Microsoft.AspNetCore.Identity;

namespace Lumen.Models
{
    public class AppRole : IdentityRole
    {
        public AppRole() { }
        public AppRole(string name) : base(name) { }
    }
}
