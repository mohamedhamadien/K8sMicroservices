using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }
    private static readonly List<Order> Orders = new()
    {
        new Order
        {
            Id = 1,
            ProductId = 1,
            Quantity = 2
        }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Orders);
    }
    
    [HttpGet("product/{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var response = await _httpClient.GetAsync(
            $"http://product-service:8080/api/products/{id}");

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var product = await response.Content.ReadAsStringAsync();

        return Content(product, "application/json");
    }
}