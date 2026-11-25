using Rent.Contracts.DTOs.Owner;
using Rent.Domain.Entities;

namespace Rent.Application.Interfaces.IRepository
{
    public interface IOwnerRepository
    {
        Task<(IEnumerable<Property> Properties, int TotalRecords)> ListAllPropertiesAsync(string propType, int page, int pageSize, int id);
        Task<Property>? ListPropertiesHierarchyAsync(int id, int page, int pageSize);
    }
}
