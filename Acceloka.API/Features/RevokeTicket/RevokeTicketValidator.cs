using FluentValidation;

namespace Acceloka.API.Features.RevokeTicket
{
    public class RevokeTicketValidator : AbstractValidator<RevokeTicketCommand>
    {
        public RevokeTicketValidator()
        {
            RuleFor(x => x.BookedTicketId)
                .GreaterThan(0)
                .WithMessage("BookedTicketId must be greater than 0");

            RuleFor(x => x.TicketCode)
                .NotEmpty()
                .WithMessage("TicketCode is required");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");
        }
    }
}