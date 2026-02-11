using FluentValidation;

namespace Acceloka.API.Features.EditBookedTicket
{
    public class EditBookedTicketValidator : AbstractValidator<EditBookedTicketCommand>
    {
        public EditBookedTicketValidator()
        {
            RuleFor(x => x.BookedTicketId)
                .GreaterThan(0)
                .WithMessage("BookedTicketId must be greater than 0");

            RuleFor(x => x.Tickets)
                .NotEmpty()
                .WithMessage("At least one ticket must be provided");

            RuleForEach(x => x.Tickets).ChildRules(ticket =>
            {
                ticket.RuleFor(t => t.TicketCode)
                    .NotEmpty()
                    .WithMessage("TicketCode is required");

                ticket.RuleFor(t => t.NewQuantity)
                    .GreaterThan(0)
                    .WithMessage("NewQuantity must be greater than 0");
            });
        }
    }
}