using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;


    //ma18043のVRライブ用のPacketParseプログラムをHoloLens用に改良したもの
    //元ネタは\\192.168.1.3\personal\ma18043\研究関連\プログラム
    public static class Packet
    {
        public static List<string> GetHeader()
        {
            return new List<string>
            {
                "user", "poor", "rawData", "blink", "baRatio", "command", "trigger",
                "timeGeneData", "timeRecvOnClient", "timeRecvOnServer", "timeRecvOnApp", "timeGeneStimulus",
                "processedOnApp", "message"
            };
        }
        //don't use StreamReader
        public static Payload GetPayloadFrom(ref Stream ns)
        {
            while (CheckSyncCodeFrom(ref ns) == false) ;

            try
            {
                return new Payload(Interpolate(ParsePayloadFrom(ref ns)));
            }
            catch (ArgumentException e)
            {
                return new Payload();
            }
        }

        private static bool CheckSyncCodeFrom(ref Stream ns)
        {
            var resBytes = new byte[1];

            ns.Read(resBytes, 0, 1);
            if (resBytes[0] != PacketCode.Sync)
                return false;
            ns.Read(resBytes, 0, 1);
            if (resBytes[0] != PacketCode.Sync)
                return false;

            return true;
        }

        private static Dictionary<string, string> ParsePayloadFrom(ref Stream ns)
        {
            var payload = new Dictionary<string, string>();

            var resBytes = new byte[256];
            while (true)
            {
                // Receive recognition code
                ns.Read(resBytes, 0, 1);
                int code = resBytes[0];

                // check code of terminating packet and connection
                if (code == PacketCode.EndPacket)
                    break;
                if (code == PacketCode.EndConnection)
                {
                    payload.Add("endConnection", "1");
                    continue;
                }

                // Receive length of data
                ns.Read(resBytes, 0, 1);
                int lengthData = resBytes[0];
                // Receive data
                ns.Read(resBytes, 0, lengthData);
                var receiveData = Encoding.UTF8.GetString(resBytes, 0, lengthData);

                // Add to payload
                if (code == PacketCode.UserId)
                    payload.Add("user", receiveData);
                else if (code == PacketCode.Poor)
                    payload.Add("poor", receiveData);
                else if (code == PacketCode.RawData)
                    payload.Add("rawData", receiveData);
                else if (code == PacketCode.Blink)
                    payload.Add("blink", receiveData);
                else if (code == PacketCode.BaRatio)
                    payload.Add("baRatio", receiveData);
                else if (code == PacketCode.Command)
                    payload.Add("command", receiveData);
                else if (code == PacketCode.Trigger)
                    payload.Add("trigger", receiveData);
                else if (code == PacketCode.TimeGeneratedData)
                    payload.Add("timeGeneData", receiveData);
                else if (code == PacketCode.TimeRecvDataOnClient)
                    payload.Add("timeRecvOnClient", receiveData);
                else if (code == PacketCode.TimeRecvDataOnServer)
                    payload.Add("timeRecvOnServer", receiveData);
                else if (code == PacketCode.TimeRecvOnApp)
                    payload.Add("timeRecvOnApp", receiveData);
                else if (code == PacketCode.TimeGeneratedStimulus)
                    payload.Add("timeGeneStimulus", receiveData);
                else if (code == PacketCode.ProcessedOnApp)
                    payload.Add("processedOnApp", receiveData);
                else if (code == PacketCode.Message)
                    payload.Add("message", receiveData);
            }

            return payload;
        }

        private static Dictionary<string, string> Interpolate(IReadOnlyDictionary<string, string> payload)
        {
            return new Dictionary<string, string>
            {
                {"user", payload.ContainsKey("user") ? payload["user"] : null},
                {"poor", payload.ContainsKey("poor") ? payload["poor"] : null},
                {"rawData", payload.ContainsKey("rawData") ? payload["rawData"] : null},
                {"blink", payload.ContainsKey("blink") ? payload["blink"] : null},
                {"baRatio", payload.ContainsKey("baRatio") ? payload["baRatio"] : null},
                {"command", payload.ContainsKey("command") ? payload["command"] : null},
                {"trigger", payload.ContainsKey("trigger") ? payload["trigger"] : null},
                {"timeGeneData", payload.ContainsKey("timeGeneData") ? payload["timeGeneData"] : null},
                {"timeRecvOnClient", payload.ContainsKey("timeRecvOnClient") ? payload["timeRecvOnClient"] : null},
                {"timeRecvOnServer", payload.ContainsKey("timeRecvOnServer") ? payload["timeRecvOnServer"] : null},
                {"timeRecvOnApp", payload.ContainsKey("timeRecvOnApp") ? payload["timeRecvOnApp"] : null},
                {"timeGeneStimulus", payload.ContainsKey("timeGeneStimulus") ? payload["timeGeneStimulus"] : null},
                {"processedOnApp", payload.ContainsKey("processedOnApp") ? payload["processedOnApp"] : null},
                {"message", payload.ContainsKey("message") ? payload["message"] : null},
                {"endConnection", payload.ContainsKey("endConnection") ? payload["endConnection"] : null},
            };
        }

        public static byte[] MakePacket(Dictionary<string, string> payload)
        {
            return MakePacket(
                !string.IsNullOrEmpty(payload["user"]) ? payload["user"] : null,
                !string.IsNullOrEmpty(payload["poor"]) ? payload["poor"] : null,
                !string.IsNullOrEmpty(payload["rawData"]) ? payload["rawData"] : null,
                !string.IsNullOrEmpty(payload["blink"]) ? payload["blink"] : null,
                !string.IsNullOrEmpty(payload["baRatio"]) ? payload["baRatio"] : null,
                !string.IsNullOrEmpty(payload["command"]) ? payload["command"] : null,
                !string.IsNullOrEmpty(payload["trigger"]) ? payload["trigger"] : null,
                !string.IsNullOrEmpty(payload["timeGeneData"]) ? payload["timeGeneData"] : null,
                !string.IsNullOrEmpty(payload["timeRecvOnClient"]) ? payload["timeRecvOnClient"] : null,
                !string.IsNullOrEmpty(payload["timeRecvOnServer"]) ? payload["timeRecvOnServer"] : null,
                !string.IsNullOrEmpty(payload["timeRecvOnApp"]) ? payload["timeRecvOnApp"] : null,
                !string.IsNullOrEmpty(payload["timeGeneStimulus"]) ? payload["timeGeneStimulus"] : null,
                !string.IsNullOrEmpty(payload["processedOnApp"]) ? payload["processedOnApp"] : null,
                !string.IsNullOrEmpty(payload["message"]) ? payload["message"] : null,
                !string.IsNullOrEmpty(payload["endConnection"]) && int.Parse(payload["endConnection"]) > 0
            );
        }

        public static byte[] MakePacket(Dictionary<string, string> payload, bool endConnection)
        {
            return MakePacket(
                !string.IsNullOrEmpty(payload["user"]) ? payload["user"] : null,
                !string.IsNullOrEmpty(payload["poor"]) ? payload["poor"] : null,
                !string.IsNullOrEmpty(payload["rawData"]) ? payload["rawData"] : null,
                !string.IsNullOrEmpty(payload["blink"]) ? payload["blink"] : null,
                !string.IsNullOrEmpty(payload["baRatio"]) ? payload["baRatio"] : null,
                !string.IsNullOrEmpty(payload["command"]) ? payload["command"] : null,
                !string.IsNullOrEmpty(payload["trigger"]) ? payload["trigger"] : null,
                !string.IsNullOrEmpty(payload["timeGeneData"]) ? payload["timeGeneData"] : null,
                !string.IsNullOrEmpty(payload["timeRecvOnClient"]) ? payload["timeRecvOnClient"] : null,
                !string.IsNullOrEmpty(payload["timeRecvOnServer"]) ? payload["timeRecvOnServer"] : null,
                !string.IsNullOrEmpty(payload["timeRecvOnApp"]) ? payload["timeRecvOnApp"] : null,
                !string.IsNullOrEmpty(payload["timeGeneStimulus"]) ? payload["timeGeneStimulus"] : null,
                !string.IsNullOrEmpty(payload["processedOnApp"]) ? payload["processedOnApp"] : null,
                !string.IsNullOrEmpty(payload["message"]) ? payload["message"] : null,
                endConnection
            );
        }

        private static byte[] MakePacket(
            string userId = null, string poor = null, string rawData = null, string blink = null,
            string betaAlpha = null, string command = null, string trigger = null,
            string timeGeneData = null, string timeRecvOnClient = null, string timeRecvOnServer = null,
            string timeRecvOnApp = null, string timeGeneStimulus = null, string processedOnApp = null,
            string message = null, bool endConnection = false)
        {
            var payload = new ArrayList();
            if (userId != null)
                payload.AddRange(MakePacketByteElements(PacketCode.UserId, userId));
            if (poor != null)
                payload.AddRange(MakePacketByteElements(PacketCode.Poor, poor));
            if (rawData != null)
                payload.AddRange(MakePacketByteElements(PacketCode.RawData, rawData));
            if (blink != null)
                payload.AddRange(MakePacketByteElements(PacketCode.Blink, blink));
            if (betaAlpha != null)
                payload.AddRange(MakePacketByteElements(PacketCode.BaRatio, betaAlpha));
            if (command != null)
                payload.AddRange(MakePacketByteElements(PacketCode.Command, command));
            if (trigger != null)
                payload.AddRange(MakePacketByteElements(PacketCode.Trigger, trigger));
            if (timeGeneData != null)
                payload.AddRange(MakePacketByteElements(PacketCode.TimeGeneratedData, timeGeneData));
            if (timeRecvOnClient != null)
                payload.AddRange(MakePacketByteElements(PacketCode.TimeRecvDataOnClient, timeRecvOnClient));
            if (timeRecvOnServer != null)
                payload.AddRange(MakePacketByteElements(PacketCode.TimeRecvDataOnServer, timeRecvOnServer));
            if (timeRecvOnApp != null)
                payload.AddRange(MakePacketByteElements(PacketCode.TimeRecvOnApp, timeRecvOnApp));
            if (timeGeneStimulus != null)
                payload.AddRange(MakePacketByteElements(PacketCode.TimeGeneratedStimulus, timeGeneStimulus));
            if (processedOnApp != null)
                payload.AddRange(MakePacketByteElements(PacketCode.ProcessedOnApp, processedOnApp));
            if (message != null)
                payload.AddRange(MakePacketByteElements(PacketCode.Message, message));
            if (endConnection)
                payload.AddRange(new[] { PacketCode.EndConnection });

            var packetData = new ArrayList(new[] { PacketCode.Sync, PacketCode.Sync });
            packetData.AddRange(payload);
            packetData.AddRange(new[] { PacketCode.EndPacket });

            return (byte[])packetData.ToArray(typeof(byte));
        }

        private static byte[] MakePacketByteElements(byte code, string data)
        {
            var codeBytes = new[] { code };
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var lenDataBytes = new[] { BitConverter.GetBytes(dataBytes.Length)[0] };

            var payload = new ArrayList(codeBytes);
            payload.AddRange(lenDataBytes);
            payload.AddRange(dataBytes);

            return (byte[])payload.ToArray(typeof(byte));
        }

        public static Payload MakePayload(
            string userId = null, bool poor = false, double rawData = double.NaN, bool blink = false,
            double betaAlpha = double.NaN, string command = null, string trigger = null,
            long timeGenerated = 0, long timeRecvOnClient = 0, long timeRecvOnServer = 0, long timeRecvOnApp = 0,
            long timeGeneratedStimulus = 0, bool processedOnApp = false, string message = null,
            bool endConnection = false)
        {
            return new Payload(
                userId, poor, rawData, blink, betaAlpha, command, trigger, timeGenerated, timeRecvOnClient,
                timeRecvOnServer, timeRecvOnApp, timeGeneratedStimulus, processedOnApp, message, endConnection
            );
        }

        private static class PacketCode
        {
            public const byte Sync = 0xa0;
            public const byte EndPacket = 0xa1;
            public const byte EndConnection = 0xa2;

            public const byte RawData = 0xd0;
            public const byte BaRatio = 0xd1;

            public const byte Poor = 0xc0;
            public const byte Command = 0xc1;
            public const byte Blink = 0xc2;
            public const byte Trigger = 0xc3;

            public const byte TimeGeneratedData = 0xe0;
            public const byte TimeRecvDataOnClient = 0xe1;
            public const byte TimeRecvDataOnServer = 0xe2;
            public const byte TimeRecvOnApp = 0xe3;
            public const byte TimeGeneratedStimulus = 0xe4;

            public const byte UserId = 0xf0;
            public const byte Message = 0xf1;
            public const byte ProcessedOnApp = 0xf2;
        }
    }

    public class Payload
    {
        public string UserId { get; set; }
        public MainData Data { get; }
        public bool ProcessedOnApp { get; set; }
        public TimeData Time { get; }
        public string Message { get; set; }
        public bool EndConnection { get; }

        public Payload(
            string userId = null, bool poor = false, double rawData = double.NaN, bool blink = false,
            double betaAlpha = double.NaN, string command = null, string trigger = null,
            long timeGenerated = 0, long timeRecvOnClient = 0, long timeRecvOnServer = 0, long timeRecvOnApp = 0,
            long timeGeneratedStimulus = 0, bool processedOnApp = false, string message = null,
            bool endConnection = false)
        {
            UserId = userId;
            Data = new MainData(poor, rawData, blink, betaAlpha, command, trigger);
            ProcessedOnApp = processedOnApp;
            Time = new TimeData(timeGenerated, timeRecvOnClient, timeRecvOnServer,
                timeRecvOnApp, timeGeneratedStimulus);
            Message = message;
            EndConnection = endConnection;
        }

        public Payload(IReadOnlyDictionary<string, string> payload)
        {
            var user = payload["user"];

            var poor =
                !string.IsNullOrEmpty(payload["poor"]) && (int.Parse(payload["poor"]) > 0);
            var rawData =
                !string.IsNullOrEmpty(payload["rawData"]) ? double.Parse(payload["rawData"]) : double.NaN;
            var blink =
                !string.IsNullOrEmpty(payload["blink"]) && (int.Parse(payload["blink"]) > 0);

            var betaAlpha =
                !string.IsNullOrEmpty(payload["baRatio"]) ? double.Parse(payload["baRatio"]) : double.NaN;
            var command = payload["command"];
            var trigger = payload["trigger"];

            var timeGeneData =
                !string.IsNullOrEmpty(payload["timeGeneData"]) ? long.Parse(payload["timeGeneData"]) : 0;
            var timeRecvOnClient =
                !string.IsNullOrEmpty(payload["timeRecvOnClient"]) ? long.Parse(payload["timeRecvOnClient"]) : 0;
            var timeRecvOnServer =
                !string.IsNullOrEmpty(payload["timeRecvOnServer"]) ? long.Parse(payload["timeRecvOnServer"]) : 0;
            var timeRecvOnApp =
                !string.IsNullOrEmpty(payload["timeRecvOnApp"]) ? long.Parse(payload["timeRecvOnApp"]) : 0;
            var timeGeneStimulus =
                !string.IsNullOrEmpty(payload["timeGeneStimulus"]) ? long.Parse(payload["timeGeneStimulus"]) : 0;
            var processedOnApp =
                !string.IsNullOrEmpty(payload["processedOnApp"]) && (int.Parse(payload["processedOnApp"]) > 0);
            var message = payload["message"];

            var endConnection =
                !string.IsNullOrEmpty(payload["endConnection"]) && (int.Parse(payload["endConnection"]) > 0);

            UserId = user;
            Data = new MainData(poor, rawData, blink, betaAlpha, command, trigger);
            ProcessedOnApp = processedOnApp;
            Time = new TimeData(timeGeneData, timeRecvOnClient, timeRecvOnServer,
                timeRecvOnApp, timeGeneStimulus);
            Message = message;
            EndConnection = endConnection;
        }

        public Dictionary<string, string> ToDict()
        {
            return new Dictionary<string, string>
            {
                {"user", UserId},
                {"poor", $"{(Data.Poor ? 1 : 0)}"},
                {"rawData", double.IsNaN(Data.RawData) ? null : $"{Data.RawData}"},
                {"blink", $"{(Data.Blink ? 1 : 0)}"},
                {"baRatio", double.IsNaN(Data.BetaAlpha) ? null : $"{Data.BetaAlpha}"},
                {"command", Data.Command},
                {"trigger", Data.Trigger},
                {"timeGeneData", Time.GeneratedData == 0 ? null : $"{Time.GeneratedData}"},
                {"timeRecvOnClient", Time.RecvOnClient == 0 ? null : $"{Time.RecvOnClient}"},
                {"timeRecvOnServer", Time.RecvOnServer == 0 ? null : $"{Time.RecvOnServer}"},
                {"timeRecvOnApp", Time.RecvOnApp == 0 ? null : $"{Time.RecvOnApp}"},
                {"timeGeneStimulus", Time.GeneratedStimulus == 0 ? null : $"{Time.GeneratedStimulus}"},
//                {"processedOnApp", $"{(ProcessedOnApp ? 1 : 0)}"},
                {"processedOnApp", ProcessedOnApp ? "1" : null},
                {"message", Message},
                {"endConnection", $"{(EndConnection ? 1 : 0)}"},
            };
        }

        public override string ToString()
        {
            var dict = ToDict();
            var str = dict.Keys.Aggregate("[", (current, key) => current + $"{key}: {dict[key]}, ");
            str += "]";

            return str;
        }

        public class MainData
        {
            public MainData(bool poor = false, double rawData = double.NaN, bool blink = false,
                double betaAlpha = double.NaN, string command = null, string trigger = null)
            {
                Poor = poor;
                RawData = rawData;
                Blink = blink;
                BetaAlpha = betaAlpha;
                Command = command;
                Trigger = trigger;
            }

            public bool Poor { get; }
            public double RawData { get; }
            public bool Blink { get; }
            public double BetaAlpha { get; }
            public string Command { get; set; }
            public string Trigger { get; set; }
        }

        public class TimeData
        {
            public TimeData(long generatedData = 0, long recvOnClient = 0, long recvOnServer = 0,
                long recvOnApp = 0, long generatedStimulus = 0)
            {
                GeneratedData = generatedData;
                RecvOnClient = recvOnClient;
                RecvOnServer = recvOnServer;
                RecvOnApp = recvOnApp;
                GeneratedStimulus = generatedStimulus;
            }

            public long GeneratedData { get; set; }
            public long RecvOnClient { get; set; }
            public long RecvOnServer { get; set; }
            public long RecvOnApp { get; set; }
            public long GeneratedStimulus { get; set; }
        }
    }