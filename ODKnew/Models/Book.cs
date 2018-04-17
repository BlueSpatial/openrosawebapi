using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODKnew.Models
{
    public class Book
    {
        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Author { get; set; }
        public virtual string Genre { get; set; }
    }
}