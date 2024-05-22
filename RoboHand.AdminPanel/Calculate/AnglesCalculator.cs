using WpfApp23.Models;

namespace WpfApp23.Calculate;

public class AnglesCalculator
{
    public static Angles Recalculate(Angles angles, Variant variant) // l1, l2, X1, Y1, Z1, X2, Y2, Z2
    {
        Console.WriteLine(variant.L1);
        const int pincers = 11;
        Console.WriteLine(angles.Alpha);
        Console.WriteLine(angles.Alpha - 90);
        Console.WriteLine($"m1: {variant.L1 * Math.Cos((angles.Alpha - 90) * Math.PI / 180.0)}");
        Console.WriteLine($"m2: {variant.L2 * Math.Cos((angles.Beta - 90 - angles.Alpha + 90) * Math.PI / 180.0)}");
        Console.WriteLine(
            $"m3: {pincers * Math.Cos((angles.Gamma - 90 - angles.Beta - 90 - angles.Alpha - 90) * Math.PI / 180.0)}");

        var length1 = variant.L1 * Math.Cos((angles.Alpha - 90) * Math.PI / 180.0) +
                      variant.L2 * Math.Cos((angles.Beta - 90 - angles.Alpha + 90) * Math.PI / 180.0) +
                      pincers * Math.Cos((angles.Beta - 90 - angles.Alpha + 90 - angles.Gamma + 90) * Math.PI / 180.0);
        var xcoor1 = length1 * Math.Cos((angles.Theta - 90)*Math.PI/180) - 8.5;
        var ycoor1 = length1 * Math.Sin((angles.Theta - 90)*Math.PI/180);
        var zcoor1 = variant.L1 * Math.Sin((angles.Alpha - 90) * Math.PI / 180.0) -
                    variant.L2 * Math.Sin((angles.Beta - 90 - angles.Alpha + 90) * Math.PI / 180.0) +
                    pincers * Math.Sin((angles.Beta - 90 - angles.Alpha + 90 - angles.Gamma + 90) * Math.PI / 180.0);
        Console.WriteLine($"X: {xcoor1} Y: {ycoor1} Z: {zcoor1}");
        var angleOfVector1 = Math.Atan(ycoor1 / xcoor1) * (180 / (Math.PI)) + 90;
        xcoor1 -= Math.Abs(11 * Math.Cos(angleOfVector1 / (180 / Math.PI)));
        ycoor1 = Math.Abs(ycoor1) - Math.Abs(11 * Math.Sin(angleOfVector1 / (180 / Math.PI)));
        var dy = Math.Sqrt(Math.Pow(xcoor1, 2) + Math.Pow(ycoor1, 2));
        var d = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(zcoor1, 2));
        var q1 = Math.Atan(zcoor1 / dy);
        var q2 = Math.Acos((Math.Pow(d, 2) + Math.Pow(10.4, 2) - Math.Pow(9.7, 2)) / (2 * d * 10.4));
        var q3 = Math.Acos((Math.Pow(10.4, 2) + 9.7 * 9.7 - Math.Pow(d, 2)) / (2 * 10.4 * 9.7));
        var secondAngle1 = (Math.PI - q3) * (180 / (Math.PI)) + 90;
        var firstAngle1 = (q1 + q2) * (180 / (Math.PI)) + 90;
        var thirdAngle1 = -firstAngle1 + secondAngle1 + 90;

        return new Angles
        {
            Alpha = Convert.ToSingle(firstAngle1),
            Beta = Convert.ToSingle(secondAngle1),
            Gamma = Convert.ToSingle(thirdAngle1),
            Theta = Convert.ToSingle(angleOfVector1),
            Omega = angles.Omega
        };
    }

    public static bool Check(Angles angles, Variant variant) // l1, l2, X1, Y1, Z1, X2, Y2, Z2
    {
        const int pincers = 11;
        var length1 = variant.L1 * Math.Cos((angles.Alpha - 90) * Math.PI / 180.0) +
                      variant.L2 * Math.Cos((angles.Beta - 90 - angles.Alpha + 90) * Math.PI / 180.0) +
                      pincers * Math.Cos((angles.Beta - 90 - angles.Alpha + 90 - angles.Gamma + 90) * Math.PI / 180.0);
        var xcor1 = length1 * Math.Cos((angles.Theta - 90)*Math.PI/180) - 8.5;
        var ycor1 = length1 * Math.Sin((angles.Theta - 90)*Math.PI/180);
        var zcor1 = variant.L1 * Math.Sin((angles.Alpha - 90) * Math.PI / 180.0) -
                    variant.L2 * Math.Sin((angles.Beta - 90 - angles.Alpha + 90) * Math.PI / 180.0) +
                    pincers * Math.Sin((angles.Beta - 90 - angles.Alpha + 90 - angles.Gamma + 90) * Math.PI / 180.0);
        const double tolerance = 0.25;
        Console.WriteLine(length1);
        Console.WriteLine($"X:{xcor1} Y:{ycor1} Z:{zcor1}");
        bool check = false;
        if (Math.Abs(xcor1 - variant.X1) < tolerance &&
            Math.Abs(ycor1 - variant.Y1) < tolerance &&
            Math.Abs(zcor1 - variant.Z1) < tolerance)
        {
            check = true;
        }


        return check;
    }
}
