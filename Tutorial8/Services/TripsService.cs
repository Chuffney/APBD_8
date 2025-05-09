using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString =
        "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT * FROM Trip";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    int nameOrdinal = reader.GetOrdinal("Name");
                    int descOrdinal = reader.GetOrdinal("Description");
                    int fromOrdinal = reader.GetOrdinal("DateFrom");
                    int toOrdinal = reader.GetOrdinal("DateTo");
                    int maxPeopleOrdinal = reader.GetOrdinal("MaxPeople");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(nameOrdinal),
                        Description = reader.GetString(descOrdinal),
                        DateFrom = reader.GetDateTime(fromOrdinal),
                        DateTo = reader.GetDateTime(toOrdinal),
                        MaxPeople = reader.GetInt32(maxPeopleOrdinal)
                        //todo brakuje informacji o krajach
                    });
                }
            }
        }

        return trips;
    }

    public async Task<List<ClientTripDTO>> GetTrips(int idClient)
    {
        var trips = new List<ClientTripDTO>();
        
        string command = "SELECT * FROM Trip JOIN Client_Trip ON Trip.IdTrip = Client_Trip.IdTrip WHERE IdClient = @IdClient";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            
            if (!await ClientExists(conn, idClient))
                throw new ArgumentException("Client does not exist");
            
            cmd.Parameters.AddWithValue("@IdClient", idClient);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    int nameOrdinal = reader.GetOrdinal("Name");
                    int descOrdinal = reader.GetOrdinal("Description");
                    int fromOrdinal = reader.GetOrdinal("DateFrom");
                    int toOrdinal = reader.GetOrdinal("DateTo");
                    int maxPeopleOrdinal = reader.GetOrdinal("MaxPeople");
                    int registeredOrdinal = reader.GetOrdinal("RegisteredAt");
                    int paymentOrdinal = reader.GetOrdinal("PaymentDate");

                    ClientTripDTO dto = new ClientTripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(nameOrdinal),
                        Description = reader.GetString(descOrdinal),
                        DateFrom = reader.GetDateTime(fromOrdinal),
                        DateTo = reader.GetDateTime(toOrdinal),
                        MaxPeople = reader.GetInt32(maxPeopleOrdinal),
                        RegisteredAt = reader.GetInt32(registeredOrdinal)
                    };

                    if (reader.IsDBNull(paymentOrdinal))
                        dto.PaymentDate = null;
                    else
                        dto.PaymentDate = reader.GetInt32(paymentOrdinal);
                    
                    trips.Add(dto);
                }
            }
        }

        return trips;
    }

    private async Task<bool> ClientExists(SqlConnection conn, int idClient)
    {
        string command = "SELECT 0 FROM Client WHERE IdClient = @IdClient";
        
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                return reader.HasRows;
            }
        }
    }
}