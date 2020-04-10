using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuseiNews.Models
{
    public class NewsRead
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NewsId { get; set; }

        public virtual User User { get; set; }
        public virtual News News { get; set; }
    }
}
