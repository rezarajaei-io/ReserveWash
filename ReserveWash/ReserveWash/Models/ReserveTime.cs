namespace ReserveWash.Models
{
    public class ReserveTime
    {
        public int Id { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool IsReserved { get; set; }
        public int ServiceId { get; set; }
        public int CarwashId { get; set; }
        public Carwash? Carwash { get; set; }
        public Service Service { get; set; }
        public virtual ICollection<Reservation> Reservation { get; set; } // تایم رزرو

    }
}
