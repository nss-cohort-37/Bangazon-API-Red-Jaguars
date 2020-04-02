using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using BangazonAPI.Models;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
            private readonly IConfiguration _config;

            public ProductTypeController(IConfiguration config)
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
                        cmd.CommandText = @"SELECT Id, Name
                        FROM ProductType";
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<ProductType> productTypes = new List<ProductType>();

                        while (reader.Read())
                        {
                            ProductType productType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                               


                            };

                            productTypes.Add(productType);
                        }
                        reader.Close();

                        return Ok(productTypes);
                    }
                }
            }
            [HttpGet("{id}", Name = "GetProductType")]
            public async Task<IActionResult> Get([FromRoute] int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, Name
                        FROM ProductType
                        WHERE Id=@id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        ProductType productType = null;

                        if (reader.Read())
                        {
                            productType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),


                            };
                        }
                        reader.Close();

                        return Ok(productType);
                    }
                }
            }
            [HttpPost]
            public async Task<IActionResult> Post([FromBody] ProductType productType)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO ProductType (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                        cmd.Parameters.Add(new SqlParameter("@name", productType.Name));
                       



                        int newId = (int)cmd.ExecuteScalar();
                        productType.Id = newId;
                        return CreatedAtRoute("GetProduct", new { id = newId }, productType);
                    }
                }
            }
            [HttpPut("{id}")]
            public async Task<IActionResult> Put([FromRoute] int id, [FromBody] ProductType productType)
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE ProductType
                                                SET Name = @name
                                             
                                               
                                            WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@name", productType.Name));
                         


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
                    if (!ProductTypeExists(id))
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
                            cmd.CommandText = @"DELETE FROM ProductType WHERE Id = @id";
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
                    if (!ProductTypeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            private bool ProductTypeExists(int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, Name
                        FROM ProductType
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();
                        return reader.Read();
                    }
                }
            }
        }
    }
