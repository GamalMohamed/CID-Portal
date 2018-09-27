﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VacationsPortal.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CIDvNEXtEntities : DbContext
    {
        public CIDvNEXtEntities()
            : base("name=CIDvNEXtEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<contact> contacts { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeesVacationDay> EmployeesVacationDays { get; set; }
        public virtual DbSet<EmployeesWeekendDay> EmployeesWeekendDays { get; set; }
        public virtual DbSet<EmployeeVacation> EmployeeVacations { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Subsidary> Subsidaries { get; set; }
        public virtual DbSet<VacationBalanceBackUp> VacationBalanceBackUps { get; set; }
        public virtual DbSet<VacationsHistory> VacationsHistories { get; set; }
        public virtual DbSet<VacationsLog> VacationsLogs { get; set; }
        public virtual DbSet<VacationStatusType> VacationStatusTypes { get; set; }
        public virtual DbSet<VacationType> VacationTypes { get; set; }
        public virtual DbSet<Workload> Workloads { get; set; }
        public virtual DbSet<EmployeesView> EmployeesViews { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CIDW8RoleTypes> CIDW8RoleTypes { get; set; }
        public virtual DbSet<CashInAdvance> CashInAdvances { get; set; }
        public virtual DbSet<CashInAdvanceStatu> CashInAdvanceStatus { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<EmployeeCountry> EmployeeCountries { get; set; }
        public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public virtual DbSet<ExpenseReportStatu> ExpenseReportStatus { get; set; }
        public virtual DbSet<ExpensesDetail> ExpensesDetails { get; set; }
        public virtual DbSet<ExpensesReport> ExpensesReports { get; set; }
        public virtual DbSet<ExpensesSubCategoryType> ExpensesSubCategoryTypes { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<TravelRequest> TravelRequests { get; set; }
        public virtual DbSet<TravelRequestStatu> TravelRequestStatus { get; set; }
        public virtual DbSet<TRHotelInfo> TRHotelInfoes { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<TRItemsStatu> TRItemsStatus { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<TripsView> TripsViews { get; set; }
        public virtual DbSet<TripsView_Archive> TripsView_Archive { get; set; }
        public virtual DbSet<NonTripsView> NonTripsViews { get; set; }
        public virtual DbSet<NonTripsView_Archive> NonTripsView_Archive { get; set; }
        public virtual DbSet<TravelRequestView> TravelRequestViews { get; set; }
        public virtual DbSet<TravelRequestView_Archive> TravelRequestView_Archive { get; set; }
        public virtual DbSet<TRHotelInfoView> TRHotelInfoViews { get; set; }
        public virtual DbSet<TRHotelInfoView_Archive> TRHotelInfoView_Archive { get; set; }
        public virtual DbSet<VisasView> VisasViews { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<PublicVacation> PublicVacations { get; set; }
        public virtual DbSet<AuthUser> AuthUsers { get; set; }
    }
}
