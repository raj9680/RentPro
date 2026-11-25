using Rent.Contracts.DTOs.Owner;
using Rent.Domain.Entities;

namespace Rent.API.Extensions
{
    public static class PropertyHierarchyResponseExtention
    {
        public static PropertyHierarchyResponse ToPropertyHierarchy(Property property)
        {
            PropertyHierarchyResponse result = new PropertyHierarchyResponse();
            result.Id = property.Id;
            result.Name = property.Name;
            result.Address = property.Address;
            result.Capacity = 40;
            result.Status = "Active";

            foreach(Floor floor in property.Floors.OrderBy(f => f.Id))
            {
                FloorResponse floorResponse = new FloorResponse();
                floorResponse.Id = floor.Id;
                floorResponse.Name = floor.Name;
                floorResponse.Type = "Floor";
                floorResponse.Capacity = 4;
                floorResponse.MonthlyRent = 4;
                floorResponse.Status = "Active";

                foreach (Flat flat in floor.Flats.OrderBy(f => f.Id))
                {
                    FlatResponse flatResponse = new FlatResponse();
                    flatResponse.Id = flat.Id;
                    flatResponse.Name = flat.Name;
                    flatResponse.Type = "Flat";
                    flatResponse.Capacity = 8;
                    flatResponse.MonthlyRent = 6000;
                    flatResponse.Status = "Inactive";

                    foreach (Room room in flat.Rooms.OrderBy(f => f.Id))
                    {
                        RoomResponse roomResponse = new RoomResponse();
                        roomResponse.Id = room.Id;
                        roomResponse.Name = room.Name;
                        roomResponse.Type = "Room";
                        roomResponse.Capacity = 2;
                        roomResponse.MonthlyRent = 4400;
                        roomResponse.Status = "Active";

                        foreach (Bed bed in room.Beds.OrderBy(f => f.Id))
                        {
                            BedResponse bedResponse = new BedResponse();
                            bedResponse.Id = bed.Id;
                            bedResponse.Name = bed.Name;
                            bedResponse.Type = "Bed";
                            bedResponse.Capacity = 1;
                            bedResponse.MonthlyRent = 2400;
                            bedResponse.Status = "Inactive";
                        }

                        flatResponse.Rooms.Add(roomResponse);
                    }
                    floorResponse.Flats.Add(flatResponse);
                }
                result.Floors.Add(floorResponse);
            }
            return result;
        }
    }
}
