using WpfApp23.Models;

namespace WpfApp23.Extensions;

public static class AnglesExtensions
{
    public static string ToCommandString(this Angles angles)
    {
        return angles is { Alpha: 0, Beta: 0, Gamma: 0, Theta: 0 } ? $"srv 5 {angles.Omega}" : $"cor {angles.Alpha} {angles.Beta} {angles.Gamma} {angles.Theta}";
    }
}