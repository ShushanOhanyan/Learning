using System.Web;
using AuthenticationService;
using AuthenticationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebLearning.Controllers;

[Route("Learning/[controller]")]
[ApiController]
public class DataController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ICalendarService _calendarService;
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> GetUrl()
    {
        string redirectUrl = "https://calendar.google.com/calendar/u/2/r?pli=1";
        
        string queryString = _authenticationService.GetAuthUrl("", redirectUrl);

        return Ok(queryString);
    }
    
    [HttpGet("events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<CalendarEvent>>> GetEvents(string code)
    {
        string redirectUrl = "https://calendar.google.com/calendar/u/2/r?pli=1";

        var details = await _authenticationService.GetAuthDetailsAsync(redirectUrl, code);

        var refreshToken = details.RefreshToken;
        var email = details.Email;

        var events = _calendarService.GetAllEventsAsync(refreshToken, email, 100);

        return Ok(events);
    }

    public DataController(IAuthenticationService authenticationService, ICalendarService calendarService)
    {
        _authenticationService = authenticationService;
        _calendarService = calendarService;
    }
}