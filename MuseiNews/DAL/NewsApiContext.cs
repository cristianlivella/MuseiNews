using Microsoft.EntityFrameworkCore;
using MuseiNews.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;

namespace MuseiNews.DAL
{
    public class NewsApiContext : DbContext
    {
        public NewsApiContext(DbContextOptions<NewsApiContext> options) : base(options) {

        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsRead> NewsReads { get; set; }

    }
}
