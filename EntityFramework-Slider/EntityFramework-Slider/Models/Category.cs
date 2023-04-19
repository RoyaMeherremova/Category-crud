using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Models
{
    public class Category:BaseEntity
    {
        [Required]  //Name null ola bilmez
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }   /*Categoriden  yola cixanda Producta catmaq ucun*/
    }
}
