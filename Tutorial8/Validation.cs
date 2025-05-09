namespace Tutorial8;

public class Validation
{
    public static bool ValidateEmail(string email)
    {
        return email.Contains('@') && email.Contains('.');
    }

    public static bool ValidateTelephone(string telephone)
    {
        foreach (char c in telephone)
        {
            if (c != '+' && (c is < '0' or > '9'))
                return false;
        }
        
        return true;
    }

    public static bool ValidatePesel(string pesel)
    {
        const int peselLength = 11;
        
        if (pesel.Length != peselLength)
            return false;

        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int controlSum = 0;

        for (int i = 0; i < peselLength; i++)
        {
            if (!Char.IsDigit(pesel[i]))
                return false;
            
            controlSum += (Convert.ToInt32(pesel[i]) * weights[i]) % 10;
        }
        
        return 10 - controlSum == Convert.ToInt32(pesel[peselLength - 1]);
    }
}