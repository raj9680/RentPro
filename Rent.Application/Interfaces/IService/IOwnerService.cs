using Rent.Contracts.DTOs.Owner;

namespace Rent.Application.Interfaces.IService
{
    public interface IOwnerService
    {
        Task<PropertySummaryResponse> ListProperties(string propType, int page, int pageSize, int claimId);

        Task<PropertyHierarchyResponse> ListPropertiesHierarchy(int id, int page, int pageSize);
    }
}
