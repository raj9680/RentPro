using Rent.API.Extensions;
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
        public async Task<PropertyHierarchyResponse> ListPropertiesHierarchy(int id, int page, int pageSize)
        {
            var data = await _ownerRepository.ListPropertiesHierarchyAsync(id, page, pageSize);

            //PropertyHierarchyResponse result = PropertyHierarchyResponseExtention.ToPropertyHierarchy(data);

            #region Method 2 - Mapping

            PropertyHierarchyResponse result = new PropertyHierarchyResponse
            {
                Id = data.Id,
                Name = data.Name,
                Address = data.Address,
                Capacity = 40,
                Status = "Active",
                Floors = data.Floors.Select(f => new FloorResponse
                {
                    Id = f.Id,
                    Name = f.Name,
                    Type = "Floor",
                    Capacity = 8,
                    MonthlyRent = 4,
                    Status = "Active",
                    Flats = f.Flats.Select(fl => new FlatResponse
                    {
                        Id = fl.Id,
                        Name = fl.Name,
                        Type = "Flat",
                        Capacity = 10,
                        MonthlyRent = 2000,
                        Status = "Inactive",
                        Rooms = fl.Rooms.Select(r => new RoomResponse
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Type = "Vacant",
                            Capacity = 2,
                            MonthlyRent = 5000,
                            Status = "Vacant",
                            Beds = r.Beds.Select(b => new BedResponse
                            {
                                Id = b.Id,
                                Name = b.Name,
                                Type = "Bed",
                                Capacity = 4,
                                MonthlyRent = 2500,
                                Status = "Active"
                            }).ToList()
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            #endregion

            return result;
        }
    }
}
