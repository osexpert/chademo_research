using System;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using CanlogParser2;

namespace CanlogParser
{
    internal class Program
    {

        public static string PrintDiff<T>(T oldValue, T newValue) where T : struct, Enum
        {
            ulong oldBits = Convert.ToUInt64(oldValue);
            ulong newBits = Convert.ToUInt64(newValue);

            var added = (T)Enum.ToObject(typeof(T), newBits & ~oldBits);
            var removed = (T)Enum.ToObject(typeof(T), oldBits & ~newBits);

            string res = "";
            if (!added.Equals(default(T)))
                res += $"+{added} ";

            if (!removed.Equals(default(T)))
                res += $"-{removed}";

            return res;
        }

        interface A
        {
            string GetGroupings();
        }
        interface B
        {
            string GetGroupings();
        }
        interface IBLBC : A, B
        {
        }

        static void Main(string[] args)
        {
            //LedBlink.Maine();

            BatteryVoltage.entry(args);

            Console.WriteLine("Hello, World!");

            // charge and discharge
            var fileName = @"..\..\..\..\CanLogs\v2h_startup__charge_ffrom_solar_excess_for_a_while_then_turn_on_oven_to_create_load_then_switch_off_load__then_end_session.csv";

            //var fileName = @"..\..\..\..\CanLogs\av50_old.txt";
            //var fileName = @"..\..\..\..\CanLogs\start.candump.txt";
            //var fileName = @"..\..\..\..\CanLogs\logs.candump.txt";

            // discharge support, but not discharging (seems like trickle charging)
            //var fileName = @"..\..\..\..\CanLogs\nissan-leaf-chademo-start-stop.csv";

            //var fileName = @"..\..\..\..\CanLogs\non working charger can log.csv";
            //var fileName = @"..\..\..\..\CanLogs\ZE1-chademo-starting-and-charging.csv";
            //var fileName = @"..\..\..\..\CanLogs\2025\InstavoltBYDFail.csv";

            Console.WriteLine(fileName);

            var lines = File.ReadAllLines(fileName);

            Msg100 lastmsg100 = null;
            Msg101 lastmsg101 = null;
            Msg102 lastmsg102 = null;
            Msg108 lastmsg108 = null;
            Msg109 lastmsg109 = null;
            Msg200 lastmsg200 = null;
            Msg208 lastmsg208 = null;
            Msg209 lastmsg209 = null;

            List<IMsg> messages = new();

            List<(int msg, byte[] data, string time)> dataLines = GetMessagesAuto(lines);

            int unchanged100 = 0;

            foreach (var line in dataLines)
            {
                (int msg, byte[] data, string time) = line;
                switch (msg)
                {
                    case 0x100:
                        var m100 = Parse100(data, time);
                        messages.Add(m100);
                        bool printed = false;
                        if (lastmsg100 == null || !m100.Equals(lastmsg100))
                        {
                            foreach (var lin in m100.ToDiffLines(lastmsg100))
                            {
                                if (unchanged100 > 0)
                                {
                                    Console.WriteLine("".PadRight(unchanged100, '>'));
                                    unchanged100 = 0;
                                }

                                Console.WriteLine(lin);
                                printed = true;
                            }
                        }

                        if (printed)
                            unchanged100 = 0;
                        else
                            unchanged100++;

                        lastmsg100 = m100;
                        break;
                    case 0x101:
                        var m101 = Parse101(data, time);
                        messages.Add(m101);
                        if (lastmsg101 == null || !m101.Equals(lastmsg101))
                        {
                            foreach (var lin in m101.ToDiffLines(lastmsg101))
                            {
                                if (unchanged100 > 0)
                                {
                                    Console.WriteLine("".PadRight(unchanged100, '>'));
                                    unchanged100 = 0;
                                }

                                Console.WriteLine(lin);
                            }
                        }
                        lastmsg101 = m101;
                        break;
                    case 0x102:
                        var m102 = Parse102(data, time);
                        messages.Add(m102);
                        if (lastmsg102 == null || !m102.Equals(lastmsg102))
                        {
                            foreach (var lin in m102.ToDiffLines(lastmsg102))
                            {
                                if (unchanged100 > 0)
                                {
                                    Console.WriteLine("".PadRight(unchanged100, '>'));
                                    unchanged100 = 0;
                                }

                                Console.WriteLine(lin);
                            }
                        }
                        lastmsg102 = m102;
                        break;

                    case 0x108:
                        var m108 = Parse108(data, time);
                        messages.Add(m108);
                        if (lastmsg108 == null || !m108.Equals(lastmsg108))
                        {
                            foreach (var lin in m108.ToDiffLines(lastmsg108))
                            {
                                if (unchanged100 > 0)
                                {
                                    Console.WriteLine("".PadRight(unchanged100, '>'));
                                    unchanged100 = 0;
                                }

                                Console.WriteLine(lin);
                            }
                        }
                        lastmsg108 = m108;
                        break;
                    case 0x109:
                        var m109 = Parse109(data, time);
                        messages.Add(m109);
                        if (lastmsg109 == null || !m109.Equals(lastmsg109))
                        {
                            foreach (var lin in m109.ToDiffLines(lastmsg109))
                            {
                                if (unchanged100 > 0)
                                {
                                    Console.WriteLine("".PadRight(unchanged100, '>'));
                                    unchanged100 = 0;
                                }

                                Console.WriteLine(lin);
                            }
                        }
                        lastmsg109 = m109;
                        break;
                    case 0x200:
                        var m200 = Parse200(data, time);
                        messages.Add(m200);
                        if (lastmsg200 == null || !m200.Equals(lastmsg200))
                        {
                            foreach (var lin in m200.ToDiffLines(lastmsg200))
                            {
                                if (unchanged100 > 0)
                                {
                                    Console.WriteLine("".PadRight(unchanged100, '>'));
                                    unchanged100 = 0;
                                }

                                Console.WriteLine(lin);
                            }
                        }
                        lastmsg200 = m200;
                        break;
                    case 0x208:
                        var m208 = Parse208(data, time);
                        messages.Add(m208);
                        if (lastmsg208 == null || !m208.Equals(lastmsg208))
                        {
                            foreach (var lin in m208.ToDiffLines(lastmsg208))
                            {
                                if (unchanged100 > 0)
                                {
                                    Console.WriteLine("".PadRight(unchanged100, '>'));
                                    unchanged100 = 0;
                                }

                                Console.WriteLine(lin);
                            }
                        }
                        lastmsg208 = m208;
                        break;
                    case 0x209:
                        var m209 = Parse209(data, time);
                        messages.Add(m209);
                        if (lastmsg209 == null || !m209.Equals(lastmsg209))
                        {
                            foreach (var lin in m209.ToDiffLines(lastmsg209))
                            {
                                if (unchanged100 > 0)
                                {
                                    Console.WriteLine("".PadRight(unchanged100, '>'));
                                    unchanged100 = 0;
                                }

                                Console.WriteLine(lin);
                            }
                        }
                        lastmsg209 = m209;
                        break;

                }

            }

            Console.WriteLine("Farewell, World!");
        }

