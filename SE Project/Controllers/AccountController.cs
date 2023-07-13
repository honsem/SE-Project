using SE_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SE_Project.Filters;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SE_Project.Controllers
{
    public class AccountController : Controller
    {

        //Connection String from the Microsoft SQL Server
        private readonly string connectionString = "Server=LAPTOP-RH67UL1G\\SQLEXPRESS;Database=AssetManagementSystem;User Id=ymsmis;Password=iamking;";

        // GET: Login Page
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login Page
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoCache]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve user record from database
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "SELECT Email, Password, UserRole, IsActive FROM UserAccount WHERE Email = @LoginEmail";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            connection.Open();
                            command.Parameters.AddWithValue("@LoginEmail", model.Email);

                            SqlDataReader reader = command.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string email = reader.GetString(0);
                                    string hashedPasswordFromDb = reader.GetString(1);
                                    string userRole = reader.GetString(2);
                                    bool isActive = reader.GetBoolean(3);

                                    // Hash the entered password using the same algorithm
                                    string hashedPasswordEntered = HashPassword(model.Password);

                                    // Compare the hashed passwords
                                    bool isPasswordCorrect = hashedPasswordEntered.Equals(hashedPasswordFromDb);

                                    if (isPasswordCorrect && isActive)
                                    {
                                        Session["Email"] = email;

                                        // Get the EmployeeID based on the logged-in email
                                        string employeeID = GetEmployeeIDByEmail(email);
                                        Session["EmployeeID"] = employeeID;

                                        // Set role in session variable or authentication cookie
                                        Session["UserRole"] = (userRole == "Admin") ? "Admin" : "User";

                                        // Redirect to appropriate page
                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                            }
                            connection.Close();
                        }

                        TempData["ErrorMessage"] = "Invalid Username or Password. Please Re-Try";
                    }
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }

            }

            return View(model);
        }

        // Get: Create Ownership Record View
        public ActionResult CreateEmployeeRecord()
        {
            return View();
        }

        // POST: Create Ownership Record
        [HttpPost]
        public ActionResult CreateEmployeeRecord(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Check if EmployeeID is already registered
                        string checkIDQuery = "SELECT COUNT(*) FROM Ownership WHERE EmployeeID = @checkID";
                        SqlCommand checkIDCmd = new SqlCommand(checkIDQuery, connection);
                        checkIDCmd.Parameters.AddWithValue("@checkID", employee.EmployeeID);


                        connection.Open();

                        int count = (int)checkIDCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            ModelState.AddModelError("EmployeeID", "This employee id is already registered.");
                            return View(employee);
                        }

                        // If the employee id is new registered and not in the record.
                        string query = "INSERT INTO Ownership (EmployeeID,FirstName,LastName,Office,Department) VALUES (@EmployeeID,@FirstName,@LastName,@Office,@Department)";

                        SqlCommand command = new SqlCommand(query, connection);

                        command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Office", employee.Office);
                        command.Parameters.AddWithValue("@Department", employee.Department);

                        command.ExecuteNonQuery();

                        // Create success message
                        TempData["SuccessMessage"] = "Employee Record Registration Successfully.";

                        connection.Close();

                        return RedirectToAction("Index", "Home");
                    }
                    
                }catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
            }
            return View();
        }
       
        //Get Employee/Ownership Record
        public ActionResult GetEmployeeRecord()
        {
            try
            {
                List<Employee> employeeList = new List<Employee>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM Ownership WHERE EmployeeID != 'MS-8888'";
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Employee employee = new Employee
                            {
                                EmployeeID = reader.GetString(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Office = reader.GetString(3),
                                Department = reader.GetString(4)
                            };

                            employeeList.Add(employee);
                        }

                    }
                    connection.Close();
                }
                return View(employeeList);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }

        }

        //View Employee/Ownership Profile
        public ActionResult ViewOwnershipRecord(string EmployeeID)
        {
            try
            {
                // Retrieve the list of employeeList from the database
                List<Employee> employeeList = GetEmployeeIDFromDatabase();

                ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");

                Employee ownership = new Employee();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM Ownership WHERE EmployeeID = @EmployeeID";

                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {

                        command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ownership.EmployeeID = reader.GetString(0);
                            ownership.FirstName = reader.GetString(1);
                            ownership.LastName = reader.GetString(2);
                            ownership.Office = reader.GetString(3);
                            ownership.Department = reader.GetString(4);
                        }
                        connection.Close();
                    }
                }

                return View(ownership);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }
        }

        //ADMIN GET: Edit Employee/Ownership Record
        public ActionResult EditOwnershipRecord(string EmployeeID)
        {
            try
            {
                // Retrieve the list of employeeList from the database
                List<Employee> employeeList = GetEmployeeIDFromDatabase();

                ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");

                Employee ownership = new Employee();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM Ownership WHERE EmployeeID = @EmployeeID";

                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {

                        command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ownership.EmployeeID = reader.GetString(0);
                            ownership.FirstName = reader.GetString(1);
                            ownership.LastName = reader.GetString(2);
                            ownership.Office = reader.GetString(3);
                            ownership.Department = reader.GetString(4);
                        }
                        connection.Close();
                    }
                }

                return View(ownership);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }
        }

        //ADMIN POST: Edit Employee/Ownership Record
        [HttpPost]
        public ActionResult EditOwnershipRecord(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string sqlQuery = "UPDATE Ownership SET FirstName = @FirstName, LastName = @LastName, Office = @Office, Department = @Department WHERE EmployeeID = @EmployeeID";

                        // Open the connectionW
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            // Set the parameter values for User Account update 
                            command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                            command.Parameters.AddWithValue("@LastName", employee.LastName);
                            command.Parameters.AddWithValue("@Office", employee.Office);
                            command.Parameters.AddWithValue("@Department", employee.Department);
                            command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);

                            // Execute the User Account update query
                            command.ExecuteNonQuery();
                        }
                        
                        connection.Close();
                    
                    }
                    // Create success message
                    TempData["SuccessMessage"] = "Employee Record Update Successfully.";

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
            }
            return View();
        }

        //GET USER ACCOUNT RECORD
        public ActionResult GetUserAccountRecord()
        {
            try
            {
                List<RegistrationViewModel> accountList = new List<RegistrationViewModel>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM UserAccount";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            RegistrationViewModel account = new RegistrationViewModel
                            {
                                Email = reader.GetString(0),
                                Password = reader.GetString(1),
                                EmployeeID = reader.GetString(2),
                                UserRole = reader.GetString(3),
                                IsActive = reader.GetBoolean(4)
                            };
                            
                            accountList.Add(account);
                        }


                        connection.Close();
                    }
                }
                return View(accountList);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }
        }

        //ADMIN GET: Edit USER ACCOUNT
        public ActionResult EditUserAccountRecord(string EmployeeID)
        {         
            try
            {
                // Retrieve the list of employeeList from the database
                List<Employee> employeeList = GetEmployeeIDFromDatabase();

                ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");

                AccountUpdateViewModel userAccount = new AccountUpdateViewModel();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM UserAccount WHERE EmployeeID = @EmployeeID";

                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {

                        command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            userAccount.Email = reader.GetString(0);

                            userAccount.EmployeeID = reader.GetString(2);
                            userAccount.UserRole = reader.GetString(3);
                            userAccount.IsActive = reader.GetBoolean(4);
                        }
                        connection.Close();
                    }
                }
                return View(userAccount);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }
        }

        //ADMIN POST: Edit USER ACCOUNT
        [HttpPost]
        public ActionResult EditUserAccountRecord(AccountUpdateViewModel userAccount)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string sqlQuery = "UPDATE UserAccount SET Email = @Email, ";

                        // Check if the password field is empty
                        if (!string.IsNullOrEmpty(userAccount.Password))
                        {
                            sqlQuery += "Password = @Password, ";
                        }

                        sqlQuery += "UserRole = @UserRole, IsActive = @IsActive WHERE EmployeeID = @EmployeeID";

                        // Open the connection
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            // Set the parameter values for User Account update 
                            command.Parameters.AddWithValue("@Email", userAccount.Email);

                            // Set the password parameter only if it is not empty
                            if (!string.IsNullOrEmpty(userAccount.Password))
                            {
                                // Hash the password using your chosen algorithm
                                string hashedPassword = HashPassword(userAccount.Password);
                                command.Parameters.AddWithValue("@Password", hashedPassword);
                            }

                            command.Parameters.AddWithValue("@UserRole", userAccount.UserRole);
                            command.Parameters.AddWithValue("@IsActive", userAccount.IsActive);
                            command.Parameters.AddWithValue("@EmployeeID", userAccount.EmployeeID);

                            // Execute the User Account update query
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    // Create success message
                    TempData["SuccessMessage"] = "Account Record Update Successfully.";

                    return RedirectToAction("Index", "Home");

                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
            }
            return View(userAccount);
        }

        
        //Retrieve the Employee/Ownership Records
        private List<Employee> GetEmployeeIDFromDatabase()
        {
            List<Employee> employeeList = new List<Employee>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Ownership WHERE EmployeeID != 'MS-8888'";
                connection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            EmployeeID = reader.GetString(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Office = reader.GetString(3),
                            Department = reader.GetString(4)
                         };

                        employeeList.Add(employee);
                    }

                }
                connection.Close();
            }
            return employeeList;
            
        }
        

        //User Account Registration View(For Login Portal)
        public ActionResult Registration()
        {
            // Retrieve the list of employeeList from the database
            List<Employee> employeeList = GetEmployeeIDFromDatabase();
            
            ViewBag.EmployeeRecord = new SelectList(employeeList,"EmployeeID","EmployeeID");

            return View();
        }

        //POST: User Account Registration
        [HttpPost]
        public ActionResult Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Check if email is already registered
                        string checkEmailQuery = "SELECT COUNT(*) FROM UserAccount WHERE Email = @checkEmail";
                        SqlCommand checkEmailCmd = new SqlCommand(checkEmailQuery, connection);
                        checkEmailCmd.Parameters.AddWithValue("@checkEmail", model.Email);


                        connection.Open();

                        int count = (int)checkEmailCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            ModelState.AddModelError("Email", "This email address is already registered.");
                            return View(model);
                        }

                        string query = "INSERT INTO UserAccount (Email,Password,EmployeeID,UserRole) VALUES (@Email,@Password,@EmployeeID,@UserRole)";
                        SqlCommand command = new SqlCommand(query, connection);

                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@Password", HashPassword(model.Password)); // Hash the password
                        command.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                        command.Parameters.AddWithValue("@UserRole", model.UserRole);

                        command.ExecuteNonQuery();

                        connection.Close();

                        // Create success message
                        TempData["SuccessMessage"] = "Account Registration Completed Successfully.";

                        return RedirectToAction("Index", "Home");
                    }

                }catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
                   
            }

            // Retrieve the list of employeeList from the database
            List<Employee> employeeList = GetEmployeeIDFromDatabase();

            ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");

            return View(model);

        }

        //GET: Forgot Password View
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //POST: Forgot Password View 
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string query = "SELECT * FROM UserAccount WHERE Email = @Email";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Email", model.Email);
                            connection.Open();

                            SqlDataReader reader = command.ExecuteReader();

                            if (reader.Read())
                            {
                                // Generate a unique token for password reset
                                string resetToken = Guid.NewGuid().ToString();

                                // Save the token and expiration date in the database for the user
                                SaveResetToken(model.Email, resetToken);

                                string fromMail = "Mazak.MIS@gmail.com";
                                string fromPassword = "cpfhojkmffualojg";

                                MailMessage message = new MailMessage();
                                message.From = new MailAddress(fromMail);
                                message.Subject = "RESET YOUR PASSWORD";
                                message.To.Add(new MailAddress(model.Email));
                                message.Body = $@"<h1>Reset your password</h1>
                                      <p>Please reset your password by clicking the button below:</p>
                                      <p><a href='http://localhost:51756/Account/ResetPassword?token={resetToken}' style='display: inline-block; padding: 12px 24px; background-color: #007bff; color: #fff; text-decoration: none;'>Reset Password</a></p>";
                                message.IsBodyHtml = true;

                                var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
                                {
                                    Port = 587,
                                    Credentials = new NetworkCredential(fromMail, fromPassword),
                                    EnableSsl = true
                                };
                                smtpClient.Send(message);

                                TempData["SuccessMessage"] = "Reset Password Link Is Sent To Your Email.";

                                return RedirectToAction("Login", "Account");
                            }
                            // user does not exist in the database
                            TempData["ErrorMessage"] = "The email you entered is not registered in our system.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Login", "Account"));
                }
            }
            return View(model);
        }     


        public ActionResult ResetPassword(string token)
        {
            try
            {
                string query = "SELECT * FROM UserAccount WHERE ResetToken = @ResetToken";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ResetToken", token);
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Token is valid, retrieve the email from the database
                            string email = reader.GetString(reader.GetOrdinal("Email"));

                            // Pass the email and token to the ResetPassword view using a view model
                            ResetPasswordViewModel model = new ResetPasswordViewModel
                            {
                                Email = email,
                                Token = token
                            };

                            return View(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string selectQuery = "SELECT Password FROM UserAccount WHERE Email = @Email";
                    string updateQuery = "UPDATE UserAccount SET Password = @Password, ResetToken = NULL WHERE Email = @Email";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Retrieve the current password from the database
                        string currentPassword;
                        using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@Email", model.Email);
                            currentPassword = (string)selectCommand.ExecuteScalar();
                        }

                        // Compare the new password with the existing password
                        if (HashPassword(model.Password) == currentPassword)
                        {
                            ModelState.AddModelError("Password", "The new password must be different from the current password.");
                            return View(model);
                        }

                        // Update the password in the database
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@Password", HashPassword(model.Password));
                            updateCommand.Parameters.AddWithValue("@Email", model.Email);
                            updateCommand.ExecuteNonQuery();
                        }
                    }

                    TempData["SuccessMessage"] = "Password has been reset. Please log in with your new password.";

                    return RedirectToAction("Login", "Account");
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Login", "Account"));
                }
            }

            return View(model);
        }



        //Get: Change Password View
        public ActionResult ChangePassword()
        {
            return View();
        }

        //POST: Change Password View
        [HttpPost]
        public ActionResult ChangePassword(string EmployeeID, ResetPasswordViewModel NewPassword)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get Password History
                    string sqlGetQuery = "SELECT Password FROM UserAccount WHERE EmployeeID = @EmployeeID";

                    // Update Password History
                    string sqlUpdateQuery = "UPDATE UserAccount SET Password = @Password WHERE EmployeeID = @EmployeeID";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Get the current password from the database
                        string currentPassword;

                        using (SqlCommand getPasswordCommand = new SqlCommand(sqlGetQuery, connection))
                        {
                            getPasswordCommand.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                            currentPassword = getPasswordCommand.ExecuteScalar()?.ToString();
                        }

                        // Hash the new password
                        string hashedNewPassword = HashPassword(NewPassword.Password);

                        // Compare the new password with the current password
                        if (currentPassword != null && currentPassword.Equals(hashedNewPassword))
                        {
                            TempData["ErrorMessage"] = "The new password cannot be the same as the current password.";
                            return View();
                        }

                        // Update the password in the database
                        using (SqlCommand updatePasswordCommand = new SqlCommand(sqlUpdateQuery, connection))
                        {
                            updatePasswordCommand.Parameters.AddWithValue("@Password", hashedNewPassword);
                            updatePasswordCommand.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                            int rowsAffected = updatePasswordCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Password updated successfully
                                TempData["SuccessMessage"] = "Password changed successfully.";
                                return View();
                            }
                            else
                            {
                                // Failed to update the password
                                TempData["ErrorMessage"] = "Failed to change the password.";
                                return View();
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Login", ""));
                }
            }
            
            return View();

        }

        private string GetEmployeeIDByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT EmployeeID FROM UserAccount WHERE Email = @Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Email", email);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("UserRole");

            Session.Clear(); // Clear the session data
            Session.Abandon(); // Abandon the session



            return RedirectToAction("Login", "Account");
        }

        // Helper method to hash the password using SHA256 algorithm
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // Convert each byte to its hexadecimal representation
                }

                return builder.ToString();
            }
        }

        private void SaveResetToken(string email, string token)
        {
            // Perform the necessary database update to save the token and its associated email
            // For example, you can add a new column in the UserAccount table to store the reset token and update the corresponding row for the given email

            string query = "UPDATE UserAccount SET ResetToken = @ResetToken WHERE Email = @Email";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ResetToken", token);
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}