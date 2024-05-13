using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.Sliders;

public class CreateSliderVM
{
    [MaxLength(32, ErrorMessage = "Başlıq 32 simvoldan çox ola bilməz"), Required]
    public string Title { get; set; }
    [Range(0, 100)]
    public int Discount { get; set; }
    [MaxLength(64), Required]
    public string Subtitle { get; set; }
    [Required]
    public string ImageUrl { get; set; }
}
