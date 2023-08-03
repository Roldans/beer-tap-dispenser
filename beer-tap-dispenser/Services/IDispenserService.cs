using beer_tap_dispenser.Model;

namespace beer_tap_dispenser.Services
{
    public interface IDispenserService
    {
        bool CloseDispenserTap(int dispenserId);
        Dispenser CreateDispenser(double flowVolume);
        IEnumerable<DispenserInfo> GetAllDispensersUsageInfo();
        bool OpenDispenserTap(int dispenserId);
    }
}