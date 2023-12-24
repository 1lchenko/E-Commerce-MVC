namespace ECommerce.Models.IdentityModels
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<Order>? Orders { get; set; }


    }
}
