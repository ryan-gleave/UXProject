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
    
    public partial class TestResult
    {
        //Note: This code is generated by Entity Framework based on the data model created by the author. This code is included as I believe it is neccessary to understand the system.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TestResult()
        {
            this.TaskResults = new HashSet<TaskResult>();
        }
    
        public int TestResultId { get; set; }
        public int TestId { get; set; }
        public string RecordingFilepath { get; set; }
        public int ParticipantId { get; set; }
    
        public virtual Participant Participant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaskResult> TaskResults { get; set; }
        public virtual Test Test { get; set; }
    }
}
