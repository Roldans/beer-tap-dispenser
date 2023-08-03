using beer_tap_dispenser.Controllers;
using beer_tap_dispenser.Model;
using beer_tap_dispenser.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace beer_tap_dispenser.Tests
{
    public class DispenserControllerTests
    {
        private Mock<IDispenserService> _mockDispenserService;
        private Mock<ILogger<DispenserController>> _mockLogger;

        private DispenserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDispenserService = new Mock<IDispenserService>();
            _mockLogger = new Mock<ILogger<DispenserController>>();
            _controller = new DispenserController(_mockDispenserService.Object, _mockLogger.Object);
        }

        [Test]
        public void GetAllDispensersUsageInfo_ReturnsOkResultWithDispenserInfoList()
        {
            // Arrange
            var dispenserInfoList = new List<DispenserInfo>
            {
                new DispenserInfo { Id = 1, NumberOfUses = 5, NumberOfLitres = 10, TimeOfUse = TimeSpan.FromMinutes(15) },
                new DispenserInfo { Id = 2, NumberOfUses = 3, NumberOfLitres = 5, TimeOfUse = TimeSpan.FromMinutes(8) }
            };

            _mockDispenserService.Setup(service => service.GetAllDispensersUsageInfo()).Returns(dispenserInfoList);

            // Act
            var result = _controller.GetAllDispensersUsageInfo();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(dispenserInfoList, okResult.Value);
        }

        [Test]
        public void CreateDispenser_ValidFlowVolume_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var flowVolume = 0.5;
            var createdDispenser = new Dispenser { Id = 1, FlowVolume = flowVolume };

            _mockDispenserService.Setup(service => service.CreateDispenser(flowVolume)).Returns(createdDispenser);

            // Act
            var result = _controller.CreateDispenser(flowVolume);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdAtActionResult = (CreatedAtActionResult)result;
            Assert.AreEqual(nameof(_controller.CreateDispenser), createdAtActionResult.ActionName);
            Assert.AreEqual(createdDispenser, createdAtActionResult.Value);
        }

        [Test]
        public void CreateDispenser_NegativeFlowVolume_ReturnsBadRequestResult()
        {
            // Arrange
            var flowVolume = -0.5;

            // Act
            var result = _controller.CreateDispenser(flowVolume);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.AreEqual("Flow volume must be a positive value.", badRequestResult.Value);
        }

        [Test]
        public void OpenDispenserTap_ValidDispenserId_ReturnsOkResult()
        {
            // Arrange
            var dispenserId = 1;
            _mockDispenserService.Setup(service => service.OpenDispenserTap(dispenserId)).Returns(true);

            // Act
            var result = _controller.OpenDispenserTap(dispenserId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void OpenDispenserTap_InvalidDispenserId_ReturnsNotFoundResult()
        {
            // Arrange
            var dispenserId = 1;
            _mockDispenserService.Setup(service => service.OpenDispenserTap(dispenserId)).Returns(false);

            // Act
            var result = _controller.OpenDispenserTap(dispenserId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void CloseDispenserTap_ValidDispenserId_ReturnsOkResult()
        {
            // Arrange
            var dispenserId = 1;
            _mockDispenserService.Setup(service => service.CloseDispenserTap(dispenserId)).Returns(true);

            // Act
            var result = _controller.CloseDispenserTap(dispenserId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void CloseDispenserTap_InvalidDispenserId_ReturnsNotFoundResult()
        {
            // Arrange
            var dispenserId = 1;
            _mockDispenserService.Setup(service => service.CloseDispenserTap(dispenserId)).Returns(null);

            // Act
            var result = _controller.CloseDispenserTap(dispenserId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
