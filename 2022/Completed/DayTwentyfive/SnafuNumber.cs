namespace Puzzles.Completed.DayTwentyfive;
public class SnafuNumber
{
    private long _decNumber;

    private string _stringRep;

    public string Rep { get => _stringRep; }

    public SnafuNumber(string stringRep)
    {
        _stringRep = stringRep;

        _decNumber = ConvertToDec(stringRep);
    }

    public SnafuNumber(long decNumber)
    {
        _decNumber = decNumber;
        _stringRep = ConvertToStringRep(_decNumber);
    }

    public static long GetLongFromSnafu(string snafuString)
    {
        return ConvertToDec(snafuString);
    }

    public static string GetSnafuFromNumber(long num)
    {
        return ConvertToStringRep(num);
    }

    private static long ConvertToDec(string snafu)
    {
        long value = 0;
        var i = 0;
        foreach (var c in snafu.Reverse())
        {
            value += c switch
            {
                '-' => -(long)Math.Pow(5, i),
                '=' => -2 * (long)Math.Pow(5, i),
                '1' => (long)Math.Pow(5, i),
                '2' => 2 * (long)Math.Pow(5, i),
                _ => 0,
            };
            i++;
        }
        return value;
    }

    private static string ConvertToStringRep(long number)
    {
        string snafuString = "";

        var d = number;

        var digitArray = new List<long>();
        while ((long)Math.Floor((decimal)d / 5) != 0 || d % 5 != 0)
        {
            var r = d % 5;
            d = (long)Math.Floor((decimal)d / 5);
            digitArray.Add(r);
        }
        digitArray.Add(0);

        var digitArray2 = new List<long>();

        for (var i = 0; i < digitArray.Count; i++)
        {
            var num = digitArray[i];
            if (num == 3)
            {
                num = -2;
                digitArray[i + 1] += 1;
            }

            if (num == 4)
            {
                num = -1;
                digitArray[i + 1] += 1;
            }

            if (num == 5)
            {
                num = 0;
                digitArray[i + 1] += 1;
            }

            digitArray2.Add(num);
        }

        if (digitArray2.Count > 1 && digitArray2[digitArray2.Count - 1] == 0)
        {
            digitArray2.RemoveAt(digitArray2.Count - 1);
        }

        digitArray2.Reverse();

        foreach (var i in digitArray2)
        {
            snafuString += i switch
            {
                -1 => "-",
                -2 => "=",
                _ => i.ToString()
            };
        }

        return snafuString;
    }

    public override string ToString()
    {
        return _decNumber.ToString();
    }

    private long GreatestCommonDivisor(long a, long b)
    {
        var a_value = a;
        var b_value = b;
        var t = b_value;
        while (b_value != 0)
        {
            t = b_value;
            b_value = a_value % b_value;
            a_value = t;
        }
        return a_value;
    }
}
