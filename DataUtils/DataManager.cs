using System;
using System.Collections.Generic;
using System.Threading;

using DataUtils;

namespace DataUtils
{
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static class DataManager
    {
        static public Dictionary<ushort, MachineType> machineTypes = new Dictionary<ushort, MachineType>();
        static public Dictionary<ushort, MachineState> machineStates = new Dictionary<ushort, MachineState>();
        static public Dictionary<ushort, MachineServiceState> machineServiceStates = new Dictionary<ushort, MachineServiceState>();
        static public Dictionary<ushort, SensorType> sensorTypes = new Dictionary<ushort, SensorType>();

        static public Dictionary<ushort, Division> divisions = new Dictionary<ushort, Division>();
        static public Dictionary<ushort, City> cities = new Dictionary<ushort, City>();
        static public List<Machine> machines = new List<Machine>();

        static public City selectedCity;
        static public Division selectedDivision;
        static public MachineType selectedMachineType;
        static public MachineState selectedMachineState;
        static public MachineServiceState selectedMachineServiceState;
        static public object filterToSelect;
        static public bool onlyMyMachines;
        static public ushort storedCityID;
        static public ushort storedDivisionID;
        static public ushort storedMachineTypeID;
        static public ushort storedMachineStateID;
        static public ushort storedMachineServiceStateID;

        static public List<Node> nodes = new List<Node>();
        static private List<Node> tempNodes = new List<Node>();

        static public List<NodeFilter> nodesFilters = new List<NodeFilter>();

        static public List<NodePacket> nodePackets = new List<NodePacket>();
        static public SendToNodePacketState sendToNodePacketState;

        static public string deviceToken;
        static public UserSettings userSettings;
        static public int problemsCount = 0;
        static public int selectedSensor;
        static public ushort selectedNodesFilter = 0;
        static public ViewTypes currentView;

        static public DateTime unixEpoch = new DateTime (1970, 1, 1, 0, 0, 0);
        static public bool pbAdminFlag = false;

        public struct Request
        {
            public RequestTypes requestType;
            public ByteBuffer requestBuffer;
            public Client.RequestParseCallback requestParseCallback;
            public Client.RequestViewCallback  requestViewCallback;

            public Request(RequestTypes requestType, ByteBuffer requestBuffer, Client.RequestParseCallback requestParseCallback, Client.RequestViewCallback requestViewCallback)
            {
                this.requestType = requestType;
                this.requestBuffer = new ByteBuffer();
                if (requestBuffer != null)
                    this.requestBuffer.AddBytes(requestBuffer, 0);
                this.requestParseCallback = requestParseCallback;
                this.requestViewCallback  = requestViewCallback;
            }
        }


        static private List<Request> requestsQueue = new List<Request>();

        static private AutoResetEvent updateEvent = new AutoResetEvent(true);

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Init()
        {
            Client.Init();

            if (pbAdminFlag == false)
            {
                SheduleGetUserSettingsRequest(null);
                SheduleGetCommonTypesRequest(null);
                SheduleGetDivisionsRequest(null);
            }

            while (true)
            {
                updateEvent.WaitOne(Settings.updatePeriodMs);

                if (requestsQueue.Count != 0)
                {
                    SendRequest(requestsQueue[0]);
                    requestsQueue.RemoveAt(0);

                    if (requestsQueue.Count != 0)
                        UpdateNow();
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SendRequest(Request request)
        {
            Client.SendRegularRequest(request.requestType, request.requestBuffer, request.requestParseCallback, request.requestViewCallback);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public uint Ping 
        {
            get 
            {
                if (DateTime.Now.Subtract(Client.lastPingTime).TotalMilliseconds < Settings.pingValueLifeTime)
                    return Client.ping;
                else
                    return 0;

            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public int NotAnsweredRequestsCount 
        {
            get
            { return Client.NotAnsweredRequestsCount; }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public ConnectStates ConnectState
        {
            get
            { return Client.connectState; }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SwitchToNextServer()
        {
            Client.currentServerNum++;
            if (Client.currentServerNum >= Client.servers.Count)
                Client.currentServerNum = 0;

            Client.currentServer = Client.servers[Client.currentServerNum];
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public int CurrentServerNum
        {
            get
            { return Client.currentServerNum; }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleNodeListRequest (ushort group, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((ushort)group);

            requestsQueue.Add(new Request(RequestTypes.NodesList, requestBuffer, ParseNodesList, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleNodeFiltersRequest(Client.RequestViewCallback requestViewCallback)
        {
            requestsQueue.Add(new Request(RequestTypes.NodeFiltersList, null, ParseNodeFiltersList, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetUserSettingsRequest(Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();

            requestsQueue.Add(new Request(RequestTypes.GetUserSettings, requestBuffer, ParseGetUserSettings, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSetUserSettingsRequest(DateTime notificationsFrom, DateTime notificationsTo, bool sendNodesOfflineNotifications, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();

            if (notificationsFrom.ToUniversalTime() < unixEpoch)
                notificationsFrom = notificationsFrom.AddDays(1);
            if (notificationsTo.ToUniversalTime() < unixEpoch)
                notificationsTo = notificationsTo.AddDays(1);
            
            requestBuffer.Add((DateTime) notificationsFrom.ToUniversalTime());
            requestBuffer.Add((DateTime)notificationsTo.ToUniversalTime());
            requestBuffer.Add((bool)sendNodesOfflineNotifications);

            requestsQueue.Add(new Request(RequestTypes.SetUserSettings, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSetNodeSettingsRequest(uint nodeID, string nodeName, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddUInt3(nodeID);
            requestBuffer.AddShortString(nodeName);

            requestsQueue.Add(new Request(RequestTypes.SetNodeSettings, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSetNodePendingConnectionSettingsRequest(uint nodeID, ulong bridgeMac, uint password, byte nodeIndexOnBridge, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddUInt3(nodeID);
            requestBuffer.AddULong6(bridgeMac);
            requestBuffer.Add((uint)password);
            requestBuffer.Add((byte)nodeIndexOnBridge);

            requestsQueue.Add(new Request(RequestTypes.SetNodePendingConnectionSettings, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSendNodePacketRequest(uint nodeID, ByteBuffer packet, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddUInt3(nodeID);
            requestBuffer.AddBytes(packet, 0);

            requestsQueue.Add(new Request(RequestTypes.SendNodePacket, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleNodeLogRequest(uint nodeID, int limitFrom, int limitTo, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddUInt3(nodeID);
            requestBuffer.Add((int)limitFrom);
            requestBuffer.Add((int)limitTo);

            requestsQueue.Add(new Request(RequestTypes.NodeLog, requestBuffer, ParseNodeLog, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSendToNodePacketStateRequest(uint nodeID, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddUInt3(nodeID);

            requestsQueue.Add(new Request(RequestTypes.SendToNodePacketState, requestBuffer, ParseSendToNodePacketState, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleDeleteItemRequest(ControllerItemTypes itemType, ulong itemID, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((byte) itemType);
            requestBuffer.AddULong6(itemID);

            requestsQueue.Add(new Request(RequestTypes.DeleteItem, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetNodeValuesRequest(uint nodeID, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddUInt3(nodeID);

            requestsQueue.Add(new Request(RequestTypes.GetNodeValues, requestBuffer, ParseNodeValues, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSetNodeValuesRequest(uint nodeID, ByteBuffer valuesBuffer, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddUInt3(nodeID);
            requestBuffer.AddBytesWithSize(valuesBuffer, 0);

            requestsQueue.Add(new Request(RequestTypes.SetNodeValues, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSetBridgeSettingsRequest(ulong bridgeMac, string bridgeName, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddULong6(bridgeMac);
            requestBuffer.AddShortString(bridgeName);

            requestsQueue.Add(new Request(RequestTypes.SetBridgeSettings, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSendToBridgePacketStateRequest(ulong bridgeMac, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddULong6(bridgeMac);

            requestsQueue.Add(new Request(RequestTypes.SendToBridgePacketState, requestBuffer, ParseSendToBridgePacketState, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSendBridgePacketRequest(ulong bridgeMac, BridgeParts bridgePart, ByteBuffer packet, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddULong6(bridgeMac);
            requestBuffer.Add((byte) bridgePart);
            requestBuffer.AddBytes(packet, 0);

            requestsQueue.Add(new Request(RequestTypes.SendBridgePacket, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void ShedulePacketLogRequest(ulong bridgeMac, int limitFrom, int limitTo, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddULong6(bridgeMac);
            requestBuffer.Add((int)limitFrom);
            requestBuffer.Add((int)limitTo);

            requestsQueue.Add(new Request(RequestTypes.PacketLog, requestBuffer, ParsePacketLog, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetCommonTypesRequest(Client.RequestViewCallback requestViewCallback)
        {
            requestsQueue.Add(new Request(RequestTypes.GetCommonTypes, null, ParseGetCommonTypes, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetDivisionsRequest(Client.RequestViewCallback requestViewCallback)
        {
            requestsQueue.Add(new Request(RequestTypes.GetDivisions, null, ParseGetDivisions, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetMachinesRequest(Client.RequestViewCallback requestViewCallback)
        {
            if (machineTypes.Count == 0)
                return;

            ushort filterCityID = 0;
            if (selectedCity != null)
                filterCityID = (ushort)selectedCity.ID;
            
            ushort filterDivisionID = 0;
            if (selectedDivision != null)
                filterDivisionID = selectedDivision.ID;

            ushort filterMachineTypeCode = 0;
            if (selectedMachineType != null)
                filterMachineTypeCode = (ushort)selectedMachineType.code;

            ushort filterMachineStateCode = 0;
            if (selectedMachineState != null)
                filterMachineStateCode = (ushort)selectedMachineState.code;

            ushort filterMachineServiceStateCode = 0;
            if (selectedMachineServiceState != null)
                filterMachineServiceStateCode = (ushort)selectedMachineServiceState.code;
            
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((ushort)filterCityID);
            requestBuffer.Add((ushort)filterDivisionID);
            requestBuffer.Add((ushort)filterMachineTypeCode);
            requestBuffer.Add((ushort)filterMachineStateCode);
            requestBuffer.Add((ushort)filterMachineServiceStateCode);
            requestBuffer.Add((bool)onlyMyMachines);

            requestsQueue.Add(new Request(RequestTypes.GetMachines, requestBuffer, ParseGetMachines, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetMachineStatesLogRequest(Machine machine, ushort maxElements, Client.RequestViewCallback requestViewCallback)
        {
            if (machine == null)
                return;

            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((uint)machine.ID);
            requestBuffer.Add((ushort)maxElements);

            requestsQueue.Add(new Request(RequestTypes.GetMachineStatesLog, requestBuffer, ParseGetMachineStatesLog, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSetMachineServiceStateRequest(Machine machine, MachineServiceStateCodes state, string comment, Client.RequestViewCallback requestViewCallback)
        {
            if (machine == null)
                return;

            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((uint)machine.ID);
            requestBuffer.Add((ushort)state);
            requestBuffer.AddShortString(comment);

            requestsQueue.Add(new Request(RequestTypes.SetMachineServiceState, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetSensorHistoryDataRequest(Sensor sensor, byte valueArrayIndex, DateTime timeStart, DateTime timeEnd, ushort pointsCount, Client.RequestViewCallback requestViewCallback)
        {
            if (sensor == null)
                return;

            if (timeStart < unixEpoch)
                return;

            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((uint)sensor.ID);
            requestBuffer.Add((byte)valueArrayIndex);
            requestBuffer.Add((DateTime)timeStart.ToUniversalTime());
            requestBuffer.Add((DateTime)timeEnd.ToUniversalTime());
            requestBuffer.Add((ushort)pointsCount);

            requestsQueue.Add(new Request(RequestTypes.GetSensorHistoryData, requestBuffer, ParseGetSensorHistoryData, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetSensorBordersRequest(Sensor sensor, Client.RequestViewCallback requestViewCallback)
        {
            if (sensor == null)
                return;

            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((uint)sensor.ID);

            requestsQueue.Add(new Request(RequestTypes.GetSensorBorders, requestBuffer, ParseGetSensorBorders, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSetSensorBordersRequest(Sensor sensor, SensorBordersList sensorBorders, Client.RequestViewCallback requestViewCallback)
        {
            if ((sensor == null) || (sensorBorders == null))
                return;

            if (sensorBorders.list.Count == 0)
                return;

            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((uint)sensor.ID);

            ByteBuffer valuesBuffer = new ByteBuffer();

            valuesBuffer.Add((int)sensorBorders.maxSecondsNotOkValue);

            valuesBuffer.Add((ushort)sensorBorders.list.Count);
            foreach (var item in sensorBorders.list)
            {
                valuesBuffer.Add((byte)item.type);  
                valuesBuffer.Add((double)item.minValue);   
                valuesBuffer.Add((double)item.maxValue); 
            }

            requestBuffer.AddBytesWithSize(valuesBuffer, 0);

            requestsQueue.Add(new Request(RequestTypes.SetSensorBorders, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSetDeviceTokenRequest(string deviceToken, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((byte)PlatformTypes.iOS);
            requestBuffer.AddShortString(deviceToken);

            requestsQueue.Add(new Request(RequestTypes.SetDeviceToken, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Выдаются все темы сообщений пользователя
        //Пример: DataManager.SheduleGetMessageSubjectsRequest(DataUpdateCallback);
        static public void SheduleGetMessageSubjectsRequest(Client.RequestViewCallback requestViewCallback)
        {
            requestsQueue.Add(new Request(RequestTypes.GetMessageSubjects, null, ParseGetMessageSubjects, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Выдаются все сообщения, где пользователь Отправитель или Получатель
        //Если subjectID != 0, то дополнительно накладывается фильтр на тему
        //Если Message.senderID == DataManager.userSettings.userID, то пользователь - отправитель данного сообщение, иначе - получатель
        //Пример: DataManager.SheduleGetMessagesRequest(0, DataUpdateCallback);
        //Пример 2: DataManager.SheduleGetMessagesRequest(selectedSubjectID, DataUpdateCallback);
        static public void SheduleGetMessagesRequest(uint subjectID, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((uint)subjectID);

            requestsQueue.Add(new Request(RequestTypes.GetMessages, requestBuffer, ParseGetMessages, requestViewCallback));

            UpdateNow();
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Отправляется новое сообщение другому пользователю с UserID == recepientID
        //Если subjectID == 0, то автоматически добавляется новая тема с содержанием subjectDescription, иначе subjectDescription игнорируется
        //Пример: DataManager.SheduleSendMessageRequest(Settings.adminUserID, 0, "Фильтры", "Как использовать фильтры?", null);
        //Пример ответа на сообщение по теме: DataManager.SheduleSendMessageRequest(Settings.adminUserID, message.subjectID, null, "Все еще непонятно", DataUpdateCallback);
        static public void SheduleSendMessageRequest(int recepientID, uint subjectID, string subjectDescription, string text, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((int)recepientID);
            requestBuffer.Add((uint)subjectID);
            requestBuffer.AddShortString(subjectDescription);
            requestBuffer.AddLongString(text);

            requestsQueue.Add(new Request(RequestTypes.SendMessage, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetCamsRequest(Client.RequestViewCallback requestViewCallback)
        {
            if (machineTypes.Count == 0)
                return;

            ushort filterCityID = 0;
            if (selectedCity != null)
                filterCityID = (ushort)selectedCity.ID;

            ushort filterDivisionID = 0;
            if (selectedDivision != null)
                filterDivisionID = selectedDivision.ID;

            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.Add((ushort)filterCityID);
            requestBuffer.Add((ushort)filterDivisionID);

            requestsQueue.Add(new Request(RequestTypes.GetCams, requestBuffer, ParseGetCams, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleGetCamImageRequest(ulong bridgeMac, byte camNum, DateTime timeFrom, DateTime timeTo, bool last, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddULong6(bridgeMac);
            requestBuffer.Add((byte)camNum);
            requestBuffer.Add((DateTime)timeFrom.ToUniversalTime());
            requestBuffer.Add((DateTime)timeTo.ToUniversalTime());
            requestBuffer.Add((bool)last);

            requestsQueue.Add(new Request(RequestTypes.GetCamImage, requestBuffer, ParseGetCamImage, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void SheduleSendCamQueryRequest(ulong bridgeMac, byte camNum, Client.RequestViewCallback requestViewCallback)
        {
            ByteBuffer requestBuffer = new ByteBuffer();
            requestBuffer.AddULong6(bridgeMac);
            requestBuffer.Add((byte)camNum);

            requestsQueue.Add(new Request(RequestTypes.SendCamQuery, requestBuffer, ParseEmptyRequestAnswer, requestViewCallback));

            UpdateNow();
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void ChangeUser(string login, string password, Client.RequestViewCallback requestViewCallback)
        {
            Client.userToken = null;
            Client.login = login;
            Client.password = password;

            ReconnectToServer(requestViewCallback);

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public void ReconnectToServer (Client.RequestViewCallback requestViewCallback)
        {
            requestsQueue.Add(new Request(RequestTypes.ReconnectToServer, null, null, requestViewCallback));

            UpdateNow();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public string ServerName
        {
            get {return Client.servers[Client.currentServerNum].dnsName;}
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public string UserName
        {
            get { return Client.userName; }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public string UserToken
        {
            get { return Client.userToken; }
            set { Client.userToken = value; }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public bool LoginImpossible
        {
            get {
                if ((Client.userToken == null) && (Client.login == null))
                    return true;
                else
                    return false;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void UpdateNow()
        {
            updateEvent.Set();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseNodesList(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.NodesList)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke (RequestStates.Failed);
                return;
            }

            tempNodes.Clear();
            Node node;

            //Nodes
            ushort nodesCount = buffer.GetUShort();

            for (int s = 0; s < nodesCount; s++)
            {
                node = new Node();
                node.nodeID = buffer.GetUInt3();
                node.bridgeName = buffer.GetShortString();
                node.nodeIndexOnBridge = buffer.GetByte();
                node.nodeName = buffer.GetShortString();
                node.lastOnlineTime = buffer.GetTime().ToLocalTime();
                node.type = buffer.GetByte();
                node.value = buffer.GetDouble();
                node.battery = buffer.GetDouble();
                node.rssi = buffer.GetSByte();
                node.chipTemperature = buffer.GetSByte();

                node.kind = NodeKinds.Node;
                node.imageName = "";

                node.order = node.nodeID;
                if ((DateTime.Now - node.lastOnlineTime).TotalMinutes > 10)
                    node.order += Node.OfflineOrderWeight;

                tempNodes.Add(node);
            }

            //Nodes pending connection
            ushort nodesPendingConnectionCount = buffer.GetUShort();

            for (int s = 0; s < nodesPendingConnectionCount; s++)
            {
                node = new Node();
                node.nodeID = buffer.GetUInt3();
                node.bridgeMac = buffer.GetULong6();
                node.bridgeName = buffer.GetShortString();
                node.nodeName = buffer.GetShortString();
                node.lastOnlineTime = buffer.GetTime().ToLocalTime();
                node.rssi = buffer.GetSByte();

                node.kind = NodeKinds.NodePendingConnection;
                node.imageName = "Settings icon.png";

                node.order = node.nodeID;
                node.order += Node.NodePendingConnectionOrderWeight;
                if ((DateTime.Now - node.lastOnlineTime).TotalMinutes > 10)
                    node.order += Node.OfflineOrderWeight;

                tempNodes.Add(node);
            }

            //Bridges
            ushort bridgesCount = buffer.GetUShort();

            for (int s = 0; s < bridgesCount; s++)
            {
                node = new Node();
                node.bridgeMac       = buffer.GetULong6();
                node.bridgeName      = node.bridgeMac.ToString();
                node.version         = buffer.GetUShort();
                node.nodeName        = buffer.GetShortString();
                node.lastOnlineTime  = buffer.GetTime().ToLocalTime();
                node.maxNodes        = buffer.GetByte();
                node.framePeriod     = buffer.GetUShort();
                node.channelsVariant = buffer.GetByte();

                node.kind = NodeKinds.Bridge;
                node.imageName = "Home.png";

                node.order += Node.BridgeOrderWeight;
                if ((DateTime.Now - node.lastOnlineTime).TotalMinutes > 10)
                    node.order += Node.OfflineOrderWeight;

                tempNodes.Add(node);
            }

            tempNodes.Sort(new NodeComparer());

            nodes.Clear();

            for (int s = 0; s < tempNodes.Count; s++)
                nodes.Add(new Node(tempNodes[s]));

            requestViewCallback?.Invoke(RequestStates.Completed);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseNodeFiltersList(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.NodeFiltersList)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            nodesFilters.Clear();

            ushort nodeFilterCount = buffer.GetUShort();

            for (int s = 0; s < nodeFilterCount; s++)
            {
                nodesFilters.Add (new NodeFilter(buffer.GetUShort(), buffer.GetShortString()));
            }

            requestViewCallback?.Invoke(RequestStates.Completed);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetUserSettings (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetUserSettings)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            userSettings.notificationsFrom = buffer.GetTime().ToLocalTime();
            userSettings.notificationsTo   = buffer.GetTime().ToLocalTime();
            userSettings.sendNodesOfflineNotifications = buffer.GetBool();
            userSettings.role    = (UserRoles) buffer.GetUShort();
            userSettings.groupID = buffer.GetUShort();
            userSettings.userID = buffer.GetInt();

            requestViewCallback?.Invoke(RequestStates.Completed);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseSendNodePacket (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.SendNodePacket)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            requestViewCallback?.Invoke(RequestStates.Completed);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseEmptyRequestAnswer(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType == RequestTypes.Error)
            {
                Client.Log(buffer.GetLongString());

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            requestViewCallback?.Invoke(RequestStates.Completed);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseNodeLog (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.NodeLog)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            nodePackets.Clear();

            ushort nodePacketsCount;

            for (int s0 = 0; s0 < 2; s0++)
            {
                nodePacketsCount = buffer.GetUShort();

                for (int s = 0; s < nodePacketsCount; s++)
                {
                    nodePackets.Add(new NodePacket(buffer.GetBool(), (AckStages)buffer.GetByte(), buffer.GetTime().ToLocalTime(), buffer.GetTime().ToLocalTime(), buffer.GetBytesWithSize()));
                }
            }

            requestViewCallback?.Invoke(RequestStates.Completed);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseSendToNodePacketState (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.SendToNodePacketState)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            sendToNodePacketState.packetsInProcessCount = buffer.GetUShort();

            sendToNodePacketState.lastPacket = new NodePacket(true, (AckStages)buffer.GetByte(), buffer.GetTime().ToLocalTime(), buffer.GetTime().ToLocalTime(), buffer.GetBytesWithSize());

            requestViewCallback?.Invoke(RequestStates.Completed);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseNodeValues (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetNodeValues)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            SensorTypeCodes type = (SensorTypeCodes)buffer.GetByte(); //node type
            byte valuesCount = 0;

            if ((type == SensorTypeCodes.Pulse) || (type == SensorTypeCodes.IRDAModem))
                valuesCount = 4;
            else if (type == SensorTypeCodes.Pulse2Channel)
                valuesCount = (byte)(buffer.GetByte() * 2); //*2 for setting0


                  
            Dictionary<byte, double> nodeValues = new Dictionary<byte, double>();

            for (int s = 0; s < valuesCount; s++)
            {
                nodeValues.Add(buffer.GetByte(), buffer.GetDouble());
            }

            requestViewCallback?.Invoke(nodeValues);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseSendToBridgePacketState(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.SendToBridgePacketState)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            ushort packetsInProcessCount = buffer.GetUShort();

            BridgePacket bridgePacket = new BridgePacket(true, (BridgeParts)buffer.GetByte(), (AckStages)buffer.GetByte(), buffer.GetTime().ToLocalTime(), buffer.GetTime().ToLocalTime(), buffer.GetBytesWithSize());
            bridgePacket.packetsInProcessCount = packetsInProcessCount;

            requestViewCallback?.Invoke(bridgePacket);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParsePacketLog(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.PacketLog)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            List<Packet> packets = new List<Packet>();

            ushort packetsCount = buffer.GetUShort();

            for (int s = 0; s < packetsCount; s++)
            {
                packets.Add(new Packet (buffer.GetBool(), buffer.GetTime().ToLocalTime(), buffer.GetBytesWithSize()));
            }

            requestViewCallback?.Invoke(packets);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetCommonTypes(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetCommonTypes)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            //Machine types
            machineTypes.Clear();
            machineTypes.Add(0, new MachineType(MachineTypeCodes.None, "Все", ""));

            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                ushort code = buffer.GetUShort();
                machineTypes.Add (code, new MachineType ((MachineTypeCodes) code, buffer.GetShortString(), buffer.GetShortString()));
            }

            machineTypes.Add((ushort) MachineTypeCodes.Cam, new MachineType(MachineTypeCodes.Cam, "Видеокамера", "Cam.png"));

            //Machine states
            machineStates.Clear();
            machineStates.Add(0, new MachineState(MachineStateCodes.None, "Все", ""));

            count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                ushort code = buffer.GetUShort();
                machineStates.Add(code, new MachineState((MachineStateCodes)code, buffer.GetShortString(), buffer.GetShortString()));
            }


            //Machine service states
            machineServiceStates.Clear();
            machineServiceStates.Add(0, new MachineServiceState(MachineServiceStateCodes.None, "Все", ""));

            count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                ushort code = buffer.GetUShort();
                machineServiceStates.Add(code, new MachineServiceState((MachineServiceStateCodes)code, buffer.GetShortString(), buffer.GetShortString()));
            }

            //Sensor types
            sensorTypes.Clear();
            sensorTypes.Add(0, new SensorType(SensorTypeCodes.None, "Все", "", "", ""));

            count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                ushort code = buffer.GetUShort();
                sensorTypes.Add(code, new SensorType((SensorTypeCodes)code, buffer.GetShortString(), buffer.GetShortString(), buffer.GetShortString(), buffer.GetShortString()));
            }

            //Cities
            cities.Clear();
            cities.Add(0, new City(0, "Все", ""));

            count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                ushort code = buffer.GetUShort();
                cities.Add(code, new City(code, buffer.GetShortString(), buffer.GetShortString()));
            }

            cities.TryGetValue(storedCityID, out selectedCity);
            machineTypes.TryGetValue (storedMachineTypeID, out selectedMachineType);
            machineStates.TryGetValue(storedMachineStateID, out selectedMachineState);
            machineServiceStates.TryGetValue(storedMachineServiceStateID, out selectedMachineServiceState);

            requestViewCallback?.Invoke(RequestStates.Completed);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetDivisions (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetDivisions)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            divisions.Clear();
            divisions.Add(0, new Division(0, "Все", new City()));

            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                ushort ID = buffer.GetUShort();
                string name = buffer.GetShortString();
                City city = new City();
                cities.TryGetValue(buffer.GetUShort(), out city);
                if (city == null)
                    city = new City();
                divisions.Add(ID, new Division(ID, name, city));
            }

            divisions.TryGetValue(storedDivisionID, out selectedDivision);

            requestViewCallback?.Invoke(divisions);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetMachines(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetMachines)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            machines.Clear();

            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                Machine machine = new Machine();
                machine.ID = buffer.GetUInt();
                machineTypes.TryGetValue (buffer.GetUShort(), out machine.type);
                machineStates.TryGetValue(buffer.GetUShort(), out machine.state);
                machineServiceStates.TryGetValue(buffer.GetUShort(), out machine.serviceState);
                machine.name = buffer.GetShortString();
                divisions.TryGetValue(buffer.GetUShort(), out machine.divisionOwner);
                divisions.TryGetValue(buffer.GetUShort(), out machine.divisionPosition);
                machine.order           = buffer.GetUShort();
                machine.serviceStateTimeStart = buffer.GetTime().ToLocalTime();
                machine.inventoryID     = buffer.GetShortString();
                machine.userName        = buffer.GetShortString();
                machine.camBridgeMac    = buffer.GetULong6();
                machine.camNum          = buffer.GetByte();

                machine.CorrectNulls();

                ushort sensorsCount = buffer.GetUShort();

                for (int s1 = 0; s1 < sensorsCount; s1++)
                {
                    Sensor sensor = new Sensor();
                    sensor.ID = buffer.GetUInt();
                    sensorTypes.TryGetValue(buffer.GetUShort(), out sensor.type);
                    if (sensor.type == null)
                        sensor.type = new SensorType();
                    sensor.mainValue = buffer.GetDouble();
                    sensor.additionalValue = buffer.GetDouble();
                    sensor.lastTime = buffer.GetTime().ToLocalTime();
                    sensor.nodeID   = buffer.GetUInt3();
                    sensor.rssi = buffer.GetSByte();
                    sensor.battery = buffer.GetByte();
                    sensor.chipTemperature = buffer.GetSByte();

                    machine.sensors.Add(sensor);
                }

                if ((machine.state.code == MachineStateCodes.Failure) || (machine.serviceState.code == MachineServiceStateCodes.Broken))
                    machine.order = 1;
                else if ((machine.divisionPosition.ID != machine.divisionOwner.ID) && (machine.divisionPosition.ID != 0))
                    machine.order = 2;
                else
                {
                    if (machine.sensors.Count != 0)
                    if ((DateTime.Now - machine.sensors[0].lastTime).TotalMinutes > Settings.greyOfflineMinutes)
                        machine.order = 65000;
                }

                machines.Add(machine);
            }

            machines.Sort(new MachineComparer());

            requestViewCallback?.Invoke(machines);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetMachineStatesLog (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetMachineStatesLog)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            List<MachineStatesLogElement> machineStatesLogList = new List<MachineStatesLogElement>();

            //MachineServiceStates
            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                MachineStatesLogElement machineStatesLogElement = new MachineStatesLogElement();

                MachineServiceState machineState;
                machineServiceStates.TryGetValue(buffer.GetUShort(), out machineState);
                if (machineState == null)
                    machineState = new MachineServiceState();
                machineStatesLogElement.state = machineState;

                machineStatesLogElement.description = buffer.GetShortString();
                machineStatesLogElement.timeStart = buffer.GetTime().ToLocalTime();
                machineStatesLogElement.timeEnd = buffer.GetTime().ToLocalTime();
                if (machineStatesLogElement.timeEnd.Year == 1970)
                    machineStatesLogElement.timeEnd = DateTime.MinValue;
                
                machineStatesLogElement.userName = buffer.GetShortString();

                machineStatesLogList.Add(machineStatesLogElement);
            }

            //MachineStates
            count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                MachineStatesLogElement machineStatesLogElement = new MachineStatesLogElement();

                MachineState machineState;
                machineStates.TryGetValue(buffer.GetUShort(), out machineState);
                if (machineState == null)
                    machineState = new MachineState();
                machineStatesLogElement.state = machineState;

                machineStatesLogElement.description = buffer.GetShortString();
                machineStatesLogElement.timeStart = buffer.GetTime().ToLocalTime();
                machineStatesLogElement.timeEnd = buffer.GetTime().ToLocalTime();
                if (machineStatesLogElement.timeEnd.Year == 1970)
                    machineStatesLogElement.timeEnd = DateTime.MinValue;

                machineStatesLogElement.userName = buffer.GetShortString();

                machineStatesLogList.Add(machineStatesLogElement);
            }

            machineStatesLogList.Sort(new MachineStatesLogComparer());

            requestViewCallback?.Invoke(machineStatesLogList);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetSensorHistoryData (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetSensorHistoryData)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            List<HistoryPoint> sensorHistory = new List<HistoryPoint>();

            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                sensorHistory.Add(new HistoryPoint(buffer.GetTime().ToLocalTime(), buffer.GetDouble()));
            }

            requestViewCallback?.Invoke(sensorHistory);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetSensorBorders(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetSensorBorders)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            SensorBordersList sensorBorders = new SensorBordersList();

            sensorBorders.maxSecondsNotOkValue = buffer.GetInt();

            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                sensorBorders.list.Add(new SensorBorder((SensorBorderTypes)buffer.GetByte(), buffer.GetDouble(), buffer.GetDouble()));
            }

            requestViewCallback?.Invoke(sensorBorders);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetMessageSubjects (ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetMessageSubjects)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            List<MessageSubject> messageSubjects = new List<MessageSubject>();

            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                messageSubjects.Add(new MessageSubject(buffer.GetUInt(), buffer.GetShortString(), buffer.GetTime().ToLocalTime(), buffer.GetInt(), buffer.GetInt(), buffer.GetShortString(), buffer.GetShortString()));
            }

            requestViewCallback?.Invoke(messageSubjects);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetMessages(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetMessages)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            List<Message> messages = new List<Message>();

            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                messages.Add(new Message(buffer.GetUInt(), buffer.GetLongString(), buffer.GetTime().ToLocalTime(), buffer.GetInt(), buffer.GetInt(), buffer.GetShortString(), buffer.GetShortString()));
            }

            requestViewCallback?.Invoke(messages);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetCams(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetCams)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            ushort count = buffer.GetUShort();

            for (int s = 0; s < count; s++)
            {
                Machine machine = new Machine();
                machineTypes.TryGetValue((ushort)MachineTypeCodes.Cam, out machine.type);

                divisions.TryGetValue(buffer.GetUShort(), out machine.divisionOwner);
                machine.divisionPosition = machine.divisionOwner;
                machine.camBridgeMac = buffer.GetULong6();
                machine.camNum = buffer.GetByte();
                machine.name = buffer.GetShortString();
                machine.camTime = buffer.GetTime().ToLocalTime();
                machine.order = (ushort) (100 + machine.camNum);

                machine.CorrectNulls();

                machines.Add(machine);
            }

            machines.Sort(new MachineComparer());

            requestViewCallback?.Invoke(machines);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseGetCamImage(ByteBuffer buffer, Client.RequestViewCallback requestViewCallback)
        {
            RequestTypes requestType = (RequestTypes)buffer.GetByte();
            if (requestType != RequestTypes.GetCamImage)
            {
                if (requestType == RequestTypes.Error)
                    Client.Log(buffer.GetLongString());
                else
                {
                    Client.LogErr();
                }

                requestViewCallback?.Invoke(RequestStates.Failed);
                return;
            }

            CamImage camImage = new CamImage();
            camImage.data = new ByteBuffer();

            ushort count = buffer.GetUShort();

            if (count != 0)
            {
                camImage.time = buffer.GetTime().ToLocalTime();
                int len = buffer.GetInt();
                camImage.data.AddBytes(buffer.GetBytes(len), 0, len);
                
            }

            requestViewCallback?.Invoke(camImage);
        }
	}
}
