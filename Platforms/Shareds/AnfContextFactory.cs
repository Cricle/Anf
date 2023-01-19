#if COMPILE_EF_TIME

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Anf.WebService
{
    public class AnfContextFactory : IDesignTimeDbContextFactory<AnfDbContext>
    {
        public AnfDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnfDbContext>();
            optionsBuilder.UseInMemoryDatabase("Data Source=blog.db");

            return new AnfDbContext(optionsBuilder.Options);
        }
    }
}
#endif