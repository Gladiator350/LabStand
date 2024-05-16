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
        const double l1 = 10.4; 
        const double l2 = 9.7;
        x += 8.5;
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
            return new Angles()
            {
                Alpha = (float)firstAngle1,
                Beta = (float)D,
                Gamma = (float)secondAngle1,
                Omega = (float)thirdAngle1
            };
        }

        throw new ArgumentException("Невозможные координаты");

    }
}