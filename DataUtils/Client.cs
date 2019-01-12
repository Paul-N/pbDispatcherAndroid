//03.04.18 Авхадиев Рустем
//Обеспечивает взаимодействие с сервером (прием/отправка данных)

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;

using System.Diagnostics;
using System.Runtime.CompilerServices;


using System.Collections.Generic;

using DataUtils;
using System.Threading;

namespace DataUtils
{
    //-------------------------------------------------------------------------------------------------------------------------------------
    public enum RequestStates
    {
        None,
        InProgress,
        Completed,
        Failed
    }

    //-------------------------------------------------------------------------------------------------------------------------------------
    public enum ConnectStates
    {
        None,
        SocketConnected,
        AuthPassed
    }

    //-------------------------------------------------------------------------------------------------------------------------------------
    public static class Client
    {
        private static Socket socket;

        static public List<Server> servers = new List<Server>();
        static public Server currentServer;
        static public int currentServerNum = 0;


        private static int waitPacketTimeout = 10000;
        private static ByteBuffer socketTempBuffer = new ByteBuffer(Settings.maxPacketSize);
        private static ByteBuffer socketBuffer = new ByteBuffer(Settings.maxPacketSize);
        private static ByteBuffer packetBuffer = new ByteBuffer(Settings.maxPacketSize);
        private static ByteBuffer sendPacketBuffer = new ByteBuffer(Settings.maxPacketSize);

        public static ConnectStates connectState;

        private static bool packetAvalable = false;
        private static Stopwatch time = new Stopwatch();

        private static int bytesRec;
        private static int packetSize = 0;
        private static ushort packetCheckSum = 0;
        private static byte nextGetPacketNum = 0;
        private static byte nextSendPacketNum = 0;
        private static bool packetStarted = false;

        static public uint ping = 0;
        static public DateTime lastPingTime = DateTime.MinValue;
        static public string userName;
        static public string login = null;
        static public string password = null;

        public delegate void PacketProcessFunction(ByteBuffer buffer);
        public delegate void RequestParseCallback(ByteBuffer buffer, RequestViewCallback requestViewCallback);
        public delegate void RequestViewCallback(Object requestResult);

        private class Request
        {
            public ushort requestID;
            public RequestParseCallback requestParseCallback;
            public RequestViewCallback  requestViewCallback;

            public Request(ushort requestID, RequestParseCallback requestParseCallback, RequestViewCallback requestViewCallback)
            {
                this.requestID = requestID;
                this.requestParseCallback = requestParseCallback; 
                this.requestViewCallback  = requestViewCallback;
            }
        }

        static List<Request> requestsList = new List<Request>();
        static ushort lastRequestID = 0;

        private static Dictionary<ClientPacketTypes, PacketProcessFunction> PacketProccessor = new Dictionary<ClientPacketTypes, PacketProcessFunction>
        {
            {ClientPacketTypes.None, SendError },
            {ClientPacketTypes.RequestAnswer, ProcessRequestAnswer},
        };

