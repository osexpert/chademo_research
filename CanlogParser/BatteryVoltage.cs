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
                    new(37,  -0.1517f), // maybe drop it?
                    new(50,   0f),
                    new(80,   0.55f),
                };

        static readonly VoltPoint[] Ze1Curve =
            {
                new(20,  -0.35f),
                new(50,   0f),
                new(80,   0.55f),
            };

        static float EstimateVoltage(VoltPoint[] curve, float soc, float nomVolt, float maxVolt)
        {
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

        static float GetEstimatedBatteryVoltageV5(float target, float soc, int af)
        {
            (int? nom, int? max) = GetNomMaxVoltOverridev5(target, af);
            float maxVolt = max ?? target - 10;

            if (af == 1)
            {
                float nomVolt = nom ?? 0.875f * target + 21.25f;
                return EstimateVoltage(Ze0Curve, soc, nomVolt, maxVolt);
            }
            else
            {
                // Based on 2 extreme data points: iMiev 370 -> 330 and BMW i5 M60: 450 -> 400
                // For 410 -> 365 (leaf 40+ is 355, so 365 is not a bad estimate)
                // PS: For known targets, we set nomVolt override, so this will only be used for unknown targets
                float nomVolt = nom ?? 0.875f * target + 6.25f;
                return EstimateVoltage(Ze1Curve, soc, nomVolt, maxVolt);
            }
        }

        static (int? nom, int? max) GetNomMaxVoltOverridev5(float target, int af = 0)
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

        public static void entry(string[] args)
        {

            Console.WriteLine();

            //for (int v = 0; v <= 500; v += 1)
            {
                for (int x = 0; x <= 100; x += 1)
                {
                    var target = 410;

                    var v5g_af0 = GetEstimatedBatteryVoltageV5(target, x, 0);
                    var v5g_af1 = GetEstimatedBatteryVoltageV5(target, x, 1);

                    Console.WriteLine($"t {target}, soc {x}, af0 {(int)v5g_af0}, af1 {(int)v5g_af1}");
                }
            }

            Console.WriteLine();
        }
    }
}
