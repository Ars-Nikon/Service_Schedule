using Service_Schedule.Models;
using System.Collections.Generic;
using System.Text;

namespace Service_Schedule.Utilits
{
    public class Utilit
    {
        public static AccountViewModel ConvertUserByAccountUser(User user)
        {
            return new AccountViewModel
            {
                Id = user.Id,
                BirthDate = user.BirthDate,
                Email = user.Email?.Trim(),
                Name = user.Name?.Trim(),
                Gender = user.Gender,
                Phone = user.PhoneNumber?.Trim()
            };
        }



        public static readonly Dictionary<string, int> Mounths = new Dictionary<string, int>
        {
            {"Январь",1},
            {"Февраль",2},
            {"Март",3},
            {"Апрель",4},
            {"Май",5},
            {"Июнь",6},
            {"Июль",7},
            {"Август",8},
            {"Сентябрь",9},
            {"Октябрь",10},
            {"Ноябрь",11},
            {"Декабрь",12}
        };




    }
}
