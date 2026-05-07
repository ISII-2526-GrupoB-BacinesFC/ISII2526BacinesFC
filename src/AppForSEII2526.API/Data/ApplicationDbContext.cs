using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models; // Para que reconozca ApplicationUser y los demás

namespace AppForSEII2526.API.Data
{
    // Es vital heredar de IdentityDbContext<ApplicationUser> 
    // para que la base de datos sepa gestionar tu modelo de usuario personalizado
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Model> Models { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
    }
}