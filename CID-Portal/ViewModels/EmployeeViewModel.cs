using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VacationsPortal.Models;

namespace VacationsPortal.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? HiringDate { get; set; }

        public Role Role { get; set; }

        public Workload Workload { get; set; }

        public Country BasedOut { get; set; }

        public Employee DirectLine { get; set; }

        public Employee DottedLine { get; set; }

        public City BaseCity { get; set; }

        public int? VacationBalance { get; set; }

        public int? VacationsCarryOver { get; set; }

        public bool Resigned { get; set; }
    }
}