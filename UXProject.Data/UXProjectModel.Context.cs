﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UXProject.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class UXProjectModelContext : DbContext
    {
        public UXProjectModelContext()
            : base("name=UXProjectModelContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<TaskResult> TaskResults { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TestResult> TestResults { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
    }
}
