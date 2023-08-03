namespace beer_tap_dispenser.Model
{
    public class DispenserUsage
    {
        public int Id { get; set; }
        public int DispenserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
