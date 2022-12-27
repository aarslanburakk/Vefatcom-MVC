namespace deneme123.Models
{
    public class Contact
    {
        [Required]
        public string İsim { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]

        public string Subject { get; set; }
        [Required]

        public string Message { get; set; }

    }
}
