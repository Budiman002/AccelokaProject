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

            RuleFor(x => x.TicketCode)
                .NotEmpty()
                .WithMessage("TicketCode is required");

            RuleFor(x => x.NewQuantity)
                .GreaterThan(0)
                .WithMessage("NewQuantity must be greater than 0");
        }
    }
}