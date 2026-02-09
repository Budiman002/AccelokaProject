using Acceloka.API.Data;
using Acceloka.API.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Acceloka.API.Data
{
    public class AccelokaDbContext : DbContext
    {
        public AccelokaDbContext(DbContextOptions<AccelokaDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<BookedTicket> BookedTickets { get; set; }
        public DbSet<BookedTicketDetail> BookedTicketDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Ticket Configuration
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.Code)
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Tickets)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<BookedTicket>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.BookedDate)
                    .IsRequired();
            });

            modelBuilder.Entity<BookedTicketDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TicketCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(e => e.BookedTicket)
                    .WithMany(bt => bt.BookedTicketDetails)
                    .HasForeignKey(e => e.BookedTicketId)
                    .OnDelete(DeleteBehavior.Cascade);

 
                entity.HasOne(e => e.Ticket)
                    .WithMany(t => t.BookedTicketDetails)
                    .HasForeignKey(e => e.TicketCode)
                    .HasPrincipalKey(t => t.Code)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}