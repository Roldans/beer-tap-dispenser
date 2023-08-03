using beer_tap_dispenser.Model;

namespace beer_tap_dispenser.DAL
{
    public interface IDispenserDAL
    {
        Dispenser CreateDispenser(Dispenser dispenser);
        void EndDispenserUsage(int usageID);
        IEnumerable<Dispenser> GetAllDispensers();
        IEnumerable<DispenserUsage> GetDispenserUsages(int dispenserId);
        DispenserUsage GetOpenUsageByDispenserID(int dispenserId);
        void StartDispenserUsage(DispenserUsage usage);
    }
}