using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace DoDVAChecklist.Models
{
    public class ChecklistsContext : DbContext
    {
        public ChecklistsContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Checklist> Checklists { get; set; }
        public DbSet<ChecklistHistoryEntry> ChecklistHistoryEntries { get; set; }
    }

    [Table("Checklist")]
    public class Checklist
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ChecklistId { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        public String VeteranFirstName { get; set; }

        public String VeteranLastName { get; set; }

        public virtual ICollection<ChecklistHistoryEntry> HistoryEntries { get; set; }

        public virtual UserProfile PreviousUser { get; set; }

        public virtual UserProfile Creator { get; set; }

        [NotMapped]
        public string Title {
            get { return "Checklist created at " + CreatedDate + " by " + Creator.FullName; }
        }
    }

    [Table("ChecklistHistoryEntry")]
    public class ChecklistHistoryEntry
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EntryId { get; set; }

        [Required]
        public DateTime TransferDate { get; set; }

        public virtual UserProfile Recipient { get; set; }

        public virtual UserProfile Transferrer { get; set; }
    }
}
