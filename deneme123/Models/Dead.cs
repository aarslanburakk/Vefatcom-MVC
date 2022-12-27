using System.ComponentModel;

namespace deneme123.Models

{
    public class Dead
    {
        public int Id { get; set; }
        public string DeadNameUsername { get; set; }
       
        public int DeadAge { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}",ApplyFormatInEditMode =true)]
        public string DeadDate { get; set; }
        public bool isActive { get; set; }

        public byte[]? DeadImage { get; set; }


        [ForeignKey("City")]
        [DisplayName("City")]
        public int CityId { get; set; }   

        public virtual City City { get; set; }

        [ForeignKey("User")]
        [DisplayName("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual Comment Comment { get; set; }


    }
}
