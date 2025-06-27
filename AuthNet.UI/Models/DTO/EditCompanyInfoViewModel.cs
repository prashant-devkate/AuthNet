using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditCompanyInfoViewModel : AddCompanyViewModel
    {
        [Required]
        public new int Id { get; set; }
    }
}
