namespace ReserveWash.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public int ReserveTimeId { get; set; }
        public ReserveTime ReserveTime { get; set; }
    }
}
