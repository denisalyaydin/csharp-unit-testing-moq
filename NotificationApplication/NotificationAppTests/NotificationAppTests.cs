using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using NotificationApp.Entities;
using NotificationApp.Interfaces;
using NotificationApp.Services;
using NUnit.Framework;
using System;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using User = NotificationApp.Entities.User;

namespace NotificationApp.Tests
{
    public class NotificationAppTests
    {
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<INotifier> _mockNotifier;
        private NotificationService _notificationService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockNotifier = new Mock<INotifier>();
            _notificationService = new NotificationService(_mockUserRepo.Object, _mockNotifier.Object);
        }

        [Test]
        public void NotifyUser_WithValidActiveUser_CallsSend()
        {
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                IsActive = true
            };

            _mockUserRepo.Setup(repo => repo.GetUserById(1)).Returns(user);

            _notificationService.NotifyUser(1, "Test message");

            _mockNotifier.Verify(notifier => notifier.Send("test@test.com", "Test message"), Times.Once);
        }

        [Test]
        public void NotifyUser_WithInactiveUser_ThrowsInvalidOperationException()
        {
            var user = new User
            {
                Id = 2,
                Email = "test@test.com",
                IsActive = false
            };
            _mockUserRepo.Setup(repo => repo.GetUserById(2)).Returns(user);

            var exception = Assert.Throws<InvalidOperationException>(() => _notificationService.NotifyUser(2, "Test message"));
            Assert.That(exception.Message, Is.EqualTo("Cannot notify inactive user."));
            _mockNotifier.Verify(notifier => notifier.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        [Test]
        public void NotifyUser_WithNonExistentUser_ThrowsArgumentException()
        {
            _mockUserRepo.Setup(repo => repo.GetUserById(99)).Returns((User)null);
            var exception = Assert.Throws<ArgumentException>(() => _notificationService.NotifyUser(99, "Test message"));
            Assert.That(exception.Message, Is.EqualTo("User not found."));
            _mockNotifier.Verify(notifier => notifier.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void NotifyUser_WithNullMessage_ThrowsArgumentException(string? message)
        {
            var exception = Assert.Throws<ArgumentException>(() => _notificationService.NotifyUser(1, message));
            Assert.That(exception.Message, Is.EqualTo("Message cannot be empty."));
            _mockNotifier.Verify(notifier => notifier.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
