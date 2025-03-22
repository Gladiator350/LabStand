using WpfApp23.Models;

namespace WpfApp23.Calculate;

public class AnglesCalculator
{
    public static Angles Recalculate(Angles angles, Variant variant) // l1, l2, X1, Y1, Z1, X2, Y2, Z2
    {
        const int pincers = 11;
        var length1 = variant.L1 * Math.Cos((angles.Alpha - 90) * Math.PI / 180.0) +
                      variant.L2 * Math.Cos((angles.Beta - 90 - angles.Alpha + 90) * Math.PI / 180.0) +
                      pincers * Math.Cos((angles.Beta - 90 - angles.Alpha + 90 - angles.Gamma + 90) * Math.PI / 180.0);
        Console.WriteLine(length1);
        var xcor1 = length1 * Math.Cos((angles.Theta - 90)*Math.PI/180) - 8.5;
        Console.WriteLine(xcor1);
        var ycor1 = length1 * Math.Sin((angles.Theta - 90)*Math.PI/180);
        var zcor1 = variant.L1 * Math.Sin((angles.Alpha - 90) * Math.PI / 180.0) -
                    variant.L2 * Math.Sin((angles.Beta - 90 - angles.Alpha + 90) * Math.PI / 180.0) +
                    pincers * Math.Sin((angles.Beta - 90 - angles.Alpha + 90 - angles.Gamma + 90) * Math.PI / 180.0);
        Console.WriteLine(length1);
        Console.WriteLine($"X: {xcor1} Y: {ycor1} Z: {zcor1}");
        xcor1 += 8.5;
        double angleOfVector1 = Math.Atan(ycor1 / xcor1) * (180 / (Math.PI)) + 90;
        xcor1 -= Math.Abs(pincers * Math.Cos((angleOfVector1 - 90) / (180 / Math.PI)));
        ycor1 = Math.Abs(ycor1) - Math.Abs(pincers * Math.Sin((angleOfVector1 - 90) / (180 / Math.PI)));
        
        var dy = Math.Sqrt(Math.Pow(xcor1, 2) + Math.Pow(ycor1, 2));
        var d = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(zcor1, 2));
        var q1 = Math.Atan(zcor1 / dy);
        var q2 = Math.Acos((Math.Pow(d, 2) + Math.Pow(10.4, 2) - Math.Pow(9.7, 2)) / (2 * d * 10.4));
        var q3 = Math.Acos((Math.Pow(10.4, 2) + 9.7 * 9.7 - Math.Pow(d, 2)) / (2 * 10.4 * 9.7));
        var secondAngle1 = (Math.PI - q3) * (180 / (Math.PI)) + 90;
        var firstAngle1 = (q1 + q2) * (180 / (Math.PI)) + 90;
        var thirdAngle1 = -firstAngle1 + secondAngle1 + 90;
        Console.WriteLine($"1: {firstAngle1} 2: {angleOfVector1} 3: {secondAngle1} 4: {thirdAngle1}");

        return new Angles
        {
            Alpha = Convert.ToSingle(firstAngle1),
            Beta = Convert.ToSingle(secondAngle1),
            Gamma = Convert.ToSingle(thirdAngle1),
            Theta = Convert.ToSingle(angleOfVector1),
            Omega = angles.Omega
        };
    }

    public static string Check(Angles angles, Variant variant) // l1, l2, X1, Y1, Z1, X2, Y2, Z2
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
        Console.WriteLine($"Вариант {variant.Id} {(Math.Abs(xcor1 - variant.X1) < tolerance && Math.Abs(ycor1 - variant.Y1) < tolerance && Math.Abs(zcor1 - variant.Z1) < tolerance)} X:{xcor1} Y:{ycor1} Z:{zcor1}");
        string check = (Math.Abs(xcor1 - variant.X1) < tolerance &&
                        Math.Abs(ycor1 - variant.Y1) < tolerance &&
                        Math.Abs(zcor1 - variant.Z1) < tolerance) + $" X:{xcor1} Y:{ycor1} Z:{zcor1} ";
        return check;
    }
}
