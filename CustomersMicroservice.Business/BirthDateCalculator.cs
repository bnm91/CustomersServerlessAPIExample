using CustomersMicroservice.Models;

namespace CustomersMicroservice.Business
{
    public static class BirthDateCalculator
    {
        public static DateRange GetBirthDateRangeForAge(int age)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);

            return new DateRange
            {
                MinDate = now.AddYears(-(age + 1)).AddDays(1),
                MaxDate = now.AddYears(-age)
            };
        }
    }
}
