using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Models
{
    public class DialogsViewModel
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
    }
}
