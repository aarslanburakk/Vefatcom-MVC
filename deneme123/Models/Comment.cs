using System.ComponentModel;

namespace deneme123.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public string CommentUser { get; set; }
        public DateTime CommentDate{ get; set; }
        public string CommentContent { get; set; }
        public bool CommentİsActive { get; set; }
       
        [ForeignKey("Dead")]
        [DisplayName("Dead")]
        public int DeadID { get; set; }
        public virtual Dead Dead { get; set; }
    }
}
