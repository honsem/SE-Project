using SE_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace SE_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = "Server=LAPTOP-RH67UL1G\\SQLEXPRESS;Database=AssetManagementSystem;User Id=ymsmis;Password=iamking;";
        public ActionResult Index()
        {
            try
            {
                ViewBag.Title = "Home Page";

                // Get the user's profile information
                string emailAddress = Session["Email"].ToString();
                string employeeID = Session["EmployeeID"].ToString();

                StackedBarReportView();

                DoughnutChartReportView();

                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Index", "Home"));
            }


        }

        public void StackedBarReportView()
        {
            // Query to get the Annual Computer Status Report
            string query = "SELECT ComputerStatus, COUNT(*) AS TotalQuantity FROM Computer_Ownership GROUP BY ComputerStatus";

            List<HardwareStatusReportView> reportData = new List<HardwareStatusReportView>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        HardwareStatusReportView reportView = new HardwareStatusReportView
                        {
                            computerStatus = reader.GetString(0),
                            hardwareTotalQty = reader.GetInt32(1)
                        };

                        reportData.Add(reportView);
                    }

                    connection.Close();
                }
            }

            // Pass the reportData to the view
            ViewBag.HardwareAssetsReport = reportData;
        }

        public void DoughnutChartReportView()
        {
            // Query to get the Annual Computer Status Report
            string query = "SELECT EquipmentType, COUNT(*) AS TotalQuantity FROM Computer GROUP BY EquipmentType; ";

            List<EquipmentTypeReportView> reportData = new List<EquipmentTypeReportView>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        EquipmentTypeReportView reportView = new EquipmentTypeReportView
                        {
                            equipmentType = reader.GetString(0),
                            equipmentTotalQty = reader.GetInt32(1)

                        };


                        reportData.Add(reportView);
                    }

                    connection.Close();
                }
            }

            // Pass the reportData to the view
            ViewBag.EquipmentTypeReport = reportData;
        }
    }

}
