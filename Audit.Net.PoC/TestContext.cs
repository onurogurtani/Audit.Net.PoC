using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Audit.Net.PoC
{
   public class TestContext : AuditDbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageAudit> MessageAudits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("data source=localhost;initial catalog=TransactionTestEfCore;integrated security=true;");
    }
}
