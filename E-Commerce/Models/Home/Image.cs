using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Home;

public class Image
{
    [Required]
    public int ImageId { get; set; }
    
    [Required]
    public byte[] ImageBytes { get; set; } 
    
    [Required]
    public int ProductId { get; set; }  
    
}
