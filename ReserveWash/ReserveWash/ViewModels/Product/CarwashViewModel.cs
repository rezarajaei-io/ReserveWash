namespace ReserveWash.ViewModels.Product
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CarWashViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام کارواش الزامی است.")]
        [Display(Name = "نام کارواش")]
        public string Name { get; set; }

        [Required(ErrorMessage = "مکان کارواش الزامی است.")]
        [Display(Name = "مکان کارواش")]
        public string Address { get; set; }
        [Required(ErrorMessage = "فیلد الزامی است")]
        [Display(Name = "درباره کارواش")]
        public string Description { get; set; }
        [Display(Name = "تاریخ تاسیس")]
        public string? CreateDate { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string? UserId { get; set; }
        [NotMapped]
        public IFormFile? MainImage { get; set; }
        [NotMapped]
        public IFormFile? SubImage { get; set; }
        public string? MainImagePath { get; set; }
        public string? SubImagePath { get; set; }
        // لیست خدمات مربوط به این کارواش
    }
}

