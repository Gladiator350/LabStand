using WpfApp23.Models;

namespace WpfApp23.Extensions;

public static class AnglesExtensions
{
    public static string ToCommandString(this Angles angles)
    {
        return $"cor {angles.Alpha} {angles.Beta} {angles.Gamma} {angles.Theta}";
    }
}