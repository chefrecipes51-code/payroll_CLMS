using Microsoft.AspNetCore.Mvc;
using Payroll.WebApp.Models;
using Payroll.WebApp.CommonService;

namespace Payroll.WebApp.Controllers
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 16-Sep-2024
    /// Message detail :- Controller for handling employee data and showing details with encrypted URLs.
    /// </summary>
    public class EmployeeController : Controller
    {
        #region Sample Data
        private static List<Employee> Employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "Harshida Parmar", Position = "Software Engineer", Salary = 60000 },
            new Employee { Id = 2, Name = "Jane Smith", Position = "Project Manager", Salary = 75000 }
            // Add more employees here
        };
        #endregion
        #region Constructor 
        private readonly UrlEncryptionService _urlEncryptionService;

        /// <summary>
        /// Developer Name :- Harshida Parmar
        /// Created Date   :- 16-Sep-2024
        /// Initializes a new instance of the <see cref="EmployeeController"/> class.
        /// </summary>
        /// <param name="urlEncryptionService">The encryption service used for creating encrypted URLs for employee details.</param>
        public EmployeeController(UrlEncryptionService urlEncryptionService)
        {
            _urlEncryptionService = urlEncryptionService;
        }
        #endregion
        #region Employees CRUD
        #region List Employee Records
        /// <summary>
        /// Developer Name :- Harshida Parmar
        /// Created Date   :- 16-Sep-2024
        /// Action method that displays a list of employees, each with an encrypted URL for details.
        /// </summary>
        /// <returns>A view with a list of employees.</returns>
        public IActionResult Index()
        {
            var employeeList = Employees.Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Position = e.Position,
                Salary = e.Salary,
                EncryptedUrl = _urlEncryptionService.Encrypt("Employee", "Detail", new Dictionary<string, object> { { "id", e.Id } })
            }).ToList();

            return View(employeeList);
        }
        #endregion
        #region View Employee Details By ID
        /// <summary>
        /// Developer Name :- Harshida Parmar
        /// Created Date   :- 16-Sep-2024
        /// Action method that shows the details of a specific employee based on the decrypted ID.
        /// </summary>
        /// <param name="id">The ID of the employee to view details for.</param>
        /// <returns>A view showing the employee's details, or a 404 NotFound result if the employee does not exist.</returns>
        public IActionResult Detail(int id)
        {
            var employee = Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        #endregion
        #endregion

        #region Employee CRUD using Keyboard ShortCut
        /// <summary>
        ///  Jira Ticket    :- PAYROLL-142
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Employee CRUD using Keyboard ShortCut for testing purpose.
        ///  Created Date   :- 25-Oct-2024
        ///  Change Date    :- 25-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="model"> Employee detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        // Static data list
        private static List<Employee> employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "John Doe", Position = "Developer", Salary = 1000 },
            new Employee { Id = 2, Name = "Jane Smith", Position = "Designer", Salary = 2000 }
        };

        public IActionResult EmpIndex()
        {
            return View(employees);
        }

        [HttpPost]
        public IActionResult Add(Employee model)
        {
            model.Id = employees.Max(e => e.Id) + 1; // Generate a new ID
            employees.Add(model);
            // Perform add operation
            return Json(new { success = true, data = model });
        }

        [HttpPost]
        public IActionResult Edit(Employee model)
        {
            var employee = employees.FirstOrDefault(e => e.Id == model.Id);
            if (employee != null)
            {
                employee.Name = model.Name;
                employee.Position = model.Position;
                employee.Salary = model.Salary;
            }
            // Perform edit operation
            return Json(new { success = true, data = model });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                employees.Remove(employee);
            }
            // Perform delete operation
            return Json(new { success = true });
        }
        #endregion
    }
}


#region Code for Reference Use
//1. 
/// <summary>
/// Developer Name :- Harshida Parmar
/// Created Date   :- 16-Sep-2024
/// Action method that displays a list of employees, each with an encrypted URL for details.
/// </summary>
/// <returns>A view with a list of employees.</returns>
//public IActionResult Index1()
//{
//    var employeeList = Employees.Select(e => new EmployeeViewModel
//    {
//        Id = e.Id,
//        Name = e.Name,
//        Position = e.Position,
//        Salary = e.Salary,
//        EncryptedUrl = _urlEncryptionService.Encrypt("Employee", "Detail", new Dictionary<string, object> { { "id", e.Id } })
//    }).ToList();

//    return View(employeeList);
//}
#endregion