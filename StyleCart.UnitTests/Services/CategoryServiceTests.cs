using Xunit;
using FluentAssertions;
using Moq;
using StyleCart.Application.DTOs.Categories;
using StyleCart.Application.Interfaces;
using StyleCart.Application.Services;
using StyleCart.Domain.Entities;

namespace StyleCart.UnitTests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesCategory()
    {
        // Arrange
        var repository = new Mock<ICategoryRepository>();

        repository
            .Setup(item => item.ExistsByNameAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        repository
            .Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var service = new CategoryService(repository.Object);

        var request = new CreateCategoryRequest
        {
            Name = "Footwear",
            Description = "Shoes and sandals"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Name.Should().Be("Footwear");

        repository.Verify(item => item.AddAsync(
            It.Is<Category>(category =>
                category.Name == "Footwear" &&
                category.Description == "Shoes and sandals"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ThrowsError()
    {
        // Arrange
        var repository = new Mock<ICategoryRepository>();

        repository
            .Setup(item => item.ExistsByNameAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new CategoryService(repository.Object);

        var request = new CreateCategoryRequest
        {
            Name = "Women"
        };

        // Act
        Func<Task> action = () => service.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("A category with this name already exists.");
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ThrowsError()
    {
        // Arrange
        var repository = new Mock<ICategoryRepository>();
        var service = new CategoryService(repository.Object);

        var request = new CreateCategoryRequest
        {
            Name = " "
        };

        // Act
        Func<Task> action = () => service.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Category name is required.");
    }
}