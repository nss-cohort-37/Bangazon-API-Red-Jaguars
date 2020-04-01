using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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
      
            private readonly IConfiguration _config;

            public PaymentTypesController(IConfiguration config)
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
                    FROM PaymentType p";

                        SqlDataReader reader = cmd.ExecuteReader();
                        List<PaymentTypes> paymentTypes = new List<PaymentTypes>();

                        while (reader.Read())
                        {
                            PaymentTypes paymentType = new PaymentTypes
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                            };

                            paymentTypes.Add(paymentType);
                        }
                        reader.Close();

                        return Ok(paymentTypes);
                    }
                }
            }

            [HttpGet("{id}", Name = "GetPaymentTypes")]
            public async Task<IActionResult> Get([FromRoute] int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT p.Id, p.Name, p.Active
                    FROM PaymentType p
                        WHERE p.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        PaymentTypes paymentType = null;


                        if (reader.Read())
                        {
                            paymentType = new PaymentTypes
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                            };

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
                        cmd.CommandText = @"INSERT INTO PaymentType (Name, Active)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name, @Active)";
                        cmd.Parameters.Add(new SqlParameter("@Name", paymentType.Name));
                        cmd.Parameters.Add(new SqlParameter("@Active", paymentType.Active));



                        int newId = (int)cmd.ExecuteScalar();
                        paymentType.Id = newId;
                        return CreatedAtRoute("GetPaymentTypes", new { id = newId }, paymentType);
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
                            cmd.CommandText = @"UPDATE PaymentType
                                            SET Name = @Name,
                                                Active = @Active
                                                
                                            WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@Name", paymentType.Name));
                            cmd.Parameters.Add(new SqlParameter("@Active", paymentType.Active));
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
                            cmd.CommandText = @"DELETE FROM PaymentType WHERE Id = @id";
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
                        SELECT Id, Name, Active
                        FROM PaymentType
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();
                        return reader.Read();
                    }
                }
            }
        }
    }
