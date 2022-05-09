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
        public TimeSpan Visit_Start { get; set; }

        [Required]
        public TimeSpan Visit_End { get; set; }

        [Required]
        public Visit_Status Status { get; set; }

        public string Discription { get; set; }

        [ForeignKey("User_Id")]
        public User User { get; set; }

        [ForeignKey("Date_Id")]
        public TimeTableDate TimeTableDate { get; set; }


        public enum Visit_Status
        {
            [Display(Name = "Cвободно")]
            Free = 1,
            [Display(Name = "Зарезервировано")]
            Taken = 2,// занято
            [Display(Name = "Скрыто")]
            Hidden = 3,// скрвто
            [Display(Name = "Закончен")]
            Finished = 4,// Закончен
            [Display(Name = "Отменен")]
            Сanceled = 5, // Отменен 
            [Display(Name = "Не пришел")]
            DidNotcome = 6, // Не пришел 
        }
    }
}
