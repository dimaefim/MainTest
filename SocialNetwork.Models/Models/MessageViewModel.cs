namespace SocialNetwork.Models.Models
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public string TimeOfSend { get; set; }
    }
}