using TelephoneBooth.Game.Environments.Rooms.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.Environments.Rooms
{
  public class Room : MonoBehaviour
  {
    [Inject] private readonly IRoomsService _roomsService;
    [field: SerializeField] public RoomTypeId RoomTypeId { get; private set; }
    [field: SerializeField] public Transform[] RoomPatrolPoints { get; private set; }

    private void Start()
    {
      _roomsService.AddRoom(this);
    }
  }
}