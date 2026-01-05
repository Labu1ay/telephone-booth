using System.Collections.Generic;

namespace TelephoneBooth.Game.Environments.Rooms.Services
{
  public class RoomsService : IRoomsService
  {
    private Dictionary<RoomTypeId, Room> _rooms = new Dictionary<RoomTypeId, Room>();
    
    public void AddRoom(Room room) => _rooms.Add(room.RoomTypeId, room);
    
    public Room GetRoom(RoomTypeId roomTypeId) => _rooms[roomTypeId];
  }
}