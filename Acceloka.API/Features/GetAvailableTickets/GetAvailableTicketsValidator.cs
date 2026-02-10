using FluentValidation;

namespace Acceloka.API.Features.GetAvailableTickets
{
    public class GetAvailableTicketsValidator : AbstractValidator<GetAvailableTicketsQuery>
    {
        public GetAvailableTicketsValidator()
        {
            // OrderBy validation
            When(x => !string.IsNullOrWhiteSpace(x.OrderBy), () =>
            {
                RuleFor(x => x.OrderBy)
                    .Must(orderBy => new[] { "code", "name", "category", "eventdate", "price", "quota" }
                        .Contains(orderBy!.ToLower()))
                    .WithMessage("OrderBy must be one of: Code, Name, Category, EventDate, Price, Quota");
            });

            // OrderState validation
            When(x => !string.IsNullOrWhiteSpace(x.OrderState), () =>
            {
                RuleFor(x => x.OrderState)
                    .Must(orderState => new[] { "asc", "desc" }
                        .Contains(orderState!.ToLower()))
                    .WithMessage("OrderState must be either ASC or DESC");
            });

            // Page validation
            When(x => x.Page.HasValue, () =>
            {
                RuleFor(x => x.Page)
                    .GreaterThan(0)
                    .WithMessage("Page must be greater than 0");
            });

            // PageSize validation
            When(x => x.PageSize.HasValue, () =>
            {
                RuleFor(x => x.PageSize)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(100)
                    .WithMessage("PageSize must be between 1 and 100");
            });
        }
    }
}