        private static List<(int msg, byte[] data, string time)> GetMessagesAuto(string[] lines)
        {
            var firstLine = lines[0];
            if (firstLine == "Time Stamp,ID,Extended,Dir,Bus,LEN,D1,D2,D3,D4,D5,D6,D7,D8")
                return lines.Skip(1).Select(l => GetMessageCsv_Time_Stamp(l)).ToList();

            if (firstLine == "Index;System Time;Time Stamp;Channel;Direction;ID;Type;Format;Len;Data")
                return lines.Skip(1).Select(l => GetMessageCsv_Index(l)).ToList();

            if (firstLine.StartsWith("Rcvd msgID:"))
                return lines.Select(l => GetMessage_Rcvd_msgID(l)).ToList();

            if (lines.Take(10).Where(l => l.StartsWith(" (")).Any())
                return lines.Where(l => l.StartsWith(" (")).Select(l => GetMessageParaTime(l)).ToList();

            throw new Exception("unknown canlog format");
        }


        private static Msg101 Parse101(byte[] data, string time) => new Msg101(data, time);
        private static Msg102 Parse102(byte[] data, string time) => new Msg102(data, time);
        private static Msg108 Parse108(byte[] data, string time) => new Msg108(data, time);
        private static Msg109 Parse109(byte[] data, string time) => new Msg109(data, time);
        private static Msg100 Parse100(byte[] data, string time) => new Msg100(data, time);
        private static Msg200 Parse200(byte[] data, string time) => new Msg200(data, time);
        private static Msg208 Parse208(byte[] data, string time) => new Msg208(data, time);
        private static Msg209 Parse209(byte[] data, string time) => new Msg209(data, time);

        class Msg100 : IMsg
        {
            public byte MinimumChargeCurrent;
            public ushort MaxChargeVoltage;

            /// <summary>
            /// Initially seen as eg. 240....before changing to 100.
            /// </summary>
            public byte ChargingRatePercent;

            int IMsg.Id => 0x100;
            public string Time { get; }

            public Msg100(byte[] data, string time)
            {
                Time = time;

                MinimumChargeCurrent = data[0];

                Program.AssertZero(data[1]);
                Program.AssertZero(data[2]);
                Program.AssertZero(data[3]);

                MaxChargeVoltage = (ushort)(data[4] | data[5] << 8);
                // Allways 100%...
                ChargingRatePercent = data[6]; // Charged rate reference constant, 100% fixed

                Program.AssertZero(data[7]);
            }

            public List<string> ToDiffLines(Msg100 other)
            {
                var lines = new List<string>();

                if (other == null)
                {
                    lines.Add($"{Time}: 100.MinimumChargeCurrent (null) -> {this.MinimumChargeCurrent}");
                    lines.Add($"{Time}: 100.MaxChargeVoltage (null) -> {this.MaxChargeVoltage}");
                    lines.Add($"{Time}: 100.ChargingRatePercent (null) -> {this.ChargingRatePercent}");
                }
                else
                {
                    if (this.MinimumChargeCurrent != other.MinimumChargeCurrent)
                        lines.Add($"{Time}: 100.MinimumChargeCurrent {other.MinimumChargeCurrent} -> {this.MinimumChargeCurrent}");

                    if (this.MaxChargeVoltage != other.MaxChargeVoltage)
                        lines.Add($"{Time}: 100.MaxChargeVoltage {other.MaxChargeVoltage} -> {this.MaxChargeVoltage}");

                    if (this.ChargingRatePercent != other.ChargingRatePercent)
                        lines.Add($"{Time}: 100.ChargingRatePercent {other.ChargingRatePercent} -> {this.ChargingRatePercent}");
                }

                return lines;
            }

