using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<int> CreateClient(ClientCreateDTO client);
}