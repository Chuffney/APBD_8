using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : DBService, IClientsService
{
    public async Task<int> CreateClient(ClientCreateDTO client)
    {
        string command =
            "INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) output inserted.IdClient values(@FirstName, @LastName, @Email, @Telephone, @Pesel)";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            
            cmd.Parameters.AddWithValue("FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("LastName", client.LastName);
            cmd.Parameters.AddWithValue("Email", client.Email);
            cmd.Parameters.AddWithValue("Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("Pesel", client.Pesel);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
    }
    
    public static async Task<bool> ClientExists(SqlConnection conn, int idClient)
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
    
    public static async Task<bool> ClientExists(int idClient)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            return await ClientExists(conn, idClient);
        }
    }
}