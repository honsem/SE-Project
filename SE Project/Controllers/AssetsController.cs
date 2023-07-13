using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SE_Project.Models;

namespace SE_Project.Controllers
{
    public class AssetsController : Controller
    {
        private readonly string connectionString = "Server=LAPTOP-RH67UL1G\\SQLEXPRESS;Database=AssetManagementSystem;User Id=ymsmis;Password=iamking;";

        // GET: Assets
        public ActionResult Index()
        {
            return View();
        }

        // GET: HardwareAssets (Display All)
        public ActionResult GetHardwareAssets()
        {
            List<HardwareViewList> hardwareList = new List<HardwareViewList>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT C.IpAddress, C.ComputerName, C.EquipmentType, CONCAT(O.FirstName,' ',O.LastName) AS Name, O.Office, O.Department, CO.Condition, CO.ComputerStatus, O.EmployeeID " +
                    "FROM Computer C JOIN Computer_Ownership CO ON C.SerialNo = CO.SerialNo JOIN Ownership O ON O.EmployeeID = CO.EmployeeID";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        HardwareViewList ho = new HardwareViewList
                        {
                            IpAddress = reader.IsDBNull(0) ? null : reader.GetString(0),
                            ComputerName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            EquipmentType = reader.IsDBNull(2) ? null : reader.GetString(2),
                            FullName = reader.GetString(3),
                            Office = reader.GetString(4),
                            Department = reader.GetString(5),
                            Condition = reader.GetString(6),
                            ComputerStatus = reader.GetString(7),
                            EmployeeID = reader.GetString(8)
                        };
                        hardwareList.Add(ho);
                    }
                    connection.Close();
                }
            }
            return View(hardwareList);
        }


        //Create Hardware Assets View
        public ActionResult CreateHardwareAssets()
        {
            return View();
        }

        //POST: Create Hardware Assets
        [HttpPost]
        public ActionResult CreateHardwareAssets(HardwareAssets ha)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "INSERT INTO Computer (SerialNo,IpAddress,MacAddress,ComputerName,ComputerModel,ComputerOS,EquipmentType) VALUES (@property1, @property2,@property3, @property4,@property5,@property6, @property7)";

                        connection.Open();

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@property1", ha.SerialNo);
                        command.Parameters.AddWithValue("@property2", ha.IpAddress ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@property3", ha.MacAddress ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@property4", ha.ComputerName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@property5", ha.ComputerModel ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@property6", ha.ComputerOS ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@property7", ha.EquipmentType);
                        // Add more parameters as needed

                        command.ExecuteNonQuery();

                        // Create success message
                        TempData["SuccessMessage"] = "Hardware Asset Registration Successfully.";
                        connection.Close();
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
            }

            return View(ha);
        }

        //Set Hardware Ownership
        public ActionResult HardwareOwnership()
        {
            // Retrieve the lists from the database
            List<Employee> employeeList = GetEmployeeIDFromDatabase();
            List<HardwareAssets> compList = GetSerialNoFromDatabase();

            ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");
            ViewBag.ComputerRecord = new SelectList(compList, "SerialNo", "SerialNo");

            return View();
        }

        //POST: Set Hardware Ownership
        [HttpPost]
        public ActionResult HardwareOwnership(Hardware_Ownership ho)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        string query = "INSERT INTO Computer_Ownership (SerialNo,EmployeeID,ComputerStatus,Condition) VALUES (@property1, @property2,@property3, @property4)";

                        connection.Open();

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@property1", ho.SerialNo);
                        command.Parameters.AddWithValue("@property2", ho.EmployeeID);
                        command.Parameters.AddWithValue("@property3", ho.ComputerStatus);
                        command.Parameters.AddWithValue("@property4", ho.Condition);

                        command.ExecuteNonQuery();

                        // Create success message
                        TempData["SuccessMessage"] = "Hardware Ownership Registration Successfully.";
                        connection.Close();
                    }

                    return RedirectToAction("Index", "Home");

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Error number for primary key violation
                    {
                        // Handle the primary key violation here
                        // You can display an error message to the user or take appropriate action
                    }
                    else
                    {
                        // Handle other SQL exceptions
                    }
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
            }

            // Retrieve the lists from the database
            List<Employee> employeeList = GetEmployeeIDFromDatabase();
            List<HardwareAssets> compList = GetSerialNoFromDatabase();

            ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");
            ViewBag.ComputerRecord = new SelectList(compList, "SerialNo", "SerialNo");

            return View(ho);
            
        }
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
        private List<HardwareAssets> GetSerialNoFromDatabase()
        {
            List<HardwareAssets> compList = new List<HardwareAssets>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Computer";

                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        HardwareAssets comp = new HardwareAssets
                        {
                            SerialNo = reader.GetString(0),
                            IpAddress = reader.IsDBNull(1) ? null : reader.GetString(1),
                            MacAddress = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ComputerName = reader.IsDBNull(3) ? null : reader.GetString(3),
                            ComputerModel = reader.IsDBNull(4) ? null : reader.GetString(4),
                            ComputerOS = reader.IsDBNull(5) ? null : reader.GetString(5),
                            EquipmentType = reader.GetString(6)
                        };

                        compList.Add(comp);
                    }

                }
                connection.Close();
            }
            return compList;
        }

        //ADMIN GET: Edit Hardware Assets (Get Information)
        public ActionResult EditHardwareAssets(string EmployeeID)
        {
            Edit_Hardware_Ownership_View hardwareOwnership = new Edit_Hardware_Ownership_View();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT H.SerialNo, H.IpAddress, H.MacAddress, H.ComputerName, H.ComputerModel, H.ComputerOS, H.EquipmentType, HO.EmployeeID, HO.ComputerStatus, HO.Condition " +
                    "FROM Computer H JOIN Computer_Ownership HO ON H.SerialNo = HO.SerialNo WHERE HO.EmployeeID = @EmployeeID";
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {

                        command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {

                            hardwareOwnership.SerialNo = reader.GetString(0);
                            hardwareOwnership.IpAddress = reader.IsDBNull(1) ? null : reader.GetString(1);
                            hardwareOwnership.MacAddress = reader.IsDBNull(2) ? null : reader.GetString(2);
                            hardwareOwnership.ComputerName = reader.IsDBNull(3) ? null : reader.GetString(3);
                            hardwareOwnership.ComputerModel = reader.IsDBNull(4) ? null : reader.GetString(4);
                            hardwareOwnership.ComputerOS = reader.IsDBNull(5) ? null : reader.GetString(5);
                            hardwareOwnership.EquipmentType = reader.GetString(6);
                            hardwareOwnership.EmployeeID = reader.GetString(7);
                            hardwareOwnership.ComputerStatus = reader.GetString(8);
                            hardwareOwnership.Condition = reader.GetString(9);

                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }

            return View(hardwareOwnership);
        }

        /* ADMIN POST: UPDATEH HARDWARE */
        [HttpPost]
        public ActionResult EditHardwareAssets(Edit_Hardware_Ownership_View hardwareList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //For Hardware Ownership Table
                    string sqlQueryForHO = "UPDATE Computer_Ownership SET ComputerStatus = @ComputerStatus, Condition = @Condition WHERE SerialNo = @SerialNo AND EmployeeID = @EmployeeID";

                    //For Hardware Table
                    string sqlQueryForH = "UPDATE Computer SET IpAddress = @IpAddress, MacAddress = @MacAddress, ComputerName = @ComputerName, ComputerModel = @ComputerModel, ComputerOS = @ComputerOS, EquipmentType = @EquipmentType WHERE SerialNo = @SerialNo";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Open the connection
                        connection.Open();

                        // Create a new SqlCommand object for updating Hardware Ownership
                        using (SqlCommand commandHO = new SqlCommand(sqlQueryForHO, connection))
                        {
                            // Set the parameter values for Hardware Ownership update
                            commandHO.Parameters.AddWithValue("@ComputerStatus", hardwareList.ComputerStatus);
                            commandHO.Parameters.AddWithValue("@Condition", hardwareList.Condition);
                            commandHO.Parameters.AddWithValue("@SerialNo", hardwareList.SerialNo);
                            commandHO.Parameters.AddWithValue("@EmployeeID", hardwareList.EmployeeID);

                            // Execute the Hardware Ownership update query
                            commandHO.ExecuteNonQuery();
                        }

                        // Create a new SqlCommand object for updating Hardware
                        using (SqlCommand commandH = new SqlCommand(sqlQueryForH, connection))
                        {
                            // Set the parameter values for Hardware update
                            commandH.Parameters.AddWithValue("@IpAddress", hardwareList.IpAddress ?? (object)DBNull.Value);
                            commandH.Parameters.AddWithValue("@MacAddress", hardwareList.MacAddress ?? (object)DBNull.Value);
                            commandH.Parameters.AddWithValue("@ComputerName", hardwareList.ComputerName ?? (object)DBNull.Value);
                            commandH.Parameters.AddWithValue("@ComputerModel", hardwareList.ComputerModel ?? (object)DBNull.Value);
                            commandH.Parameters.AddWithValue("@ComputerOS", hardwareList.ComputerOS ?? (object)DBNull.Value);
                            commandH.Parameters.AddWithValue("@EquipmentType", hardwareList.EquipmentType);
                            commandH.Parameters.AddWithValue("@SerialNo", hardwareList.SerialNo);

                            // Execute the Hardware update query
                            commandH.ExecuteNonQuery();
                        }

                        // Close the connection
                        connection.Close();
                    }
                    // Create success message
                    TempData["SuccessMessage"] = "Hardware Ownership Update Successfully.";

                    return RedirectToAction("Index", "Home");
            }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
            }

            // Retrieve the lists from the database
            List<Employee> employeeList = GetEmployeeIDFromDatabase();
            List<HardwareAssets> compList = GetSerialNoFromDatabase();

            ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");
            ViewBag.ComputerRecord = new SelectList(compList, "SerialNo", "SerialNo");

            return View(hardwareList);
        }

        //Software Assets
        //Create Software Assets View
        public ActionResult CreateSoftwareAssets()
        {
            // Retrieve the lists from the database
            List<Employee> employeeList = GetEmployeeIDFromDatabase();

            ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");

            return View();
        }

        //POST: Create Hardware Assets
        [HttpPost]
        public ActionResult CreateSoftwareAssets(SoftwareAssets sa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "INSERT INTO Software (SerialNo,SoftwareName,PurchaseDate,ExpiryDate,Cost,Vendor,EmployeeID) VALUES (@property1, @property2,@property3, @property4,@property5,@property6, @property7)";

                        connection.Open();

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@property1", sa.SerialNo);
                        command.Parameters.AddWithValue("@property2", sa.SoftwareName);
                        command.Parameters.AddWithValue("@property3", sa.PurchaseDate);
                        command.Parameters.AddWithValue("@property4", sa.ExpiryDate);
                        command.Parameters.AddWithValue("@property5", sa.Cost);
                        command.Parameters.AddWithValue("@property6", sa.Vendor);
                        command.Parameters.AddWithValue("@property7", sa.EmployeeID);
                        // Add more parameters as needed

                        command.ExecuteNonQuery();

                        // Create success message
                        TempData["SuccessMessage"] = "Software Registration Successfully.";
                        connection.Close();
                    }

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
            }

            // Retrieve the lists from the database
            List<Employee> employeeList = GetEmployeeIDFromDatabase();

            ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");


            return View(sa);
        }

        //List the Software Assets Details
        public ActionResult GetSoftwareAssets()
        {
            List<SoftwareViewList> softwareList = new List<SoftwareViewList>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT S.SerialNo, S.SoftwareName, S.PurchaseDate, S.ExpiryDate, S.Cost, S.Vendor, CONCAT(O.FirstName,' ',O.LastName) AS Name, O.Department, O.Office, S.EmployeeID " +
                    "FROM Software S JOIN Ownership O ON S.EmployeeID = O.EmployeeID";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SoftwareViewList so = new SoftwareViewList
                        {
                            SerialNo = reader.GetString(0),
                            SoftwareName = reader.GetString(1),
                            PurchaseDate = reader.GetDateTime(2).ToString("yyyy-MM-dd"),
                            ExpiryDate = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                            Cost = (double)reader.GetDecimal(4),
                            Vendor = reader.GetString(5),
                            FullName = reader.GetString(6),
                            Department = reader.GetString(7),
                            Office = reader.GetString(8),
                            EmployeeID = reader.GetString(9)
                        };

                        softwareList.Add(so);
                    }
                    connection.Close();
                }
            }
            return View(softwareList);
        }
        //ADMIN GET: Edit Software Assets (Get Information)
        public ActionResult EditSoftwareAssets(string EmployeeID)
        {
            SoftwareViewList software = new SoftwareViewList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT SerialNo, SoftwareName, PurchaseDate, ExpiryDate, Cost, Vendor, EmployeeID FROM Software WHERE EmployeeID = @EmployeeID";

                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            software.SerialNo = reader.GetString(0);
                            software.SoftwareName = reader.GetString(1);
                            software.PurchaseDate = reader.GetDateTime(2).ToString("yyyy-MM-dd");
                            software.ExpiryDate = reader.GetDateTime(3).ToString("yyyy-MM-dd");
                            software.Cost = (double)reader.GetDecimal(4);
                            software.Vendor = reader.GetString(5);
                            software.EmployeeID = reader.GetString(6);
                        }
                        connection.Close();
                    }
                }
                return View(software);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }

        }


        /* ADMIN POST: UPDATEH SOFTWARE */
        [HttpPost]
        public ActionResult EditSoftwareAssets(SoftwareViewList software)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //For Software Table
                    string query = "UPDATE Software SET SoftwareName = @SoftwareName, PurchaseDate = @PurchaseDate, ExpiryDate = @ExpiryDate," +
                        " Cost = @Cost, Vendor = @Vendor, EmployeeID = @EmployeeID WHERE SerialNo = @SerialNo";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Open the connection
                        connection.Open();

                        // Create a new SqlCommand object for updating Software
                        using (SqlCommand commandHO = new SqlCommand(query, connection))
                        {
                            // Set the parameter values for Software Ownership
                            commandHO.Parameters.AddWithValue("SerialNo", software.SerialNo);
                            commandHO.Parameters.AddWithValue("@SoftwareName", software.SoftwareName);
                            commandHO.Parameters.AddWithValue("@PurchaseDate", software.PurchaseDate);
                            commandHO.Parameters.AddWithValue("@ExpiryDate", software.ExpiryDate);
                            commandHO.Parameters.AddWithValue("@Cost", software.Cost);
                            commandHO.Parameters.AddWithValue("@Vendor", software.Vendor);
                            commandHO.Parameters.AddWithValue("@EmployeeID", software.EmployeeID);

                            // Execute the Software update query
                            commandHO.ExecuteNonQuery();
                        }
                        
                        // Close the connection
                        connection.Close();
       
                    }
                    // Create success message
                    TempData["SuccessMessage"] = "Software Record Update Successfully.";

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
                }
            }
            // Retrieve the lists from the database
            List<Employee> employeeList = GetEmployeeIDFromDatabase();

            ViewBag.EmployeeRecord = new SelectList(employeeList, "EmployeeID", "EmployeeID");

            return View(software);
        }


    }
}