using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Models
{
    public class MainPageViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string DateOfBirthString => DateOfBirth.ToString("dd.MM.yyyy");

        public string AboutMe { get; set; }
        public byte[] MainPhoto { get; set; }
        public string MainPhotoString => Convert.ToBase64String(MainPhoto);
    }
}
