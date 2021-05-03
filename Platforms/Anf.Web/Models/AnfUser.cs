using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Models
{
    public class AnfUser : IdentityUser<long>
    {
        public AnfUser()
        {
        }

        public AnfUser(string userName) : base(userName)
        {
        }
    }
}
