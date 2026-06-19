using TelephoneBooth.Game.ElectricalSystem.Tumbler;

namespace TelephoneBooth.Game.ElectricalSystem.Services.TumblerOrderService
{
  public interface ITumblerOrderService
  {
    void AddTumblerButton(TumblerButton tumblerButton);
    void CheckCorrectEnableTumblers();
  }
}