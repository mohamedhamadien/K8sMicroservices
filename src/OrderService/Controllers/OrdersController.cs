using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly HttpClient _httpClient;

    public OrdersController(OrderDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders.ToListAsync();

        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("product/{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var productServiceUrl = Environment.GetEnvironmentVariable( "PRODUCT_SERVICE_URL");
        
        var response = await _httpClient.GetAsync(
            $"{productServiceUrl}/api/products/{id}");

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var product = await response.Content.ReadAsStringAsync();

        return Content(product, "application/json");
    }
}