using ReserveWash.Utilities;
using System.Globalization;

namespace ReserveWash.Middlewares
{
    public class PersianCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public PersianCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // تنظیم فرهنگ فارسی
            //var persianCulture = new PersianCulture();
            //CultureInfo.CurrentCulture = persianCulture;
            //CultureInfo.CurrentUICulture = persianCulture;

            //// تبدیل تاریخ‌های ورودی از جلالی به میلادی
            //if (context.Request.Method == HttpMethods.Post)
            //{
            //    var form = context.Request.Form;
            //    var convertedForm = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(form);

            //    foreach (var key in form.Keys)
            //    {
            //        if (form[key].ToString().Contains("/")) // فرض بر این است که تاریخ جلالی با "/" جدا شده است
            //        {
            //            var parts = form[key].ToString().Split('/');
            //            if (parts.Length == 3 &&
            //                int.TryParse(parts[0], out int jalaliYear) &&
            //                int.TryParse(parts[1], out int jalaliMonth) &&
            //                int.TryParse(parts[2], out int jalaliDay))
            //            {
            //                try
            //                {
            //                    // تبدیل تاریخ جلالی به میلادی
            //                    DateTime gregorianDate = DateConverter.JalaliToGregorian(jalaliYear, jalaliMonth, jalaliDay);
            //                    convertedForm[key] = gregorianDate.ToString("yyyy-MM-dd");
            //                }
            //                catch (Exception ex)
            //                {
            //                    // مدیریت خطا در صورت بروز مشکل در تبدیل تاریخ
            //                }
            //            }
            //        }
            //    }

            //    // استفاده از دیکشنری تبدیل‌شده به جای فرم اصلی
            //    context.Request.Form = new FormCollection(convertedForm);
            //}

            // ادامه پردازش درخواست
            await _next(context);
        }
    }
}
