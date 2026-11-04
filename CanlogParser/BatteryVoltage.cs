using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanlogParser
{
    internal class BatteryVoltage
    {
        struct VoltPoint
        {
            public byte Soc;
            public float Factor;

            public VoltPoint(byte soc, float fac) : this()
            {
                Soc = soc;
                Factor = fac;
            }
        };

        static readonly VoltPoint[] Ze0Curve =
            {
                    new(0,   -2.56f),
                    new(12,  -1.00f),
                    new(37,  -0.1517f),
                    new(50,   0f),
                    new(80,   0.55f),
                };

        static readonly VoltPoint[] CoreCurve =
            {
                new(20,  -0.35f),
                new(50,   0f),
                new(80,   0.55f),
            };

        static float EstimateVoltage(VoltPoint[] curve, float soc, float nomVolt, float maxVolt)
        {
            //float maxVolt = target - 10;
            float topHalf = maxVolt - nomVolt;

            int lastSegment = curve.Length - 1;
            int segment = 0;
            while (segment < lastSegment && soc >= curve[segment + 1].Soc)
                segment++;

            bool isLast = segment == lastSegment;

            float socLower = curve[segment].Soc;
            float socUpper = isLast ? 100f : curve[segment + 1].Soc;

            float voltLower = nomVolt + curve[segment].Factor * topHalf;
            float voltUpper = isLast ? maxVolt : nomVolt + curve[segment + 1].Factor * topHalf;

            float fraction = (soc - socLower) / (socUpper - socLower);
            return voltLower + fraction * (voltUpper - voltLower);
        }

        /*
         * static float EstimateVoltage(VoltPoint[] curve, float target, float soc, float nomVolt)
{
    float maxVolt = target - 10;
    float topHalf = maxVolt - nomVolt;

    bool hasExplicitEnd = curve[curve.Length - 1].Soc >= 100f;
    int lastSegment = hasExplicitEnd ? curve.Length - 2 : curve.Length - 1;

    int segment = 0;
    while (segment < lastSegment && soc >= curve[segment + 1].Soc)
        segment++;

    bool toImplicitEnd = !hasExplicitEnd && segment == lastSegment;

    float socLower = curve[segment].Soc;
    float socUpper = toImplicitEnd ? 100f : curve[segment + 1].Soc;

    float voltLower = nomVolt + curve[segment].Factor * topHalf;
    float voltUpper = toImplicitEnd ? maxVolt : nomVolt + curve[segment + 1].Factor * topHalf;

    float fraction = (soc - socLower) / (socUpper - socLower);
    return voltLower + fraction * (voltUpper - voltLower);
}*/

        static float GetEstimatedBatteryVoltageV5_new(float target, float soc, int af)
        {
            

            (int? nom, int? max) = GetNomMAxVoltOverridev5(target, af);

            float maxVolt = max ?? target - 10;

            if (af == 1)
            {
                float nomVolt = nom ?? 0.875f * target + 21.25f;
                //if (nomVolt == 0.0f)
                //    nomVolt = 0.875f * target + 21.25f;
                return EstimateVoltage(Ze0Curve, soc, nomVolt, maxVolt);
            }
            else
            {

  //              if (nomVolt == 0)
    //            {
                    // Based on 2 extreme data points: iMiev 370 -> 330 and BMW i5 M60: 450 -> 400
                    // For 410 -> 365 (leaf 40+ is 355, so 365 is not a bad estimate)
                    // PS: For known targets, we set nomVolt override, so this will only be used for unknown targets
                float nomVolt = nom ?? 0.875f * target + 6.25f;
//                }
                return EstimateVoltage(CoreCurve, soc, nomVolt, maxVolt);
            }
        }

        static float GetEstimatedBatteryVoltageV5_ze0(float target, float soc, float nomVolt = 0, float maxVolt = 0)
        {
            if (maxVolt == 0)
                maxVolt = target - 10;

            if (nomVolt == 0)
            {
                nomVolt = 0.875f * target + 21.25f; // 380 for target 410 (ZE0)
            }

            float topHalf = maxVolt - nomVolt;

            float volt0 = nomVolt - 2.56f * topHalf;     // 328
            float volt12 = nomVolt - 1.00f * topHalf;    // 360, knee: steep rise ends
            float volt37 = nomVolt - 0.1517f * topHalf;  // 377, knee: steep rise ends, plateau starts
            float volt80 = nomVolt + 0.55f * topHalf;    // delta 50-80

            if (soc < 12.0f)
            {
                float fraction = soc / 12.0f;
                return volt0 + fraction * (volt12 - volt0);
            }
            else if (soc < 37.0f)
            {
                float fraction = (soc - 12.0f) / 25.0f;
                return volt12 + fraction * (volt37 - volt12);
            }

            // if dropping 37% knee, can use this instead:
            //else if (soc < 50.0f)
            //{
            //    float fraction = (soc - 12.0f) / 38.0f;
            //    return volt12 + fraction * (nomVolt - volt12);
            //}

            else if (soc < 50.0f)
            {
                float fraction = (soc - 37.0f) / 13.0f;
                return volt37 + fraction * (nomVolt - volt37);
            }
            else if (soc < 80.0f)
            {
                float fraction = (soc - 50.0f) / 30.0f;
                return nomVolt + fraction * (volt80 - nomVolt);
            }
            else
            {
                float fraction = (soc - 80.0f) / 20.0f;
                return volt80 + fraction * (maxVolt - volt80);
            }
        }

        static float GetEstimatedBatteryVoltageV5_core(float target, float soc, float nomVolt = 0, float maxVolt = 0)
        {
            if (maxVolt == 0)
                maxVolt = target - 10;

            if (nomVolt == 0)
            {
                // Based on 2 extreme data points: iMiev 370 -> 330 and BMW i5 M60: 450 -> 400
                // For 410 -> 365 (leaf 40+ is 355, so 365 is not a bad estimate)
                // PS: For known targets, we set nomVolt override, so this will only be used for unknown targets
                nomVolt = 0.875f * target + 6.25f;
            }

            float topHalf = maxVolt - nomVolt;

            float volt20 = nomVolt - 0.35f * topHalf;  // delta 20-50
            float volt80 = nomVolt + 0.55f * topHalf;  // delta 50-80

            if (soc < 50.0f)
            {
                float fraction = (soc - 20.0f) / 30.0f;
                return volt20 + fraction * (nomVolt - volt20);
            }
            else if (soc < 80.0f)
            {
                float fraction = (soc - 50.0f) / 30.0f;
                return nomVolt + fraction * (volt80 - nomVolt);
            }
            else
            {
                float fraction = (soc - 80.0f) / 20.0f;
                return volt80 + fraction * (maxVolt - volt80);
            }
        }


        static (int? nom, int? max) GetNomMAxVoltOverridev5(float target, int af = 0)
        {
            // Get nomvolt for some known targets
            //        if (target == 370)
            //          return (330, null); // iMiev
            //            else if (target == 450)
            //              return (400, null); // BMW i5 M60 
            if (target == 410)
            {
                if (af == 0)
                    return (355, null); // Leaf 40+
                else if (af == 1)
                    return (380, null); // Leaf 20-30
            }
            else if (target == 336) // Peugeot iOn
            {
                return (310, 336);

            }

            return (null, null); // unknown -> use formula
        }

        static float GetEstimatedBatteryVoltageV5(float target, float soc, int af)
        {
            (int? nom, int? max) = GetNomMAxVoltOverridev5(target, af);
            if (af == 1)
                return GetEstimatedBatteryVoltageV5_ze0(target, soc, nom ?? 0, max ?? 0);
            else
                return GetEstimatedBatteryVoltageV5_core(target, soc, nom ?? 0, max ?? 0);
        }

        public static void entry(string[] args)
        {

            Console.WriteLine();

            for (int v = 0; v <= 500; v += 1)
            {

                for (int x = 0; x <= 100; x += 1)
                {
                    var target = v;// 410;

                    var v5g_af0 = GetEstimatedBatteryVoltageV5(target, x, 0);
                    var v5g_af1 = GetEstimatedBatteryVoltageV5(target, x, 1);

                    var v5g_af0_new = GetEstimatedBatteryVoltageV5_new(target, x, 0);
                    var v5g_af1_new = GetEstimatedBatteryVoltageV5_new(target, x, 1);

                    if (v5g_af0 != v5g_af0_new)
                    {
                        // feil!!!
                        // v5g_af0 = 328.75
                        // v5g_af0_new = 344.583344
                        // target = 410
                        // x = 0
                    }
                    if (v5g_af1 != v5g_af1_new)
                    {
                    }

                    Console.WriteLine($"t {v}, soc {x}, af0 {(int)v5g_af0}, af1 {(int)v5g_af1}");

                }
            }

            Console.WriteLine();
        }
    }
}
