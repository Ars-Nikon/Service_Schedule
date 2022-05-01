using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Schedule.Models
{
    public class TimeTableTimeVisit
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string User_Id { get; set; }

        [Required]
        public Guid Date_Id { get; set; }

        [Required]
        public DateTime Visit_Start { get; set; }

        [Required]
        public DateTime Visit_End { get; set; }

        [Required]
        public  int Status { get; set; }

        public string Discription{ get; set; }

        [ForeignKey("User_Id")]
        public User User { get; set; }

        [ForeignKey("Date_Id")]
        public TimeTableDate TimeTableDate { get; set; }
    }
}
