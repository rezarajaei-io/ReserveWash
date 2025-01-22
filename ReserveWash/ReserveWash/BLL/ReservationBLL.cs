using ReserveWash.Utilities;
using ReserveWash.ViewModels.Product;

namespace ReserveWash.BLL
{
    public class ReservationBLL
    {
        public static ReservationViewModel MakeReserveItems(string carwashId, string carId, string reserveDate, string serviceId)
        {
            ReservationViewModel resereVM= new ReservationViewModel()
            {
                CarwashId = int.Parse(carwashId),
                CarId = int.Parse(carId),
                ReservationDate = DateConverter.ConvertJalaliToGregorian(reserveDate),
                ServiceId = 1
            };

            return resereVM;
        }
    }
}
