using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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
                    cmd.CommandText = "SELECT Id, Name, Budget FROM Department";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Department> departments = new List<Department>();

                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        };
                        
                        departments.Add(department);
                    }
                    reader.Close();

                    return Ok(departments);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Gets department specified by id.</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            Id, Name, Budget 
                        FROM Department 
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Department department = null;

                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                        };
                        
                    }
                    reader.Close();

                    return Ok(department);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Department (Name, Budget)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name, @Budget)";
                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                    int newId = (int)cmd.ExecuteScalar();
                    department.Id = newId;
                    return CreatedAtRoute("GetDeparment", new { id = newId }, department);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Updates department specified by id.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Department
                                            SET Name = @Name,
                                                Budget = @Budget
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@Budget", department.Budget));
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
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool DepartmentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, Budget
                        FROM Department
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}