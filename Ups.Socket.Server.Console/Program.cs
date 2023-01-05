using System.Net.Sockets;
using System.Net;
using System.Text;

Console.WriteLine("Socket Server \n");

IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("127.0.0.1");
IPAddress ipAddress = ipHostInfo.AddressList[0];

IPEndPoint ipEndPoint = new(ipAddress, 52296);


using Socket listener = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

var handler = await listener.AcceptAsync();
while (true)
{
    try
    {
        // Receive message.
        var buffer = new byte[1_024];
        var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer, 0, received);

        var eom = "<|EOM|>";
        if (response.IndexOf(eom) > -1 /* is end of message */)
        {
            Console.WriteLine(
                $"Socket server recibio mensaje: \"{response.Replace(eom, "").Trim()}\"");

            var ackMessage = "<|ACK|>";
            var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            await handler.SendAsync(echoBytes, 0);
            //Console.WriteLine(
            //    $"Socket server envia acknowledgment: \"{ackMessage}\"");

        }
    }
    catch (Exception)
    {
        Console.WriteLine("Se Perdio Conexion");
        Environment.Exit(1);
    }
}
