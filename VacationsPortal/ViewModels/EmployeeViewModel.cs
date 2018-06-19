using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VacationsPortal.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string BasedOut { get; set; }

        public string DirectLine { get; set; }

        public string DottedLine { get; set; }

        public DateTime? HiringDate { get; set; }

        public int? VacationBalance { get; set; }

        public int? VacationsCarryOver { get; set; }
    }
}