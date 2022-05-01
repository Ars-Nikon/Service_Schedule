using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Schedule.Models
{
    public class TimeTableDate
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Spec_Id { get; set; }

        [Column(TypeName = "Date")]
        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("Spec_Id")]
        public Specialist Specialist { get; set; }

        public List<TimeTableTimeVisit> Times { get; set; }
    }
}
