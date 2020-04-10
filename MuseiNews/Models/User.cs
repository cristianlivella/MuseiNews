using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuseiNews.Models
{
    public class User
    {
        public int Id { get; set; }
        public int InternalId { get; set; }
        public int ClientId { get; set; }

        public virtual ICollection<NewsRead> NewsReads { get; set; }
    }
}
