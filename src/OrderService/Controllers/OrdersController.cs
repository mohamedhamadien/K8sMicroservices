using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
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
}