using Rent.Application.Interfaces.IRepository;
using Rent.Application.Interfaces.IService;
using Rent.Contracts.DTOs.Owner;

namespace Rent.Application.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        public OwnerService(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        public async Task<PropertySummaryResponse> ListProperties(string propType, int page, int pageSize, int id)
        {
            var (properties, totalRecords) = await _ownerRepository.ListAllPropertiesAsync(propType, page, pageSize, id);

            int totalPages = (int)Math.Ceiling(((double)totalRecords/pageSize));

            var items = properties.Select(p => new PropertyResponse
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                City = p.City,
                EmptySpace = 5,
                Capacity = 20,
                Occupied = 12,
                Status = p.Status.ToString(),
            }).ToList();

            return new PropertySummaryResponse
            {
                Items = items,
                TotalPages = totalPages,
                TotalRecords = totalRecords,
                CurrentPage = page,
            };
        }

        // change
        public async Task<List<FloorResponse>> ListPropertiesHierarchy(int id, int page, int pageSize)
        {
            var data = await _ownerRepository.ListPropertiesHierarchyAsync(id, page, pageSize);

            List<FloorResponse> floors = data.Select(f => new FloorResponse
            {
                Id = f.Id,
                Name = f.Name,
                Type = "Floor",
                Capacity = 9,
                MonthlyRent = 4,
                Status = "Active",
                Flats = f.Flats.Select(x => new FlatResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = "Flat",
                    Capacity = 10,
                    MonthlyRent = 4000,
                    Status = "Inactive",
                    Rooms = x.Rooms.Select(r => new RoomResponse
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Type = "Active",
                        Capacity = 12,
                        MonthlyRent = 2000,
                        Status = "Active",
                        Beds = r.Beds.Select(b => new BedResponse
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Type = "Bed",
                            Capacity = 14,
                            MonthlyRent = 3000,
                            Status = "Active"
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).ToList();

            return floors;
        }
    }
}
