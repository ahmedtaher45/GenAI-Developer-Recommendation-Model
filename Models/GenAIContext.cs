using Microsoft.EntityFrameworkCore;

namespace GenAI_Recommendation_Model.Models
{
    public class GenAIContext : DbContext
    {
        public GenAIContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Developer> Developers { get; set; }

    }
}
