using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventPlanner.Models.Comments
{
    public class CommentViewModel
    {
        public string Content { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public int Id { get; internal set; }
    }
}
