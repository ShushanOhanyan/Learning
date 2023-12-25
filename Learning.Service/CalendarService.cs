using AuthenticationService.Models;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace AuthenticationService;

public interface ICalendarService
{
    Task<List<CalendarEvent>> GetAllEventsAsync(string refreshToken, string email, int maxEvents);
}

public class GoogleCalendarService : ICalendarService
{
    private readonly IAuthenticationService _authenticationService;
    
    public Task<List<CalendarEvent>> GetAllEventsAsync(string refreshToken, string email, int maxEvents)
    {
        // Create the Calendar service
        var service = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = _authenticationService.GetUserCredential(refreshToken)
        });

        var events = GetEvents(service, email, 100);


        return Task.FromResult(GetEvents(service, email, maxEvents));
    }
    
    private List<CalendarEvent> GetEvents(CalendarService service, string calendarId, int maxEvents)
    {
        List<CalendarEvent> calendarEvents = new List<CalendarEvent>();

        EventsResource.ListRequest request = service.Events.List("primary");
        
        request.MaxResults = maxEvents;

        // Fetch the events
        Events events = request.Execute();

        if (events.Items != null && events.Items.Count > 0)
        {
            foreach (var eventItem in events.Items)
            {
                calendarEvents.Add(new CalendarEvent()
                {
                    Created = eventItem.CreatedRaw,
                    Summary = eventItem.Summary,
                });
            }
        }



        return calendarEvents;
    }

    public GoogleCalendarService(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
}