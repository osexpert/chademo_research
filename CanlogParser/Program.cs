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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var fileName = @"..\..\..\..\CanLogs\v2h_startup__charge_ffrom_solar_excess_for_a_while_then_turn_on_oven_to_create_load_then_switch_off_load__then_end_session.csv";
            //var fileName = @"..\..\..\..\CanLogs\av50_old.txt";
            //var fileName = @"..\..\..\..\CanLogs\start.candump.txt";
            //var fileName = @"..\..\..\..\CanLogs\logs.candump.txt";
            //var fileName = @"..\..\..\..\CanLogs\nissan-leaf-chademo-start-stop.csv";
            //var fileName = @"..\..\..\..\CanLogs\non working charger can log.csv";
            //var fileName = @"..\..\..\..\CanLogs\ZE1-chademo-starting-and-charging.csv";
            //var fileName = @"..\..\..\..\CanLogs\2025\InstavoltBYDFail.csv";

            Console.WriteLine(fileName);

            var lines = File.ReadAllLines(fileName);

            msg100 lastmsg100 = null;
            msg101 lastmsg101 = null;
            msg102 lastmsg102 = null;
            msg108 lastmsg108 = null;
            msg109 lastmsg109 = null;
            msg200 lastmsg200 = null;
            msg208 lastmsg208 = null;
            msg209 lastmsg209 = null;

            List<can_msg> messages = new();

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
                                    Console.WriteLine("".PadRight(unchanged100, '.'));
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
                                    Console.WriteLine("".PadRight(unchanged100, '.'));
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
                                    Console.WriteLine("".PadRight(unchanged100, '.'));
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
                                    Console.WriteLine("".PadRight(unchanged100, '.'));
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
                                    Console.WriteLine("".PadRight(unchanged100, '.'));
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
                                    Console.WriteLine("".PadRight(unchanged100, '.'));
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
                                    Console.WriteLine("".PadRight(unchanged100, '.'));
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
                                    Console.WriteLine("".PadRight(unchanged100, '.'));
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

            var sm = new ChademoChargerStateMachine();
            sm.SetMessages(messages);
            sm.ChargerLoop();
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

        enum ChargerState
        {
            /// <summary>
            /// First time Charger Available volta and current is set
            /// </summary>
            Start_WaitForChargerAvailableVoltAndCurrent,
            SendCarStartSignal, // D1
            WaitForCarReadyToCharge,
            CarReadyToCharge, // LockPlug
            WaitForChargerHot, // D2
            WaitForCarContactorsClosed,
            WaitForCarAskingAmps,
            //WaitForChargerReadyForPowerDelivery,
            ChargingLoop,
            Stopping_WaitForLowAmpsDelivered,
            Stopping_WaitForLowVoltsDelivered,
            Stopping_WaitForCarContactorsOpen, // D2 OFF
            Stopping_WaitForCarContactorsOpenIn500ms, // D1 OFF, UnlockPlug
            End
        }

        class CarData
        {
            public ushort MaxChargeVoltage;// = 435;
            public byte MaxChargingTimeMins;
            internal ushort TargetVoltage;

            internal byte AskingAmps;
            internal byte SocPercent;
            /// <summary>
            /// To be safe, only use SOC,MaxChargingTimeMins,BatteryCapacityKwh after car is ready to charge (before they can have invalid values).
            /// </summary>
            internal CarStatus Status;
            internal CarFaults Faults;
            internal float BatteryCapacityKwh;
            internal byte ChargingRatePercent;
        }

        class ChargerData
        {
            /// <summary>
            /// Initial status is stopped
            /// </summary>
            internal ChargerStatus Status = ChargerStatus.CHARGER_STATUS_STOPPED;


            internal ushort AvailableOutputVoltage; //??

            /// <summary>
            /// If true, the charger support helping the car to do welding detection (by lowering the voltage)
            /// </summary>
            internal bool SupportWeldingDetection;

            public byte AvailableOutputCurrent;// ??

            internal ushort OutputCurrent;
            internal ushort OutputVoltage;

            // initial value from car, charger count it down
            internal byte RemainingChargeTimeMins;
            internal ushort ThresholdVoltage;
            
        }

        //class ExtData
        //{
        //    internal byte AvailableOutputCurrent;
        //    internal ushort AvailableOutputVoltage;
        //}

        class ChademoChargerStateMachine
        {
            CarData _carData = new();
            ChargerData _chargerData = new();
            //ExtData _extData = new();

            internal ChargerState _state = ChargerState.Start_WaitForChargerAvailableVoltAndCurrent;

            public void ChargerLoop()
            {
                // normally this will be done after ccs har done some progress and we get volts and amps from ccs. But fake it here.
                //SetChargerMaxVoltAndCurrent(500, 118, true);

                while (true)
                {
                    ChangerStateMachine();
                    Thread.Sleep(10);
                }
            }








            /// <summary>
            /// Initially, soc percent and capacity show weird and wrong values.
            /// But when car start asking for amps, we assume they are settled/valid.
            /// </summary>
            public bool SocAndCapacityCanBeUsedSafely()
            {
                return _state > ChargerState.WaitForCarAskingAmps;
            }

            private void SetChargerMaxVoltAndCurrent(ushort volt, byte amps)//, bool performWeldingDetection)
            {
                if (_state == ChargerState.Start_WaitForChargerAvailableVoltAndCurrent)
                {
                    _chargerData.AvailableOutputVoltage = volt;
                    _chargerData.AvailableOutputCurrent = amps;
                    //_chargerData.PerformWeldingDetection = performWeldingDetection;

                    // this is roundtripped from the car, but in the first messages, its the same as AvailableOutputVoltage.
                    //_chargerData.ThresholdVoltage = volt;

                    //_chargerData.State = ChargerState.SendStart;
                }
            }

            public bool ChargingStopping() => _state >= ChargerState.Stopping_WaitForLowAmpsDelivered;

            byte _delayCycles;
            bool _simulatedChargerReady;
            bool _simulatedChargerStopped;

            can_msg _m;

            string lastLog = "";

            private void ChangerStateMachine()
            {
                can_msg m = GetNextMsgFromCar();
                _m = m;

                if (m != null)
                {
                    if (m.Id == 0x100)
                    {
                        var m100 = (msg100)m;

                        _carData.MaxChargeVoltage = m100.MaxChargeVoltage;
                        _chargerData.ThresholdVoltage = m100.MaxChargeVoltage;
                        _carData.ChargingRatePercent = m100.ChargingRatePercent;
                    }
                    else if (m.Id == 0x101)
                    {
                        var m101 = (msg101)m;

                        if (m101.MaxChargingTime10Sec == 0xff)
                            _carData.MaxChargingTimeMins = m101.MaxChargingTimeMins;
                        else
                            _carData.MaxChargingTimeMins = (byte)(m101.MaxChargingTime10Sec / 6);

                        _carData.BatteryCapacityKwh = m101.BatteryCapacityKwh;
                        // take this as initial value for the charger countdown
                        // if (_carData.MaxChargingTimeMins > 0 && _chargerData.RemainingChargeTimeMins == 0)
                        //   _chargerData.RemainingChargeTimeMins = _carData.MaxChargingTimeMins;

                    }
                    else if (m.Id == 0x102)
                    {
                        var m102 = (msg102)m;

                        _carData.TargetVoltage = m102.TargetVoltage;
                        
                        _carData.AskingAmps = m102.AskingAmps;
                        if (_carData.AskingAmps > 200)
                            _carData.AskingAmps = 200; // adapter support max 200A so clip it.
                        _carData.SocPercent = m102.SocPercent;
                        _carData.Faults = m102.Faults;
                        _carData.Status = m102.Status;
                    }
                    else if (m.Id == 0x108)
                    {
                        var pm108 = (msg108)m;

                        _chargerData.AvailableOutputCurrent = pm108.AvailableOutputCurrent;
                        if (_chargerData.AvailableOutputCurrent > 200)
                            _chargerData.AvailableOutputCurrent = 200; // adapter support max 200A so make sure, so make sure we clip it.

                        _chargerData.AvailableOutputVoltage = pm108.AvailableOutputVoltage;

                        //_chargerData.ThresholdVoltage = pm108.ThresholdVoltage;
                    }
                    else if (m.Id == 0x109)
                    {
                        var pm109 = (msg109)m;

                        _chargerData.OutputCurrent = pm109.OutputCurrent;
                        _chargerData.OutputVoltage = pm109.OutputVoltage;

                        _chargerData.RemainingChargeTimeMins = pm109.RemainingChargeTimeMins;

                        if (!_simulatedChargerReady && (pm109.Status & ChargerStatus.CHARGER_STATUS_STOPPED) == 0)
                        {
                            _simulatedChargerReady = true;
                            Log("Simluate ready " + _m.Time);
                        }

                        if (_simulatedChargerReady && !_simulatedChargerStopped && (pm109.Status & ChargerStatus.CHARGER_STATUS_STOPPED) != 0)
                        {
                            _simulatedChargerStopped = true;
                            Log("Simluate stop " + _m.Time);
                        }
                        //                        _chargerData.Status = pm109.Status;
                        //if (_chargerData.State >= ChargerState.ChargingLoop && )
                        //    _simulatedChargerStopped = true;
                    }
                }

                var log = $"CH: Avail:{_chargerData.AvailableOutputVoltage}V, {_chargerData.AvailableOutputCurrent}A Out:{_chargerData.OutputVoltage}V, {_chargerData.OutputCurrent}A Tres:{_chargerData.ThresholdVoltage}V St:{_chargerData.Status} Rem_t:{_chargerData.RemainingChargeTimeMins}m CAR: Want:{_carData.AskingAmps}A Max:{_carData.MaxChargeVoltage}V Err:{_carData.Faults} St:{_carData.Status} Soc:{_carData.SocPercent}% Max_t:{_carData.MaxChargingTimeMins}m Cap:{_carData.BatteryCapacityKwh}KWh Target:{_carData.TargetVoltage}V ChargingRate={_carData.ChargingRatePercent}%";
                if (log != lastLog)
                    Log(log);
                lastLog = log;
                //Log($"Asking {_carData.AskingAmps}A, output {_chargerData.OutputCurrent}A {_chargerData.OutputVoltage}V");

                if (_state < ChargerState.ChargingLoop && (_carData.Status & CarStatus.CAR_STATUS_STOP_BEFORE_CHARGING) != 0)
                {
                    Log("Car stopped before starting");
                    // cancel before start. go straight to rundown.
                    SetState(ChargerState.Stopping_WaitForLowAmpsDelivered);
                }
                if (_state < ChargerState.ChargingLoop && ChargerStopBeforeCharging())
                {
                    Log("Charger stopped before starting");
                    // cancel before start. go straight to rundown.
                    SetState(ChargerState.Stopping_WaitForLowAmpsDelivered);
                }
                if (_state < ChargerState.ChargingLoop && AdapterStopBeforeCharging())
                {
                    Log("Adapter stopped before starting");
                    // cancel before start. go straight to rundown.
                    SetState(ChargerState.Stopping_WaitForLowAmpsDelivered);
                }


                if (_state == ChargerState.Start_WaitForChargerAvailableVoltAndCurrent)
                {
                    if (_chargerData.AvailableOutputVoltage > 0 && _chargerData.AvailableOutputCurrent > 0)
                    {
                        //_chargerData.PerformWeldingDetection = true;

                        //_chargerData.AvailableOutputCurrent = _extData.AvailableOutputCurrent;
                        //_chargerData.AvailableOutputVoltage = _extData.AvailableOutputVoltage;

                        // this is roundtripped from the car, but in the first messages, its the same as AvailableOutputVoltage.

                        _chargerData.ThresholdVoltage = _chargerData.AvailableOutputVoltage;

                        SetState(ChargerState.SendCarStartSignal);
                    }
                }
                else if (_state == ChargerState.SendCarStartSignal)
                {
                    Log("start");

                    SetD1(true);

                    SetState(ChargerState.WaitForCarReadyToCharge);
                }
                else if (_state == ChargerState.WaitForCarReadyToCharge)
                {
                    if ((_carData.Status & CarStatus.CAR_STATUS_READY_TO_CHARGE) != 0 && GetK() == true)
                    {
                        SetState(ChargerState.CarReadyToCharge);
                    }
                }
                else if (_state == ChargerState.CarReadyToCharge)
                {
                    // Mismatch between spec and log:
                    // Spec: Lock charging connector -> Insulation test
                    // Log: charger uses ca 4 seconds to gradually increase the voltage. Then it locks the plug and continue the gradually increase in voltage in 4 more seconds?

                    LockChargingPlug(true);
                    // Add artificial delay here?
                    _chargerData.Status |= ChargerStatus.CHARGER_STATUS_PLUG_LOCKED;

                    // Ramp-up/down: 0volt -> max volt -> 0volt (allthou logs show it often stays high / lowers to batt/target)
                    // End of insulation test: measured volt <= 20v. BUT logs tell a different story...volt is kept after insultayion test, only lowerd to eg. 380 (from eg. 480)
                    InsulationTest();

                    SetState(ChargerState.WaitForChargerHot);
                }
                else if (_state == ChargerState.WaitForChargerHot)
                {
                    if (PreChargeDone_PowerDeliveryOk_AdapterContactorClosed_Hot())
                    {
                        // This means the charger has its voltage at nominal voltage we gave it and is ready to charge

                        // this will give the car the 12v it needs to acticate contactors
                        SetD2(true);

                        SetState(ChargerState.WaitForCarContactorsClosed);
                    }
                }
                else if (_state == ChargerState.WaitForCarContactorsClosed)
                {
                    if ((_carData.Status & CarStatus.CAR_STATUS_CONTACTOR_OPEN) == 0)
                    {
                        // Contactors closed

                        // Adapter set 2 GPIO's at this point. They do not fit into the spec/flow chart in any way...so its not easy to tell what they are.
                        AdapterGpioStuffAfterContactorClosed();

                        // At this point, deliveredVolts should match battery + 10v?
                        // No...I think it should be 0 and gradually increased.

                        // Next, AskingAmps is going to increase. It take approx 2 seconds between the transition from 0 to 2 amps in the log.
                        // Only after this does the charger remove its CHARGER_STATUS_STOP

                        SetState(ChargerState.WaitForCarAskingAmps);
                    }
                }
                else if (_state == ChargerState.WaitForCarAskingAmps)
                {
                    if (_carData.AskingAmps > 0)
                    {
                        // At this point (car asked for amps), CAR_STATUS_STOP_BEFORE_CHARGING is no longer valid (State >= ChargingLoop)
                        // this is the trigger for the charger to turn off CHARGER_STATUS_STOP and instead turn on CHARGER_STATUS_CHARGING

                        // Even thou charger not delivering amps yet, we set these flags.
                        _chargerData.Status |= ChargerStatus.CHARGER_STATUS_CHARGING;
                        _chargerData.Status &= ~ChargerStatus.CHARGER_STATUS_STOPPED;

                        // Take car as initial value and countdown the minutes
                        _chargerData.RemainingChargeTimeMins = _carData.MaxChargingTimeMins;

                        CarAskingForAmps_ChargingStarted_ChargerShouldStartDeliveringAmps();

                        SetState(ChargerState.ChargingLoop);
                    }
                }
                else if (_state == ChargerState.ChargingLoop)
                {
                    Log("Charging");

                    // Check the adapter abort-button?
                    // check for too long time since asking for amps?

                    // Spec: k-signal and CAR_NOT_READY_TO_CHARGE both exist to make sure at least one of them reach the charger in case of cable error.
                    // But also some code only set AskingAmps = 0.

                    StopReason stopReason = StopReason.NONE;
                    //if (_carData.AskingAmps == 0) stopReason |= StopReason.CAR_ASK_FOR_ZERO_AMPS; no....this is not a valid reason!
                    if ((_carData.Status & CarStatus.CAR_STATUS_READY_TO_CHARGE) == 0) stopReason |= StopReason.CAR_NOT_READY_TO_CHARGE;
                    if ((_carData.Status & CarStatus.CAR_STATUS_NOT_IN_PARK) != 0) stopReason |= StopReason.CAR_NOT_IN_PARK;
                    if (GetK() == false) stopReason |= StopReason.CAR_K_OFF;
                    if (ChargingStoppedByCharger()) stopReason |= StopReason.CHARGER;
                    if (StopButtonOnAdapter()) stopReason |= StopReason.ADAPTER_STOP_BUTTON;
                    // TODO: timeout
                    // TODO: faults?

                    if (stopReason != StopReason.NONE)
                    {
                        Log("Stopping: " + stopReason);

                        // Checking for State >= ChargerState.Stopping_WaitForLowAmpsDelivered is probably better if we need to know we are in this state, instead of mutating the car data
                        // _carData.AskingAmps = 0;

                        // TODO: reason the charger stopped?
                        _chargerData.Status |= ChargerStatus.CHARGER_STATUS_STOPPED;

                        // reset countdown?
                        _chargerData.RemainingChargeTimeMins = 0;

                        // make sure ccs stop delivering amps and turn down volts (if stop initiated by adapter or car)
                        StopPowerDelivery();

                        SetState(ChargerState.Stopping_WaitForLowAmpsDelivered);
                    }
                }
                else if (_state == ChargerState.Stopping_WaitForLowAmpsDelivered)
                {
                    // Spec says <= 5 amps. Weird but true... Why not 0 or 1?
                    // TODO: IS this only something the car need to do? According to spec...yes. But the log show that CHARGER_STATUS_CHARGING flag is only driven by OutputCurrent...
                    if (_chargerData.OutputCurrent <= 5)
                    {
                        // remove charging flag
                        _chargerData.Status &= ~ChargerStatus.CHARGER_STATUS_CHARGING;
                        // If terminated by charger: In response, car will/should turn of K-pin (if not already).

                        // The log take 2 seconds from remove of CHARGER_STATUS_CHARGING to car log showing CAR_STATUS_CONTACTOR_OPEN

                        // charger should drop output voltage <= 10 and then plug is unlocked.
                        StopVoltageDelivery();

                        SetState(ChargerState.Stopping_WaitForLowVoltsDelivered);
                    }
                }
                else if (_state == ChargerState.Stopping_WaitForLowVoltsDelivered)
                {
                    if (_chargerData.OutputVoltage <= 10)
                    {
                        // If charger tell car it support welding detection, charger should help the car: (its the car that perform the welding detection)
                        // The circuit voltage shall drop below 25 % of circuit voltage, which is monitored before EV
                        // contactors are opened, within 1 s after the charger terminates charging output and EV contactors
                        // are opened.

                        // The vehicle shall carry out the welding detection within 4 s from charging output stop(output
                        // current falls below 5 A and “Charger status” flag = 0) to open of switch (d1)and(d2).
                        // Then car will open contactors.
                        // So I guess if charger says he support WD but does not lower voltage, the process is stuck...

                        // So I guess a charger should only say it support welding detection if it does these things? What if it says it support but does not lower?

                        // We already do this by doing StopVoltageDelivery()?

                        SetState(ChargerState.Stopping_WaitForCarContactorsOpen);
                    }
                }
                else if (_state == ChargerState.Stopping_WaitForCarContactorsOpen)
                {
                    // The car will open the contactor by itself, when amps drop <= 5.
                    // It will then do welding detection (or not) based on what we told it in M108 CarWeldingDetection

                    if ((_carData.Status & CarStatus.CAR_STATUS_CONTACTOR_OPEN) != 0)
                    {
                        // Next we open car contactors (the car may already have opened them too)
                        SetD2(false);

                        _delayCycles = 5; // spec says, after setting D2:false wait 0.5sec before setting D1:false
                        SetState(ChargerState.Stopping_WaitForCarContactorsOpenIn500ms);
                    }
                }
                else if (_state == ChargerState.Stopping_WaitForCarContactorsOpenIn500ms)
                {
                    if (_delayCycles > 0)
                    {
                        _delayCycles--;
                        return;
                    }

                    SetD1(false);

                    // safe to unlock plug
                    LockChargingPlug(false);
                    // remove plug locked flag
                    _chargerData.Status &= ~ChargerStatus.CHARGER_STATUS_PLUG_LOCKED;

                    SetState(ChargerState.End);
                }
                else if (_state == ChargerState.End)
                {
                    // nop
                    Log("The end");
                }


                // send 108 and 109 to car
                var m108 = new msg108();
                m108.PerformWeldingDetection = _chargerData.SupportWeldingDetection;
                m108.AvailableOutputCurrent = _chargerData.AvailableOutputCurrent;
                                                                                        
                m108.ThresholdVoltage = _chargerData.ThresholdVoltage; // this value just seem to roundtrip from the car
                m108.AvailableOutputVoltage = _chargerData.AvailableOutputVoltage;
                SendMsg(m108);

                var m109 = new msg109();
                m109.ChademoRawVersion = 2; // 2:chademo 1.0
                m109.OutputCurrent = _chargerData.OutputCurrent;
                m109.OutputVoltage = _chargerData.OutputVoltage;

                if (_chargerData.RemainingChargeTimeMins > 0)
                {
                    m109.RemainingChargeTime10Sec = 0xff; // 0xff: use mins TODO: but is this correct when sending 0 time?? made a special case for 0
                    m109.RemainingChargeTimeMins = _chargerData.RemainingChargeTimeMins;
                }
                else
                {
                    m109.RemainingChargeTime10Sec = 0;
                    m109.RemainingChargeTimeMins = 0;
                }

                m109.Status = _chargerData.Status;
                SendMsg(m109);
            }

            private bool ChargerStopBeforeCharging()
            {
                return false;
            }
            private bool AdapterStopBeforeCharging()
            {
                return false;
            }

            private void AdapterGpioStuffAfterContactorClosed()
            {
                // TODO: adapter set 2 gpios here
            }

            private bool PreChargeDone_PowerDeliveryOk_AdapterContactorClosed_Hot()
            {
                // the voltage has reached/is stable at nominal batt voltage
                //
                //return true;
                return _simulatedChargerReady;
            }

            //private void StartVoltageDelivery()
            //{

            //}

            private void InsulationTest()
            {
                // only for documentation
            }

            //private bool ChargerReadyForPowerDelivery()
            //{
            //    return _simulatedChargerReady;
            //}

            private void StopVoltageDelivery()
            {
                //
            }

            private void CarAskingForAmps_ChargingStarted_ChargerShouldStartDeliveringAmps()
            {
                // ccs power delivery start
            }

            private void Log(string v)
            {
                Console.WriteLine(v);
            }

            private void SetState(ChargerState state)
            {
                _state = state;
                Console.WriteLine($"Enter state: {state}, curr msg time {_m?.Time}");
            }

            private static void StopPowerDelivery()
            {
                // Tell ccs to stop power delivery
            }

            private static bool StopButtonOnAdapter()
            {
                // check if adapter stop button pressed
                return false;
            }

            private bool ChargingStoppedByCharger()
            {
                // If ccs stopped charging
                //return false;
                //simulate when playback
                //return chargerMsgStopId == "49978707";
                return _simulatedChargerStopped;
            }

            private static void LockChargingPlug(bool value)
            {
                // only for documentation
            }

            private static void SendMsg(can_msg msg)
            {
                // TODO
            }

            private static void SetD1(bool v)
            {
                // TODO
                // Adapter does nothing here.... Maybe it set D1 from the start...or at same time as D2...
            }

            private static void SetD2(bool value)
            {
                // TODO
            }

            private static bool GetK()
            {
                // pin set by car, says ready to charge
                return true;
            }

            List<can_msg> _messages;

            private can_msg GetNextMsgFromCar()
            {
                if (_messages.Count == 0)
                    return null;

                var m = _messages[0];
                _messages.RemoveAt(0);
                return m;
            }

            internal void SetMessages(List<can_msg> messages)
            {
                _messages = messages;
            }
        }

        private static msg101 Parse101(byte[] data, string time)
        {
            return new msg101(data, time);
        }

        private static msg102 Parse102(byte[] data, string time)
        {
            return new msg102(data, time);
        }

        private static msg108 Parse108(byte[] data, string time)
        {
            return new msg108(data, time);
        }

        private static msg109 Parse109(byte[] data, string time)
        {
            return new msg109(data, time);
        }

        private static msg100 Parse100(byte[] data, string time)
        {
            return new msg100(data, time);
        }

        private static msg200 Parse200(byte[] data, string time)
        {
            return new msg200(data, time);
        }
        private static msg208 Parse208(byte[] data, string time)
        {
            return new msg208(data, time);
        }
        private static msg209 Parse209(byte[] data, string time)
        {
            return new msg209(data, time);
        }

        class msg100 : can_msg
        {
            public byte MinimumChargeCurrent;
            public ushort MaxChargeVoltage;

            /// <summary>
            /// Initially seen as eg. 240....before changing to 100.
            /// </summary>
            public byte ChargingRatePercent;

            int can_msg.Id => 0x100;
            public string Time { get; }

            public msg100(byte[] data, string time)
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

            public List<string> ToDiffLines(msg100 other)
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
                var other = (msg100)obj;
                return this.ChargingRatePercent == other.ChargingRatePercent &&
                    this.MinimumChargeCurrent == other.MinimumChargeCurrent &&
                    this.MaxChargeVoltage == other.MaxChargeVoltage;
            }
        }

        private static void AssertZero(byte v)
        {
            if (v != 0)
                throw new Exception("Not zero");
        }

        interface can_msg
        {
            string Time { get; }
            int Id { get; }
        }

        class msg101 : can_msg
        {
            int can_msg.Id => 0x101;

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

            public msg101(byte[] data, string time)
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

            public List<string> ToDiffLines(msg101 other)
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
                var other = (msg101)obj;
                return this.MaxChargingTimeMins == other.MaxChargingTimeMins &&
                    this.MaxChargingTime10Sec == other.MaxChargingTime10Sec &&
                    this.EstimatedChargingTimeMins == other.EstimatedChargingTimeMins &&
                    this.BatteryCapacityKwh == other.BatteryCapacityKwh;
            }
        }

        class msg102 : can_msg
        {
            int can_msg.Id => 0x102;

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

            public msg102(byte[] data, string time)
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

            public List<string> ToDiffLines(msg102 other)
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
                        lines.Add($"{Time}: 102.Faults {other.Faults} -> {this.Faults}");

                    if (this.Status != other.Status)
                        lines.Add($"{Time}: 102.Status {other.Status} -> {this.Status}");

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
                var other = (msg102)obj;
                return this.ChademoRawVersion == other.ChademoRawVersion &&
                    this.TargetVoltage == other.TargetVoltage &&
                    this.AskingAmps == other.AskingAmps &&
                    this.Faults == other.Faults &&
                    this.Status == other.Status &&
                    this.SocPercent == other.SocPercent;

            }
        }

        class msg108 : can_msg
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

            int can_msg.Id => 0x108;
            public string Time { get; }

            public msg108(byte[] data, string time)
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

            public msg108()
            {
            }

            public List<string> ToDiffLines(msg108 other)
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
                var other = (msg108)obj;
                return this.PerformWeldingDetection == other.PerformWeldingDetection &&
                    this.AvailableOutputCurrent == other.AvailableOutputCurrent &&
                    this.AvailableOutputVoltage == other.AvailableOutputVoltage &&
                    this.ThresholdVoltage == other.ThresholdVoltage;
            }
        }

        class msg109 : can_msg
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

            int can_msg.Id => 0x109;
            public string Time { get; }

            public msg109(byte[] data, string time)
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

            public msg109()
            {
            }

            public List<string> ToDiffLines(msg109 other)
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
                        lines.Add($"{Time}: 109.Status {other.Status} -> {this.Status}");
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
                var other = (msg109)obj;
                return this.RemainingChargeTimeMins == other.RemainingChargeTimeMins &&
                    this.DischargeCompatitible == other.DischargeCompatitible &&
                    this.RemainingChargeTime10Sec == other.RemainingChargeTime10Sec &&
                    this.ChademoRawVersion == other.ChademoRawVersion &&
                    this.OutputCurrent == other.OutputCurrent &&
                    this.OutputVoltage == other.OutputVoltage &&
                    this.Status == other.Status;
            }
        }

        class msg200 : can_msg
        {
            byte MaximumDischargeCurrentInverted;
            UInt16 MinimumDischargeVoltage;
            byte MinimumBatteryDischargeLevel;
            byte MaxRemainingCapacityForCharging;

            int can_msg.Id => 0x200;
            public string Time { get; }

            public msg200(byte[] data, string time)
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

            public msg200()
            {
            }

            public List<string> ToDiffLines(msg200 other)
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
                var other = (msg200)obj;
                return this.MaximumDischargeCurrentInverted == other.MaximumDischargeCurrentInverted &&
                    this.MinimumDischargeVoltage == other.MinimumDischargeVoltage &&
                    this.MinimumBatteryDischargeLevel == other.MinimumBatteryDischargeLevel &&
                    this.MaxRemainingCapacityForCharging == other.MaxRemainingCapacityForCharging;
            }
        }


        class msg208 : can_msg
        {

            byte PresentDischargeCurrentInverted;
            UInt16 AvailableInputVoltage;
            byte AvailableInputCurrentInverted;
            //byte Unused4;
            //byte Unused5;
            UInt16 LowerThresholdVoltage;

            int can_msg.Id => 0x208;
            public string Time { get; }

            public msg208(byte[] data, string time)
            {
                Time = time;

                PresentDischargeCurrentInverted = data[0];
                AvailableInputVoltage = (ushort)(data[1] | data[2] << 8);
                AvailableInputCurrentInverted = data[3];

                Program.AssertZero(data[4]);
                Program.AssertZero(data[5]);

                LowerThresholdVoltage = (ushort)(data[6] | data[7] << 8);

            }

            public msg208()
            {
            }

            public List<string> ToDiffLines(msg208 other)
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
                var other = (msg208)obj;
                return this.PresentDischargeCurrentInverted == other.PresentDischargeCurrentInverted &&
                    this.AvailableInputVoltage == other.AvailableInputVoltage &&
                    this.AvailableInputCurrentInverted == other.AvailableInputCurrentInverted &&
                    this.LowerThresholdVoltage == other.LowerThresholdVoltage;
            }
        }




        class msg209 : can_msg
        {
            byte SequenceControlNumber;
            UInt16 RemainingDischargeTime;
            //uint8_t Unused3;
            //uint8_t Unused4;
            //uint8_t Unused5;
            //uint8_t Unused6;
            //uint8_t Unused7;

            int can_msg.Id => 0x209;
            public string Time { get; }

            public msg209(byte[] data, string time)
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

            public msg209()
            {
            }

            public List<string> ToDiffLines(msg209 other)
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
                var other = (msg209)obj;
                return this.SequenceControlNumber == other.SequenceControlNumber &&
                    this.RemainingDischargeTime == other.RemainingDischargeTime;
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
