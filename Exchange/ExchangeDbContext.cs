using Exchange.Entities;
using Microsoft.EntityFrameworkCore;

namespace Exchange
{
    public class ExchangeDbContext : DbContext
    {
        public DbSet<User> Users { get; protected set; }
    }
}