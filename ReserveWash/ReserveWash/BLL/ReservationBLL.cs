using ReserveWash.Models;
using ReserveWash.Utilities;
using ReserveWash.ViewModels.Product;

namespace ReserveWash.BLL
{
    public class ReservationBLL
    {
        public static Reservation MakeReserveModelItems(int carId, int reserveTimeId)
        {
            Reservation resereVM= new Reservation()
            {
                CarId = carId,
                ReserveTimeId = reserveTimeId,
            };

            return resereVM;
        }
    }
}
