using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Models
{
    public class UserMessageViewModel
    {
        public int UserId { get; set; }
        public string Photo { get; set; }
        public string Author { get; set; }
    }
}
