using System.IO.Ports;
using System.Windows;
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
        _serialPort.ReadTimeout = 18000;
        _serialPort.Open();
    }
    public async Task SendCommand(Command command, CancellationToken cancellationToken = default)
    {
        foreach (var angles in command.Angles)
        {
            Console.WriteLine(angles.ToCommandString());
            await SendAngles(angles);
        }
    }

    public Task SendAngles(Angles angles)
    {
        _serialPort.WriteLine(angles.ToCommandString());
        string answerMessage = angles.ToAnswerString().Replace("cor", "fin").Replace("srv", "fin");
        Console.WriteLine(answerMessage);
        try
        {
            var message = "0";
            do
            {
                message = _serialPort.ReadLine().Trim();
                //Console.WriteLine(message);
                if (answerMessage == message)
                {
                    Console.WriteLine("ok");
                    return Task.CompletedTask;
                }

                if (message.Length > 0)
                {
                    Console.WriteLine(string.Format("received from ardu: {0}", message));
                }
            } while (!message.Contains(answerMessage));
            Console.WriteLine("After while");
            throw new Exception("Неверный ответ");
        }
        catch (Exception e)
        {
            MessageBox.Show($"ardu read eaxception: {e.Message}" );
            return Task.CompletedTask;
        }
    }
}