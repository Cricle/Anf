using Microsoft.AspNetCore.Identity;

namespace Anf.ChannelModel.Entity
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
