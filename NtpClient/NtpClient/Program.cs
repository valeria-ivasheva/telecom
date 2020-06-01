using System;
using System.Net;
using System.Net.Sockets;

namespace NtpClient
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Дата и время:");
      var temp = GetNetworkTime();
      Console.WriteLine(temp);
    }

    public static DateTime GetNetworkTime()
    {
      const string ntpServer = "time.windows.com";
      var ntpData = new byte[48];
      ntpData[0] = 0x1B;

      var addresses = Dns.GetHostEntry(ntpServer).AddressList;
      var ipEndPoint = new IPEndPoint(addresses[0], 123);

      using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
      {
        socket.Connect(ipEndPoint);
        socket.Send(ntpData);
        socket.Receive(ntpData);
        socket.Close();
      }

      var intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
      var fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];

      var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
      var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

      return networkDateTime.ToLocalTime();
    }
  }
}

