namespace WpfApp23.Models;

public class Angles
{
    public int Id { get; set; }
    public float Alpha { get; set; }
    public float Beta { get; set; }
    public float Gamma { get; set; }
    public float Theta { get; set; }
    public float Omega { get; set; }

    public static Angles FromCoordinates(double x, double y, double z)
    {
        //double d1;
        Console.WriteLine($"{x} {y} {z}");
        const double l1 = 10.4; 
        const double l2 = 9.7;
        x = x + 8.5;
        double D = Math.Atan(y / x) * (180 / (Math.PI)) + 90;
        x = x - Math.Abs(11 * Math.Cos((D - 90) / (180 / Math.PI)));
        y = Math.Abs(y) - Math.Abs(11 * Math.Sin((D - 90) / (180 / Math.PI)));
        if ((Math.Pow(x, 2) + Math.Pow(z, 2) + Math.Pow(y, 2)) <= Math.Pow(l1 + l2, 2))
        {
            var dy = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            var d = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(z, 2));
            var q1 = Math.Atan(z / dy);
            var q2 = Math.Acos((Math.Pow(d, 2) + Math.Pow(10.4, 2) - Math.Pow(9.7, 2)) / (2 * d * 10.4));
            var q3 = Math.Acos((Math.Pow(10.4, 2) + 9.7 * 9.7 - Math.Pow(d, 2)) / (2 * 10.4 * 9.7));
            var secondAngle1 = (Math.PI - q3) * (180 / (Math.PI)) + 90;
            var firstAngle1 = (q1 + q2) * (180 / (Math.PI)) + 90;
            var thirdAngle1 = -firstAngle1 + secondAngle1 + 90;
            Console.WriteLine($"1: {firstAngle1} 2: {secondAngle1} 3: {thirdAngle1} 4: {D}");
            return new Angles()
            {
                Alpha = (float)firstAngle1,
                Beta = (float)secondAngle1,
                Gamma = (float)thirdAngle1,
                Theta = (float)D,
                Omega = 0
            };
        }

        throw new ArgumentException("Невозможные координаты");

    }

    public static Angles FromCommandText(string message)
    {
        Angles angles = new Angles();
        string[] answer = message.Split(" ");
        var anglesAlpha = angles.Alpha;
        int intValue = 0;
        bool successV = Int32.TryParse(answer[0], out intValue);
        if (successV)
        {
            angles.Id = intValue;// 1 cor 25 40 25 67 65 cor 25 40 25 67 65 cor 25 40 25 67 65
        }
        else
        {
            Console.WriteLine("некоректная запись Варианта");
        }

        float floatValue = 0;
        bool successA = float.TryParse(answer[2], out floatValue);
        if (successA)
        {
            angles.Alpha = floatValue;
        }
        else
        {
            Console.WriteLine("некоректная запись угла Альфа");
        }

        successA = float.TryParse(answer[3], out floatValue);
        if (successA)
        {
            angles.Beta = floatValue;
        }
        else
        {
            Console.WriteLine("некоректная запись угла Бетта");
        }

        successA = float.TryParse(answer[4], out floatValue);
        if (successA)
        {
            angles.Gamma = floatValue;
        }
        else
        {
            Console.WriteLine("некоректная запись угла Гамма");
        }

        successA = float.TryParse(answer[5], out floatValue);
        if (successA)
        {
            angles.Theta = floatValue;
        }
        else
        {
            Console.WriteLine("некоректная запись угла Тетта");
        }

        successA = float.TryParse(answer[6], out floatValue);
        if (successA)
        {
            angles.Omega = floatValue;
        }
        else
        {
            Console.WriteLine("некоректная запись угла Омега");
        }

        return angles;
    }
}