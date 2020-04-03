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
    public class ProductController : ControllerBase
    { 
 
            private readonly IConfiguration _config;

            public ProductController(IConfiguration config)
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
            public async Task<IActionResult> Get([FromQuery] string q, [FromQuery] string sortBy, [FromQuery] string asc)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, ProductTypeId, Price, Title, Description, CustomerId, DateAdded
                        FROM Product WHERE 1=1";
                    if (q != null)
                    {
                        cmd.CommandText += " AND Title LIKE @Title OR Description LIKE @Description";
                        cmd.Parameters.Add(new SqlParameter("@Title", "%" + q + "%"));
                        cmd.Parameters.Add(new SqlParameter("@Description", "%" + q + "%"));
                    }
                    
                        if (sortBy == "recent")
                        {
                            cmd.CommandText += " ORDER BY DateAdded DESC";

                        }
                         if (sortBy == "price")
                        {   if (asc == "true")
                        {
                            cmd.CommandText += " ORDER BY Price ASC";
                        }
                         if (asc=="false")
                        {
                            cmd.CommandText += " ORDER BY PRICE DESC";
                        }
                        }

                        

                    

                    SqlDataReader reader = cmd.ExecuteReader();
                        List<Product> products = new List<Product>();
                    

                        while (reader.Read())
                        {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))


                            };

                            products.Add(product);
                        }
                        reader.Close();

                        return Ok(products);
                    }
                }
            }
            [HttpGet("{id}", Name = "GetProduct")]
            public async Task<IActionResult> Get([FromRoute] int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, ProductTypeId, Price, Title, Description, CustomerId, DateAdded
                        FROM Product
                        WHERE Id =@id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Product product = null;

                        if (reader.Read())
                        {
                            product = new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))


                            }; 
                        }
                        reader.Close();

                        return Ok(product);
                    }
                }
            }
            [HttpPost]
            public async Task<IActionResult> Post([FromBody] Product product)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Product (Price, Title, Description, ProductTypeId, CustomerId, DateAdded)
                                        OUTPUT INSERTED.Id
                                        VALUES (@price, @title, @description, @customerId, @productTypeId, @dateAdded)";
                    DateTime dateTimeVariable = DateTime.Now;
                        cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                        cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                        cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                        cmd.Parameters.Add(new SqlParameter("@dateAdded", dateTimeVariable));
                        cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));



                    int newId = (int)cmd.ExecuteScalar();
                        product.Id = newId;
                    product.DateAdded = dateTimeVariable;
                        return CreatedAtRoute("GetProduct", new { id = newId }, product);
                    }
                }
            }
            [HttpPut("{id}")]
            public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product product)
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE Product
                                            SET Title = @title,
                                                Description = @description,
                                                Price = @price,
                                                DateAdded=@dateAdded,
                                                CustomerId=@customerId,
                                                ProductTypeId=@productTypeId
                                               
                                            WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                            cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                            cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                            cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
                            cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                            cmd.Parameters.Add(new SqlParameter("@dateAdded", product.DateAdded));




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
                    if (!ProductExists(id))
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
                            cmd.CommandText = @"DELETE FROM Product WHERE Id = @id";
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
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            private bool ProductExists(int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, Price, Title, Description, DateAdded, CustomerId, ProductTypeId
                        FROM Product
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();
                        return reader.Read();
                    }
                }
            }
        }
    }
