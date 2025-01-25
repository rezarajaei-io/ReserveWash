using System.Collections.Generic;

namespace ReserveWash.ViewModels.Product
{

    public class AdminDashboardViewModel
    {
        public int TotalReservations { get; set; }
        public decimal TotalIncome { get; set; }
        public int CarsCount { get; set; }
        public List<ReservationInfo> RecentReservations { get; set; }
        public List<ServicePopularity> PopularServices { get; set; }
        public List<IncomeData> MonthlyIncome { get; set; }
    }

    public class ReservationInfo
    {
        public string CarName { get; set; }
        public string ServiceName { get; set; }
        public DateTime ReservationDate { get; set; }
    }

    public class ServicePopularity
    {
        public string ServiceName { get; set; }
        public int UsageCount { get; set; }
    }

    public class IncomeData
    {
        public string Month { get; set; }
        public decimal Income { get; set; }
    }

}

