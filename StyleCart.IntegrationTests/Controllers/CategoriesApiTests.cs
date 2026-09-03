using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace StyleCart.IntegrationTests.Controllers;

public class CategoriesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoriesApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}