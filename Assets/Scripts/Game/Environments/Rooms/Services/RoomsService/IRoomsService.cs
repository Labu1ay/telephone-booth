namespace TelephoneBooth.Game.Environments.Rooms.Services
{
  public interface IRoomsService
  {
    void AddRoom(Room room);
    Room GetRoom(RoomTypeId roomTypeId);
  }
}