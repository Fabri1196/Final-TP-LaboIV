using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pharmacy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerMedication>().HasKey(sc => new { sc.Customerid, sc.Medicationid });

            modelBuilder.Entity<Customer>().HasMany(s => s.CustomerMedication);
            modelBuilder.Entity<Medication>().HasMany(p => p.CustomerMedication);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Category> category { get; set; }
        public DbSet<Lab> lab { get; set; }
        public DbSet<Customer> customer { get; set; }
        public DbSet<Medication> medication { get; set; }
        public DbSet<HealthcareSystem> healthcareSystem { get; set; }
        public DbSet<CustomerMedication> customerMedication { get; set; }
    }
}
