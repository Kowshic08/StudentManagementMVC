using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentManagementMVC.Models;
using System.Data;

namespace StudentManagementMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly IConfiguration _configuration;

        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            string connectionString =
                _configuration.GetConnectionString("StudentDB");

            List<Student> students = new List<Student>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Students";

                SqlDataAdapter da = new SqlDataAdapter(query, con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    students.Add(new Student
                    {
                        StudentId = Convert.ToInt32(row["StudentId"]),
                        Name = row["Name"].ToString(),
                        Age = Convert.ToInt32(row["Age"]),
                        Class = row["Class"].ToString(),
                        Email = row["Email"].ToString()
                    });
                }
            }

            return View(students);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(student);
            }

            string connectionString =
                _configuration.GetConnectionString("StudentDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Students
                                (Name, Age, Class, Email)
                                VALUES
                                (@Name, @Age, @Class, @Email)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@Age", student.Age);
                cmd.Parameters.AddWithValue("@Class", student.Class);
                cmd.Parameters.AddWithValue("@Email", student.Email);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            Student student = new Student();

            string connectionString =
                _configuration.GetConnectionString("StudentDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query =
                    "SELECT * FROM Students WHERE StudentId = @StudentId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentId", id);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    student.StudentId =
                        Convert.ToInt32(reader["StudentId"]);

                    student.Name =
                        reader["Name"].ToString();

                    student.Age =
                        Convert.ToInt32(reader["Age"]);

                    student.Class =
                        reader["Class"].ToString();

                    student.Email =
                        reader["Email"].ToString();
                }
            }

            return View(student);
        }

        [HttpPost]
        public IActionResult Edit(Student student)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(student);
            }

            string connectionString =
                _configuration.GetConnectionString("StudentDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Students
                                SET Name = @Name,
                                    Age = @Age,
                                    Class = @Class,
                                    Email = @Email
                                WHERE StudentId = @StudentId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@StudentId", student.StudentId);

                cmd.Parameters.AddWithValue(
                    "@Name", student.Name);

                cmd.Parameters.AddWithValue(
                    "@Age", student.Age);

                cmd.Parameters.AddWithValue(
                    "@Class", student.Class);

                cmd.Parameters.AddWithValue(
                    "@Email", student.Email);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            string connectionString =
                _configuration.GetConnectionString("StudentDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query =
                    "DELETE FROM Students WHERE StudentId = @StudentId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentId", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}