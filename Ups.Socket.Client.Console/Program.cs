using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Socket Client \n");

IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("127.0.0.1");
IPAddress ipAddress = ipHostInfo.AddressList[0];

IPEndPoint ipEndPoint = new(ipAddress, 52296);

using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);
while (true)
{

    Console.WriteLine("Ingrese operacion a realizar");
    Console.WriteLine("---------------------------------------");
    Console.WriteLine("S => Suma");
    Console.WriteLine("R => Restar");
    Console.WriteLine("M => Multiplicar");
    Console.WriteLine("D => Dividir");
    Console.WriteLine("Q => Salir");
    Console.WriteLine("---------------------------------------");

    var input = Console.ReadLine()?.ToUpper();

    if (input.Equals("Q"))
        Environment.Exit(1);

    Console.WriteLine("-- Ingrese Primer Numero:");
    var firstNumber = Convert.ToInt16(Console.ReadLine());
    Console.WriteLine("-- Ingrese Segundo Numero:");
    var secondNumber = Convert.ToInt16(Console.ReadLine());

    var resultOperation = 0;
    var typeOperation = string.Empty;
    var charTypeOperation = string.Empty;

    switch (input)
    {
        case "S":
            resultOperation = Math.Abs(firstNumber + secondNumber);
            typeOperation = "Suma";
            charTypeOperation = "+";
            break;
        case "R":
            resultOperation = Math.Abs(firstNumber - secondNumber);
            typeOperation = "Resta";
            charTypeOperation = "-";
            break;
        case "M":
            resultOperation = Math.Abs(firstNumber * secondNumber);
            typeOperation = "Multiplicacion";
            charTypeOperation = "*";
            break;
        case "D":
            resultOperation = Math.Abs(firstNumber / secondNumber);
            typeOperation = "Division";
            charTypeOperation = "/";
            break;
        default:
            Environment.Exit(1);
            break;
    }

    // Send message.
    var message = $"Resultado de Operacion {typeOperation} ({firstNumber} {charTypeOperation} {secondNumber}) = {resultOperation} <|EOM|>";
    var messageBytes = Encoding.UTF8.GetBytes(message);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);
    //Console.WriteLine($"Socket client sent message: \"{message}\"");

    // Receive ack.
    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    if (response == "<|ACK|>")
    {
        Console.WriteLine("\n--------------------------------------------------");
        Console.WriteLine(
            $"Socket client received acknowledgment: \"{response}\"");
        Console.WriteLine("----------------------------------------------------\n");
    }
}

client.Shutdown(SocketShutdown.Both);
