using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ITripsService _tripsService;
    private readonly IClientsService _clientsService;

    public ClientsController(ITripsService tripsService, IClientsService clientsService)
    {
        _tripsService = tripsService;
        _clientsService = clientsService;
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

    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] ClientCreateDTO client)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!(Validation.ValidateEmail(client.Email) &&
              Validation.ValidateTelephone(client.Telephone) &&
              Validation.ValidatePesel(client.Pesel)))
            return BadRequest();
        
        int newId = await _clientsService.CreateClient(client);
        return Ok(newId);
    }

    [HttpPost("{clientId}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClient(int clientId, int tripId)
    {
        if (!await ClientsService.ClientExists(clientId))
            return NotFound("No such client");
        if (!await TripsService.TripExists(tripId))
            return NotFound("No such trip");

        if (await _tripsService.RegisterClient(clientId, tripId) == false)
            return BadRequest();
        
        return Ok();
    }
}