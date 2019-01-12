//Авхадиев Рустем
using System;
using System.Net;
using System.Collections.Generic;

namespace DataUtils
{
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class Machine 
    {
        public uint ID; 
        public string name;
        public MachineType type = new MachineType();
        public MachineState state = new MachineState();
        public MachineServiceState serviceState = new MachineServiceState();
        public DateTime serviceStateTimeStart;
        public Division divisionOwner = new Division();
        public Division divisionPosition = new Division();
        public ushort order;
        public string inventoryID;
        public string userName;

        public ulong camBridgeMac;
        public byte camNum;
        public DateTime camTime;

        public List<Sensor> sensors = new List<Sensor>();


        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CorrectNulls()
        {
            if (type == null)
                type = new MachineType();
            if (state == null)
                state = new MachineState();
            if (serviceState == null)
                serviceState = new MachineServiceState();
            if (divisionOwner == null)
                divisionOwner = new Division();
            if (divisionPosition == null)
                divisionPosition = new Division();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetNameStr()
        {
            if (name != null)
                return name;
            else
                return type.name;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetDivisionStr()
        {
            if (serviceState.code == MachineServiceStateCodes.Service)
                return divisionOwner.name + " -> " + userName;
            else if ((divisionPosition.ID != divisionOwner.ID) && (divisionPosition.ID != 0))
                return divisionOwner.name + " -> " + divisionPosition.name + " (" + divisionPosition.city.name + ")";
            else
                return divisionOwner.name + " (" + divisionOwner.city.name + ")";
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class Classifier
    {
        public ushort ID;
        public string name;
        public string iconName = "";
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class MachineType : Classifier
    {
        public MachineTypeCodes code;

        public MachineType (MachineTypeCodes code, string name, string iconName)
        {
            this.ID = (ushort)code;
            this.code = code;
            this.name = name;
            this.iconName = iconName;
        }

        public MachineType() { }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class MachineState : Classifier
    {
        public MachineStateCodes code;

        public MachineState(MachineStateCodes code, string name, string iconName)
        {
            this.ID = (ushort)code;
            this.code = code;
            this.name = name;
            this.iconName = iconName;
        }

        public MachineState() { }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public enum MachineStateCodes
    {
        None = 0,
        Idle = 1,
        WasteWork = 2,
        Work = 3,
        Failure = 4,

        Count
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class MachineServiceState : Classifier
    {
        public MachineServiceStateCodes code;

        public MachineServiceState(MachineServiceStateCodes code, string name, string iconName)
        {
            this.ID = (ushort)code;
            this.code = code;
            this.name = name;
            this.iconName = iconName;
        }

        public MachineServiceState(){}
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public enum MachineServiceStateCodes
    {
        None = 0,
        Work = 1,
        Broken = 2,
        Service = 3,
        Offline = 4,

        Count
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public enum MachineStateKinds
    {
        None = 0,
        State = 1,
        ServiceState = 2,

        Count
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class MachineStatesLogElement
    {
        public Classifier state = new Classifier();
        public string description;
        public DateTime timeStart;
        public DateTime timeEnd;
        public string userName;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class Sensor
    {
        public uint ID; 
        public SensorType type = new SensorType();
        public double mainValue;
        public double additionalValue;
        public DateTime lastTime;
        public uint nodeID;
        public sbyte rssi;
        public byte battery;
        public sbyte chipTemperature;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class SensorType : Classifier
    {
        public SensorTypeCodes code;
        public string mainValueSymbol;
        public string additionalValueSymbol;

        public SensorType(SensorTypeCodes code, string name, string iconName, string mainValueSymbol, string additionalValueSymbol)
        {
            this.code = code;
            this.name = name;
            this.iconName = iconName;
            this.mainValueSymbol = mainValueSymbol;
            this.additionalValueSymbol = additionalValueSymbol;
        }

        public SensorType(){}
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public enum SensorTypeCodes
    {
        None,
        Temperature = 1,
        Pulse = 2,
        ADCMonitor = 3,
        Thermocouple = 4,
        Accel = 5,
        Pulse2Channel = 6,
        IRDAModem = 7,
        Switcher = 8,
        RadioButton = 9,
        HallRotate = 10
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public enum SensorValueArrayIndexes
    {
        StoredLastValue = 0,
        MainValue = 1,
        AdditionalValue = 2,
        OtherValuesStartIndex = 3
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class SensorBorder
    {
        public SensorBorderTypes type;
        public double minValue;
        public double maxValue;

        public SensorBorder(SensorBorderTypes type, double minValue, double maxValue)
        {
            this.type = type;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public SensorBorder(){}
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class SensorBordersList
    {
        public int maxSecondsNotOkValue;
        public List<SensorBorder> list = new List<SensorBorder>();
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public enum SensorBorderTypes
    {
        None   = 0,
        Ignore = 1,
        Ok     = 2,
        InstantAlarm = 2
    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class Division : Classifier
    {
        public City city = new City();

        public Division (ushort ID, string name, City city)
        {
            this.ID = ID;
            this.name = name;
            this.city = city;
        }

        public Division()
        {
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class City : Classifier
    {
        public string shortName;

        public City(ushort ID, string name, string shortName)
        {
            this.ID = ID;
            this.name = name;
            this.shortName = shortName;
        }

        public City() { }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public enum MachineTypeCodes
    {
        None             = 0,
        Refrigerator     = 1,
        Freezer          = 2,
        Oven             = 3,
        ProofingChamber  = 4,
        Mixer            = 5,
        Sifter           = 6,
        Room             = 7,
        CounterElectricity = 8,
        CounterWater       = 9,

        Cam                = 200
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class MachineComparer : IComparer<Machine>
    {
        public int Compare(Machine m1, Machine m2)
        {
            if ((m1.order < 100) || (m2.order < 100))
                return m1.order == m2.order ? 0 : (m1.order < m2.order ? -1 : 1);

            if ((m1.type.code == MachineTypeCodes.Cam) && (m2.type.code != MachineTypeCodes.Cam))
                return 1;

            if ((m1.type.code != MachineTypeCodes.Cam) && (m2.type.code == MachineTypeCodes.Cam))
                return -1;

            if (m1.divisionOwner.city.ID != m2.divisionOwner.city.ID)
                return m1.divisionOwner.city.ID < m2.divisionOwner.city.ID ? -1 : 1;
            
            if (m1.divisionOwner.ID != m2.divisionOwner.ID)
                return m1.divisionOwner.ID < m2.divisionOwner.ID ? -1 : 1;

            return m1.order == m2.order ? 0 : (m1.order < m2.order ? -1 : 1);
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class MachineStatesLogComparer : IComparer<MachineStatesLogElement>
    {
        public int Compare(MachineStatesLogElement l1, MachineStatesLogElement l2)
        {
            if ((l1.timeEnd == DateTime.MinValue) && (l1.state is MachineServiceState))
                return -1;
            else if ((l2.timeEnd == DateTime.MinValue) && (l2.state is MachineServiceState))
                return 1;
            
            return l1.timeStart == l2.timeStart ? 0 : (l1.timeStart > l2.timeStart ? -1 : 1);
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct HistoryPoint 
    {
        public double value;
        public DateTime time;

        public HistoryPoint(DateTime time, double value)
        {
            this.value = value;
            this.time = time;
        }

        public override string ToString()
        {
            return time.ToString() + " " + value.ToString();
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class CamImage
    {
        public DateTime time;
        public ByteBuffer data = new ByteBuffer();
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class FilterObject
    {
        public string leftTitle;
        public string mainTitle;
        public string iconName;
        public object obj;

        public FilterObject(string leftTitle, string mainTitle, string iconName, object obj)
        {
            this.leftTitle = leftTitle;
            this.mainTitle = mainTitle;
            this.iconName = iconName;
            this.obj = obj;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct MessageSubject
    {
        public uint ID;
        public string description;
        public DateTime time;
        public int senderID;
        public int recepientID;
        public string senderName;
        public string recepientName;

        public MessageSubject(uint ID, string description, DateTime time, int senderID, int recepientID, string senderName, string recepientName)
        {
            this.ID = ID;
            this.description = description;
            this.time = time;
            this.senderID = senderID;
            this.recepientID = recepientID;
            this.senderName = senderName;
            this.recepientName = recepientName;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct Message
    {
        public uint subjectID;
        public string description;
        public DateTime time;
        public int senderID;
        public int recepientID;
        public string senderName;
        public string recepientName;

        public Message(uint subjectID, string description, DateTime time, int senderID, int recepientID, string senderName, string recepientName)
        {
            this.subjectID = subjectID;
            this.description = description;
            this.time = time;
            this.senderID = senderID;
            this.recepientID = recepientID;
            this.senderName = senderName;
            this.recepientName = recepientName;
        }
    }




    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class Node
    {
        public uint nodeID;
        public string bridgeName;
        public byte nodeIndexOnBridge;
        public string nodeName;
        public DateTime lastOnlineTime;
        public byte type;
        public double value;
        public double battery;
        public sbyte rssi;
        public sbyte chipTemperature;

        public ulong  bridgeMac;
        public ushort version;
        public ushort channelsVariant;
        public ushort framePeriod;
        public byte   maxNodes;

        public NodeKinds kind;
        public string imageName;
        public ulong order;

        public const int offlineMinMinutes = 10;
        public const ulong NodePendingConnectionOrderWeight = 1000 * 1000;
        public const ulong BridgeOrderWeight = 1000 * 1000 * 1000;
        public const ulong OfflineOrderWeight = (ulong)(1000 * 1000 * 1000) * 1000;

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public Node(Node node)
        {
            this.nodeID             = node.nodeID;
            this.bridgeName         = node.bridgeName;
            this.nodeIndexOnBridge  = node.nodeIndexOnBridge;
            this.nodeName           = node.nodeName;
            this.lastOnlineTime     = node.lastOnlineTime;
            this.type               = node.type;
            this.value              = node.value;
            this.battery            = node.battery;
            this.rssi               = node.rssi;
            this.chipTemperature    = node.chipTemperature;
            this.bridgeMac          = node.bridgeMac;
            this.version            = node.version;
            this.channelsVariant    = node.channelsVariant;
            this.framePeriod        = node.framePeriod;
            this.maxNodes           = node.maxNodes;

            this.kind               = node.kind;
            this.imageName          = node.imageName;
            this.order              = node.order;
        }
        public Node() { }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node n1, Node n2)
        {
            return n1.order == n2.order ? 0 :
                     (n1.order < n2.order ? -1 : 1);
        }
    }

    public enum NodeKinds
    {
        Node,
        NodePendingConnection,
        Bridge
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class NodePacket
    {
        public bool directionToNode;
        public uint nodeID;
        public AckStages ackStage;
        public DateTime ackTime;
        public DateTime sendTime;
        public ByteBuffer dataBuffer = new ByteBuffer();
        public NodePacketTypeAndSettingsType packetType;
        public string description;
        public string header;

        public NodePacket(bool directionToNode, AckStages ackStage, DateTime ackTime, DateTime sendTime, byte[] _dataBuffer)
        {
            this.directionToNode = directionToNode;

            this.ackStage = ackStage;
            this.ackTime = ackTime;
            this.sendTime = sendTime;

            dataBuffer.Reset();
            dataBuffer.AddBytes(_dataBuffer);
            dataBuffer.index = 0;

            string subType;

            if (this.directionToNode == false)
                packetType.nodePacketType = dataBuffer.ParsePacketFromNode(out subType, out this.description, out this.header);
            else
                packetType = dataBuffer.ParsePacketToNode(out this.description, false);
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class BridgePacket
    {
        public bool directionToBridge;
        public ulong bridgeMac;
        public BridgeParts bridgePart;
        public AckStages ackStage;
        public DateTime ackTime;
        public DateTime sendTime;
        public ByteBuffer dataBuffer = new ByteBuffer();
        public BridgePacketAttributes packetAttributes;
        public string description;
        public string header;

        public ushort packetsInProcessCount;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public BridgePacket(bool directionToBridge, BridgeParts bridgePart, AckStages ackStage, DateTime ackTime, DateTime sendTime, byte[] _dataBuffer)
        {
            this.directionToBridge = directionToBridge;

            this.ackStage = ackStage;
            this.ackTime = ackTime;
            this.sendTime = sendTime;

            dataBuffer.Reset();
            dataBuffer.AddBytes(_dataBuffer);
            dataBuffer.index = 0;

            string subType;

            if (this.directionToBridge == false)
                packetAttributes.bridgePacketType = dataBuffer.ParsePacketFromBridge(out subType, out this.description, out this.header);
            else
                packetAttributes = dataBuffer.ParsePacketToBridge(out this.description, out this.header, false);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public BridgePacket()
        {
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class Packet
    {
        public bool fromServer;
        public DateTime time;
        public ByteBuffer dataBuffer = new ByteBuffer();
        public PacketTypes packetType;
        public string packetSubType;
        public string description;
        public bool error;

        public Packet(bool fromServer, DateTime time, byte[] _dataBuffer)
        {
            this.fromServer = fromServer;
            this.time = time;

            dataBuffer.Reset();
            dataBuffer.AddBytes(_dataBuffer);
            dataBuffer.index = 0;

            dataBuffer.ParsePacket(fromServer, out packetType, out packetSubType, out description);

            error = false;

            if ((fromServer == true) && (packetType == PacketTypes.Log))
                error = true;
            if (packetSubType.Contains("Error"))
                error = true;
            if (description.Contains("Error"))
                error = true;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct SendToNodePacketState
    {
        public ushort packetsInProcessCount;
        public NodePacket lastPacket;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct NodePacketTypeAndSettingsType
    {
        public NodePacketTypes nodePacketType;
        public NodeSettingsTypes nodeSettingsType;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct BridgePacketAttributes
    {
        public BridgeParts bridgePart;
        public BridgePacketTypes   bridgePacketType;
        public BridgeCommands      bridgeCommand;
        public BridgeSettingsTypes bridgeSettingsType;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct NodeFilter
    {
        public ushort group;
        public string name;

        public NodeFilter(ushort group, string name)
        {
            this.group = group;
            this.name = name;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct Server
    {
        public string name;
        public string dnsName;
        public byte[] ipv4Address;
        public int port;

        public Server(string name, string dnsName, byte[] ipv4Address, int port)
        {
            this.name = name;
            this.dnsName = dnsName;
            this.ipv4Address = ipv4Address;
            this.port = port;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct UserSettings
    {
        public DateTime notificationsFrom;
        public DateTime notificationsTo;
        public bool     sendNodesOfflineNotifications;
        public int      userID;
        public UserRoles role;
        public ushort   groupID;
    }

    public enum UserRoles : ushort
    {
        None = 0,
        Admin = 1,
        User = 2
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public struct sensorHistoryData
    {
        public float value;
        public DateTime time;

        public sensorHistoryData(float _value, DateTime _time)
        {
            value = _value;
            time = _time;
        }
    }

    public enum ViewTypes
    {
        None,
        Cam,
        Log
    }

    public enum PacketTypes : byte
    {
        None = 0,
        Auth = 1,
        Ping = 2,
        SensorData = 3,
        File = 4,
        Log = 5,
        NodePacket = 6,
        BridgePacket = 7,
        ReAuth = 8,
        Cam = 9
    }

    public enum PacketSubTypes : byte
    {
        FileStart = 1,
        FileContinue = 2,
        FileEnd = 3,
        CamStart = 4,
        CamContinue = 5,
        CamEnd = 6
    }

    public enum ClientPacketTypes : byte
    {
        None = 0,
        Request = 1,
        RequestAnswer = 2,
        Log = 5
    }

    public enum HeaderOffsets : byte
    {
        PacketSize = 0,
        CheckSum = 4,
        FirstEnd = 6,
        PacketNum = 6,
        SecondEnd = 7
    }


    public enum RequestTypes : byte
    {
        ReconnectToServer = 0,
        AuthByToken = 1,
        Error = 2,
        SensorsList = 3,
        NodesList = 4,
        NodeFiltersList = 5,
        GetUserSettings = 6,
        AuthByLogin = 7,
        SetUserSettings = 8,
        SetNodeSettings = 9,
        SendNodePacket = 10,
        SetNodePendingConnectionSettings = 11,
        NodeLog = 12,
        SendToNodePacketState = 13,
        DeleteItem            = 14,
        GetNodeValues         = 15,
        SetNodeValues         = 16,
        SetBridgeSettings     = 17,
        SendToBridgePacketState = 18,
        SendBridgePacket      = 19,
        PacketLog             = 20,

        GetCommonTypes        = 21,
        GetDivisions          = 22,
        GetMachines           = 23,
        GetMachineStatesLog   = 24,
        SetMachineServiceState = 25,
        GetSensorHistoryData   = 26,
        GetSensorBorders       = 27,
        SetSensorBorders       = 28,
        SetDeviceToken         = 29,
        GetMessageSubjects     = 30,
        GetMessages            = 31,
        SendMessage            = 32,
        GetCams                = 33,
        GetCamImage            = 34,
        SendCamQuery           = 35,

        Count
    }

    public enum PlatformTypes : byte
    {
        None = 0,
        iOS = 1,
        Android = 2
    }

    public enum ControllerItemTypes : byte
    {
        Invalid = 0,
        NodePendingConnection = 1,
        Node = 2,
        Bridge = 3
    }

    public enum BridgePacketTypes
    {
        PacketFromNode = 1,
        PacketToNode = 2,
        Ack = 3,
        Cmd = 4,
        Alive = 5,
        BridgeError = 6,
        BridgeAck = 7,
        BridgeReadFlashResponse = 8,
        BridgeVersionReport = 9,
        BridgeWriteFlashFinishResponse = 10,
        WillRebootNotification = 11,
        ReadPacketsToNodeBufferResponse = 12,
        PortionAck = 13,
        RegisterNodeAck = 14,
        RawUartPacketAck = 15,
        PacketToNodeSendReport = 16,
        GetTempResponce = 17
            
    };

    public enum NodePacketTypes : byte
    {
        RegisterNode = 1,
        Settings = 2,
        ReadFlash = 3,
        WriteFlashStart = 4,
        WriteFlashContinue = 5,
        WriteFlashFinish = 6,
        WriteFlashAbort = 7,
        Reboot = 8,
        RebootIfCrcMatch = 9,
        SensorQuery = 10,
        FullReset = 11,

        InitiateNodeRequest = 13,
        InitiateNodeResponse = 14,
        InitiateNodeConfirmation = 15,
        NetJoinRequest = 16,
        NetJoinResponse = 17,
        NetJoinConfirmation = 18,
        BridgeInstantSettings = 19,
        BridgeLog = 30,

        ReadFlashResponse = 31,
        WriteFlashFinishResponse = 32,
        DoNotLog = 33,

        RebootReport = 40,
        BridgeAck = 41,
        NodeAck = 42,
        Alarm = 43,
        Alive = 44,
        Error = 45,
        PulseCount = 46,
        Pulse = 47,
        ProbeScan = 48,
        Pulse2ChannelCount = 49,

        Temperature = 50,
        TermoCouple = 51,
        ADCValue = 52,
        Accel = 53,
        Stats = 54,
        IrDAQueryResponse = 55,
        SwitcherState = 56,
        InstantPacket = 57,
        RadioButtonPress = 58,
        RawPacket = 59,
        InstantRxReport = 60,
        HallRotate = 61,
        IrDAQueryShortResponse = 62,
        PulseCountShort = 63,
        HallRotateStart = 64,
        HallRotateContinue = 65,
        HallRotateFinish = 66,        
        AccelStart = 67,
        AccelContinue = 68,
        AccelFinish = 69
    }


    public enum BridgeCommands
    {
        PauseUART = 1,
        StopRadio,
        StartRadio,
        AddNode,
        StartUart,
        ReadFlash,
        WriteFlashStart,
        WriteFlashContinue,
        WriteFlashFinish,
        WriteFlashAbort,
        Reboot,
        RebootIfCrcMatch,
        ReadPacketsToNodeBuffer,
        Settings,
        ReconnectToServer,
        SendCam,
        ClearPacketsToNodeBuffer,
        ClearNodes,
        SendRawPacket = 19,
        SendInstantPacket = 20,
        ResetHAP = 21,
        SetRawUartMode = 22,
        ResetCC1310 = 23,
        SendRawUartPacket = 24,
        SetRawUartTimeout = 25,
        SendHttpReq = 26,
        ClearCams = 27,
        AddCam = 28,
        GetTemp = 29

        
    }

    public enum BridgeParts
    {
        cc3220 = 1,
        cc1310 = 2
    }

    public enum AckStages
    {
        CC3220 = 1,
        CC1310 = 2,
        Node = 3
    }

    public enum LogTypes : byte
    {
        None = 0,
        Error = 1,
        CriticalError = 2,
        Mark1 = 3,
        Mark2 = 4,
        HTTPGetHeader = 5
    }

    public enum LogParts
    {
        Packets = 1,
        ServerRuntime = 2,
        ClientRuntime = 3,
        Push = 4,
        UserApp = 5
    };

    public enum BridgeSettingsTypes : byte
    {
        None = 0,
        StopLog = 1,
        Time    = 2,
        SendCamPeriod = 3,
        ServerAddress = 4,
        APList        = 5,
        Global        = 6, 
        RadioProfile  = 7,
        AlwaysSendFullJoinResponse = 8,
        Cam           = 9,

        Count
    }

    public enum NodeSettingsTypes : byte
    {
        None = 0,
        MaxRetries = 1,
        AlivePeriod = 2,
        CompbRef = 3,
        UseCompb = 4,
        Time = 5,
        Tariff = 6,
        Stats = 7,
        TempAlert = 8,
        Threshold = 9,
        SensorPeriod = 10,
        Accel = 11,
        MinPulsePeriod = 12,
        SensorPowerTimeout = 13,
        TuneMode = 14,
        RxTimeout = 15,
        PeriodicalQueries = 16,
        IrDATiming        = 17,
        Button            = 18,
        InitialChannelsState = 19,
        SensorDataMode = 20,
        ADCBorders = 21
    }

    public class ProbeName
    {
        public ProbeName (long mac, string name)
        {
            this.mac = mac;
            this.name = name;
        }

        public long mac;
        public string name;
    }


    static public class FormatUtils
    {
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        static public string PeriodStr(DateTime timeStart, DateTime timeEnd)
        {
            string result = "";
            var diff = timeEnd - timeStart;

            if (diff.TotalDays > 1)
                result += diff.TotalDays.ToString ("F0") + " д. ";

            result += diff.Hours.ToString ("D2") + " ч. ";

            if (diff.TotalDays < 1)
                result += diff.Minutes.ToString("D2") + " м.";

            return result;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //Реализует работу с простым буфером байтовых данных для отправки/получения пакетов на/от сервер/сервера
    [Serializable]
    public class ByteBuffer
    {
        public byte[] data;
        public int length = 0;
        public int index = 0;
        public bool debug = false;

        public ByteBuffer()
        {
            data = new byte[1];
        }

        public ByteBuffer(int maxLength)
        {
            data = new byte[maxLength];
        }

        public override string ToString()
        {
            return "length: " + length + " index: " + index;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset(int element = 0)
        {
            length = element;
            index = element;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Shift(int elementsCount)
        {
            int temp = length - elementsCount;

            for (int s = 0; s < temp; s++)
                data[s] = data[s + elementsCount];

            index -= elementsCount;
            length -= elementsCount;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public int CheckSum(int startIndex = 0, int sumLength = -1)
        {
            int result = 0;
            int endIndex = length;

            if (sumLength != -1)
                endIndex = startIndex + sumLength;

            for (int s = startIndex; s < endIndex; s++)
                result += data[s];

            return result;
        }

        static ushort[] crcTable = new ushort[256];
        static bool crcTabelInitiated = false;

        //--------------------------------------------------------------------------------------------------------------------------
        private void MakeCRC16Table()
        {
            ushort r;
            int s, s1;

            for (s = 0; s < 256; s++)
            {
                r = (ushort)(((ushort)s) << 8);

                for (s1 = 0; s1 < 8; s1++)
                {
                    if ((r & (1 << 15)) != 0)
                        r = (ushort)((r << 1) ^ 0x8005);
                    else
                        r = (ushort)(r << 1);
                }
                crcTable[s] = r;
            }

            crcTabelInitiated = true;
        }

        //--------------------------------------------------------------------------------------------------------------------------
        public ushort GetCRC16(int len = -1)
        {
            if (crcTabelInitiated == false)
                MakeCRC16Table();

            if (len == -1)
                len = length - index;

            if (index + len > length)
                return 0;

            ushort crc;
            crc = 0xFFFF;
            for (int s = index; s < index + len; s++)
            {
                crc = (ushort)(crcTable[((crc >> 8) ^ data[s]) & 0xFF] ^ (crc << 8));
            }
            crc ^= 0xFFFF;

            return crc;
        }

        //--------------------------------------------------------------------------------------------------------------------------
        public ushort GetCRC16Modbus(int len = -1)
        {
            if (len == -1)
                len = length - index;

            if (index + len > length)
                return 0;

            int j;
            uint reg_crc = 0xFFFF;
            for (int s = index; s < index + len; s++)
            {
                reg_crc ^= data[s];
                for (j = 0; j < 8; j++)
                {
                    if ((reg_crc & 0x01) != 0)
                        reg_crc = (reg_crc >> 1) ^ 0xA001;
                    else
                        reg_crc = reg_crc >> 1;
                }
            }

            return (ushort)reg_crc;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void MaintainCapacity(int bytesToAdd)
        {
            if ((length + bytesToAdd) > data.Length)
            {
                byte[] newData = new byte[length * 2 + bytesToAdd];
                Array.Copy(data, newData, length);

                data = newData;
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool DebugCheck(int readLen)
        {
            if ((data.Length < length) || ((length < index + readLen) && (length != 0)) || (index < 0) || (readLen < 0))
                return false;

            return true;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(bool param)
        {
            MaintainCapacity(1);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 1;
            length += 1;

            //DebugLog (index + ") bool " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(byte param)
        {
            MaintainCapacity(1);

            data[index] = param;
            index++;
            length++;

            //DebugLog (index + ") byte " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(short param)
        {
            MaintainCapacity(2);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 2;
            length += 2;

            // DebugLog(index + ") short " + param);
        }

        public void Add(ushort param)
        {
            MaintainCapacity(2);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 2;
            length += 2;

            //DebugLog (index + ") ushort " + param);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(int param)
        {
            MaintainCapacity(4);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 4;
            length += 4;

            //DebugLog (index + ") int " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(uint param)
        {
            MaintainCapacity(4);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 4;
            length += 4;

            //DebugLog (index + ") int " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(long param)
        {
            MaintainCapacity(8);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 8;
            length += 8;

            //DebugLog (index + ") long " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(ulong param)
        {
            MaintainCapacity(8);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 8;
            length += 8;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(float param)
        {
            MaintainCapacity(4);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 4;
            length += 4;

            //DebugLog (index + ") float " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(double param)
        {
            MaintainCapacity(8);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 8;
            length += 8;

            //DebugLog (index + ") float " + param);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(decimal param)
        {
            Add((int)(param * 1000)); //считаем, что в приложении все дробные числа имеют ровно 3 знака после запятой

            //DebugLog (index + ") decimal " + param);
        }

        public ByteBuffer Clone()
        {
            ByteBuffer buffer = new ByteBuffer(this.length);
            buffer.data = new byte[data.Length];
            Array.Copy(data, buffer.data, data.Length);
            buffer.index = index;
            buffer.length = length;
            return buffer;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddLongString (string param)
        {
            if (param == null)
            {
                Add((int)0);
                return;
            }
            //TODO: Учесть little/big - endian
            byte[] temp = System.Text.Encoding.UTF8.GetBytes(param);

            Add((int)temp.Length);

            MaintainCapacity(temp.Length);

            temp.CopyTo(data, index);
            index += temp.Length;
            length += temp.Length;

            // DebugLog(index + ") string " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddShortString(string param)
        {
            if (param == null)
            {
                Add((byte)0);
                return;
            }

            //TODO: Учесть little/big - endian
            byte[] temp = System.Text.Encoding.UTF8.GetBytes(param);

            Add((byte)temp.Length);

            MaintainCapacity(temp.Length);

            temp.CopyTo(data, index);
            index += temp.Length;
            length += temp.Length;

            // DebugLog(index + ") string " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddUInt3(uint param)
        {
            MaintainCapacity(4);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 3;
            length += 3;

            //DebugLog (index + ") int " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddULong6(ulong param)
        {
            MaintainCapacity(8);

            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 6;
            length += 6;

            //DebugLog (index + ") long " + param);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(DateTime time)
        {
            MaintainCapacity(4);

            if (time < new DateTime(1970, 1, 1))
            {
                Add((uint)0);
                return;
            }

            uint unixTime = (uint)(time - new DateTime(1970, 1, 1)).TotalSeconds;

            Add((uint) unixTime);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddBytes(byte[] source, int _index = -1, int _length = -1)
        {
            if (source == null)
                return;
            
            if (_index == -1)
                _index = 0;

            if (_length == -1)
                _length = source.Length;

            if (source.Length < _index + _length)
                return;

            MaintainCapacity(_length);

            Array.Copy(source, _index, data, index, _length);

            index += _length;
            length += _length;


            // DebugLog(index + ") BYTES length" + _length);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddBytes(ByteBuffer source, int _index = -1, int _length = -1)
        {
            if (_length == 0)
                return;

            if (source == null)
                return;

            if (_index == -1)
                _index = source.index;

            if (_length == -1)
                _length = source.length - _index;

            AddBytes(source.data, _index, _length);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddBytesWithSize (ByteBuffer source, int _index = -1, int _length = -1)
        {
            if (_length == 0)
                return;

            if (source == null)
                return;
            
            if (_index == -1)
                _index = source.index;

            if (_length == -1)
                _length = source.length - _index;

            Add ((ushort)_length);

            AddBytes(source.data, _index, _length);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddHex(string hexSource, int _index = 0, int _length = -1)
        {
            if (_length == 0)
                return;

            if (hexSource == null)
                return;

            if (_length == -1)
                _length = hexSource.Length;

            for (int s = _index; s < _length; s += 2)
            {
                string b = hexSource.Substring(s, 2);
                Add((byte)Convert.ToByte(b, 16));
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddHexWithSize (string hexSource, int _index = 0, int _length = -1)
        {
            if (_length == 0)
                return;

            if (hexSource == null)
            {
                Add((ushort)0);
                return;
            }

            if (_length == -1)
                _length = hexSource.Length;

            if ((_length - _index) % 2 != 0)
            {
                Add ((ushort)0);
                return;
            }

            Add ((ushort)((_length - _index) / 2));

            for (int s = _index; s < _length; s += 2)
            {
                string b = hexSource.Substring(s, 2);
                Add((byte)Convert.ToByte(b, 16));
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Add(bool[,] param)
        {
            if (param == null)
            {
                Add((ushort)0);
                Add((ushort)0);
                return;
            }

            Add((ushort)param.GetLength(0));
            Add((ushort)param.GetLength(1));

            for (int x = 0; x < param.GetLength(0); x++)
                for (int y = 0; y < param.GetLength(1); y++)
                    Add((bool)param[x, y]);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddFixedString(string param)
        {
            if (param == null)
                return;
            
            //TODO: Учесть little/big - endian
            byte[] temp = System.Text.Encoding.UTF8.GetBytes(param);

            MaintainCapacity(temp.Length);

            temp.CopyTo(data, index);
            index += temp.Length;
            length += temp.Length;

            // DebugLog(index + ") string " + param);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /*public void Add(object param)
        {
            if (param is System.Byte)
                Add((byte)param);

            if (param is System.Int16)
                Add((short)param);

            if (param is System.Int32)
                Add((int)param);

            if (param is System.Int64)
                Add((long)param);

            if (param is System.Decimal)
                Add((decimal)param);


            if (param is System.String)
                Add((string)param);
        }*/

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Rewrite(byte param)
        {
            if (DebugCheck(1) == false)
                return;
            
            data[index] = param;
            index++;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Rewrite(short param)
        {
            if (DebugCheck(2) == false)
                return;
            
            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 2;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Rewrite(ushort param)
        {
            if (DebugCheck(2) == false)
                return;
            
            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 2;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Rewrite(int param)
        {
            if (DebugCheck(4) == false)
                return;
            
            byte[] temp = BitConverter.GetBytes(param);
            temp.CopyTo(data, index);
            index += 4;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool GetBool()
        {
            if (DebugCheck(1) == false)
                return false;
            
            bool result = BitConverter.ToBoolean(data, index);

            index += 1;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public byte GetByte()
        {
            if (DebugCheck(1) == false)
                return 0;
            
            byte result = data[index];

            index++;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public byte WatchByte()
        {
            if (DebugCheck(1) == false)
                return 0;
            
            byte result = data[index];

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public sbyte GetSByte()
        {
            if (DebugCheck(1) == false)
                return 0;
            
            sbyte result = (sbyte)data[index];

            index++;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public short GetShort()
        {
            if (DebugCheck(2) == false)
                return 0;
            
            short result = BitConverter.ToInt16(data, index);

            index += 2;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ushort GetUShort()
        {
            if (DebugCheck(2) == false)
                return 0;
            
            ushort result = BitConverter.ToUInt16(data, index);

            index += 2;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public int GetInt()
        {
            if (DebugCheck(4) == false)
                return 0;
            
            int result = BitConverter.ToInt32(data, index);

            index += 4;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public uint GetUInt()
        {
            if (DebugCheck(4) == false)
                return 0;
            
            uint result = BitConverter.ToUInt32(data, index);

            index += 4;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public uint GetUInt3()
        {
            if (DebugCheck(3) == false)
                return 0;
            
            byte[] temp = new byte[4];

            Array.Copy(data, index, temp, 0, 3);

            uint result = BitConverter.ToUInt32(temp, 0);

            index += 3;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public uint WatchUInt3()
        {
            if (DebugCheck(3) == false)
                return 0;
            
            byte[] temp = new byte[4];

            Array.Copy(data, index, temp, 0, 3);

            uint result = BitConverter.ToUInt32(temp, 0);

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public long GetLong()
        {
            if (DebugCheck(8) == false)
                return 0;
            
            long result = BitConverter.ToInt64(data, index);

            index += 8;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ulong GetULong()
        {
            if (DebugCheck(8) == false)
                return 0;
            
            ulong result = BitConverter.ToUInt64(data, index);

            index += 8;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ulong GetULong6()
        {
            if (DebugCheck(6) == false)
                return 0;
            
            byte[] temp = new byte[8];

            Array.Copy(data, index, temp, 0, 6);

            ulong result = BitConverter.ToUInt64(temp, 0);

            index += 6;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public float GetFloat()
        {
            if (DebugCheck(4) == false)
                return 0;
            
            float result = BitConverter.ToSingle(data, index);

            index += 4;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public double GetDouble()
        {
            if (DebugCheck(8) == false)
                return 0;
            
            double result = BitConverter.ToDouble(data, index);

            index += 8;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public DateTime GetTime()
        {
            if (DebugCheck(4) == false)
                return DateTime.MinValue;
            
            uint seconds = BitConverter.ToUInt32(data, index);

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);

            DateTime result = dateTimeOffset.UtcDateTime;

            index += 4;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetLongString()
        {
            int strLength = GetInt();

            if (DebugCheck(strLength) == false)
                return null;

            if (strLength == 0)
                return null;

            strLength = Math.Min(length - index, strLength);

            string result = System.Text.Encoding.UTF8.GetString(data, index, strLength);

            index += strLength;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetShortString()
        {
            int strLength = GetByte();

            if (DebugCheck(strLength) == false)
                return null;
            
            if (strLength == 0)
                return null;

            strLength = Math.Min(length - index, strLength);

            string result = System.Text.Encoding.UTF8.GetString(data, index, strLength);

            index += strLength;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetFixedString(int strLength)
        {
            if (strLength == 0)
                return "";

            strLength = Math.Min(length - index, strLength);

            if (DebugCheck(strLength) == false)
                return null;

            string result = System.Text.Encoding.UTF8.GetString(data, index, strLength);

            index += strLength;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public byte[] GetBytes(int _length = -1)
        {
            if (_length == -1)
                _length = length;

            if (DebugCheck(_length) == false)
                return null;
            
            byte[] result = new byte[_length];

            Array.Copy(data, index, result, 0, _length);

            // DebugLog(index + ") BYTES Length: " + _length);

            index += _length;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public byte[] GetBytesWithSize()
        {
            ushort len = GetUShort();

            if (DebugCheck(len) == false)
                return null;

            byte[] result = new byte[len];

            Array.Copy(data, index, result, 0, len);

            index += len;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetHexString(int len)
        {
            if (len == 0)
                return "";
            if ((len > length - index) || (len < 0))
                return "GetHexString Error len: " + len + ".";

            string result = BitConverter.ToString(data, index, len).Replace("-", " ");
            index += len;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetHexStringWithSize ()
        {
            ushort len = GetUShort();

            if (len == 0)
                return "";
            if ((len > length - index) || (len < 0))
                return "GetHexStringWithSize Error len: " + len + ".";

            string result = BitConverter.ToString(data, index, len).Replace("-", " ");
            index += len;

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetHexString2(int len)
        {
            if (len == 0)
                return "";
            if ((len > length - index) || (len < 0))
                return "Error len: " + len + ".";

            string temp = BitConverter.ToString(data, index, len).Replace("-", "");
            string result = "";

            for (int s = 0; s < len; s++)
            {
                result += temp[temp.Length - s * 2 - 1];
                result += temp[temp.Length - s * 2 - 2];
                result += ":";
            }

            return result;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public float CalcTermocoupleTemp (float volt, float zeroTemp)
        {
            float[] thermoCoupleTable = {-2.243f, -1.889f, -1.527f, -1.156f, -0.778f, -0.392f, 0, 0.397f, 0.798f, 1.203f, 1.612f, 2.023f, 2.436f, 2.851f, 3.267f, 3.682f, 4.096f, 4.509f, 4.920f, 5.328f, 5.735f, 6.138f, 6.540f, 6.941f, 7.340f, 7.739f, 8.138f, 8.539f, 8.940f, 9.343f, 9.747f, 10.153f, 10.561f, 10.971f, 11.382f, 11.795f, 12.209f, 12.624f, 13.040f, 13.457f, 13.874f, 14.293f};

            for (int s = 0; s < thermoCoupleTable.Length; s++)
                thermoCoupleTable[s] *= 50;

            if (volt < thermoCoupleTable[1])
                return -999;

            if (volt > thermoCoupleTable[thermoCoupleTable.Length - 1])
                return 999;


            float result = 0;
            for (int s = 0; s < thermoCoupleTable.Length; s++)
            {
                if (thermoCoupleTable[s] > volt)
                {
                    result = (s * 10) - 60 - ((thermoCoupleTable[s] - volt) / (thermoCoupleTable[s] - thermoCoupleTable[s - 1]) * 10);
                    break;
                }
            }

            return zeroTemp + result;
        }

        const int NodePacketPostfix = 9;
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public NodePacketTypes ParsePacketFromNode (out string subType, out string description, out string header)
        {
            subType = "";
            header = "";
            description = "";

            NodePacketTypes nodePacketType = NodePacketTypes.Error;

            try
            {
                nodePacketType = (NodePacketTypes)GetByte();

                subType = nodePacketType.ToString();
                header = nodePacketType.ToString();

                if (nodePacketType == NodePacketTypes.ProbeScan)
                {
                    description += ", mac: " + GetHexString(6) + ", count: " + GetByte() + ", rssi: " + GetSByte() + ", channel: " + GetByte();
                    if (GetBool() == true)
                        description += " >";
                    else
                        description += " <";

                    return nodePacketType;
                }

                uint nodeID = 0;

                if ((nodePacketType != NodePacketTypes.BridgeAck) && (nodePacketType != NodePacketTypes.InstantPacket))
                    nodeID = GetUInt3();

                description += "ID: " + nodeID;

                if ((nodePacketType != NodePacketTypes.NetJoinRequest) && (nodePacketType != NodePacketTypes.InstantPacket) && (nodePacketType != NodePacketTypes.BridgeAck))
                    description += ", seq: " + GetByte();

                //header += "ID: " + nodeID;

                switch (nodePacketType)
                {
                    case NodePacketTypes.InitiateNodeRequest:
                        description += ", mac: " + GetULong() + ", type: " + GetByte() + ", app version: " + GetByte();
                        break;

                    case NodePacketTypes.InitiateNodeConfirmation:
                        description += ", mac: " + GetULong();
                        break;

                    case NodePacketTypes.NetJoinConfirmation:
                        description += ", node index: " + GetByte();
                        break;

                    case NodePacketTypes.RebootReport:
                        description += ", version: " + GetUShort() + ", reboot reason: " + GetByte() + ", temp: " + GetSByte();
                        byte battRaw = GetByte();
                        float batt = 2;

                        batt += (float)(battRaw << 1) / 256;

                        description += ", batt: " + batt.ToString("F2");

                        break;

                    case NodePacketTypes.NodeAck:
                        description += ", crc: " + GetUShort();
                        break;

                    case NodePacketTypes.Stats:
                        if (length - index > 12) //full stats
                        {
                            description += ", succes: " + GetByte() + ", failed: " + GetByte() + ", min rssi: " + GetSByte() + ", avg rssi: " + GetSByte() + ", max active ms: " + GetByte() + ", avg active ms: " + GetByte() + ", temp: " + GetSByte();
                            battRaw = GetByte();
                            batt = 2 + ((float)(battRaw << 1) / 256);

                            description += ", batt: " + batt.ToString("F2");

                            battRaw = GetByte();
                            batt = 2 + ((float)(battRaw << 1) / 256);

                            description += ", batt on end: " + batt.ToString("F2");
                        }
                        else
                        {
                            battRaw = GetByte();
                            batt = 2 + ((float)(battRaw << 1) / 256);

                            description += ", batt: " + batt.ToString("F2");

                            battRaw = GetByte();
                            batt = 2 + ((float)(battRaw << 1) / 256);

                            description += ", batt on end: " + batt.ToString("F2") + ", temp: " + GetSByte();
                        }

                        break;

                    case NodePacketTypes.Alarm:
                        description += ", mac: " + GetULong() + ", adc: " + GetByte();
                        break;

                    case NodePacketTypes.PulseCount:
                        description += ", t: " + GetByte() + ", pulse: " + GetUInt() + ", total: " + GetUInt() + ", s: " + GetByte() + ", VLow: " + GetUShort() + ", VHigh: " + GetUShort() + ", PLow: " + GetUShort() + ", PHigh: " + GetUShort() +
                                                    ", node time: " + GetTime().ToLocalTime().ToString("dd.MM.yy HH:mm:ss");
                        break;
                    case NodePacketTypes.PulseCountShort:
                        description += ", t: " + GetByte() + ", pulse: " + GetUInt();
                        break;

                    case NodePacketTypes.Pulse2ChannelCount:
                        description += ", ch1: " + GetUInt() + ", ch2: " + GetUInt() + ", minLowP: " + GetUShort() + ", minHighP: " + GetUShort();
                        break;

                    case NodePacketTypes.TermoCouple:
                        {
                            float temp = 0;
                            float hum = 0;

                            ushort[] adc = new ushort[4];

                            for (int s0 = 0; s0 < 4; s0++)
                            {
                                adc[s0] = GetUShort();
                            }

                            temp = GetUShort() * 165 / 65536 - 40;
                            hum = GetByte() * 100 / 256;

                            ushort VDDS = GetUShort();

                            for (int s0 = 0; s0 < 4; s0++)
                            {
                                float termoCoupleTemp = CalcTermocoupleTemp (4300f / 4096f * (adc[s0] - VDDS / 10f), temp);

                                description += ", adc" + s0 + ": " + adc[s0] + ", temp: " + termoCoupleTemp.ToString("F0");
                            }

                            description += ", temp: " + temp + ", hum: " + hum + ", vdds: " + VDDS;

                            break;
                        }
                    case NodePacketTypes.Temperature:
                        {
                            float temp = 0;
                            float hum = 0;

                            temp = GetUShort() * 165 / 65536 - 40;
                            hum = GetByte() * 100 / 256;

                            description += ", temp: " + temp + ", hum: " + hum;

                            break;
                        }
                    case NodePacketTypes.ADCValue:
                        description += ", adc: " + GetUShort() + ", adcL: " + GetUShort() + ", adcH: " + GetUShort() + ", count: " + GetUInt();
                        break;

                    case NodePacketTypes.Accel:
                        description += ", count: " + GetUInt() + ", motion: " + GetByte() + ", aX: " + GetSByte() + ", aY: " + GetSByte() + ", aZ: " + GetSByte();
                        break;

                    case NodePacketTypes.AccelContinue:
                        description += ", aX: " + GetSByte() + ", aY: " + GetSByte() + ", aZ: " + GetSByte();
                        break;

                    case NodePacketTypes.AccelFinish:
                        description += ", seconds: " + GetUInt();
                        break;

                    case NodePacketTypes.Error:
                        while (index < length - NodePacketPostfix)
                            description += ", code: " + GetByte() + ", value: " + GetInt();
                        break;

                    case NodePacketTypes.ReadFlashResponse:
                        description += ", data: " + GetHexString(length - index);
                        break;

                    case NodePacketTypes.WriteFlashFinishResponse:
                        description += ", crc: " + GetUShort();
                        break;

                    case NodePacketTypes.BridgeLog:
                        description += ", time: " + GetUInt() + ", code: " + GetByte() + ", value: " + GetInt();
                        //buffer.index = 0;
                        //result += ", hex: " + buffer.GetHexString(buffer.length - buffer.index);
                        break;

                    case NodePacketTypes.IrDAQueryResponse:
                        description += ", rxTimeout: " + GetByte();
                        GetByte();
                        byte len = GetByte();
                        description += ", len: " + len + ", query N: " + GetByte();
                        description += ", pulses count: '" + GetHexString(len) + "'";
                        description += ", data: '" + GetHexString(len) + "'";
                        //                        buffer.index = buffer.length - NodePacketPostfix;
                        break;

                    case NodePacketTypes.IrDAQueryShortResponse:
                        description += ", tariff: " + GetByte() + ", crcOk: " + GetBool() + ", data: '" + GetHexString(4) + "'";
                        break;

                    case NodePacketTypes.SwitcherState:
                        {
                            description += ", mask: ";
                            ushort val = GetUShort();
                            for (int s = 0; s < 4; s++)
                            {
                                for (int s1 = 0; s1 < 4; s1++)
                                {
                                    description += (val & 1).ToString();

                                    val = (ushort)(val >> 1);
                                }
                                description += " ";
                            }

                            description += ", state: ";
                            val = GetUShort();

                            for (int s = 0; s < 4; s++)
                            {
                                for (int s1 = 0; s1 < 4; s1++)
                                {
                                    description += (val & 1).ToString();

                                    val = (ushort)(val >> 1);
                                }
                                description += " ";
                            }
                            break;
                        }

                    case NodePacketTypes.RadioButtonPress:
                        description += ", N: " + GetByte();
                        description += ", data: '" + GetHexString(length - index - NodePacketPostfix) + "'";
                        break;

                    case NodePacketTypes.InstantPacket:
                        description += ", data: '" + GetHexString(length - index - NodePacketPostfix) + "'";
                        break;

                    case NodePacketTypes.BridgeAck:
                        description += ", another bridge diffMs: " + GetByte();
                        break;

                    case NodePacketTypes.InstantRxReport:
                        description += ", data: '" + GetHexString(length - index - NodePacketPostfix) + "'";
                        break;

                    case NodePacketTypes.HallRotate:
                        description += ", count: " + GetUInt();
                        break;

                    case NodePacketTypes.HallRotateContinue:
                        description += ", rmp: " + GetUShort();
                        break;

                    case NodePacketTypes.HallRotateFinish:
                        description += ", count: " + GetUInt() + ", seconds: " + GetUInt();
                        break;

                }

                if (index != length - NodePacketPostfix)
                {
                    index = 0;
                    description += " Error len '" + GetHexString(length - index - NodePacketPostfix) + "'";

                    index = length - NodePacketPostfix;

                }

                description += ", rssi bridge: " + GetSByte();
                description += ", time: " + GetTime().ToLocalTime().ToString("dd.MM.yy HH:mm:ss");
                description += ", bridge frame: " + GetUShort();
                description += ", channel: " + GetByte();
                description += ", diffMs: " + GetSByte();
            }
            catch
            {
                description = "Error parse: " + description + GetHexString(length - index);
            }

            return nodePacketType;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public NodePacketTypeAndSettingsType ParsePacketToNode(out string description, bool nodeIDInPacket = true)
        {
            NodePacketTypeAndSettingsType result = new NodePacketTypeAndSettingsType();

            description = "";

            try
            {
                if (nodeIDInPacket == true)
                {
                    uint nodeID = GetUInt3();
                    description += "Addr: " + nodeID;
                }

                NodePacketTypes nodePacketType = (NodePacketTypes)GetByte();

                result.nodePacketType = nodePacketType;

                uint password = 0;

                password = GetUInt();
                description += ", pass: " + password;

                description += " " + nodePacketType.ToString();

                switch (nodePacketType)
                {
                    case NodePacketTypes.InitiateNodeResponse:

                        description += ", mac: " + GetULong() + ", nodeID: " + GetUInt3() + ", sendAlivePeriod: " + GetUInt();

                        byte channelsCount = GetByte();

                        description += ", channelsCount: " + channelsCount;

                        for (int s = 0; s < channelsCount; s++)
                        {
                            description += ", " + s + ") frequency: " + GetUInt() + ", phy: " + GetByte();
                        }

                        break;

                    case NodePacketTypes.Settings:

                        NodeSettingsTypes nodeSettingsType = (NodeSettingsTypes)GetByte();
                        result.nodeSettingsType = nodeSettingsType;

                        description += ", " + nodeSettingsType;

                        if (nodeSettingsType == NodeSettingsTypes.AlivePeriod)
                            description += ": " + GetUInt();
                        else if (nodeSettingsType == NodeSettingsTypes.Time)
                            description += "";
                        else if (nodeSettingsType == NodeSettingsTypes.Tariff)
                        {
                            while (index < length)
                            {
                                description += ", s: " + GetTime().TimeOfDay + ", p: " + GetTime().TimeOfDay + ", n: " + GetByte();
                            }

                        }
                        else if (nodeSettingsType == NodeSettingsTypes.Stats)
                            description += ", everyXPacket: " + GetByte() + ", variant: " + GetByte();
                        else if (nodeSettingsType == NodeSettingsTypes.Threshold)
                            description += ": L: " + GetUShort() + ", H: " + GetUShort();
                        else if (nodeSettingsType == NodeSettingsTypes.SensorPeriod)
                            description += ", period: " + GetUInt() + ", finish: " + GetUInt();
                        else if (nodeSettingsType == NodeSettingsTypes.Accel)
                        {
                            description += ". nX: " + GetByte() + ", nY: " + GetByte() + ", nZ: " + GetByte() + ", threshold: " + GetByte();

                            int initArrayLen = GetByte();
                            description += ", initArrayLen: " + initArrayLen;

                            for (int s = 0; s < initArrayLen; s++)
                            {
                                description += ", " + GetHexString(2);
                            }
                        }
                        else if (nodeSettingsType == NodeSettingsTypes.MinPulsePeriod)
                            description += ": " + GetUInt();
                        else if (nodeSettingsType == NodeSettingsTypes.SensorPowerTimeout)
                            description += ": " + GetUShort();
                        else if (nodeSettingsType == NodeSettingsTypes.TuneMode)
                            description += ": " + GetByte();
                        else if (nodeSettingsType == NodeSettingsTypes.RxTimeout)
                            description += ": " + GetUShort();
                        else if (nodeSettingsType == NodeSettingsTypes.PeriodicalQueries)
                        {
                            byte len = GetByte();
                            description += ": l.len: " + len;
                            description += ", '" + GetHexString(len) + "'";

                            byte count = GetByte();
                            description += ", count: " + count + ". ";
                            for (int s = 0; s < count; s++)
                            {
                                description += s + ". r.cnt: " + GetByte();
                                len = GetByte();
                                description += ", len: " + len;
                                description += ", '" + GetHexString(len) + "'";
                            }

                        }
                        else if (nodeSettingsType == NodeSettingsTypes.IrDATiming)
                            description += ": start trigger period: " + GetUShort() + ", half pulse timeout: " + GetUShort();
                        else if (nodeSettingsType == NodeSettingsTypes.Button)
                            description += ": " + GetHexString(length - index);
                        else if (nodeSettingsType == NodeSettingsTypes.InitialChannelsState)
                        {
                            description += ", : ";
                            ushort val = GetUShort();

                            for (int s = 0; s < 4; s++)
                            {
                                for (int s1 = 0; s1 < 4; s1++)
                                {
                                    description += (val & 1).ToString();

                                    val = (ushort)(val >> 1);
                                }
                                description += " ";
                            }

                            description += ", period: " + GetByte();
                        }
                        else if (nodeSettingsType == NodeSettingsTypes.SensorDataMode)
                            description += ": " + GetByte();
                        else if (nodeSettingsType == NodeSettingsTypes.ADCBorders)
                            description += ", adcLow: " + GetUShort() + ", adcHigh: " + GetUShort();
                        else
                            description += "data: '" + GetHexString(length - index) + "'";

                        break;

                    case NodePacketTypes.RegisterNode:

                        description += "Error: , New addr: " + GetByte();
                        break;

                    case NodePacketTypes.ReadFlash:
                        description += " , page: " + GetByte() + " , offset: " + GetUShort() + " , len: " + GetByte();
                        break;

                    case NodePacketTypes.WriteFlashStart:
                        description += " , page: " + GetByte() + " , len: " + GetUShort();
                        break;

                    case NodePacketTypes.WriteFlashContinue:
                        description += ", pos: " + GetUShort() + " , len: " + (length - index).ToString() + ", data: " + GetHexString(length - index);
                        break;

                    case NodePacketTypes.WriteFlashFinish:
                        description += " , page: " + GetByte() + " , len: " + GetUShort() + " , crc: " + GetUShort();
                        break;

                    case NodePacketTypes.RebootIfCrcMatch:
                        description += " , page: " + GetByte() + " , len: " + GetUShort() + " , crc: " + GetUShort();
                        break;

                    case NodePacketTypes.SensorQuery:
                        description += ": " + GetHexString(length - index);
                        break;

                    case NodePacketTypes.NetJoinResponse: 
                        if (length - index == 11 - 4)
                            description += ", currFrame: " + GetUShort() + ", currentPseudoRandomValue: " + GetInt() + ", diff: " + GetSByte();
                        else if (length - index == 18 - 4)
                            description += ", nodeIndex: " + GetByte() + ", currFrame: " + GetUShort() + ", framesCount: " + GetUShort() + ", framePeriodMs: " + GetUShort() + ", pseudoRandomA: " + GetByte() + ", pseudoRandomB: " + GetByte() + ", currentPseudoRandomValue: " + GetInt() + ", diff: " + GetSByte();
                        else if (length - index == 2)
                            description += ", tryAgainTimeoutMs: " + GetUShort();


                        break;

                }

                if (index != length)
                {
                    index = 0;
                    description += "Error len. data '" + GetHexString(length - index) + "'";
                }
            }
            catch
            {
                index = 0;
                description = "Error parse: " + description + GetHexString(length);
            }

            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public BridgePacketAttributes ParsePacketToBridge(out string description, out string header, bool bridgePartInPacket = true)
        {
            description = "";
            header = "";

            BridgePacketAttributes result = new BridgePacketAttributes();

            try
            {
                if (bridgePartInPacket == true)
                {
                    BridgeParts bridgePart = (BridgeParts)GetByte();

                    result.bridgePart = bridgePart;
                    header = bridgePart.ToString();
                    description += bridgePart.ToString();
                }

                BridgeCommands bridgeCommand = (BridgeCommands)GetByte();
                result.bridgeCommand = bridgeCommand;

                BridgeSettingsTypes settingsTypes;

                header += ", " + bridgeCommand.ToString();
                description += ", " + bridgeCommand.ToString();

                switch (bridgeCommand)
                {
                    case BridgeCommands.AddNode:
                        description += " , NodeID: " + GetUInt3() + ", pass: " + GetUInt() + ", node index: " + GetByte();
                        break;

                    case BridgeCommands.ReadFlash:
                        description += " , page: " + GetByte() + " , offset: " + GetUShort() + " , len: " + GetByte();
                        break;

                    case BridgeCommands.WriteFlashStart:
                        description += " , page: " + GetByte() + " , len: " + GetUShort();
                        break;

                    case BridgeCommands.WriteFlashContinue:
                        description += " , len: " + (length - 3).ToString() + ", data: " + GetHexString(length - index);
                        break;

                    case BridgeCommands.WriteFlashFinish:
                        description += " , page: " + GetByte() + " , len: " + GetUShort() + " , crc: " + GetUShort();
                        break;

                    case BridgeCommands.RebootIfCrcMatch:
                        description += " , page: " + GetByte() + " , len: " + GetUShort() + " , crc: " + GetUShort();
                        break;

                    case BridgeCommands.ClearPacketsToNodeBuffer:
                        description += " , nodeID: " + GetUInt3() + " , packetType: " + (NodePacketTypes)GetByte();
                        break;

                    case BridgeCommands.Settings:


                        settingsTypes = (BridgeSettingsTypes)GetByte();

                        result.bridgeSettingsType = settingsTypes;
                        description += " : " + settingsTypes;

                        if (settingsTypes == BridgeSettingsTypes.APList)
                            description += ". APCount: " + GetByte() + " , AP name: '" + GetShortString() + "', key: '" + GetShortString() + "'";
                        else if (settingsTypes == BridgeSettingsTypes.ServerAddress)
                        {
                            byte serversCount = GetByte();
                            description += ". ServersCount: " + serversCount;

                            for (int s = 0; s < serversCount; s++)
                            {
                                UInt32 ip = (UInt32)IPAddress.NetworkToHostOrder((Int32)GetUInt());
                                UInt16 port = GetUShort();

                                IPAddress ipAddr = new IPAddress(ip);

                                description += ", " + s + ". ip: " + ipAddr + ", port: " + port;
                            }
                        }
                        else if (settingsTypes == BridgeSettingsTypes.StopLog)
                        {
                            description += ": " + GetByte();
                        }
                        else if (settingsTypes == BridgeSettingsTypes.Global)
                        {
                            byte globalSettings = GetByte();
                            byte debugLoop = GetByte();
                            description += ". uartBridge: " + (globalSettings & 1) / 1; //0b00000001
                            description += ", cam: " + (globalSettings & 2) / 2; //0b00000010
                            description += ", pingSmartphone: " + (globalSettings & 4) / 4; //0b00000100
                            description += ", oneChannelScan: " + (globalSettings & 8) / 8; //0b00001000
                            description += ", HAP: " + (globalSettings & 16) / 16; //0b00010000

                            description += ", debugLoop: " + debugLoop.ToString();
                        }
                        else if (settingsTypes == BridgeSettingsTypes.RadioProfile)
                        {
                            description += ". maxNodes: " + GetByte() + ", framePeriod: " + GetUShort() + ", pseudoRandomA: " + GetByte() + ", pseudoRandomB: " + GetByte();

                            byte channelsCount = GetByte();

                            description += ", channelsCount: " + channelsCount;

                            for (int s = 0; s < channelsCount; s++)
                            {
                                description += ", " + s + ") frequency: " + GetUInt() + ", phy: " + GetByte();
                            }
                        }
                        else if (settingsTypes == BridgeSettingsTypes.AlwaysSendFullJoinResponse)
                        {
                            description += ": " + GetByte();
                        }
                        else if (settingsTypes == BridgeSettingsTypes.Cam)
                        {
                            byte count = GetByte();
                            description += ". count: " + count;

                            for (int s = 0; s < count; s++)
                            {
                                description += ", " + s + ". dns name: '" + GetShortString() + "', req: '" + GetShortString().Replace((char)0x0a, '§').Replace((char)0x0d, '§') + ", port: " + GetUShort() + ", period: " + GetUInt();
                            }
                        }

                        break;

                    case BridgeCommands.SendRawPacket:
                        description += " , frequency: " + GetUInt() + " , phy: " + GetByte();
                        ushort len = GetByte();
                        description += " , len: " + len + " , data: " + GetHexString(len);
                        break;

                    case BridgeCommands.SendInstantPacket:
                        description += " , max attempts: " + GetByte() + " , data: " + GetHexString(length - index);
                        break;

                    case BridgeCommands.SetRawUartMode:
                        description += " , mode: " + GetBool();
                        break;

                    case BridgeCommands.ResetCC1310:
                        description += " , bootloader: " + GetBool();
                        break;

                    case BridgeCommands.SendRawUartPacket:
                        description += " , data: " + GetHexString(length - index);
                        break;

                    case BridgeCommands.SendCam:
                        description += " , num: " + GetByte();
                        break;

                    case BridgeCommands.SendHttpReq:
                        {
                            IPAddress ipAddr = new IPAddress((UInt32)IPAddress.NetworkToHostOrder((Int32)GetUInt()));

                            description += ", ip: " + ipAddr +", port: " + GetUShort() + ", req: '" + GetShortString().Replace((char)0x0a, '§').Replace((char)0x0d, '§') + "'";
                            break;
                        }
       

                    case BridgeCommands.AddCam:
                        description += ", dns name: '" + GetShortString() + "', req: '" + GetShortString().Replace((char)0x0a, '§').Replace((char)0x0d, '§') + "', port: " + GetUShort() + ", period: " + GetUInt();
                        break;

                }

                if (index != length)
                {
                    index = 0;
                    description += "Error len. data '" + GetHexString(length - index) + "'";

                }
            }
            catch
            {
                index = 0;
                description += "Error parse '" + GetHexString(length - index) + "'";
            }

            return result;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public BridgePacketTypes ParsePacketFromBridge (out string subType, out string description, out string header)
        {
            subType = ";";
            description = "";
            header = "";

            BridgePacketTypes result = BridgePacketTypes.BridgeError;

            try
            {
                BridgeParts bridgePart = (BridgeParts)GetByte();
                BridgePacketTypes bridgePacketType = (BridgePacketTypes)GetByte();

                result = bridgePacketType;

                header = bridgePart.ToString() + ", " + bridgePacketType.ToString();

                subType = bridgePacketType.ToString();

                description = "part: " + bridgePart;

                switch (bridgePacketType)
                {
                    case BridgePacketTypes.BridgeError:
                        description += ", code: " + GetByte() + ", value: " + GetInt();

                        int errorValue2 = GetInt();

                        if (errorValue2 != 0)
                            description += ", value2: " + errorValue2;

                        if (length > index)
                            description += ", data: " + GetHexString(length - index);

                        break;

                    case BridgePacketTypes.BridgeAck:
                        description += ", crc: " + GetUShort();
                        break;

                    case BridgePacketTypes.BridgeReadFlashResponse:
                        description += ", data: " + GetHexString(length - index);
                        break;

                    case BridgePacketTypes.BridgeVersionReport:
                        description += ", version: " + GetUShort() + ", reboot reason: " + GetByte();
                        break;

                    case BridgePacketTypes.BridgeWriteFlashFinishResponse:
                        description += ", crc: " + GetUShort();
                        break;

                    case BridgePacketTypes.ReadPacketsToNodeBufferResponse:
                        description += ", data " + GetUShort() + ": " + GetHexString(length - index);
                        break;

                    case BridgePacketTypes.RegisterNodeAck:
                        subType = bridgePacketType.ToString() + " " + (AckStages)GetByte();

                        description += ", mac: " + GetULong() + ", bridge addr: " + GetByte();
                        index += 5;
                        description += ", pass: " + GetFixedString(8) + ", addr: " + GetByte() + ", curr frame: " + GetByte() + ", frames count: " + GetByte() + ", frame period: " + GetUInt()
                                                          + ", rnd A: " + GetByte() + ", rnd B: " + GetByte() + ", rnd val: " + GetUInt();

                        byte channelsCount = GetByte();
                        description += ", ch count: " + channelsCount;
                        for (int s = 0; s < channelsCount; s++)
                        {
                            description += ", ch: " + s + ", freq: " + GetUInt() + ", phy: " + GetByte();
                        }
                        break;

                    case BridgePacketTypes.RawUartPacketAck:
                        description += ", data: " + GetHexString(length - index);
                        break;

                    case BridgePacketTypes.GetTempResponce:
                        description += ", chip temp: " + GetSByte();
                        byte battRaw = GetByte();
                        float batt = 2 + ((float)(battRaw << 1) / 256);

                        description += ", batt: " + batt.ToString("F2");

                        float temp = 0;
                        float hum = 0;

                        temp = GetUShort() * 165 / 65536 - 40;
                        hum = GetByte() * 100 / 256;

                        description += ", temp: " + temp + ", hum: " + hum;
                        break;
                }

                if (length != index)
                    description = "Error size. index:" + index + ", len: " + length + ". " + description;
                           
            }
            catch
            {
                index = 0;
                description += "Error parse '" + GetHexString(length - index) + "'";
            }

            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public uint ParsePacket(bool fromServer, out PacketTypes packetType, out string subType, out string description)
        {

            packetType = (PacketTypes)GetByte();
            subType = "";

            description = "";

            uint result = 0;

            try
            {
                switch (packetType)
                {
                    case PacketTypes.Auth:
                    case PacketTypes.ReAuth:

                        if (fromServer == false)
                            description += GetUShort().ToString();
                        else
                            description += GetTime().ToLocalTime().ToString("dd.MM.yy HH:mm:ss");
                        break;


                    case PacketTypes.Ping:

                        if (fromServer == false)
                        {
                            description += "clientTimeMS: " + GetInt().ToString();
                            description += ",    ping: " + GetUShort().ToString();
                        }
                        else
                        {
                            description += "clientTimeMS: " + GetInt().ToString();
                        }
                        break;


                    case PacketTypes.File:
                        {
                            PacketSubTypes subTypeByte = (PacketSubTypes)GetByte();

                            subType = subTypeByte.ToString();

                            if (fromServer == false)
                            {
                                if (subTypeByte == PacketSubTypes.FileStart)
                                {
                                    description += "fileType: " + GetByte().ToString();
                                    description += ",    fileSubType: " + GetInt().ToString();
                                    description += ",    fileName: " + GetShortString();
                                }
                            }
                            else
                            {
                                if (subTypeByte == PacketSubTypes.FileStart)
                                    description += ",    fileName: " + GetShortString() + ", len: " + GetUInt();
                                else if (subTypeByte == PacketSubTypes.FileEnd)
                                {
                                    description += ", len: " + GetUInt() + ", crc: " + GetULong() + ", cert: " + GetShortString() + ", sign: " + System.Text.Encoding.Default.GetString (GetBytesWithSize());

                                    //byte[] sign = GetBytesWithSize();
                                }
                                else
                                    description += ", data: " + GetHexString(length - index);
                                    
                            }

                            description += ",    part size: " + length;

                            break;
                        }

                    case PacketTypes.Cam:
                        {
                            PacketSubTypes subTypeByte = (PacketSubTypes)GetByte();

                            subType = subTypeByte.ToString();

                            if (fromServer == false)
                            {
                                if (subTypeByte == PacketSubTypes.CamEnd)
                                {
                                    description += "camNum: " + GetByte() + ", size: " + GetUInt();
                                }
                            }

                            description += ",    part size: " + length;

                            break;
                        }


                    case PacketTypes.Log:

                        byte logPart = GetByte();
                        byte logType = GetByte();

                        subType = ((LogTypes)logType).ToString();

                        if (logPart != 3)
                            description += "logPart: " + logPart + ",     ";
                        description += GetShortString();

                        break;

                    case PacketTypes.NodePacket:

                        if (fromServer == false)
                        {
                            BridgePacketTypes bridgePacketType = (BridgePacketTypes)GetByte();

                            if (bridgePacketType == BridgePacketTypes.PacketFromNode)
                            {
                                int oldIndex = index;
                                GetByte(); //skip packetType
                                result = GetUInt3();
                                index = oldIndex;

                                string header;
                                ParsePacketFromNode(out subType, out description, out header);

                            }
                            else if (bridgePacketType == BridgePacketTypes.Ack)
                            {
                                subType = bridgePacketType.ToString() + " " + (AckStages)GetByte();

                                ParsePacketToNode(out description);
                            }
                            else if (bridgePacketType == BridgePacketTypes.PacketToNodeSendReport)
                            {
                                subType = "Send report";

                                ParsePacketToNode(out description, false);
                            }
                            else
                                description += ", data: " + GetHexString(length - index);
                        }
                        else
                        {
                            ParsePacketToNode(out description);
                        }
                        break;

                    case PacketTypes.BridgePacket:

                        if (fromServer == false)
                        {
                            string header;
                            ParsePacketFromBridge(out subType, out description, out header);
                        }
                        else
                        {
                            string header;
                            ParsePacketToBridge(out description, out header);
                        }
                        break;

                    default:
                        description += ", data: " + GetHexString(length - index);
                        break;
                }

            }
            catch
            {
                index = 0;
                description = "Error parse: " + description + GetHexString(length);
                return 0;
            }

            return result;
        }


    }


}