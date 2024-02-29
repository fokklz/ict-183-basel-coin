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

            var auditEntries = await OnBeforeSaveChanges(userId);
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private async Task<List<BalanceAudit>> OnBeforeSaveChanges(string userId)
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

                    var email = (await Users.FirstOrDefaultAsync(u => u.Id == userId) ?? new ApplicationUser { Email = "Very SUS. Please contact your admin" }).Email;

                    var auditEntry = new BalanceAudit
                    {
                        UserId = userId,
                        Owner = entry.Entity.UserId,
                        Email = email ?? "Very SUS. Please contact your admin",
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
                    var balanceEntry = ChangeTracker.Entries<Balance>().FirstOrDefault(e => e.Entity.UserId == auditEntry.Owner);
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
