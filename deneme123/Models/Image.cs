using System.ComponentModel;

namespace deneme123.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string VefatAd { get; set; }
        public string VefatSoyad { get; set; }
        public byte[] Picture { get; set; }

    }
}
