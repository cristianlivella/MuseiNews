using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MuseiNews.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public long Timestamp { get; set; }
        public string Picture { get; set; }

        [NotMapped]
        public virtual bool Read {get; set;}
    }
}
