using FormulaOneApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneApp.Data
{
    public class AppDbcontext : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public AppDbcontext(DbContextOptions<AppDbcontext> options)
            : base(options)
        {
            
        }
    }
}
