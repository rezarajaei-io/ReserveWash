﻿namespace ReserveWash.ViewModels.Product
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CarWashViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام کارواش الزامی است.")]
        [Display(Name = "نام کارواش")]
        public string Name { get; set; }

        [Required(ErrorMessage = "مکان کارواش الزامی است.")]
        [Display(Name = "مکان کارواش")]
        public string Address { get; set; }
        [Required(ErrorMessage = "امتیاز الزامی است")]
        [Display(Name = "امتیاز")]
        public int Star { get; set; }

        // لیست خدمات مربوط به این کارواش
        public List<CarWashServiceViewModel> Services { get; set; } = new List<CarWashServiceViewModel>();

        // لیست فیدبک‌ها مربوط به این کارواش
        public List<FeedbackViewModel> Feedbacks { get; set; } = new List<FeedbackViewModel>();
    }

}
