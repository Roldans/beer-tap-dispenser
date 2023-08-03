using System;
using System.Collections.Generic;
using System.Linq;
using beer_tap_dispenser.DAL;
using beer_tap_dispenser.Model;
using Microsoft.Extensions.Logging;

namespace beer_tap_dispenser.Services
{
    public class DispenserService : IDispenserService
    {
        private readonly IDispenserDAL _dispenserDAL;
        private readonly ILogger<DispenserService> _logger;

        public DispenserService(IDispenserDAL dispenserDAL, ILogger<DispenserService> logger)
        {
            _dispenserDAL = dispenserDAL ?? throw new ArgumentNullException(nameof(dispenserDAL));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Dispenser CreateDispenser(double flowVolume)
        {
            if (flowVolume <= 0)
            {
                _logger.LogError("Flow volume must be greater than 0.");
                throw new ArgumentException("Flow volume must be greater than 0.", nameof(flowVolume));
            }

            var dispenser = new Dispenser
            {
                FlowVolume = flowVolume,
            };

            _logger.LogInformation($"Creating new dispenser with FlowVolume: {flowVolume}");
            return _dispenserDAL.CreateDispenser(dispenser);
        }

        public bool OpenDispenserTap(int dispenserId)
        {
            var usage = _dispenserDAL.GetOpenUsageByDispenserID(dispenserId);
            if (usage != null)
            {
                _logger.LogWarning($"Attempted to open tap for dispenser {dispenserId}, but tap is already open.");
                // Tap is already open
                return false;
            }

            var newUsage = new DispenserUsage
            {
                DispenserId = dispenserId,
                StartTime = DateTime.Now,
                EndTime = null,
            };

            _dispenserDAL.StartDispenserUsage(newUsage);
            _logger.LogInformation($"Tap opened for dispenser {dispenserId} at {newUsage.StartTime}");
            return true;
        }

        public bool CloseDispenserTap(int dispenserId)
        {
            var usage = _dispenserDAL.GetOpenUsageByDispenserID(dispenserId);
            if (usage == null || usage.EndTime != null)
            {
                _logger.LogWarning($"Attempted to close tap for dispenser {dispenserId}, but tap is already closed or not in use.");
                // Tap is already closed or not in use
                return false;
            }

            usage.EndTime = DateTime.Now;
            _dispenserDAL.EndDispenserUsage(usage.Id);
            _logger.LogInformation($"Tap closed for dispenser {dispenserId} at {usage.EndTime}");
            return true;
        }

        public IEnumerable<DispenserInfo> GetAllDispensersUsageInfo()
        {
            var allDispensers = _dispenserDAL.GetAllDispensers();

            foreach (var dispenser in allDispensers)
            {
                var dispenserInfo = new DispenserInfo { Id = dispenser.Id };
                var dispenserUsages = _dispenserDAL.GetDispenserUsages(dispenser.Id);

                dispenserInfo.dispenserUsageInfos = dispenserUsages.Select(usage =>
                {
                    TimeSpan usageDuration = usage.EndTime.HasValue
                        ? usage.EndTime.Value - usage.StartTime
                        : DateTime.Now - usage.StartTime;

                    double totalAmount = usageDuration.TotalSeconds * dispenser.FlowVolume;
                    dispenserInfo.NumberOfLitres += totalAmount;
                    dispenserInfo.TimeOfUse += usageDuration;
                    dispenserInfo.NumberOfUses++;

                    _logger.LogInformation($"Dispenser {dispenser.Id}, Usage {usage.Id} - Duration: {usageDuration}, Amount: {totalAmount}");

                    return new DispenserUsageInfo
                    {
                        UsageId = usage.Id,
                        UsageDuration = usageDuration,
                        TotalAmount = totalAmount
                    };
                }).ToList();

                yield return dispenserInfo;
            }
        }
    }
}
