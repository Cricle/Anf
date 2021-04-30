using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web
{
    public class AnfDbContext : DbContext
    {
        public AnfDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AnfDbContext()
        {
        }
    }
}
