using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditTaxSettingViewModel : AddTaxSettingViewModel
    {
        [Required]
        public new int Id { get; set; }
    }
}
