using System.ComponentModel.DataAnnotations;
namespace ReserveWash.ViewModels.Product
{
    public class CarWashServiceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام خدمت الزامی است.")]
        [Display(Name = "نام خدمت")]
        public string Name { get; set; }

        [Required(ErrorMessage = "قیمت خدمت الزامی است.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "قیمت باید بزرگتر از صفر باشد.")]
        [Display(Name = "قیمت خدمت")]
        public decimal Price { get; set; }

        // شناسه کارواش مربوطه
        public int CarwashId { get; set; }
    }

}
