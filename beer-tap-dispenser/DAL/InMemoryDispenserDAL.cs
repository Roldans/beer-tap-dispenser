using System;
using System.Collections.Generic;
using System.Linq;
using beer_tap_dispenser.Model;
using Microsoft.Extensions.Logging;

namespace beer_tap_dispenser.DAL
{
    public class InMemoryDispenserDAL : IDispenserDAL
    {
        private readonly List<Dispenser> _dispensers;
        private readonly List<DispenserUsage> _dispenserUsages;
        private readonly ILogger<InMemoryDispenserDAL> _logger;

        public InMemoryDispenserDAL(ILogger<InMemoryDispenserDAL> logger)
        {
            _dispensers = new List<Dispenser>();
            _dispenserUsages = new List<DispenserUsage>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Dispenser CreateDispenser(Dispenser dispenser)
        {
            if (dispenser == null)
            {
                throw new ArgumentNullException(nameof(dispenser));
            }

            dispenser.Id = _dispensers.Count + 1;
            _dispensers.Add(dispenser);
            _logger.LogInformation($"Created new dispenser with Id: {dispenser.Id}, FlowVolume: {dispenser.FlowVolume}");
            return dispenser;
        }

        public IEnumerable<Dispenser> GetAllDispensers()
        {
            return _dispensers;
        }

        public void StartDispenserUsage(DispenserUsage usage)
        {
            if (usage == null)
            {
                throw new ArgumentNullException(nameof(usage));
            }

            usage.Id = _dispenserUsages.Count + 1;
            _dispenserUsages.Add(usage);
            _logger.LogInformation($"Started dispenser usage with Id: {usage.Id}, DispenserId: {usage.DispenserId}, StartTime: {usage.StartTime}");
        }

        public void EndDispenserUsage(int usageID)
        {
            var usage = _dispenserUsages.FirstOrDefault(u => u.Id == usageID);
            if (usage != null)
            {
                usage.EndTime = DateTime.Now;
                _logger.LogInformation($"Ended dispenser usage with Id: {usage.Id}, EndTime: {usage.EndTime}");
            }
            else
            {
                _logger.LogWarning($"Attempted to end dispenser usage with Id: {usageID}, but usage not found.");
            }
        }

        public DispenserUsage GetOpenUsageByDispenserID(int dispenserId)
        {
            return _dispenserUsages.FirstOrDefault(u => u.DispenserId == dispenserId && u.EndTime == null);
        }

        public IEnumerable<DispenserUsage> GetDispenserUsages(int dispenserId)
        {
            return _dispenserUsages.Where(u => u.DispenserId == dispenserId).ToList();
        }
    }
}
