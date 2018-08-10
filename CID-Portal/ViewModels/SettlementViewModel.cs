using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VacationsPortal.Models;

namespace VacationsPortal.ViewModels
{
    public class SettlementViewModel
    {
        public ExpensesReport ExpensesReport { get; set; }

        public CashInAdvance CashInAdvance { get; set; }
    }
}