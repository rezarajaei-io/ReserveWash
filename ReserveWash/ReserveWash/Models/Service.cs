using System.ComponentModel.DataAnnotations;

namespace ReserveWash.Models
{
    public class Service
    {
        public int Id { get; set; }
        [MaxLength(250, ErrorMessage = "نام نمیتواند بیشتر از 250 کاراکتر باشد")]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CarwashId { get; set; } // کلید خارجی به CarWash
        [MaxLength(15, ErrorMessage = " تاریخ ایجاد نمیتواند بیشتر از 15 کاراکتر باشد")]
        public string CreateDate { get; set; }
        public virtual Carwash Carwash { get; set; } // ناوبری به کارواش
        public virtual ICollection<ReserveTime> ReserveTime { get; set; } // تایم رزرو

    }
}
