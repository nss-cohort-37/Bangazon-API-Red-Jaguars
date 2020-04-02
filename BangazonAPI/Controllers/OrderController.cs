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
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
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
                    cmd.CommandText = @"SELECT o.Id AS OrderId, o.CustomerId, o.UserPaymentTypeId,
                        p.ProductTypeId, p.CustomerId, p.Price, p.Description, p.Title, p.DateAdded, p.Id AS ProductId
                        FROM [Order] o
                        LEFT JOIN OrderProduct op ON op.OrderId = o.Id 
                        LEFT JOIN Product p ON p.Id = op.ProductId";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Order> orders = new List<Order>();

                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            //UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"))
                            Products = new List<Product>()
                            };
                        order.Products.Add(new Product()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        });



                        
                        if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId"))) {
                            order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                        }
                        orders.Add(order);
                    }
                    reader.Close();

                    return Ok(orders);
                }
            }
        }
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT o.Id AS OrderId, o.CustomerId, o.UserPaymentTypeId,
                        p.ProductTypeId, p.CustomerId, p.Price, p.Description, p.Title, p.DateAdded, p.Id AS ProductId
                        FROM [Order] o
                        LEFT JOIN OrderProduct op ON op.OrderId = o.Id 
                        LEFT JOIN Product p ON p.Id = op.ProductId
                    WHERE OrderId = @Id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Order order = null;

                    while (reader.Read())
                    {
                        if (order == null)
                        {
                            order = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                //UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"))
                                Products = new List<Product>()
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                            {
                                order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                            }
                        }

                       
                        order.Products.Add(new Product()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        


                        });
                        
                    }
                    
                    reader.Close();

                    return Ok(order);
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO [Order] (CustomerId, UserPaymentTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@CustomerId, @UserPaymentTypeId)";
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@UserPaymentTypeId", order.UserPaymentTypeId));




                    int newId = (int)cmd.ExecuteScalar();
                    order.Id = newId;
                    return CreatedAtRoute("GetOrder", new { id = newId }, order);
                }
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE [Order]
                                                SET CustomerId = @CustomerId, UserPaymentTypeId = @UserPaymentTypeId

                                             
                                               
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@UserPaymentTypeId", order.UserPaymentTypeId));



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
                if (!OrderExists(id))
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
                        cmd.CommandText = @"DELETE FROM [Order] WHERE Id = @id";
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
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, CustomerId, UserPaymentTypeId
                        FROM [Order]
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
