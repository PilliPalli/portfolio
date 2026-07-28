namespace Portfolio.Models;

public static class PortfolioProfile
{
    private static readonly DateTime BirthDate = new(2002, 9, 3);

    public static int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;

            if (BirthDate.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
