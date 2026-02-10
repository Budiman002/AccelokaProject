using FluentValidation;

namespace Acceloka.API.Features.GetBookedTicketDetail
{
    public class GetBookedTicketDetailValidator : AbstractValidator<GetBookedTicketDetailQuery>
    {
        public GetBookedTicketDetailValidator()
        {
            RuleFor(x => x.BookedTicketId)
                .GreaterThan(0)
                .WithMessage("BookedTicketId must be greater than 0");
        }
    }
}