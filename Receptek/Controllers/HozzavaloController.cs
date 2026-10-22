using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReceptekApi.Models;

namespace ReceptekApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HozzavaloController : ControllerBase
    {
        private readonly string _connectionString;

        public HozzavaloController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? "Server=localhost;Database=receptek;User=root;Password=;";
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = new List<Hozzavalo>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "SELECT * FROM hozzavalo";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Hozzavalo
                {
                    Id = reader.GetInt32("Id"),
                    IngredientName = reader.GetString("IngredientName"),
                    Quantity = reader.GetString("Quantity"),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                    AddedTime = reader.GetDateTime("AddedTime"),
                    UpdateTime = reader.GetDateTime("UpdateTime"),
                    SzakacsId = reader.GetInt32("SzakacsId")
                });
            }

            return Ok(list);
        }
}