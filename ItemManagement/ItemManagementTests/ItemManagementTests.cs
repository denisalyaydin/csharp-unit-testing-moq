using NUnit.Framework;
using Moq;
using ItemManagementApp.Services;
using ItemManagementLib.Repositories;
using ItemManagementLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace ItemManagement.Tests
{
    [TestFixture]
    public class ItemServiceTests
    {
        private Mock<IItemRepository> _mockRepository;
        private ItemService _itemService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IItemRepository>();
            _itemService = new ItemService(_mockRepository.Object);
        }

        [Test]
        public void AddItem_ShouldCallAddItemOnRepository()
        {
            _itemService.AddItem("Pen");

            _mockRepository.Verify(repo => repo.AddItem(It.Is<Item>(item => item.Name == "Pen")), Times.Once);

        }

        [Test]
        public void GetAllItems_ShouldReturnAllItems()
        {
            var expectedItems = new List<Item>
            {
                new Item { Id = 1, Name = "Pen" },
                new Item { Id = 2, Name = "Pencil" },
                new Item { Id = 3, Name = "Eraser" }
            };
            _mockRepository.Setup(repo => repo.GetAllItems()).Returns(expectedItems);
            var result = _itemService.GetAllItems();

            Assert.That(result, Is.SameAs(expectedItems));
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetItemById_ShouldReturnItem()
        {
            var expectedItem = new Item { Id = 1, Name = "Pen" };
            _mockRepository.Setup(repo => repo.GetItemById(1)).Returns(expectedItem);
            var result = _itemService.GetItemById(1);
            Assert.That(result, Is.SameAs(expectedItem));
        }


        [Test]
        public void UpdateItem_ShouldCallUpdateItemOnRepository()
        {
            var itemToUpdate = new Item { Id = 1, Name = "Old" };
            _mockRepository.Setup(repo => repo.GetItemById(1)).Returns(itemToUpdate);
            _itemService.UpdateItem(1, "New");
            Assert.That(itemToUpdate.Name, Is.EqualTo("New"));
        }

    }
}