        static public string userToken = null; //"ANXqVh70xrM59VC9OxqK3UZVP5HBDjlf8iVzjhTPygXAkUMJah";

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Init()
        {
            time.Start();

            servers.Clear();
            servers.Add(new Server("kvm-prod", "clientserver-prod.ifuture.su", new byte[4] { 95, 181, 230, 17 }, 81));
            servers.Add(new Server("kvm-dev", "clientserver-dev.ifuture.su", new byte[4] { 95, 183, 13, 1 }, 81));


            Client.currentServer = Client.servers[Client.currentServerNum];

            while (true)
            {
                ConnectToServer();

                if (connectState == ConnectStates.AuthPassed)
                    break;

                Thread.Sleep(1000);
            }

            Client.Log("Init client Token = '" + userToken + "'");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void Update()
        {
            ProcessReceive();

            if (!packetAvalable)
                return;

            packetBuffer.index = 0;
            ClientPacketTypes packetType = (ClientPacketTypes)packetBuffer.GetByte();

            PacketProccessor[packetType](packetBuffer);

            packetBuffer.Reset();
            packetAvalable = false;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ProcessReceive()
        {
            if ((socket == null) || (socket.Connected == false))
                return;

            if (socket.Available != 0)
            {
                bytesRec = socket.Receive(socketTempBuffer.data);

                Array.Copy(socketTempBuffer.data, 0, socketBuffer.data, socketBuffer.length, bytesRec);
                socketBuffer.length += bytesRec;

                //Client.Log (DateTime.Now + " socketTempBuffer. Size " + bytesRec + " socketBuffer.length " + socketBuffer.length + " : "+ BitConverter.ToString (socketTempBuffer.data, 0, Math.Min (50, bytesRec)));
            }


            if (packetAvalable)
                return;

            if (!packetStarted)
            {
                if (socketBuffer.length < (int)HeaderOffsets.FirstEnd)
                    return;

                packetSize = socketBuffer.GetInt();
                packetCheckSum = socketBuffer.GetUShort();
                packetStarted = true;
            }

            if (packetStarted)
            {
                if ((socketBuffer.length - socketBuffer.index) >= packetSize)
                {
                    if (packetCheckSum != socketBuffer.GetCRC16(packetSize))
                    {
                        Client.Log("Ошибка приема пакета. Контрольные суммы не совпали: " + packetCheckSum + " / " + socketBuffer.GetCRC16(packetSize), LogTypes.CriticalError);
                        return;
                    }

                    byte getPacketNum = socketBuffer.GetByte();
                    if (getPacketNum != nextGetPacketNum)
                    {
                        Client.Log("getPacketNum: " + getPacketNum + ".  nextGetPacketNum: " + nextGetPacketNum, LogTypes.CriticalError);
                        nextGetPacketNum = getPacketNum;
                    }
                    nextGetPacketNum++;

                    Array.Copy(socketBuffer.data, socketBuffer.index, packetBuffer.data, 0, packetSize - 1);
                    packetBuffer.index = 0;
                    packetBuffer.length = packetSize - 1;
                    packetAvalable = true;
                    packetStarted = false;

                    socketBuffer.index += packetSize - 1;
                    socketBuffer.Shift(socketBuffer.index);

                    //Client.Log (DateTime.Now.TimeOfDay + " Packet. Size " + packetSize + " : "+ BitConverter.ToString (packetBuffer.data, 0, packetSize) + "socketBuffer.length: " + socketBuffer.length);
                }


            }

        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ConnectToSocket(IPAddress ipAddr, int port)
        {
            try
            {
                if (socket != null)
                    if (socket.Connected == true)
                        socket.Close();

                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

                socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("socket.ReceiveBufferSize " + socket.ReceiveBufferSize);
                socket.ReceiveBufferSize = 3 * 1024 * 1024;
                Console.WriteLine("socket.ReceiveBufferSize 2 " + socket.ReceiveBufferSize);

                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = ipEndPoint;

                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
                {
                    if (e.SocketError != SocketError.Success)
                    {
                        Console.WriteLine("Ошибка подключения к серверу 1" + ipEndPoint + "  " + e.SocketError.ToString(), LogTypes.Error);
                    }
                });

                socket.ConnectAsync(socketEventArg);

                Stopwatch waitTimer = new Stopwatch();
                waitTimer.Start();

                while (true)
                {
                    if (waitTimer.ElapsedMilliseconds > 4000)
                    {
                        Console.WriteLine("Ошибка подключения к серверу 2" + ipEndPoint, LogTypes.Error);
                        break;
                    }

                    if (socket.Connected)
                    {
                        Console.WriteLine("Подключение к серверу успешно " + ipEndPoint);
                        connectState = ConnectStates.SocketConnected;
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ConnectToServer()
        {
            connectState = ConnectStates.None;
            lastRequestID = 0;
            requestsList.Clear();
            socketBuffer.Reset();
            userName = "";

            try
            {
                IPHostEntry serverMachineInfo = Dns.GetHostEntry(currentServer.dnsName);
                IPAddress ipAddr;

                for (int s = 0; s < serverMachineInfo.AddressList.Length; s++)
                {
                    ipAddr = serverMachineInfo.AddressList[s];

                    Console.WriteLine("Trying to connect by DNS '" + currentServer.dnsName + "', ipv4: " + ipAddr);

                    ConnectToSocket(ipAddr, currentServer.port);

                    if (socket.Connected == true)
                        break;
                }

                if (socket.Connected == false)
                    for (int s = 0; s < serverMachineInfo.AddressList.Length; s++)
                    {
                        ipAddr = serverMachineInfo.AddressList[s].MapToIPv6();

                        Console.WriteLine("Trying to connect by DNS '" + currentServer.dnsName + "', ipv6: " + ipAddr);

                        ConnectToSocket(ipAddr, currentServer.port);

                        if (socket.Connected == true)
                            break;
                    }

                if (socket.Connected == false)
                {
                    ipAddr = new IPAddress(currentServer.ipv4Address);

                    Console.WriteLine("Trying to connect ipv4: " + ipAddr);

                    ConnectToSocket(ipAddr, currentServer.port);
                }

                if (socket.Connected == false)
                    return;

                nextSendPacketNum = 0;
                nextGetPacketNum = 0;

                if (userToken != null)
                    SendAuthByTokenRequest(userToken, Login);
                else if (login != null)
                    SendAuthByLoginRequest(login, password, Login);
                else
                    socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SendAuthByTokenRequest(string _token, RequestParseCallback requestParseCallback)
        {
            sendPacketBuffer.Reset((int)HeaderOffsets.SecondEnd);

            sendPacketBuffer.Add((byte)ClientPacketTypes.Request);
            sendPacketBuffer.Add((byte)RequestTypes.AuthByToken);
            sendPacketBuffer.Add((ushort)Settings.clientVersion);
            sendPacketBuffer.AddShortString (_token);

            SendRequestToServer(requestParseCallback, null);

            WaitForPacket(ClientPacketTypes.RequestAnswer, waitPacketTimeout);

            Update();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SendAuthByLoginRequest(string login, string password, RequestParseCallback requestParseCallback)
        {
            sendPacketBuffer.Reset((int)HeaderOffsets.SecondEnd);

            sendPacketBuffer.Add((byte)ClientPacketTypes.Request);
            sendPacketBuffer.Add((byte)RequestTypes.AuthByLogin);
            sendPacketBuffer.Add((ushort)Settings.clientVersion);
            sendPacketBuffer.AddShortString(login);
            sendPacketBuffer.AddShortString(password);

            SendRequestToServer(requestParseCallback, null);

            WaitForPacket(ClientPacketTypes.RequestAnswer, waitPacketTimeout);

            Update();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SendRegularRequest(RequestTypes requestType, ByteBuffer requestBuffer, RequestParseCallback requestParseCallback, RequestViewCallback requestViewCallback)
        {
            if (requestType == RequestTypes.ReconnectToServer)
            {
                Init();
                requestViewCallback?.Invoke(RequestStates.Completed);
                return;
            }

            if ((DateTime.Now - lastPingTime).TotalMilliseconds > Settings.pingValueLifeTime * 3)
                Init();

            if ((connectState != ConnectStates.AuthPassed) && (userToken == null) && (login == null))
                return;

            sendPacketBuffer.Reset((int)HeaderOffsets.SecondEnd);

            sendPacketBuffer.Add((byte)ClientPacketTypes.Request);
            sendPacketBuffer.Add((byte)requestType);

            if (requestBuffer != null)
                sendPacketBuffer.AddBytes(requestBuffer, 0);

            SendRequestToServer(requestParseCallback, requestViewCallback);

            if (WaitForPacket(ClientPacketTypes.RequestAnswer, waitPacketTimeout) == false)
            {
                Console.WriteLine("socket.Connected: " + socket.Connected);
                socket.Disconnect(false);
                ConnectToServer();
            }

            Update();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SendRequestToServer(RequestParseCallback requestParseCallback, RequestViewCallback requestViewCallback)
        {
            ushort requestID = lastRequestID++;

            requestsList.Add(new Request(requestID, requestParseCallback, requestViewCallback));

            sendPacketBuffer.Add((ushort)requestID);
            sendPacketBuffer.Add((uint) time.ElapsedMilliseconds);

            SendBufferToServer();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Login(ByteBuffer buffer, RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if ((requestType != RequestTypes.AuthByLogin) && (requestType != RequestTypes.AuthByToken))
            { 
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                return;
            }

            connectState = ConnectStates.AuthPassed;

            ushort serverVersion = buffer.GetUShort();

            userName = buffer.GetShortString();

            if (requestType == RequestTypes.AuthByLogin)
            {
                userToken = buffer.GetShortString();
                Client.Log ("Get token: " + userToken);
            }

            if (Settings.serverVersion != serverVersion)
                Client.Log("Версии клиента и сервера различаются " + serverVersion + " - " + Settings.serverVersion + ". Обновите клиент", LogTypes.Error);

            Client.Log("Auth success. Server version: " + serverVersion);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void DisconnectFromServer()
        {
            socket.Close();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static private void SendBufferToServer()
        {
            try
            {
                if (!socket.Connected)
                {
                    ConnectToServer();
                    Console.WriteLine("Переподключаем разорванное соединение...");

                    if (!socket.Connected)
                    {
                        Console.WriteLine("Подключение неуспешно!");
                        return;
                    }
                }

                sendPacketBuffer.index = (int) HeaderOffsets.PacketNum;
                sendPacketBuffer.Rewrite(nextSendPacketNum);

                //ushort checkSum = sendPacketBuffer.GetCRC16((int)HeaderOffsets.FirstEnd);

                sendPacketBuffer.index = (int)HeaderOffsets.FirstEnd;
                ushort checkSum = sendPacketBuffer.GetCRC16(sendPacketBuffer.length - sendPacketBuffer.index);
                sendPacketBuffer.index = 0;
                sendPacketBuffer.Rewrite((int)(sendPacketBuffer.length - (int)HeaderOffsets.FirstEnd));
                sendPacketBuffer.Rewrite(checkSum);


                Console.WriteLine ("Send: " + BitConverter.ToString (sendPacketBuffer.data, 0, sendPacketBuffer.length));

                socket.Send(sendPacketBuffer.data, 0, sendPacketBuffer.length, SocketFlags.None);
                nextSendPacketNum++;
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + ex.ToString());
            }
        }

        //Блокирует приложение до получения с сервера пакета определенного типа, уничтожает все пришедшие пакеты другого типа---------------------------------------------------------------------------------------------------------------------------------------------------------
        private static bool WaitForPacket(ClientPacketTypes packetType, long maxWaitTime)
        {
            Stopwatch waitTimer = new Stopwatch();
            waitTimer.Start();

            while (true)
            {
                if (socketBuffer.length == 0)
                {
                    if (waitTimer.ElapsedMilliseconds > maxWaitTime)
                    {
                        Client.Log("Ответ от сервера не пришел (socketBuffer.length == 0) " + packetType, LogTypes.Error, LogParts.UserApp);
                        return false;
                    }
                }
                else
                {
                    if (waitTimer.ElapsedMilliseconds > maxWaitTime * 2)
                    {
                        Client.Log("Ответ от сервера не пришел (socketBuffer.length == " + socketBuffer.length + " " + packetType, LogTypes.Error, LogParts.UserApp);
                        return false;
                    }
                }

                ProcessReceive();

                if (!packetAvalable)
                    continue;

                packetBuffer.index = 0;

                ClientPacketTypes receivePacketType = (ClientPacketTypes)packetBuffer.GetByte();
                if (packetType == receivePacketType)
                    return true;
                else
                    Client.Log ("Пропущен пакет " + (ClientPacketTypes) receivePacketType, LogTypes.None, LogParts.UserApp);

                packetBuffer.Reset();
                packetAvalable = false;
            }

        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SendError(ByteBuffer buffer)
        {
            Log("Error - None packet", LogTypes.CriticalError);
        }

        //Отправляет сообщение в лог на сервер---------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Log(string text, LogTypes logType = LogTypes.None, LogParts logPart = LogParts.UserApp)
        {
            Console.WriteLine(text);

            if (connectState != ConnectStates.AuthPassed)
                return;

            sendPacketBuffer.Reset((int)HeaderOffsets.SecondEnd);

            sendPacketBuffer.Add((byte)ClientPacketTypes.Log);
            sendPacketBuffer.Add((byte)logPart);
            sendPacketBuffer.Add((byte)logType);
            sendPacketBuffer.AddLongString(text);

            SendBufferToServer();

        }

        //Отправляет сообщение об ошибке в лог на сервер---------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void LogErr(string message = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            if (message == "")
                message = "Error";
            //Console.WriteLine(DateTime.Now + "." + message + " in [" + caller + "] [" + lineNumber + "]");

            Log (DateTime.Now + "." + message + " in [" + caller + "] [" + lineNumber + "]", LogTypes.Error);
        }

        //Обрабатывает ответ сервера на ранее поданный запрос-----------------------------------------------------------------------------------------------------------------------------------
        public static void ProcessRequestAnswer(ByteBuffer buffer)
        {
            ushort requestID = buffer.GetUShort();
            ping = (uint) time.ElapsedMilliseconds - buffer.GetUInt();
            lastPingTime = DateTime.Now;

            Console.WriteLine ("ProcessRequestAnswer " + requestID + ". ping: " + ping);

            int requestIndex = requestsList.FindIndex ((obj) => obj.requestID == requestID);

            if (requestIndex < 0)
            {
                Client.Log("Ошибка с запросом к серверу № " + requestID, LogTypes.Error);
                RequestTypes requestType = (RequestTypes)buffer.GetByte();
                Client.Log("requestType: " + requestType.ToString() + " size: " + buffer.length, LogTypes.Error);

                return;
            }
            requestsList [requestIndex].requestParseCallback(buffer, requestsList[requestIndex].requestViewCallback);

            requestsList.RemoveAt(requestIndex);

        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int NotAnsweredRequestsCount
        {
            get { return requestsList.Count; }
        }
    }
}
