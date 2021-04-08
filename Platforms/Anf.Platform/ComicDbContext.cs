using Anf.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf
{
    public class ComicDbContext : DbContext
    {
        public ComicDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ComicDbContext()
        {
        }

        public DbSet<Bookshelf> Bookshelfs => Set<Bookshelf>();
    }
}
