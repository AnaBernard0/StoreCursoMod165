using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreCursoMod165.Models;

namespace StoreCursoMod165.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //Tables 
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
    }
}
