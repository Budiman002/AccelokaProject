using Acceloka.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.API.Data
{
    public static class DbSeeder
    {
        public static void SeedData(AccelokaDbContext context)
        {
            // Check if data already exists
            if (context.Categories.Any())
            {
                return;
            }

            // Seed Categories
            var categories = new List<Category>
            {
                new Category { Name = "Cinema" },
                new Category { Name = "Transportasi Darat" },
                new Category { Name = "Transportasi Laut" },
                new Category { Name = "Hotel" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Seed Tickets
            var tickets = new List<Ticket>
            {
                // Cinema tickets
                new Ticket
                {
                    Code = "C001",
                    Name = "Ironman CGV",
                    CategoryId = categories.First(c => c.Name == "Cinema").Id,
                    EventDate = DateTime.SpecifyKind(new DateTime(2022, 3, 1, 12, 0, 0), DateTimeKind.Utc),
                    Price = 50000000,
                    Quota = 99
                },
                new Ticket
                {
                    Code = "C002",
                    Name = "Black Panther",
                    CategoryId = categories.First(c => c.Name == "Cinema").Id,
                    EventDate = DateTime.SpecifyKind(new DateTime(2022, 3, 1, 12, 0, 0), DateTimeKind.Utc),
                    Price = 50000000,
                    Quota = 99
                },

                // Transportasi Darat tickets
                new Ticket
                {
                    Code = "TD001",
                    Name = "Bus jawa-sumatra",
                    CategoryId = categories.First(c => c.Name == "Transportasi Darat").Id,
                    EventDate = DateTime.SpecifyKind(new DateTime(2022, 3, 2, 17, 59, 0), DateTimeKind.Utc),
                    Price = 50000000,
                    Quota = 80
                },

                // Transportasi Laut tickets
                new Ticket
                {
                    Code = "TL001",
                    Name = "Kapal Ferri jawa-sumatra",
                    CategoryId = categories.First(c => c.Name == "Transportasi Laut").Id,
                    EventDate = DateTime.SpecifyKind(new DateTime(2022, 3, 2, 17, 59, 0), DateTimeKind.Utc),
                    Price = 50000000,
                    Quota = 70
                },

                // Hotel tickets
                new Ticket
                {
                    Code = "H001",
                    Name = "Ibis Hotel Jakarta 21-23",
                    CategoryId = categories.First(c => c.Name == "Hotel").Id,
                    EventDate = DateTime.SpecifyKind(new DateTime(2022, 3, 2, 17, 59, 0), DateTimeKind.Utc),
                    Price = 50000000,
                    Quota = 76
                }
            };

            context.Tickets.AddRange(tickets);
            context.SaveChanges();
        }
    }
}