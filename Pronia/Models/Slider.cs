using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;
public class Slider : BaseEntity
{
    [MaxLength(32), Required]
    public string Title { get; set; }
    [Range(0,100)]
    public int Discount { get; set; }
    [MaxLength(64), Required]
    public string Subtitle{ get; set; }
    [Required]
    public string ImageUrl { get; set; }
}

