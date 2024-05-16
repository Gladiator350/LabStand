using WpfApp23.Models;

namespace WpfApp23.Services.Interfaces;

public interface IArduinoService
{
    Task SendCommand(Command command, CancellationToken cancellationToken = default);
    Task SendAngles(Angles angles);
}