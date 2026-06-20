using System;
using System.Net;
using System.Net.Sockets;

namespace StonedAges;

public class ClientSocket
{
    private string localIP = "192.168.0.2";

    private ushort _bufSize = ushort.MaxValue;

    private bool localattempt;

    public GameState _gamestate;

    public Socket _socket;

    private byte[] _buffer;

    public ClientSocket()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public void Connect(string ipAddress, int port)
    {
        try
        {
            _socket.BeginConnect(new IPEndPoint(Dns.GetHostEntry(ipAddress).AddressList[0], port), ConnectCallback, null);
        }
        catch
        {
        }
    }

    private void ConnectCallback(IAsyncResult result)
    {
        if (_socket.Connected)
        {
            _socket.EndConnect(result);
            GameWindow.ConnectedToServer = true;
            Console.WriteLine("Connection successful.");
            _buffer = new byte[_bufSize];
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceivedCallback, _socket);
            byte[] array = new byte[4];
            byte[] bytes = BitConverter.GetBytes((ushort)array.Length);
            byte[] bytes2 = BitConverter.GetBytes((ushort)1);
            Array.Copy(bytes, array, 2);
            Array.Copy(bytes2, 0, array, 2, 2);
            _socket.Send(array);
        }
        else if (!localattempt)
        {
            localattempt = true;
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(localIP), GameWindow._port), ConnectCallback, null);
        }
    }

    private void ReceivedCallback(IAsyncResult result)
    {
        try
        {
            int num = _socket.EndReceive(result);
            byte[] array = new byte[num];
            Array.Copy(_buffer, array, array.Length);
            PacketHandler.Handle(array, _socket, _gamestate);
            _buffer = new byte[_bufSize];
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceivedCallback, _socket);
        }
        catch
        {
            _gamestate.SystemMsg("The connection with the server has been lost.", 3);
            GameWindow.ConnectedToServer = false;
        }
    }

    public void Send(byte[] data)
    {
        try
        {
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, _socket);
        }
        catch (SocketException ex)
        {
            if (ex.SocketErrorCode == SocketError.WouldBlock || ex.SocketErrorCode == SocketError.IOPending || ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
            {
                Console.WriteLine(ex.SocketErrorCode);
            }
        }
    }

    private void SendCallback(IAsyncResult result)
    {
        Socket socket = (Socket)result.AsyncState;
        socket.EndSend(result);
    }
}
