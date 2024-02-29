using BaselCoin2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BaselCoin2.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Balance> Balances { get; set; }

        public DbSet<BalanceAudit> BalanceAudits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Balance>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<BalanceAudit>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BalanceAudit>()
                .HasOne(p => p.Balance)
                .WithMany()
                .HasForeignKey(p => p.BalanceId).OnDelete(DeleteBehavior.NoAction);
        }

        /// <summary>
        /// Use the method with the userId parameter to save changes to the database asynchronously.
        /// </summary>
        /// <param name="userId">The user ID for whom the changes are being saved.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of objects written to the underlying database.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<int> SaveChangesAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var hasUser = !string.IsNullOrEmpty(userId);

            if (!hasUser)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }

            var auditEntries = OnBeforeSaveChanges(userId);
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<BalanceAudit> OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<BalanceAudit>();

            foreach (var entry in ChangeTracker.Entries<Balance>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    var current = (decimal)(entry.CurrentValues["Amount"] ?? 0);
                    var before = entry.State == EntityState.Added ? 0 : (decimal)(entry.OriginalValues["Amount"] ?? 0);

                    var amount = current - before;

                    var auditEntry = new BalanceAudit
                    {
                        UserId = userId,
                        BalanceId = entry.State == EntityState.Added ? 0 : entry.State == EntityState.Deleted ? null : entry.Entity.Id,
                        Amount = amount,
                        BalanceBefore = before,
                        BalanceAfter = current,
                        Action = entry.State == EntityState.Added ? "Created" :
                                 entry.State == EntityState.Modified ? "Updated" :
                                 entry.State == EntityState.Deleted ? "Deleted" : "Unknown",
                        Date = DateTime.Now
                    };
                    auditEntries.Add(auditEntry);
                }
            }

            return auditEntries;
        }

        private async Task OnAfterSaveChanges(List<BalanceAudit> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return;

            foreach (var auditEntry in auditEntries)
            {
                // For entries that were added, update the BalanceId with the generated ID.
                if (auditEntry.BalanceId == 0)
                {
                    var balanceEntry = ChangeTracker.Entries<Balance>().FirstOrDefault(e => e.Entity.UserId == auditEntry.UserId);
                    if (balanceEntry != null)
                    {
                        auditEntry.BalanceId = balanceEntry.Entity.Id;
                    }
                }
            }

            BalanceAudits.AddRange(auditEntries);
            await base.SaveChangesAsync();
        }


    }
}
