using System.ComponentModel.DataAnnotations;

namespace ReserveWash.Models
{
    public class Carwash
    {
        public int Id { get; set; }
        [MaxLength(256, ErrorMessage = "نظرات نمیتواند بیشتر از 256 کاراکتر باشد")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }
        [MaxLength(15, ErrorMessage = " تاریخ ایجاد نمیتواند بیشتر از 15 کاراکتر باشد")]
        public string CreateDate { get; set; }
        public string? MainImagePath { get; set; }
        public string? SubImagePath { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // ناوبری به کاربر
        public virtual ICollection<Service> Services { get; set; } // ارتباط با خدمات کارواش
        public virtual ICollection<ReserveTime> ReserveTime { get; set; } // تایم رزرو

    }
}
