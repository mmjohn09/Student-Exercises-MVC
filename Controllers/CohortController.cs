using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;

namespace StudentExercisesMVC.Controllers
{
    public class CohortController : Controller
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
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
        // GET: Cohorts
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id,
                                        c.Name
                                        FROM Cohorts c";
                                        

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("Name")),
                        };

                        cohorts.Add(cohort);
                    }

                    reader.Close();

                    return View(cohorts);
                }
            }
        }

        // GET: Cohorts/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id,
                                        c.Name
                                        FROM Cohorts c
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;
                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }
                    reader.Close();

                    return View(cohort);
                }
            }
        }

        // GET: Cohorts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cohorts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cohort cohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cohorts VALUES (@Name)";

                    cmd.Parameters.Add(new SqlParameter("@Name", cohort.CohortName));
        
                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // GET: Cohorts/Edit/5
        public ActionResult Edit(int id)
        {
            var cohort = GetCohortById(id);
            return View(cohort);
        }

        // POST: Cohorts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cohort cohort)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Cohorts
                                            SET Name = @Name
                                            WHERE Id = @Id";

                        cmd.Parameters.Add(new SqlParameter("@Name", cohort.CohortName));
                        cmd.Parameters.Add(new SqlParameter("@Id", cohort.Id));

                        SqlDataReader reader = cmd.ExecuteReader();

                        Cohort aCohort = null;

                        if (reader.Read())
                        {
                            aCohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortName = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }

                        reader.Close();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Cohorts/Delete/5
        public ActionResult Delete(int id)
        {
            var cohort = GetCohortById(id);
            return View(cohort);
        }

        // POST: Cohorts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Cohorts WHERE Id = @id;";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return RedirectToAction(nameof(Index));
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        //private List<Cohort> GetAllCohorts()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = "SELECT Id, Name FROM Cohorts";
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            List<Cohort> cohorts = new List<Cohort>();
        //            while (reader.Read())
        //            {
        //                cohorts.Add(new Cohort
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    CohortName = reader.GetString(reader.GetOrdinal("Name")),
        //                });
        //            }

        //            reader.Close();

        //            return cohorts;
        //        }
        //    }
        //}

        private Cohort GetCohortById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT Id, 
                                         Name
                                         FROM Cohorts
                                         WHERE id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort newCohort = null;

                    if (reader.Read())
                    {
                        newCohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }

                    reader.Close();
                    return newCohort;
                }
            }
        }
    }
}