using beer_tap_dispenser.Model;
using MongoDB.Driver;

namespace beer_tap_dispenser.DAL
{
    public class DispenserDAL : IDispenserDAL
    {
        private readonly IMongoCollection<Dispenser> _dispenserCollection;
        private readonly IMongoCollection<DispenserUsage> _dispenserUsageCollection;

        public DispenserDAL(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _dispenserCollection = database.GetCollection<Dispenser>("dispensers");
            _dispenserUsageCollection = database.GetCollection<DispenserUsage>("dispenserUsages");
        }

        public Dispenser CreateDispenser(Dispenser dispenser)
        {
            _dispenserCollection.InsertOne(dispenser);
            return dispenser;
        }

        public IEnumerable<Dispenser> GetAllDispensers()
        {
            return _dispenserCollection.Find(Builders<Dispenser>.Filter.Empty).ToList();
        }

        public void StartDispenserUsage(DispenserUsage usage)
        {
            _dispenserUsageCollection.InsertOne(usage);
        }

        public void EndDispenserUsage(int usageID)
        {
            var filter = Builders<DispenserUsage>.Filter.Eq(u => u.Id, usageID);
            var update = Builders<DispenserUsage>.Update
                .CurrentDate(u => u.EndTime);

            _dispenserUsageCollection.UpdateOne(filter, update);
        }
        public DispenserUsage GetOpenUsageByDispenserID(int dispenserId)
        {
            var filter = Builders<DispenserUsage>.Filter.Or(
                Builders<DispenserUsage>.Filter.Eq(u => u.DispenserId, dispenserId),
                Builders<DispenserUsage>.Filter.Eq(u => u.EndTime.HasValue, false));
            return _dispenserUsageCollection.Find(filter).FirstOrDefault();
        }
        public IEnumerable<DispenserUsage> GetDispenserUsages(int dispenserId)
        {
            var filter = Builders<DispenserUsage>.Filter.Eq(u => u.DispenserId, dispenserId);
            return _dispenserUsageCollection.Find(filter).ToList();
        }

    }
}
