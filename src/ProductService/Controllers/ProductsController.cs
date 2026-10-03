using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using StackExchange.Redis;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
  private readonly ProductDbContext _context;
  private readonly IDatabase _redis;
    public ProductsController(ProductDbContext context, IConnectionMultiplexer redis)
    {
        _context = context;
        _redis = redis.GetDatabase();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var Products = await _context.Products.ToListAsync();

        return Ok(Products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = $"product:{id}";
        //check if the product is in the cache
        var cachedProduct = await _redis.StringGetAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
                    Console.WriteLine("CACHE HIT");

            return Content(cachedProduct!, "application/json");
        }   
                Console.WriteLine("CACHE MISS");

        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return NotFound();

        //store the product in cache 
        var json = JsonSerializer.Serialize(product);

        await _redis.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5));

        return Ok(product);
    }
}