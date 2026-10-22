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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "SELECT * FROM hozzavalo WHERE Id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var item = new Hozzavalo
                {
                    Id = reader.GetInt32("Id"),
                    IngredientName = reader.GetString("IngredientName"),
                    Quantity = reader.GetString("Quantity"),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                    AddedTime = reader.GetDateTime("AddedTime"),
                    UpdateTime = reader.GetDateTime("UpdateTime"),
                    SzakacsId = reader.GetInt32("SzakacsId")
                };
                return Ok(item);
            }

            return NotFound(new { message = "Hozzávaló nem található." });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HozzavaloDto dto)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = @"INSERT INTO hozzavalo (IngredientName, Quantity, Description, AddedTime, UpdateTime, SzakacsId) 
                           VALUES (@name, @qty, @desc, NOW(), NOW(), @szakacsId);
                           SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", dto.IngredientName);
            cmd.Parameters.AddWithValue("@qty", dto.Quantity);
            cmd.Parameters.AddWithValue("@desc", (object?)dto.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@szakacsId", dto.SzakacsId);

            var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Sikeresen hozzáadva." });
        }
    }
}