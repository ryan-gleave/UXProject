//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class TaskResult
    {
        //Note: This code is generated by Entity Framework based on the data model created by the author. This code is included as I believe it is neccessary to understand the system.
        public int TaskResultId { get; set; }
        public int TaskId { get; set; }
        public int TestResultId { get; set; }
        public string URL { get; set; }
        public bool Abandoned { get; set; }
        public System.TimeSpan TimeStarted { get; set; }
        public System.TimeSpan TimeTaken { get; set; }
        public string NavigationPath { get; set; }
    
        public virtual Task Task { get; set; }
        public virtual TestResult TestResult { get; set; }
    }
}
