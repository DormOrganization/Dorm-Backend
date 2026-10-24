using System.Data;
using DormAPI.DTOs;
using DormAPI.Database.EF.Contexts;
using DormAPI.Database.EF.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Text.Json;


namespace DormAPI.Database.Ado.Repositories
{
    public class ChatBotRepository
    {

        private readonly DormContext _context;
        private readonly string _connectionString;
        public ChatBotRepository (IConfiguration configuration, DormContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        public async Task<string> ExecuteSql(string query)
        {
            using(SqlConnection  con = new SqlConnection(_connectionString))
            {
                using(SqlCommand com = new SqlCommand(query, con))
                {
                    var rows = new List<Dictionary<string, object?>>();

                    try
                    {
                        await con.OpenAsync();

                        using (SqlDataReader reader = await com.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object?>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] =
                                        await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                }

                                rows.Add(row);

                            }

                        }

                    }catch( Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    return JsonSerializer.Serialize(rows);



                }
            }
        }

        // ===========================
        //  Room and Room Allocation 
        // ==========================

        public async Task<List<dynamic>> GetAvailabeRooms()
        {
            var AvailabeRooms = await _context.Rooms
                .AsNoTracking()
                .Select(r => new {
                    r.Floor.Building.BuildingName,
                    r.Floor,
                    r.RoomNumber,
                    r.Capacity,
                    r.AvailableBeds,
                    
                   
                })
                .ToListAsync();

            return AvailabeRooms.Cast<dynamic>().ToList();
        }
    }
}