            public override string ToString()
            {
                return $"Msg:100, MinimumChargeCurrent:{MinimumChargeCurrent}, MaxChargeVoltage:{MaxChargeVoltage}, ChargingRatePercent:{ChargingRatePercent}";
            }
            public override bool Equals(object obj)
            {
                var other = (Msg100)obj;
                return this.ChargingRatePercent == other.ChargingRatePercent &&
                    this.MinimumChargeCurrent == other.MinimumChargeCurrent &&
                    this.MaxChargeVoltage == other.MaxChargeVoltage;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        private static void AssertZero(byte v)
        {
            if (v != 0)
                throw new Exception("Not zero");
        }

        interface IMsg
        {
            string Time { get; }
            int Id { get; }
        }

        class Msg101 : IMsg
        {
            int IMsg.Id => 0x101;

            /// <summary>
            /// 0xff: use MaxChargingTimeMins instead
            /// </summary>
            public byte MaxChargingTime10Sec;

            /// <summary>
            /// This value is reflected in the charger message 109.RemainingChargeTimeMins. Charger will take this as initial mins and count it down.
            /// </summary>
            public byte MaxChargingTimeMins;

            public byte EstimatedChargingTimeMins;

            public float BatteryCapacityKwh;

            public string Time { get; }

            public Msg101(byte[] data, string time)
            {
                Time = time;
                Program.AssertZero(data[0]);

                MaxChargingTime10Sec = data[1];
                MaxChargingTimeMins = data[2]; // = 90; //ask for how long of a charge? Charging will be forceably stopped if we hit this time (from any side?)

                // estimated charging time mins
                // Added in Chademo 1.0.1 so the (old) leaf does not exmit this? Seems to always be 0?
                EstimatedChargingTimeMins = data[3];// = 60; //how long we think the charge will actually take

                Program.AssertZero(data[4]);

                // 0 first, then 55, then correct :-)
                BatteryCapacityKwh = (ushort)(data[5] | data[6] << 8) * 0.11f;

                Program.AssertZero(data[7]);
            }

            public List<string> ToDiffLines(Msg101 other)
            {
                var lines = new List<string>();

                if (other == null)
                {
                    lines.Add($"{Time}: 101.MaxChargingTime10Sec (null) -> {this.MaxChargingTime10Sec}");
                    lines.Add($"{Time}: 101.MaxChargingTimeMins (null) -> {this.MaxChargingTimeMins}");
                    lines.Add($"{Time}: 101.EstimatedChargingTimeMins (null) -> {this.EstimatedChargingTimeMins}");
                    lines.Add($"{Time}: 101.BatteryCapacityKwh (null) -> {this.BatteryCapacityKwh}");
                }
                else
                {
                    if (this.MaxChargingTime10Sec != other.MaxChargingTime10Sec)
                        lines.Add($"{Time}: 101.MaxChargingTime10Sec {other.MaxChargingTime10Sec} -> {this.MaxChargingTime10Sec}");

                    if (this.MaxChargingTimeMins != other.MaxChargingTimeMins)
                        lines.Add($"{Time}: 101.MaxChargingTimeMins {other.MaxChargingTimeMins} -> {this.MaxChargingTimeMins}");

                    if (this.EstimatedChargingTimeMins != other.EstimatedChargingTimeMins)
                        lines.Add($"{Time}: 101.EstimatedChargingTimeMins {other.EstimatedChargingTimeMins} -> {this.EstimatedChargingTimeMins}");

                    if (this.BatteryCapacityKwh != other.BatteryCapacityKwh)
                        lines.Add($"{Time}: 101.BatteryCapacityKwh {other.BatteryCapacityKwh} -> {this.BatteryCapacityKwh}");
                }

                return lines;
            }

            public override string ToString()
            {
                return $"Msg:101, MaxChargingTime10Sec:{MaxChargingTime10Sec}" +
                    $", MaxChargingTimeMins:{MaxChargingTimeMins}, EstimatedChargingTimeMins:{EstimatedChargingTimeMins}, BatteryCapacityKwh:{BatteryCapacityKwh}";
            }

            public override bool Equals(object obj)
            {
                var other = (Msg101)obj;
                return this.MaxChargingTimeMins == other.MaxChargingTimeMins &&
                    this.MaxChargingTime10Sec == other.MaxChargingTime10Sec &&
                    this.EstimatedChargingTimeMins == other.EstimatedChargingTimeMins &&
                    this.BatteryCapacityKwh == other.BatteryCapacityKwh;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        class Msg102 : IMsg
        {
            int IMsg.Id => 0x102;

            public byte ChademoRawVersion;
            /// <summary>
            /// Typically max battery voltage + 10, so its not _really_ the target voltage. Its more like the charging voltage.
            /// </summary>
            public ushort TargetVoltage;

            public byte AskingAmps;

            public CarFaults Faults;
            public CarStatus Status;

            /// <summary>
            /// Not sure if this is correct? All my logs show 73!!! I wonder if this is a lie?
            /// Someone send values way over 100 too...
            /// It generally can not be truested for logic
            /// </summary>
            public byte SocPercent;

            public string Time { get; }

            public Msg102(byte[] data, string time)
            {
                Time = time;
                ChademoRawVersion = data[0];// 1: v0.9, 2: v1.0

                // This is the charging voltage? I think so...
                TargetVoltage = (ushort)(data[1] | data[2] << 8);
                // Leaf 410 typical

                AskingAmps = data[3];
                Faults = (CarFaults)data[4];
                Status = (CarStatus)data[5];
                //uint8_t kiloWattHours = msg.data[6];

                // Start as 3, then 1, then to the real value:-)
                SocPercent = data[6];

                Program.AssertZero(data[7]);
            }

            public List<string> ToDiffLines(Msg102 other)
            {
                var lines = new List<string>();

                if (other == null)
                {
                    lines.Add($"{Time}: 102.ChademoRawVersion (null) -> {this.ChademoRawVersion}");
                    lines.Add($"{Time}: 102.TargetVoltage (null) -> {this.TargetVoltage}");
                    lines.Add($"{Time}: 102.AskingAmps (null) -> {this.AskingAmps}");
                    lines.Add($"{Time}: 102.Faults (null) -> {this.Faults}");
                    lines.Add($"{Time}: 102.Status (null) -> {this.Status}");
                    lines.Add($"{Time}: 102.SocPercent (null) -> {this.SocPercent}");
                }
                else
                {
                    if (this.ChademoRawVersion != other.ChademoRawVersion)
                        lines.Add($"{Time}: 102.ChademoRawVersion {other.ChademoRawVersion} -> {this.ChademoRawVersion}");

                    if (this.TargetVoltage != other.TargetVoltage)
                        lines.Add($"{Time}: 102.TargetVoltage {other.TargetVoltage} -> {this.TargetVoltage}");

                    if (this.AskingAmps != other.AskingAmps)
                        lines.Add($"{Time}: 102.AskingAmps {other.AskingAmps} -> {this.AskingAmps}");

                    if (this.Faults != other.Faults)
                        lines.Add($"{Time}: 102.Faults {other.Faults}: {PrintDiff(other.Faults, this.Faults)}");

                    if (this.Status != other.Status)
                        lines.Add($"{Time}: 102.Status {other.Status}: {PrintDiff(other.Status, this.Status)}");

                    if (this.SocPercent != other.SocPercent)
                        lines.Add($"{Time}: 102.SocPercent {other.SocPercent} -> {this.SocPercent}");
                }

                return lines;
            }

            

            public override string ToString()
            {
                return $"Msg:102, ChademoRawVersion:{ChademoRawVersion}, TargetVoltage:{TargetVoltage}, AskingAmps:{AskingAmps}, Faults:{Faults}" +
                    $", Status:{Status}, SocPercent:{SocPercent}";
            }
            public override bool Equals(object obj)
            {
                var other = (Msg102)obj;
                return this.ChademoRawVersion == other.ChademoRawVersion &&
                    this.TargetVoltage == other.TargetVoltage &&
                    this.AskingAmps == other.AskingAmps &&
                    this.Faults == other.Faults &&
                    this.Status == other.Status &&
                    this.SocPercent == other.SocPercent;

            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        class Msg108 : IMsg
        {
            /// <summary>
            /// Car should perform welding detection after charging
            /// </summary>
            public bool PerformWeldingDetection;

            public ushort AvailableOutputVoltage;
            public byte AvailableOutputCurrent;

            /// <summary>
            /// Threshold voltage for terminating the charging process to protect the car battery
            /// BUT WHY IS THE CHARGER SENDING THIS TO THE CAR????? This is info the car is telling the changer.....
            /// </summary>
            public ushort ThresholdVoltage;

            int IMsg.Id => 0x108;
            public string Time { get; }

            public Msg108(byte[] data, string time)
            {
                Time = time;
                // refered to as a bit in Using-OCPP-with-CHAdeMO.pdf: H'108.0.0: Welding detection Identifier = 0 (v2.0.1))

                PerformWeldingDetection = data[0] == 1;

                AvailableOutputVoltage = (ushort)(data[1] | data[2] << 8);
                AvailableOutputCurrent = data[3];

                // Threshold voltage for terminating the charging process to protect the car battery
                // BUT WHY IS THE CHARGER SENDING THIS TO THE CAR?????
                ThresholdVoltage = (ushort)(data[4] | data[5] << 8);

                Program.AssertZero(data[6]);
                Program.AssertZero(data[7]);
            }

            public List<string> ToDiffLines(Msg108 other)
            {
                var lines = new List<string>();

                if (other == null)
                {
                    lines.Add($"{Time}: 108.PerformWeldingDetection (null) -> {this.PerformWeldingDetection}");
                    lines.Add($"{Time}: 108.AvailableOutputVoltage (null) -> {this.AvailableOutputVoltage}");
                    lines.Add($"{Time}: 108.AvailableOutputCurrent (null) -> {this.AvailableOutputCurrent}");
                    lines.Add($"{Time}: 108.ThresholdVoltage (null) -> {this.ThresholdVoltage}");
                }
                else
                {
                    if (this.PerformWeldingDetection != other.PerformWeldingDetection)
                        lines.Add($"{Time}: 108.PerformWeldingDetection {other.PerformWeldingDetection} -> {this.PerformWeldingDetection}");

                    if (this.AvailableOutputVoltage != other.AvailableOutputVoltage)
                        lines.Add($"{Time}: 108.AvailableOutputVoltage {other.AvailableOutputVoltage} -> {this.AvailableOutputVoltage}");

                    if (this.AvailableOutputCurrent != other.AvailableOutputCurrent)
                        lines.Add($"{Time}: 108.AvailableOutputCurrent {other.AvailableOutputCurrent} -> {this.AvailableOutputCurrent}");

                    if (this.ThresholdVoltage != other.ThresholdVoltage)
                        lines.Add($"{Time}: 108.ThresholdVoltage {other.ThresholdVoltage} -> {this.ThresholdVoltage}");
                }

                return lines;
            }

            public override string ToString()
            {
                return $"Msg:108, CarWeldingDetection={PerformWeldingDetection}, AvailableOutputVoltage={AvailableOutputVoltage}" +
                    $", AvailableOutputCurrent={AvailableOutputCurrent}, ThresholdVoltage={ThresholdVoltage}";
            }

            public override bool Equals(object obj)
            {
                var other = (Msg108)obj;
                return this.PerformWeldingDetection == other.PerformWeldingDetection &&
                    this.AvailableOutputCurrent == other.AvailableOutputCurrent &&
                    this.AvailableOutputVoltage == other.AvailableOutputVoltage &&
                    this.ThresholdVoltage == other.ThresholdVoltage;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        class Msg109 : IMsg
        {
            public byte ChademoRawVersion;

            public ushort OutputVoltage;
            public ushort OutputCurrent;
            private bool DischargeCompatitible;

            /// <summary>
            /// If 0xFF, use RemainingChargeTimeMins
            /// </summary>
            public byte RemainingChargeTime10Sec;

            /// <summary>
            /// Remaining time to charge. Charger decrement this until 0 and then charger forcefully stops.
            /// </summary>
            public byte RemainingChargeTimeMins;

            public ChargerStatus Status;

            int IMsg.Id => 0x109;
            public string Time { get; }

            public Msg109(byte[] data, string time)
            {
                Time = time;
                ChademoRawVersion = data[0]; // 1:0.9 2:1.0 etc.

                OutputVoltage = (ushort)(data[1] | data[2] << 8);

                OutputCurrent = data[3];

                // nissan-leaf-chademo-start-stop.csv HAS VALUE 1 HERE
                DischargeCompatitible = data[4] == 1;

                Status = (ChargerStatus)data[5];

                RemainingChargeTime10Sec = data[6];
                RemainingChargeTimeMins = data[7];
            }

            public List<string> ToDiffLines(Msg109 other)
            {
                var lines = new List<string>();

                if (other == null)
                {
                    lines.Add($"{Time}: 109.RemainingChargeTimeMins (null) -> {this.RemainingChargeTimeMins}");
                    lines.Add($"{Time}: 109.DischargeCompatitible (null) -> {this.DischargeCompatitible}");
                    lines.Add($"{Time}: 109.RemainingChargeTime10Sec (null) -> {this.RemainingChargeTime10Sec}");
                    lines.Add($"{Time}: 109.ChademoRawVersion (null) -> {this.ChademoRawVersion}");
                    lines.Add($"{Time}: 109.OutputCurrent (null) -> {this.OutputCurrent}");
                    lines.Add($"{Time}: 109.OutputVoltage (null) -> {this.OutputVoltage}");
                    lines.Add($"{Time}: 109.Status (null) -> {this.Status}");
                }
                else
                {
                    if (this.RemainingChargeTimeMins != other.RemainingChargeTimeMins)
                        lines.Add($"{Time}: 109.RemainingChargeTimeMins {other.RemainingChargeTimeMins} -> {this.RemainingChargeTimeMins}");

                    if (this.DischargeCompatitible != other.DischargeCompatitible)
                        lines.Add($"{Time}: 109.DischargeCompatitible {other.DischargeCompatitible} -> {this.DischargeCompatitible}");

                    if (this.RemainingChargeTime10Sec != other.RemainingChargeTime10Sec)
                        lines.Add($"{Time}: 109.RemainingChargeTime10Sec {other.RemainingChargeTime10Sec} -> {this.RemainingChargeTime10Sec}");

                    if (this.ChademoRawVersion != other.ChademoRawVersion)
                        lines.Add($"{Time}: 109.ChademoRawVersion {other.ChademoRawVersion} -> {this.ChademoRawVersion}");

                    if (this.OutputCurrent != other.OutputCurrent)
                        lines.Add($"{Time}: 109.OutputCurrent {other.OutputCurrent} -> {this.OutputCurrent}");

                    if (this.OutputVoltage != other.OutputVoltage)
                        lines.Add($"{Time}: 109.OutputVoltage {other.OutputVoltage} -> {this.OutputVoltage}");

                    if (this.Status != other.Status)
                        lines.Add($"{Time}: 109.Status {other.Status}: {PrintDiff(other.Status, this.Status)}");
                }

                return lines;
            }

            public override string ToString()
            {
                return $"Msg:109, ChademoRawVersion={ChademoRawVersion}, OutputVoltage={OutputVoltage}, OutputCurrent={OutputCurrent}, Status={Status}" +
                    $", RemainingChargeTime10Sec={RemainingChargeTime10Sec}, RemainingChargeTimeMins={RemainingChargeTimeMins}, DischargeCompatible:{DischargeCompatitible}";
            }

            public override bool Equals(object obj)
            {
                var other = (Msg109)obj;
                return this.RemainingChargeTimeMins == other.RemainingChargeTimeMins &&
                    this.DischargeCompatitible == other.DischargeCompatitible &&
                    this.RemainingChargeTime10Sec == other.RemainingChargeTime10Sec &&
                    this.ChademoRawVersion == other.ChademoRawVersion &&
                    this.OutputCurrent == other.OutputCurrent &&
                    this.OutputVoltage == other.OutputVoltage &&
                    this.Status == other.Status;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        class Msg200 : IMsg
        {
            byte MaximumDischargeCurrentInverted;
            UInt16 MinimumDischargeVoltage;
            byte MinimumBatteryDischargeLevel;
            byte MaxRemainingCapacityForCharging;

            int IMsg.Id => 0x200;
            public string Time { get; }

            public Msg200(byte[] data, string time)
            {
                Time = time;

                MaximumDischargeCurrentInverted = data[0];

                Program.AssertZero(data[1]);
                Program.AssertZero(data[2]);
                Program.AssertZero(data[3]);

                MinimumDischargeVoltage = (ushort)(data[4] | data[5] << 8);
                MinimumBatteryDischargeLevel = data[6];
                MaxRemainingCapacityForCharging = data[7];

            }

            public List<string> ToDiffLines(Msg200 other)
            {
                var lines = new List<string>();

                if (other == null)
                {
                    lines.Add($"{Time}: 200.MaximumDischargeCurrentInverted (null) -> {this.MaximumDischargeCurrentInverted}");
                    lines.Add($"{Time}: 200.MinimumDischargeVoltage (null) -> {this.MinimumDischargeVoltage}");
                    lines.Add($"{Time}: 200.MinimumBatteryDischargeLevel (null) -> {this.MinimumBatteryDischargeLevel}");
                    lines.Add($"{Time}: 200.MaxRemainingCapacityForCharging (null) -> {this.MaxRemainingCapacityForCharging}");
                }
                else
                {
                    if (this.MaximumDischargeCurrentInverted != other.MaximumDischargeCurrentInverted)
                        lines.Add($"{Time}: 200.MaximumDischargeCurrentInverted {other.MaximumDischargeCurrentInverted} -> {this.MaximumDischargeCurrentInverted}");

                    if (this.MinimumDischargeVoltage != other.MinimumDischargeVoltage)
                        lines.Add($"{Time}: 200.MinimumDischargeVoltage {other.MinimumDischargeVoltage} -> {this.MinimumDischargeVoltage}");

                    if (this.MinimumBatteryDischargeLevel != other.MinimumBatteryDischargeLevel)
                        lines.Add($"{Time}: 200.MinimumBatteryDischargeLevel {other.MinimumBatteryDischargeLevel} -> {this.MinimumBatteryDischargeLevel}");

                    if (this.MaxRemainingCapacityForCharging != other.MaxRemainingCapacityForCharging)
                        lines.Add($"{Time}: 200.MaxRemainingCapacityForCharging {other.MaxRemainingCapacityForCharging} -> {this.MaxRemainingCapacityForCharging}");
                }

                return lines;
            }

            public override string ToString()
            {
                return $"Msg:200, MaximumDischargeCurrentInverted={MaximumDischargeCurrentInverted}, MinimumDischargeVoltage={MinimumDischargeVoltage}" +
                    $", MinimumBatteryDischargeLevel={MinimumBatteryDischargeLevel}, MaxRemainingCapacityForCharging={MaxRemainingCapacityForCharging}";
            }

            public override bool Equals(object obj)
            {
                var other = (Msg200)obj;
                return this.MaximumDischargeCurrentInverted == other.MaximumDischargeCurrentInverted &&
                    this.MinimumDischargeVoltage == other.MinimumDischargeVoltage &&
                    this.MinimumBatteryDischargeLevel == other.MinimumBatteryDischargeLevel &&
                    this.MaxRemainingCapacityForCharging == other.MaxRemainingCapacityForCharging;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }


        class Msg208 : IMsg
        {

            byte PresentDischargeCurrentInverted;
            UInt16 AvailableInputVoltage;
            byte AvailableInputCurrentInverted;
            //byte Unused4;
            //byte Unused5;
            UInt16 LowerThresholdVoltage;

            int IMsg.Id => 0x208;
            public string Time { get; }

            public Msg208(byte[] data, string time)
            {
                Time = time;

                PresentDischargeCurrentInverted = data[0];
                AvailableInputVoltage = (ushort)(data[1] | data[2] << 8);
                AvailableInputCurrentInverted = data[3];

                Program.AssertZero(data[4]);
                Program.AssertZero(data[5]);

                LowerThresholdVoltage = (ushort)(data[6] | data[7] << 8);

            }

            public List<string> ToDiffLines(Msg208 other)
            {
                var lines = new List<string>();

                if (other == null)
                {
                    lines.Add($"{Time}: 208.PresentDischargeCurrentInverted (null) -> {this.PresentDischargeCurrentInverted}");
                    lines.Add($"{Time}: 208.AvailableInputVoltage (null) -> {this.AvailableInputVoltage}");
                    lines.Add($"{Time}: 208.AvailableInputCurrentInverted (null) -> {this.AvailableInputCurrentInverted}");
                    lines.Add($"{Time}: 208.LowerThresholdVoltage (null) -> {this.LowerThresholdVoltage}");
                }
                else
                {
                    if (this.PresentDischargeCurrentInverted != other.PresentDischargeCurrentInverted)
                        lines.Add($"{Time}: 208.PresentDischargeCurrentInverted {other.PresentDischargeCurrentInverted} -> {this.PresentDischargeCurrentInverted}");

                    if (this.AvailableInputVoltage != other.AvailableInputVoltage)
                        lines.Add($"{Time}: 208.AvailableInputVoltage {other.AvailableInputVoltage} -> {this.AvailableInputVoltage}");

                    if (this.AvailableInputCurrentInverted != other.AvailableInputCurrentInverted)
                        lines.Add($"{Time}: 208.AvailableInputCurrentInverted {other.AvailableInputCurrentInverted} -> {this.AvailableInputCurrentInverted}");

                    if (this.LowerThresholdVoltage != other.LowerThresholdVoltage)
                        lines.Add($"{Time}: 208.LowerThresholdVoltage {other.LowerThresholdVoltage} -> {this.LowerThresholdVoltage}");
                }

                return lines;
            }

            public override string ToString()
            {
                return $"Msg:208, PresentDischargeCurrentInverted={PresentDischargeCurrentInverted}, AvailableInputVoltage={AvailableInputVoltage}" +
                    $", AvailableInputCurrentInverted={AvailableInputCurrentInverted}, LowerThresholdVoltage={LowerThresholdVoltage}";
            }

            public override bool Equals(object obj)
            {
                var other = (Msg208)obj;
                return this.PresentDischargeCurrentInverted == other.PresentDischargeCurrentInverted &&
                    this.AvailableInputVoltage == other.AvailableInputVoltage &&
                    this.AvailableInputCurrentInverted == other.AvailableInputCurrentInverted &&
                    this.LowerThresholdVoltage == other.LowerThresholdVoltage;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }




        class Msg209 : IMsg
        {
            byte SequenceControlNumber;
            UInt16 RemainingDischargeTime;
            //uint8_t Unused3;
            //uint8_t Unused4;
            //uint8_t Unused5;
            //uint8_t Unused6;
            //uint8_t Unused7;

            int IMsg.Id => 0x209;
            public string Time { get; }

            public Msg209(byte[] data, string time)
            {
                Time = time;

                SequenceControlNumber = data[0];
                RemainingDischargeTime = (ushort)(data[1] | data[2] << 8);
                Program.AssertZero(data[3]);
                Program.AssertZero(data[4]);
                Program.AssertZero(data[5]);
                Program.AssertZero(data[6]);
                Program.AssertZero(data[7]);

            }

            public List<string> ToDiffLines(Msg209 other)
            {
                var lines = new List<string>();

                if (other == null)
                {
                    lines.Add($"{Time}: 209.SequenceControlNumber (null) -> {this.SequenceControlNumber}");
                    lines.Add($"{Time}: 209.RemainingDischargeTime (null) -> {this.RemainingDischargeTime}");
                }
                else
                {
                    if (this.SequenceControlNumber != other.SequenceControlNumber)
                        lines.Add($"{Time}: 209.SequenceControlNumber {other.SequenceControlNumber} -> {this.SequenceControlNumber}");

                    if (this.RemainingDischargeTime != other.RemainingDischargeTime)
                        lines.Add($"{Time}: 209.RemainingDischargeTime {other.RemainingDischargeTime} -> {this.RemainingDischargeTime}");
                }

                return lines;
            }

            public override string ToString()
            {
                return $"Msg:209, SequenceControlNumber={SequenceControlNumber}, RemainingDischargeTime={RemainingDischargeTime}";
            }

            public override bool Equals(object obj)
            {
                var other = (Msg209)obj;
                return this.SequenceControlNumber == other.SequenceControlNumber &&
                    this.RemainingDischargeTime == other.RemainingDischargeTime;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Example:
        /// " (2022-06-02 22:43:42.714008)  can1  700   [8]  01 02 00 00 06 00 00 00"
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static (int msg, byte[] data, string time) GetMessageParaTime(string line)
        {
            var msgStr = line.Substring(37, 3);
            var timeStr = line.Substring(13, 15);
            var data = line.Substring(48);
            var bytesStr = data.Split(' ');
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = byte.Parse(bytesStr[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }


            return (int.Parse(msgStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture), bytes, timeStr);
        }

        /// <summary>
        /// Example:
        /// Time Stamp,ID,Extended,Dir,Bus,LEN,D1,D2,D3,D4,D5,D6,D7,D8
        /// 4858169,00000102,false,Rx,0,8,01,90,01,00,00,01,00,00,
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static (int msg, byte[] data, string time) GetMessageCsv_Time_Stamp(string line)
        {
            var parts = line.Split(",");

            byte[] bytes = new byte[8];
            for (int i = 6; i < 6 + 8; i++)
            {
                bytes[i - 6] = byte.Parse(parts[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return (int.Parse(parts[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture), bytes, parts[0]);
        }

        /// <summary>
        /// Example:
        /// Index;System Time;Time Stamp;Channel;Direction;ID;Type;Format;Len;Data
        /// 0;02:15,7;0x478640;ch1;ñà¨Š;108;¨ì–í™ó;?©ž•™ó;8;x| 00 00 00 00 80 01 F4 00 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static (int msg, byte[] data, string time) GetMessageCsv_Index(string line)
        {
            var parts = line.Split(";");

            var data = parts[9];

            data = data.Replace("x|", null);
            var dataArr = data.Split(' ').Where(s => s.Trim().Length > 0).ToArray();
            if (dataArr.Length != 8)
                throw new Exception("not 8 bytes");

            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = byte.Parse(parts[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return (int.Parse(parts[5], NumberStyles.HexNumber, CultureInfo.InvariantCulture), bytes, parts[2]);
        }



        /// <summary>
        /// Example: 
        /// Rcvd msgID: 0x404; 06; 6B; FF; FE; 00; 00; 00; 00  00:01:09.839
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static (int msg, byte[] data, string time) GetMessage_Rcvd_msgID(string line)
        {
            line = line.Substring(14);
            var ts = line.Substring(line.Length - 12);
            line = line.Substring(0, line.Length - 12);

            var parts = line.Split(";");


            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = byte.Parse(parts[i + 1], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return (int.Parse(parts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture), bytes, ts);
        }


        [Flags]
        public enum CarFaults : byte
        {
            CAR_FAULT_OVER_VOLT = 1,
            CAR_FAULT_UNDER_VOLT = 2,
            CAR_FAULT_DEV_AMPS = 4,
            CAR_FAULT_OVER_TEMP = 8,
            CAR_FAULT_DEV_VOLT = 16
        }

        [Flags]
        public enum CarStatus : byte
        {
            CAR_STATUS_READY_TO_CHARGE = 1,// charging enabled/allowed
            CAR_STATUS_NOT_IN_PARK = 2, // shifter not in safe state (0: park 1: other)
            CAR_STATUS_ERROR = 4, // car did something dumb (fault caused by the car or the charger and detected by the car)

            /// <summary>
            /// Set the flag to 0 when the vehicle relay is closed, and set as 1 after the termination of welding detection.
            /// </summary>
            CAR_STATUS_CONTACTOR_OPEN = 8, // main contactor open (Special: 0: During contact sticking detection, 1: Contact sticking detection completed). Called StatusVehicle in docs!!!

            CAR_STATUS_STOP_BEFORE_CHARGING = 16, // charger stop before charging (changed my mind:-)

            /// <summary>
            /// </summary>
            CAR_STATUS_LEGACY_DYNAMIC_CONTROL = 64,

            CAR_STATUS_DISCHARGE_COMPATIBLE = 128, // car is V2X compatible (can deliver power to grid)
        }

        [Flags]
        public enum ChargerStatus : byte
        {
            /// <summary>
            /// during rundown: This is tied 1:1 with OutputCurrent > 0. Meaning we can be stopped, but still charging since amps > 0.
            /// During startup, CHARGER_STATUS_CHARGING is set and then amps are still 0.
            /// </summary>
            CHARGER_STATUS_CHARGING = 1, // 0: standby 1: charging (power transfer from charger)

            CHARGER_STATUS_ERROR = 2, // something went wrong (fault caused by (or inside) the charger)
            CHARGER_STATUS_PLUG_LOCKED = 4, // connector is currently locked (electromagnetic lock, plug locked into the car)
            CHARGER_STATUS_INCOMPAT = 8,// parameters between vehicle and charger not compatible (battery incompatible?)
            CHARGER_STATUS_CAR_ERROR = 16, // problem with the car, such as improper connection (or something wrong with the battery?)
            CHARGER_STATUS_STOPPED = 32, //charger is stopped (charger shutdown or end of charging). this is also initially set to stop, before charging.
        }

        [Flags]
        public enum StopReason : ushort
        {
            NONE = 0,
            CAR_CAN_AMPS_TIMEOUT = 1,
//            CAR_ASK_FOR_ZERO_AMPS = 2, // asking for 0 amps
            CAR_NOT_READY_TO_CHARGE = 4,
            CAR_NOT_IN_PARK = 8,
            CAR_K_OFF = 16,
            /// <summary>
            /// Typically the ccs charger want us to stop
            /// </summary>
            CHARGER = 32,
            ADAPTER_STOP_BUTTON = 64,
        }

    }
}
