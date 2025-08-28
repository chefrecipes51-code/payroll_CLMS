using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class Contractorprofile : BaseModel
    {
        public string Contractor_Name { get; set; }
        public int Contractor_ID { get; set; }
        public string License_No { get; set; }
        public string EPF_No { get; set; }
        public string CompanyName { get; set; }
        public int Company_Id { get; set; }
        public string LIN_No { get; set; }
        public bool Is_SubContractor { get; set; }
        public string HasSubcontractor { get; set; }
        public int Max_Labour_Count { get; set; }
        public string Email_Id { get; set; }
        public string Contact_No { get; set; }
        public string TAN_No { get; set; }
        public string PAN_No { get; set; }
        public string Address { get; set; }
    }
}
