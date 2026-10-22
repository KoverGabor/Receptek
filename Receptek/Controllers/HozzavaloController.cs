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
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] HozzavaloDto dto)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = @"UPDATE hozzavalo 
                           SET IngredientName = @name, Quantity = @qty, Description = @desc, UpdateTime = NOW(), SzakacsId = @szakacsId 
                           WHERE Id = @id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", dto.IngredientName);
            cmd.Parameters.AddWithValue("@qty", dto.Quantity);
            cmd.Parameters.AddWithValue("@desc", (object?)dto.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@szakacsId", dto.SzakacsId);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                return NotFound(new { message = "Módosítandó hozzávaló nem található." });
            }

            return Ok(new { message = "Sikeresen frissítve." });
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "DELETE FROM hozzavalo WHERE Id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                return NotFound(new { message = "A törlendő elem nem található." });
            }

            return Ok(new { message = "Sikeresen törölve." });
        }
        [HttpGet("szakacs/{szakacsId:int}")]
        public async Task<IActionResult> GetSzakacsData(int szakacsId)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "SELECT Name, Email FROM szakacs WHERE Id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", szakacsId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new
                {
                    Name = reader.GetString("Name"),
                    Email = reader.GetString("Email")
                });
            }

            return NotFound(new { message = "Szakács nem található." });
        }
        [HttpGet("szakacs/{szakacsId:int}/hozzavalok")]
        public async Task<IActionResult> GetSzakacsWithIngredients(int szakacsId)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = @"SELECT s.Name AS SzakacsName, h.IngredientName, h.Quantity 
                           FROM szakacs s 
                           LEFT JOIN hozzavalo h ON s.Id = h.SzakacsId 
                           WHERE s.Id = @id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", szakacsId);

            using var reader = await cmd.ExecuteReaderAsync();
            string? szakacsName = null;
            var ingredients = new List<object>();

            while (await reader.ReadAsync())
            {
                szakacsName ??= reader.GetString("SzakacsName");
                if (!reader.IsDBNull(reader.GetOrdinal("IngredientName")))
                {
                    ingredients.Add(new
                    {
                        IngredientName = reader.GetString("IngredientName"),
                        Quantity = reader.GetString("Quantity")
                    });
                }
            }

            if (szakacsName == null)
            {
                return NotFound(new { message = "Szakács nem található." });
            }

            return Ok(new
            {
                SzakacsName = szakacsName,
                Ingredients = ingredients
            });
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetTotalCount()
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "SELECT COUNT(*) FROM hozzavalo";
            using var cmd = new MySqlCommand(sql, conn);

            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return Ok(new { TotalCount = count });
        }
        [HttpGet("szakacs/{szakacsId:int}/count")]
        public async Task<IActionResult> GetCountBySzakacs(int szakacsId)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "SELECT COUNT(*) FROM hozzavalo WHERE SzakacsId = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", szakacsId);

            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return Ok(new { SzakacsId = szakacsId, IngredientCount = count });
        }
    }
}