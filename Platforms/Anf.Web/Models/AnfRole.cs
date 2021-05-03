using Microsoft.AspNetCore.Identity;

namespace Anf.Web.Models
{
    public class AnfRole : IdentityRole<long>
    {
        public AnfRole()
        {
        }

        public AnfRole(string roleName) : base(roleName)
        {
        }
    }
}
