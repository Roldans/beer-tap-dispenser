using NUnit.Framework;
using Moq;
using beer_tap_dispenser.Model;
using System;
using System.Collections.Generic;
using beer_tap_dispenser.DAL;
using beer_tap_dispenser.Services;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

namespace BeerTapDispenserTest.Services
{
    [TestFixture]
    public class DispenserServiceTests
    {
        private DispenserService _dispenserService;
        private Mock<IDispenserDAL> _mockDispenserDAL;
        private Mock<ILogger<DispenserService>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockDispenserDAL = new Mock<IDispenserDAL>();
            _mockLogger = new Mock<ILogger<DispenserService>>();
            _dispenserService = new DispenserService(_mockDispenserDAL.Object, _mockLogger.Object);
        }

        [Test]
        public void CreateDispenser_ShouldCreateDispenser()
        {
            // Arrange
            double flowVolume = 0.5;
            var expectedDispenser = new Dispenser
            {
                FlowVolume = flowVolume
            };
            _mockDispenserDAL.Setup(d => d.CreateDispenser(It.IsAny<Dispenser>())).Returns(expectedDispenser);

            // Act
            var result = _dispenserService.CreateDispenser(flowVolume);

            // Assert
            _mockDispenserDAL.Verify(d => d.CreateDispenser(It.IsAny<Dispenser>()), Times.Once);
            Assert.That(expectedDispenser.Equals(result));
        }


        [TestCase(-1)]
        [TestCase(0)]
        public void CreateDispenser_WithInvalidFlowVolume_ThrowsArgumentException(double flowVolume)
        {
            // Act and Assert
            Assert.Throws<ArgumentException>(() => _dispenserService.CreateDispenser(flowVolume));
        }

        [Test]
        public void OpenDispenserTap_ShouldOpenTap()
        {
            // Arrange
            int dispenserId = 1;
            var usage = new DispenserUsage
            {
                DispenserId = dispenserId,
                StartTime = DateTime.Now,
                EndTime = null
            };
            _mockDispenserDAL.Setup(d => d.GetOpenUsageByDispenserID(dispenserId)).Returns(usage);

            // Act
            var result = _dispenserService.OpenDispenserTap(dispenserId);

            // Assert
            _mockDispenserDAL.Verify(d => d.GetOpenUsageByDispenserID(dispenserId), Times.Once);
            _mockDispenserDAL.Verify(d => d.StartDispenserUsage(usage), Times.Never);
            Assert.IsFalse(result);
        }

        [Test]
        public void CloseDispenserTap_ShouldCloseTap()
        {
            // Arrange
            int dispenserId = 1;
            var usage = new DispenserUsage
            {
                DispenserId = dispenserId,
                StartTime = DateTime.Now,
                EndTime = null
            };
            _mockDispenserDAL.Setup(d => d.GetOpenUsageByDispenserID(dispenserId)).Returns(usage);

            // Act
            var result = _dispenserService.CloseDispenserTap(dispenserId);

            // Assert
            _mockDispenserDAL.Verify(d => d.GetOpenUsageByDispenserID(dispenserId), Times.Once);
            _mockDispenserDAL.Verify(d => d.EndDispenserUsage(usage.Id), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public void CloseDispenserTap_ShouldNotCloseTap_WhenAlreadyClosed()
        {
            // Arrange
            int dispenserId = 1;
            var usage = new DispenserUsage
            {
                DispenserId = dispenserId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(10)
            };
            _mockDispenserDAL.Setup(d => d.GetOpenUsageByDispenserID(dispenserId)).Returns(usage);

            // Act
            var result = _dispenserService.CloseDispenserTap(dispenserId);

            // Assert
            _mockDispenserDAL.Verify(d => d.GetOpenUsageByDispenserID(dispenserId), Times.Once);
            _mockDispenserDAL.Verify(d => d.EndDispenserUsage(It.IsAny<int>()), Times.Never);
            Assert.IsFalse(result);
        }

        // Similar unit tests can be created for other methods as well.
    }
}