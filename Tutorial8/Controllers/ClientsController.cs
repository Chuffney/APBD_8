using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public ClientsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }
    
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTrips(int id)
    {
        try
        {
            var trips = await _tripsService.GetTrips(id);
            if (trips.Count == 0)
                return NotFound();
            return Ok(trips);
        }
        catch (ArgumentException e)
        {
            return NotFound(e.Message);
        }
    }
}