using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ASM_SIMS.Models
{
    public class CategoryViewModel
    {
        public List<CategoryDetail> categoryList { get; set; }

    }
    public class CategoryDetail
    {
        // khai bao chi tiet tung category
        public int Id { get; set; }

        // bat buoc phai nhap, neu khong thi se hien thong bao loi
        [Required(ErrorMessage = "Name is required")]
        public string NameCategory { get; set; }

        [AllowNull]
        public string Description { get; set; }

        [Required(ErrorMessage = "Choose your Avatar")]
        public IFormFile Avatar { get; set; }

    }
}
