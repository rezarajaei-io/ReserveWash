using System;
using System.Collections.Generic;
using System.Globalization;

namespace ReserveWash.Utilities // نام فضای نام خود را اینجا وارد کنید
{
    public class PersianCulture : CultureInfo
    {
        private readonly Calendar cal;
        private readonly Calendar[] optionals;

        public PersianCulture() : this("fa-IR", true) { }

        public PersianCulture(string cultureName, bool useUserOverride) : base(cultureName, useUserOverride)
        {
            // استفاده از PersianCalendar به عنوان تقویم اصلی
            cal = new PersianCalendar();

            // ایجاد لیست جدیدی از تقویم‌ها
            var optionalCalendars = new List<Calendar> { cal };
            optionalCalendars.AddRange(base.OptionalCalendars);

            // تنظیم تقویم‌های اختیاری
            optionals = optionalCalendars.ToArray();
            DateTimeFormat.Calendar = cal;

            // تنظیم نام ماه‌ها و روزها
            DateTimeFormat.MonthNames = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", "" };
            DateTimeFormat.DayNames = new[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };
            DateTimeFormat.AMDesignator = "ق.ظ";
            DateTimeFormat.PMDesignator = "ب.ظ";
        }

        public override Calendar Calendar => cal;

        public override Calendar[] OptionalCalendars => optionals;
    }
}
