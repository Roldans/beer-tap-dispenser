using beer_tap_dispenser.DAL;
using beer_tap_dispenser.Model;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace beer_tap_dispenser.Tests
{
    public class InMemoryDispenserDALTests
    {
        private Mock<ILogger<InMemoryDispenserDAL>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<InMemoryDispenserDAL>>();
        }

        [Test]
        public void CreateDispenser_ValidDispenser_ReturnsCreatedDispenserWithId()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);
            var dispenser = new Dispenser { FlowVolume = 0.5 };

            // Act
            var result = dispenserDAL.CreateDispenser(dispenser);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Id, Is.Not.EqualTo(0)); // The Id should be assigned a non-zero value.
            Assert.AreEqual(dispenser.FlowVolume, result.FlowVolume);
        }

        [Test]
        public void CreateDispenser_NullDispenser_ThrowsArgumentNullException()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => dispenserDAL.CreateDispenser(null));
        }

        [Test]
        public void GetAllDispensers_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);

            // Act
            var result = dispenserDAL.GetAllDispensers();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetAllDispensers_ReturnsAllDispensers()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);
            dispenserDAL.CreateDispenser(new Dispenser { FlowVolume = 0.5 });
            dispenserDAL.CreateDispenser(new Dispenser { FlowVolume = 0.8 });

            // Act
            var result = dispenserDAL.GetAllDispensers();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void StartDispenserUsage_ValidUsage_AddsUsageToDispenserUsages()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);
            var dispenser = dispenserDAL.CreateDispenser(new Dispenser { FlowVolume = 0.5 });
            var usage = new DispenserUsage { DispenserId = dispenser.Id, StartTime = DateTime.Now };

            // Act
            dispenserDAL.StartDispenserUsage(usage);

            // Assert
            var allUsages = dispenserDAL.GetDispenserUsages(dispenser.Id);
            Assert.AreEqual(1, allUsages.Count());
        }

        [Test]
        public void StartDispenserUsage_NullUsage_ThrowsArgumentNullException()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => dispenserDAL.StartDispenserUsage(null));
        }

        [Test]
        public void EndDispenserUsage_ExistingUsage_UpdatesUsageEndTime()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);
            var dispenser = dispenserDAL.CreateDispenser(new Dispenser { FlowVolume = 0.5 });
            var usage = new DispenserUsage { DispenserId = dispenser.Id, StartTime = DateTime.Now };
            dispenserDAL.StartDispenserUsage(usage);

            // Act
            dispenserDAL.EndDispenserUsage(usage.Id);

            // Assert
            var endedUsage = dispenserDAL.GetDispenserUsages(dispenser.Id).FirstOrDefault(u => u.Id == usage.Id);
            Assert.NotNull(endedUsage);
            Assert.NotNull(endedUsage.EndTime);
        }

        [Test]
        public void EndDispenserUsage_NonExistingUsage_DoesNotThrowAndLogsWarning()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);

            // Act
            dispenserDAL.EndDispenserUsage(999);

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Test]
        public void GetOpenUsageByDispenserID_WithOpenUsage_ReturnsOpenUsage()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);
            var dispenser = dispenserDAL.CreateDispenser(new Dispenser { FlowVolume = 0.5 });
            var usage = new DispenserUsage { DispenserId = dispenser.Id, StartTime = DateTime.Now };
            dispenserDAL.StartDispenserUsage(usage);

            // Act
            var result = dispenserDAL.GetOpenUsageByDispenserID(dispenser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(usage.Id, result.Id);
        }

        [Test]
        public void GetOpenUsageByDispenserID_WithoutOpenUsage_ReturnsNull()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);
            var dispenser = dispenserDAL.CreateDispenser(new Dispenser { FlowVolume = 0.5 });

            // Act
            var result = dispenserDAL.GetOpenUsageByDispenserID(dispenser.Id);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public void GetDispenserUsages_ReturnsUsagesForDispenser()
        {
            // Arrange
            var dispenserDAL = new InMemoryDispenserDAL(_mockLogger.Object);
            var dispenser = dispenserDAL.CreateDispenser(new Dispenser { FlowVolume = 0.5 });
            var usage1 = new DispenserUsage { DispenserId = dispenser.Id, StartTime = DateTime.Now };
            var usage2 = new DispenserUsage { DispenserId = dispenser.Id, StartTime = DateTime.Now };
            dispenserDAL.StartDispenserUsage(usage1);
            dispenserDAL.StartDispenserUsage(usage2);

            // Act
            var result = dispenserDAL.GetDispenserUsages(dispenser.Id);

            // Assert
            Assert.AreEqual(2, result.Count());
        }
    }
}
