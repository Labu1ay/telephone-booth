using System.Collections.Generic;
using System.Linq;
using TelephoneBooth.Game.Environments.Rooms;
using TelephoneBooth.Game.Environments.Rooms.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.Services
{
  public class EnemyPatrolService : IEnemyPatrolService, IInitializable
  {
    private readonly IRoomsService _roomsService;
    
    private RoomTypeId[] _roomTypesId;
    private readonly List<RoomTypeId> _usedRoomTypesId = new ();
    private RoomTypeId _lastRoomTypeId;

    [Inject]
    public EnemyPatrolService(IRoomsService roomsService)
    {
      _roomsService = roomsService;
    }
    
    public void Initialize()
    {
      _roomTypesId = new []
      {
        RoomTypeId.Room1,
        RoomTypeId.Room2,
        RoomTypeId.Room3,
        RoomTypeId.Room4,
      };
    }

    public Vector3 GetRandomPatrolPosition()
    {
      var available = _roomTypesId.Except(_usedRoomTypesId).ToList();

      if (available.Count == 0)
      {
        _usedRoomTypesId.Clear();
        available = _roomTypesId.Where(p => p != _lastRoomTypeId).ToList();
      }

      var next = available[Random.Range(0, available.Count)];

      _usedRoomTypesId.Add(next);
      _lastRoomTypeId = next;

      var room = _roomsService.GetRoom(next);
      var position = room.RoomPatrolPoints[Random.Range(0, room.RoomPatrolPoints.Length)].position;
      
      return position;
    }
  }
}