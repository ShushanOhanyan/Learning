using Microsoft.AspNetCore.Mvc;

namespace WebLearning.Controllers;

[Route("Learning/[controller]")]
[ApiController]
public class DataController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> GetAll()
    {
        return await Task.FromResult("this is working controller");
    }
}