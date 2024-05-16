using System.IO.Ports;
using Microsoft.Extensions.Options;
using WpfApp23.Extensions;
using WpfApp23.Models;
using WpfApp23.Services.Interfaces;

namespace WpfApp23.Services;

public class ArduinoService : IArduinoService
{
    private readonly SerialPort _serialPort;

    public ArduinoService(IOptions<ArduinoSettings> options)
    {
        _serialPort = new SerialPort(options.Value.Port, options.Value.BaudRate);
        _serialPort.ReadTimeout = 2000;
        _serialPort.Open();
    }
    public Task SendCommand(Command command, CancellationToken cancellationToken = default)
    {
        return SendAngles(command.Angles);
    }

    public Task SendAngles(Angles angles)
    {
    _serialPort.WriteLine(angles.ToCommandString());
    string answerMessage = angles.ToCommandString().Replace("cor", "fin");
    try
    {
        var message = _serialPort.ReadLine().Trim();
        if (answerMessage == message)
        {
            return Task.CompletedTask;
        }
    
        throw new Exception("Неверный ответ");
    }
    catch (Exception e)
    {
        return Task.FromException(e);
    }
    }
}