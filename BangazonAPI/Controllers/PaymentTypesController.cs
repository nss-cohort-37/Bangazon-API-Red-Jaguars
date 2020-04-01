using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonAPI.Models;
using System.Data;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypesController : ControllerBase
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PaymentTypeController : Controller
        {
            private readonly IConfiguration _config;

            public PaymentTypeController(IConfiguration config)
            {
                _config = config;
            }

            public SqlConnection Connection
            {
                get
                {
                    return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                }
            }

            [HttpGet]
            public async Task<IActionResult> Get()
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT p.Id, p.Name, p.Active
                    FROM PaymentTypes p";

                        SqlDataReader reader = cmd.ExecuteReader();
                        List<PaymentTypes> paymentTypes = new List<PaymentTypes>();

                        while (reader.Read())
                        {
                            PaymentTypes paymentType = new PaymentTypes
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Active = reader.GetString(reader.GetOrdinal("Active")),
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
                            {
                                paymentType.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                            }
                            paymentTypes.Add(paymentType);
                        }
                        reader.Close();

                        return Ok(paymentTypes);
                    }
                }
            }

            [HttpGet("{id}", Name = "GetPaymentTypess")]
            public async Task<IActionResult> Get([FromRoute] int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT 
                            d.Id, d.Name, d.Breed, d.OwnerId, d.Notes, o.Name AS OwnerName, o.NeighborhoodId, o.Address, o.Phone, d.Notes 
                        FROM PaymentTypes d
                        LEFT JOIN Owner o ON d.OwnerId = o.Id
                        WHERE d.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        PaymentTypes paymentType = null;

                        if (reader.Read())
                        {
                            paymentType = new PaymentTypes
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Owner = new Owner
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                    Phone = reader.GetString(reader.GetOrdinal("Phone"))
                                }
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
                            {
                                paymentType.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                            }
                        }
                        reader.Close();

                        return Ok(paymentType);
                    }
                }
            }

            [HttpPost]
            public async Task<IActionResult> Post([FromBody] PaymentTypes paymentType)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO PaymentTypes (Name, Breed, OwnerId, Notes)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @breed, @ownerId, @notes)";
                        cmd.Parameters.Add(new SqlParameter("@name", paymentType.Name));
                        cmd.Parameters.Add(new SqlParameter("@breed", paymentType.Breed));
                        cmd.Parameters.Add(new SqlParameter("@ownerId", paymentType.OwnerId));
                        cmd.Parameters.Add(new SqlParameter("@notes", (object)paymentType.Notes ?? DBNull.Value));


                        int newId = (int)cmd.ExecuteScalar();
                        paymentType.Id = newId;
                        return CreatedAtRoute("GetPaymentTypess", new { id = newId }, paymentType);
                    }
                }
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PaymentTypes paymentType)
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE PaymentTypes
                                            SET Name = @name,
                                                Breed = @breed,
                                                OwnerId = @ownerId,
                                                Notes = @notes
                                            WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@name", paymentType.Name));
                            cmd.Parameters.Add(new SqlParameter("@breed", paymentType.Breed));
                            cmd.Parameters.Add(new SqlParameter("@ownerId", paymentType.OwnerId));
                            cmd.Parameters.Add(new SqlParameter("@notes", (object)paymentType.Notes ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return new StatusCodeResult(StatusCodes.Status204NoContent);
                            }
                            throw new Exception("No rows affected");
                        }
                    }
                }
                catch (Exception)
                {
                    if (!PaymentTypesExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete([FromRoute] int id)
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"DELETE FROM PaymentTypes WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return new StatusCodeResult(StatusCodes.Status204NoContent);
                            }
                            throw new Exception("No rows affected");
                        }
                    }
                }
                catch (Exception)
                {
                    if (!PaymentTypesExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }


            private bool PaymentTypesExists(int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, Name, Breed, OwnerId, Notes
                        FROM PaymentTypes
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();
                        return reader.Read();
                    }
                }
            }
        }
}