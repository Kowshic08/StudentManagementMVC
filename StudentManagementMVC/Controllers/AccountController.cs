using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentManagementMVC.Models;

namespace StudentManagementMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            string connectionString =
                _configuration.GetConnectionString("StudentDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(*)
                                 FROM Users
                                 WHERE Username = @Username
                                 AND Password = @Password";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Username", login.Username);
                cmd.Parameters.AddWithValue("@Password", login.Password);

                con.Open();

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 1)
                {
                    HttpContext.Session.SetString("IsAdmin", "true");

                    return RedirectToAction("Index", "Student");
                }
            }

            ViewBag.Error = "Invalid Username or Password";

            return View(login);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}