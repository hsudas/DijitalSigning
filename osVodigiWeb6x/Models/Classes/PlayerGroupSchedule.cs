using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayerGroupSchedule
    {
        public int PlayerGroupScheduleID { get; set; }
        public int PlayerGroupID { get; set; }
        public int ScreenID { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int? ToDay { get; set; }
        public int? ToHour { get; set; }
        public int? ToMinute { get; set; }
        public int? RepeatCount { get; set; }
    }
}