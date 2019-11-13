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
    public class StudentsController : Controller
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
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
        // GET: Students
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id,
                                        s.FirstName,
                                        s.LastName,
                                        s.SlackHandle,
                                        s.CohortId,
                                        c.Id,
                                        c.Name
                                        FROM Students s
                                        LEFT JOIN Cohorts c 
                                        ON s.CohortId = c.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Student> students = new List<Student>();
                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortName = reader.GetString(reader.GetOrdinal("Name"))
                            }

                        };

                        students.Add(student);
                    }

                    reader.Close();

                    return View(students);
                }
            }
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id,
                                        s.FirstName,
                                        s.LastName,
                                        s.SlackHandle,
                                        s.CohortId
                                        FROM Students s
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;
                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

                        };
                    }
                    reader.Close();

                    return View(student);
                }
            }
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            var viewModel = new StudentCreateViewModel();
            var cohorts = GetAllCohorts();
            var selectItems = cohorts
                .Select(cohort => new SelectListItem
                {
                    Text = cohort.CohortName,
                    Value = cohort.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose cohort...",
                Value = "0"
            });
            viewModel.Cohorts = selectItems;
            return View(viewModel);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentCreateViewModel model)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Students
                ( FirstName, LastName, SlackHandle, CohortId )
                VALUES
                ( @firstName, @lastName, @slackHandle, @cohortId )";
                    cmd.Parameters.Add(new SqlParameter("@firstName", model.Student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", model.Student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", model.Student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", model.Student.CohortId));
                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            var student = GetStudentById(id);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Students 
                                            SET FirstName = @FirstName,
                                                LastName = @LastName,
                                                SlackHandle = @SlackHandle,
                                                CohortId = @CohortId
                                            WHERE Id = @Id";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@SlackHandle", student.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@Id", student.Id));

                        SqlDataReader reader = cmd.ExecuteReader();

                        Student aStudent = null;

                        if (reader.Read())
                        {
                            aStudent = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
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

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            var student = GetStudentById(id);
            return View(student);
        }

        // POST: Students/Delete/5
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
                        cmd.CommandText = "DELETE FROM StudentExercises WHERE StudentId = @id;" +
                                          "DELETE FROM Students WHERE Id = @id";

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

        private List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cohorts";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        cohorts.Add(new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }

                    reader.Close();

                    return cohorts;
                }
            }
        }

        private Student GetStudentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT s.Id, 
                                         s.FirstName, 
                                         s.LastName, 
                                         s.SlackHandle, 
                                         s.CohortId,
                                         c.Id, c.Name
                                         FROM Students s
                                         LEFT JOIN Cohorts c ON s.CohortId = c.Id
                                         WHERE s.id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Student newStudent = null;

                    if (reader.Read())
                    {
                        newStudent = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortName = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        };
                    }

                    reader.Close();
                    return newStudent;
                }
            }
        }
    }
}