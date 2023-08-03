namespace beer_tap_dispenser.Model
{
    public class DispenserUsageInfo
    {
        public int UsageId { get; set; }
        public TimeSpan UsageDuration { get; set; }
        public double TotalAmount { get; set; }
    }
}
