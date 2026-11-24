using Microsoft.EntityFrameworkCore;
using Rent.Application.Interfaces.IRepository;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly RentProDbContext _dbContext;
        public OwnerRepository(RentProDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(IEnumerable<Property> Properties, int TotalRecords)> ListAllPropertiesAsync(string propType, int page, int pageSize, int id)
        {
            IQueryable<Property> query = _dbContext.Properties
                .AsNoTracking()
                .Where(x => x.OwnerId == id);

            int totalRecords = await query.CountAsync();

            var properties = await query
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (properties, totalRecords);
        }

        public async Task<List<Floor>>? ListPropertiesHierarchyAsync(int id, int page, int pageSize)
        {
            // page = Math.Max(page, 1);
            // pageSize = Math.Clamp(pageSize, 1, 100);

            List<Floor> floors = await _dbContext.Floors
                .AsNoTracking()
                .AsSplitQuery()

                .Where(f => f.PropId == id)
                .OrderBy(f => f.Id)
                .ThenBy(f => f.Id)

                .Skip((page - 1) * pageSize)
                .Take(pageSize)

                .Include(f => f.Flats)
                    .ThenInclude(fl => fl.Rooms)
                        .ThenInclude(r => r.Beds)

                .ToListAsync();

            return floors;
        }
    }
}
