using System.ComponentModel.DataAnnotations;

namespace ReserveWash.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string UserId { get; set; } // کلید خارجی به ApplicationUser
        public ApplicationUser User { get; set; }
        [MaxLength(512, ErrorMessage = "نظرات نمیتواند بیشتر از 512 کاراکتر باشد")]
        public string Comments { get; set; }
        public int CarwashId { get; set; } // کلید خارجی به CarWash

        public virtual Carwash Carwash { get; set; } // ناوبری به کارواش

    }
}
