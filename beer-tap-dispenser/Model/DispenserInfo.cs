namespace beer_tap_dispenser.Model
{
    public class DispenserInfo
    {
        public int Id { get; set; }
        public int NumberOfUses { get; set; }
        public TimeSpan TimeOfUse { get; set; }
        public double NumberOfLitres { get; set; }
        public IEnumerable<DispenserUsageInfo> dispenserUsageInfos { get; set; } = Enumerable.Empty<DispenserUsageInfo>();

    }
}
