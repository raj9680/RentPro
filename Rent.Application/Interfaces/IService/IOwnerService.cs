using Rent.Contracts.DTOs.Owner;
using Rent.Domain.Entities;

namespace Rent.Application.Interfaces.IService
{
    public interface IOwnerService
    {
        Task<PropertySummaryResponse> ListProperties(string propType, int page, int pageSize, int claimId);

        Task<List<FloorResponse>> ListPropertiesHierarchy(int id, int page, int pageSize);
    }
}
