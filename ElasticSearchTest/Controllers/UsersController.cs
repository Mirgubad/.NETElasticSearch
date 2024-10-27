using ElasticSearchTest.Models;
using ElasticSearchTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{

    private readonly ILogger<UsersController> _logger;
    private readonly IElasticService _elasticService;

    public UsersController(IElasticService elasticService)
    {
        _elasticService = elasticService;
    }

    [HttpPost("create-index")]

    public async Task<IActionResult> CreateIndex(string indexName)
    {
        await _elasticService.CreateIndexIfNotExistsAsync(indexName);
        return Ok($"Index {indexName} created or already exists.");
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        var result = await _elasticService.AddOrUpdateUser(user);

        return result ? Ok("User added successfully")
                      : BadRequest("Failed to add user");
    }


    [HttpPost("update-user")]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        var result = await _elasticService.AddOrUpdateUser(user);

        return result ? Ok("User added or updated successfully")
                      : BadRequest("Failed to adding or updating user");
    }


    [HttpGet("get-user/{key}")]
    public async Task<IActionResult> GetUser(string key)
    {
        var user = await _elasticService.Get(key);

        return user != null ? Ok(user)
                            : NotFound("User not found");
    }

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _elasticService.GetAll();

        return users != null ? Ok(users)
                             : NotFound("No users found");
    }

    [HttpDelete("remove-user/{key}")]
    public async Task<IActionResult> RemoveUser(string key)
    {
        var result = await _elasticService.RemoveUser(key);

        return result ? Ok("User removed successfully")
                      : NotFound("User not found");
    }

}