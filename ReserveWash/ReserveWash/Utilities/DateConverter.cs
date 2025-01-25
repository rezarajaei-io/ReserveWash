using System;
using System.Globalization;

namespace ReserveWash.Utilities
{
    public static class DateConverter
    {
        public static DateTime JalaliToGregorian(int year, int month, int day)
        {
            // الگوریتم تبدیل تاریخ جلالی به میلادی
            // این الگوریتم باید بر اساس قوانین تقویم جلالی پیاده‌سازی شود
            // به عنوان مثال، اینجا یک تبدیل ساده آورده شده است
            DateTime gregorianDate = new DateTime(year + 621, month, day); // این فقط یک تخمین است
            return gregorianDate;
        }

        public static (int year, int month, int day) GregorianToJalali(DateTime date)
        {
            // الگوریتم تبدیل تاریخ میلادی به جلالی
            // این الگوریتم باید بر اساس قوانین تقویم جلالی پیاده‌سازی شود
            int jalaliYear = date.Year - 621; // این فقط یک تخمین است
            int jalaliMonth = date.Month;
            int jalaliDay = date.Day;
            return (jalaliYear, jalaliMonth, jalaliDay);
        }

        public static DateTime ConvertJalaliToGregorian(string jalaliDate)
        {
            jalaliDate = jalaliDate.Replace('-', '/'); // درصورت وارد شدن - به / تبدیل میشود
            // جدا کردن سال، ماه و روز
            string[] dateParts = jalaliDate.Split('/');

            if (dateParts.Length != 3)
            {
                throw new ArgumentException("Invalid Jalali date format. Expected format: yyyy/MM/dd");
            }

            // تبدیل رشته‌ها به عدد صحیح
            int year = int.Parse(dateParts[0]);
            int month = int.Parse(dateParts[1]);
            int day = int.Parse(dateParts[2]);

            // استفاده از PersianCalendar برای تبدیل تاریخ
            PersianCalendar persianCalendar = new PersianCalendar();
            DateTime gregorianDate = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);

            return gregorianDate;
        }
        public static string GregorianToJalaliStringWithTime(DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            // استخراج سال، ماه و روز از تاریخ میلادی
            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);
            int day = persianCalendar.GetDayOfMonth(date);

            // استخراج ساعت، دقیقه و ثانیه
            int hour = date.Hour;
            int minute = date.Minute;
            int second = date.Second;

            // فرمت‌بندی خروجی به صورت رشته
            return $"{year}/{month:D2}/{day:D2} {hour:D2}:{minute:D2}:{second:D2}";
        }
        public static string GregorianToJalaliStringPersianMonth(DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            // استخراج سال، ماه و روز از تاریخ میلادی
            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);

            // آرایه‌ای از نام‌های فارسی ماه‌ها
            string[] months = new string[]
            {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
            };

            // تبدیل ماه به نام فارسی
            string monthName = months[month - 1];  // چون ماه‌ها در PersianCalendar از 1 شروع می‌شود، باید 1 از آن کم کنیم

            // فرمت‌بندی خروجی به صورت رشته با نمایش نام ماه
            return $"{monthName} - {year}";
        }

    }

}
