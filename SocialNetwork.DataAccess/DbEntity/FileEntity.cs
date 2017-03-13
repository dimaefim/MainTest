using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class FileEntity : IdNameEntity
    {
        public string MimeType { get; set; }
        public DateTime DateCreated { get; set; }
        public string Notes { get; set; }
        public byte[] Content { get; set; }
    }
}
