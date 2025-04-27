using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;

using System.Text.Json;


public class TSSCommunicator : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;

    // Properties to store the latest received data

    // public class TSSData {
    //     public uint Timestamp { get; set; }
    //     public uint CommandNumber { get; set; }
    //     public float OutputData { get; set; }
    // }
    public uint LastTimestamp { get; private set; }
    public uint LastCommandNumber { get; private set; }
    public float LastOutputData { get; private set; }
    public bool HasNewData { get; private set; }

    // Configuration
    public string tssIPAddress = "127.0.0.1"; // Replace with the TSS IP address
    public int tssPort = 14141;              // TSS port
    public int localPort = 14142;            // Local port for receiving responses

    void Start()
    {
        // Initialize UDP client and start the receive thread
        udpClient = new UdpClient(localPort);
        udpClient.EnableBroadcast = true;

        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("TSS Communicator initialized.");
    }

    void Update()
    {
        // Example: Send a command when pressing the Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            uint timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            uint commandNumber = 17; // Replace with your desired command number
            float inputData = 0f; // Replace with your input data

            SendCommand(timestamp, commandNumber, inputData);
        }
    }

    public void setHasNewDataFalse(){
        HasNewData = false;
    }
 

    private void ReceiveData()
    {
        try
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);

                if (data.Length >= 12) // Ensure the response is at least 12 bytes
                {
                    byte[] timestampBytes = new byte[4];
                    byte[] commandBytes = new byte[4];
                    byte[] outputDataBytes = new byte[4];

                    Array.Copy(data, 0, timestampBytes, 0, 4);
                    Array.Copy(data, 4, commandBytes, 0, 4);
                    Array.Copy(data, 8, outputDataBytes, 0, 4);

                    // Convert from big-endian if necessary
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(timestampBytes);
                        Array.Reverse(commandBytes);
                        Array.Reverse(outputDataBytes);
                    }

                    LastTimestamp = BitConverter.ToUInt32(timestampBytes, 0);
                    LastCommandNumber = BitConverter.ToUInt32(commandBytes, 0);
                    LastOutputData = BitConverter.ToSingle(outputDataBytes, 0);
                    HasNewData = true;

                    //Debug.Log($"Received Response - Timestamp: {LastTimestamp}, Command: {LastCommandNumber}, Output Data: {LastOutputData}");
                }
                else
                {
                    Debug.LogWarning("Received incomplete response.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in ReceiveData: {e.Message}");
        }
    }

    public async Task SendCommand(uint timestamp, uint commandNumber, float inputData)
    {
        try
        {
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);
            byte[] commandBytes = BitConverter.GetBytes(commandNumber);
            byte[] inputDataBytes = BitConverter.GetBytes(inputData);

            // Convert to big-endian if necessary (Unity uses little-endian by default)
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
                Array.Reverse(commandBytes);
                Array.Reverse(inputDataBytes);
            }

            byte[] packet = new byte[12];
            Array.Copy(timestampBytes, 0, packet, 0, 4);
            Array.Copy(commandBytes, 0, packet, 4, 4);
            Array.Copy(inputDataBytes, 0, packet, 8, 4);

            IPEndPoint tssEndPoint = new IPEndPoint(IPAddress.Parse(tssIPAddress), tssPort);
            await udpClient.SendAsync(packet, packet.Length, tssEndPoint);

            //Debug.Log($"Sent Command - Timestamp: {timestamp}, Command: {commandNumber}, Input Data: {inputData}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in SendCommand: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        // Clean up resources on exit
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }

        udpClient?.Close();
    }
}