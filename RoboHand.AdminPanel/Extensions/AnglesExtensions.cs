using WpfApp23.Models;

namespace WpfApp23.Extensions;

public static class AnglesExtensions
{
    public static string ToCommandString(this Angles angles)
    {
        return angles is { Alpha: 0, Beta: 0, Gamma: 0, Theta: 0 } ? $"srv 5 {angles.Omega}" : $"cor {Math.Round(angles.Alpha, 2)} { Math.Round(angles.Beta, 2)} {Math.Round(angles.Gamma, 2)} {Math.Round(angles.Theta, 2)}";
    }

    public static string ToAnswerString(this Angles angles)
    {
        return angles is { Alpha: 0, Beta: 0, Gamma: 0, Theta: 0 } ? $"srv 5 {Math.Round(angles.Omega, 0)}" : $"cor {Math.Round(angles.Alpha)} {Math.Round(angles.Beta)} {Math.Round(angles.Gamma)} {Math.Round(angles.Theta)}";
    }
}