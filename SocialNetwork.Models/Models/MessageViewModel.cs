namespace SocialNetwork.Models.Models
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public string TimeOfSend { get; set; }
    }
}