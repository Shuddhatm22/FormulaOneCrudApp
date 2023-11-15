using FormulaOneApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneApp.Data
{
    public class AppDbcontext : IdentityDbContext
    {
        public DbSet<Team> Teams { get; set; }

        public AppDbcontext(DbContextOptions<AppDbcontext> options)
            : base(options)
        {
            
        }
    }
}
