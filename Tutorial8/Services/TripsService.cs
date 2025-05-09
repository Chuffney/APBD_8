using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : DBService, ITripsService
{
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT * FROM Trip";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
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
                        //todo wciąż brakuje informacji o krajach
                    });
                }
            }
        }

        return trips;
    }

    public async Task<List<ClientTripDTO>> GetTrips(int idClient)
    {
        var trips = new List<ClientTripDTO>();
        
        const string command = "SELECT * FROM Trip JOIN Client_Trip ON Trip.IdTrip = Client_Trip.IdTrip WHERE IdClient = @IdClient";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            
            if (!await ClientsService.ClientExists(conn, idClient))
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

    //wwraca true jeśli udało się zarejestrować klienta, false jeśli nie było już miejsca
    public async Task<bool> RegisterClient(int idClient, int idTrip)
    {
        const string command = "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate) VALUES (@IdClient, @IdTrip, @date, NULL)";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            if (!await CanFitMorePeople(conn, idTrip))
                return false;

            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@IdClient", idClient);
                cmd.Parameters.AddWithValue("@IdTrip", idTrip);
                cmd.Parameters.AddWithValue("@date", DateToInt(DateTime.Now));

                cmd.ExecuteNonQueryAsync();
                return true;
            }
        }
    }

    private static int DateToInt(DateTime date)
    {
        return date.Year * 10000 + date.Month * 100 + date.Day;
    }

    private async Task<bool> CanFitMorePeople(SqlConnection conn, int idTrip)
    {
        const string command = "select 0 from Trip where IdTrip = @IdTrip and (select count(1) from Client_Trip where Client_Trip.IdTrip = @IdTrip) < MaxPeople";

        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdTrip", idTrip);
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                return reader.HasRows;
            }
        }
    }

    private static async Task<bool> TripExists(SqlConnection conn, int idTrip)
    {
        const string command = "SELECT 0 FROM Trip WHERE IdTrip = @IdTrip";
        
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                return reader.HasRows;
            }
        }
    }

    public static async Task<bool> TripExists(int idTrip)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            return await TripExists(conn, idTrip);
        }
    }
}