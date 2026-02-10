using Acceloka.API.Data;
using Acceloka.API.Features.BookTicket;
using Acceloka.API.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.API.Features.BookTicket
{
    public class BookTicketValidator : AbstractValidator<BookTicketCommand>
    {
        private readonly AccelokaDbContext _context;

        public BookTicketValidator(AccelokaDbContext context)
        {
            _context = context;

            // Basic validations
            RuleFor(x => x.Tickets)
                .NotEmpty()
                .WithMessage("Tickets list cannot be empty");

            RuleForEach(x => x.Tickets).ChildRules(ticket =>
            {
                ticket.RuleFor(x => x.KodeTicket)
                    .NotEmpty()
                    .WithMessage("Ticket code is required");

                ticket.RuleFor(x => x.Qty)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");
            });

            // VALIDATION 1: Ticket code must exist
            RuleFor(x => x.Tickets)
                .MustAsync(async (tickets, cancellation) =>
                {
                    var codes = tickets.Select(t => t.KodeTicket).Distinct().ToList();
                    var existingCodes = await _context.Tickets
                        .Where(t => codes.Contains(t.Code))
                        .Select(t => t.Code)
                        .ToListAsync(cancellation);

                    return codes.All(code => existingCodes.Contains(code));
                })
                .WithMessage("One or more ticket codes do not exist");

            // VALIDATION 2: No duplicate ticket codes
            RuleFor(x => x.Tickets)
                .Must(tickets =>
                {
                    var codes = tickets.Select(t => t.KodeTicket).ToList();
                    return codes.Count == codes.Distinct().Count();
                })
                .WithMessage("Duplicate ticket codes are not allowed");

            // VALIDATION 3: Sufficient quota
            RuleFor(x => x.Tickets)
                .MustAsync(async (tickets, cancellation) =>
                {
                    foreach (var item in tickets)
                    {
                        var ticket = await _context.Tickets
                            .FirstOrDefaultAsync(t => t.Code == item.KodeTicket, cancellation);

                        if (ticket == null || ticket.Quota < item.Qty)
                        {
                            return false;
                        }
                    }
                    return true;
                })
                .WithMessage("Insufficient quota for one or more tickets");

            // VALIDATION 4: Event date must be in the future
            RuleFor(x => x.Tickets)
                .MustAsync(async (tickets, cancellation) =>
                {
                    var codes = tickets.Select(t => t.KodeTicket).ToList();
                    var ticketDates = await _context.Tickets
                        .Where(t => codes.Contains(t.Code))
                        .Select(t => t.EventDate)
                        .ToListAsync(cancellation);

                    return ticketDates.All(date => date > DateTime.UtcNow);
                })
                .WithMessage("Cannot book tickets for past events");
        }
    }
}