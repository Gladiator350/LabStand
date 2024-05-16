using WpfApp23.Models;

namespace WpfApp23.Calculate;

public class AnglesCalculator
{
        Angles Recalculate(Angles angles, Variant variant)// l1, l2, X1, Y1, Z1, X2, Y2, Z2
        {
            const int pincers = 11;
            var length1 = variant.L1 * Math.Cos(angles.Alpha) + variant.L2 * Math.Cos(angles.Alpha + angles.Beta) + pincers * Math.Cos(angles.Alpha + angles.Beta + angles.Gamma);
            var xcoor1 = length1 * Math.Cos(angles.Theta);
            var ycoor1 = length1 * Math.Sin(angles.Theta);
            var zcoor1 = (variant.L1 * Math.Sin(angles.Alpha) + variant.L2 * Math.Sin(angles.Alpha + angles.Beta) + pincers * Math.Sin(angles.Alpha + angles.Beta + angles.Gamma));
            xcoor1 = xcoor1 + 8.5;
            var angleOfVector1 = Math.Atan(ycoor1 / xcoor1) * (180 / (Math.PI));
            xcoor1 -= Math.Abs(11 * Math.Cos(angleOfVector1 / (180 / Math.PI)));
            ycoor1 = Math.Abs(ycoor1) - Math.Abs(11 * Math.Sin(angleOfVector1 / (180 / Math.PI)));
            var dy = Math.Sqrt(Math.Pow(xcoor1, 2) + Math.Pow(ycoor1, 2));
            var d = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(zcoor1, 2));
            var q1 = Math.Atan(zcoor1 / dy);
            var q2 = Math.Acos((Math.Pow(d, 2) + Math.Pow(10.4, 2) - Math.Pow(9.7, 2)) / (2 * d * 10.4));
            var q3 = Math.Acos((Math.Pow(10.4, 2) + 9.7 * 9.7 - Math.Pow(d, 2)) / (2 * 10.4 * 9.7));
            var secondAngle1 = (Math.PI - q3) * (180 / (Math.PI));
            var firstAngle1 = (q1 + q2) * (180 / (Math.PI));
            var thirdAngle1 = -firstAngle1 + secondAngle1;
        
            return new Angles
            {
                Alpha = Convert.ToSingle(firstAngle1),
                Beta = Convert.ToSingle(secondAngle1),
                Gamma = Convert.ToSingle(thirdAngle1),
                Theta = Convert.ToSingle(angleOfVector1),
                Omega = angles.Omega
            };
        }
}