using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class RockAnalyzer : MonoBehaviour
{
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 14141;

    private const int POST_COMMAND = 1103;
    private const int COMMAND_START = 27;

    private static readonly string[] propertyOrder = new string[]
    {
        "SiO2", "TiO2", "Al203", "FeO", "MnO",
        "MgO", "CaO", "K2O", "P2O3", "Other"
    };

    public Dictionary<string, float> AnalyzeRock()
    {
        SelectRockEva1();
        Thread.Sleep(500); // Slight delay like Python version

        var results = new Dictionary<string, float>();

        for (int i = 0; i < propertyOrder.Length; i++)
        {
            float? value = SendGetRequest(COMMAND_START + i);
            if (value.HasValue)
            {
                results[propertyOrder[i]] = (float)Math.Round(value.Value, 4);
            }
        }

        return results;
    }

    private void SelectRockEva1()
    {
        using (UdpClient client = new UdpClient())
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT);

            byte[] packet = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0)), 0, packet, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(POST_COMMAND)), 0, packet, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(BitConverter.GetBytes(0f), 0))), 0, packet, 8, 4);

            client.Send(packet, packet.Length, endpoint);
        }
    }

    private float? SendGetRequest(int commandId)
    {
        using (UdpClient client = new UdpClient())
        {
            client.Client.ReceiveTimeout = 2000; // 2 seconds
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT);

            byte[] packet = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0)), 0, packet, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(commandId)), 0, packet, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0)), 0, packet, 8, 4);

            client.Send(packet, packet.Length, endpoint);

            try
            {
                byte[] response = client.Receive(ref endpoint);
                if (response.Length >= 12)
                {
                    byte[] floatBytes = new byte[4];
                    Array.Copy(response, 8, floatBytes, 0, 4);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(floatBytes);

                    return BitConverter.ToSingle(floatBytes, 0);
                }
                else
                {
                    Debug.LogWarning($"Incomplete response for command {commandId}");
                    return null;
                }
            }
            catch (SocketException)
            {
                Debug.LogWarning($" No response for command {commandId}");
                return null;
            }
        }
    }
}
