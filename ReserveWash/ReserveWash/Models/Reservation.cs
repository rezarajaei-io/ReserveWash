namespace ReserveWash.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime ReservationDate { get; set; }
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public Carwash? Carwash { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
