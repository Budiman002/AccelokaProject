using Acceloka.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.API.Features.GetAvailableTickets
{
    public class GetAvailableTicketsHandler : IRequestHandler<GetAvailableTicketsQuery, GetAvailableTicketsResponse>
    {
        private readonly AccelokaDbContext _context;

        public GetAvailableTicketsHandler(AccelokaDbContext context)
        {
            _context = context;
        }

        public async Task<GetAvailableTicketsResponse> Handle(GetAvailableTicketsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Tickets
                .Include(t => t.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.NamaKategori))
            {
                query = query.Where(t => t.Category.Name.ToLower().Contains(request.NamaKategori.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(request.KodeTicket))
            {
                query = query.Where(t => t.Code.ToLower().Contains(request.KodeTicket.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(request.NamaTicket))
            {
                query = query.Where(t => t.Name.ToLower().Contains(request.NamaTicket.ToLower()));
            }

            if (request.Harga.HasValue)
            {
                query = query.Where(t => t.Price <= request.Harga.Value);
            }

            if (request.TanggalEventMin.HasValue)
            {
                var minDate = DateTime.SpecifyKind(request.TanggalEventMin.Value, DateTimeKind.Utc);
                query = query.Where(t => t.EventDate >= minDate);
            }

            if (request.TanggalEventMax.HasValue)
            {
                var maxDate = DateTime.SpecifyKind(request.TanggalEventMax.Value, DateTimeKind.Utc);
                query = query.Where(t => t.EventDate <= maxDate);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(searchLower) ||
                    t.Name.ToLower().Contains(searchLower) ||
                    t.Category.Name.ToLower().Contains(searchLower) ||
                    t.Price.ToString().Contains(searchLower) ||
                    t.Quota.ToString().Contains(searchLower)
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
            {
                var isDescending = request.OrderState?.ToUpper() == "DESC";
                query = request.OrderBy.ToLower() switch
                {
                    "code" => isDescending ? query.OrderByDescending(t => t.Code) : query.OrderBy(t => t.Code),
                    "name" => isDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                    "category" => isDescending ? query.OrderByDescending(t => t.Category.Name) : query.OrderBy(t => t.Category.Name),
                    "eventdate" => isDescending ? query.OrderByDescending(t => t.EventDate) : query.OrderBy(t => t.EventDate),
                    "price" => isDescending ? query.OrderByDescending(t => t.Price) : query.OrderBy(t => t.Price),
                    "quota" => isDescending ? query.OrderByDescending(t => t.Quota) : query.OrderBy(t => t.Quota),
                    _ => query.OrderBy(t => t.Code)
                };
            }
            else
            {
                query = query.OrderBy(t => t.Code);
            }

            int? currentPage = null;
            int? totalPages = null;

            if (request.Page.HasValue && request.Page.Value > 0)
            {
                var pageSize = request.PageSize ?? 10;
                currentPage = request.Page.Value;
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                query = query
                    .Skip((currentPage.Value - 1) * pageSize)
                    .Take(pageSize);
            }

            var tickets = await query
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Code = t.Code,
                    Name = t.Name,
                    Category = t.Category.Name,
                    EventDate = t.EventDate,
                    Price = t.Price,
                    Quota = t.Quota
                })
                .ToListAsync(cancellationToken);

            return new GetAvailableTicketsResponse
            {
                Tickets = tickets,
                TotalCount = totalCount,
                CurrentPage = currentPage,
                TotalPages = totalPages
            };
        }
    }
}