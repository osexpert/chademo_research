using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanlogParser
{
    internal class BatteryVoltage
    {
        static float GetEstimatedBatteryVoltageV1(float target, float soc)
        {
            float maxVolt = target - 10;
            //            float nomVolt = 0.58f * target + 117.2f; // Linear interpolation/extrapolation
            float nomVolt = 0.5f * target + 150.0f; // Linear interpolation/extrapolation
            float minVolt = nomVolt - (maxVolt - nomVolt);

            float deltaLow = 0.14f * (nomVolt - minVolt);   // Steeper drop below 20%
            float deltaHigh = 0.10f * (maxVolt - nomVolt);  // Shallower rise above 80%

            float volt20 = nomVolt - deltaLow;
            float volt80 = nomVolt + deltaHigh;

            if (soc < 20.0f)
            {
                return minVolt + (soc / 20.0f) * (volt20 - minVolt);
            }
            else if (soc < 50.0f)
            {
                return volt20 + ((soc - 20.0f) / 30.0f) * (nomVolt - volt20);
            }
            else if (soc < 80.0f)
            {
                return nomVolt + ((soc - 50.0f) / 30.0f) * (volt80 - nomVolt);
            }
            else
            {
                return volt80 + ((soc - 80.0f) / 20.0f) * (maxVolt - volt80);
            }
        }


        static float GetEstimatedBatteryVoltageV2(float target, float soc)
        {
            float maxVolt = target - 10;

            // leaf: 410 -> 355, iMiev: 370 -> 330
            float nomVolt = 0.625f * target + 98.75f;

            float minVolt = nomVolt - (maxVolt - nomVolt);

            float deltaLow = 0.35f * (nomVolt - minVolt);
            float deltaHigh = 0.55f * (maxVolt - nomVolt);

            float volt20 = nomVolt - deltaLow;
            float volt80 = nomVolt + deltaHigh;

            if (soc < 50.0f)
            {
                return volt20 + ((soc - 20.0f) / 30.0f) * (nomVolt - volt20);
            }
            else if (soc < 80.0f)
            {
                return nomVolt + ((soc - 50.0f) / 30.0f) * (volt80 - nomVolt);
            }
            else
            {
                return volt80 + ((soc - 80.0f) / 20.0f) * (maxVolt - volt80);
            }
        }

        static float GetEstimatedBatteryVoltageV3(float target, float soc, float nomVolt = 0)
        {
            float maxVolt = target - 10;

            if (nomVolt == 0)
            {
                if (target < 410)
                {
                    // Low segment: iMiev 370 -> 330 and meets high segment at 410
                    nomVolt = 0.625f * target + 98.75f;
                }
                else
                {
                    // High segment: Leaf 40: 410 -> 355, BMW i5 M60: 450 -> 400
                    nomVolt = 1.125f * target - 106.25f;
                }
            }

            float minVolt = nomVolt - (maxVolt - nomVolt);

            float deltaLow = 0.35f * (nomVolt - minVolt);
            float deltaHigh = 0.55f * (maxVolt - nomVolt);

            float volt20 = nomVolt - deltaLow;
            float volt80 = nomVolt + deltaHigh;

            if (soc < 50.0f)
            {
                return volt20 + ((soc - 20.0f) / 30.0f) * (nomVolt - volt20);
            }
            else if (soc < 80.0f)
            {
                return nomVolt + ((soc - 50.0f) / 30.0f) * (volt80 - nomVolt);
            }
            else
            {
                return volt80 + ((soc - 80.0f) / 20.0f) * (maxVolt - volt80);
            }
        }


        static float GetEstimatedBatteryVoltageV4_core(float target, float soc, float nomVolt = 0)
        {
            float maxVolt = target - 10;

            if (nomVolt == 0)
            {
                // Based on 2 extreme data points: iMiev 370 -> 330 and BMW i5 M60: 450 -> 400
                // For 410 -> 365 (leaf 20-30kwh is 380, leaf 40+ is 355, so 365 is not a bad estimate)
                // PS: For known targets, we set nomVoltOverride, so this will only be used for unknown targets
                nomVolt = 0.875f * target + 6.25f;
            }

            float minVolt = nomVolt - (maxVolt - nomVolt); // assume symetric min/max around nomvolt

            float deltaLow = 0.35f * (nomVolt - minVolt); // delta between 20-50
            float deltaHigh = 0.55f * (maxVolt - nomVolt); // delta between 50-80

            float volt20 = nomVolt - deltaLow;
            float volt80 = nomVolt + deltaHigh;

            if (soc < 50.0f)
            {
                return volt20 + ((soc - 20.0f) / 30.0f) * (nomVolt - volt20);
            }
            else if (soc < 80.0f)
            {
                return nomVolt + ((soc - 50.0f) / 30.0f) * (volt80 - nomVolt);
            }
            else
            {
                return volt80 + ((soc - 80.0f) / 20.0f) * (maxVolt - volt80);
            }
        }

        static int GetNomVoltOverridev4(float target, int af = 0)
        {
            // Get nomvolt for some known targets
            if (target == 370)
                return 330; // iMiev
            else if (target == 450)
                return 400; // BMW i5 M60 
            else if (target == 410)
            {
                if (af == 0)
                    return 355; // Leaf 40+
                else if (af == 1)
                    return 380; // Leaf 20-30
            }

            return 0; // unknown -> use formula
        }

        static float GetEstimatedBatteryVoltageV4(float target, float soc, int af)
        {
            var nom = GetNomVoltOverridev4(target, af);
            return GetEstimatedBatteryVoltageV4_core(target, soc, nom);
        }



        static float GetEstimatedBatteryVoltageV5_core(float target, float soc, float nomVolt = 0, float adjustBelowSoc = 0, float adjustBelowFactor = 0f)
        {
            float maxVolt = target - 10;

            if (nomVolt == 0)
            {
                // Based on 2 extreme data points: iMiev 370 -> 330 and BMW i5 M60: 450 -> 400
                // For 410 -> 365 (leaf 20-30kwh is 380, leaf 40+ is 355, so 365 is not a bad estimate)
                // PS: For known targets, we set nomVoltOverride, so this will only be used for unknown targets
                nomVolt = 0.875f * target + 6.25f;
            }

            float minVolt = nomVolt - (maxVolt - nomVolt); // symetric

            float deltaLow = 0.35f * (nomVolt - minVolt);  // delta 20-50
            float volt20 = nomVolt - deltaLow;

            float deltaHigh = 0.55f * (maxVolt - nomVolt); // delta 50-80
            float volt80 = nomVolt + deltaHigh;

            if (adjustBelowSoc != 0 && soc < adjustBelowSoc)
            {
                float volt0 = minVolt - adjustBelowFactor * (volt20 - minVolt);
                return volt0 + (soc / adjustBelowSoc) * (volt20 - volt0);
            }
            else if (soc < 50.0f)
            {
                return volt20 + ((soc - 20.0f) / 30.0f) * (nomVolt - volt20);
            }
            else if (soc < 80.0f)
            {
                return nomVolt + ((soc - 50.0f) / 30.0f) * (volt80 - nomVolt);
            }
            else
            {
                return volt80 + ((soc - 80.0f) / 20.0f) * (maxVolt - volt80);
            }
        }

        static (int nok, int steeperBelowSoc, float adjustBelowFactor) GetNomVoltOverridev5(float target, int af = 0)
        {
            // Get nomvolt for some known targets
            if (target == 370)
                return (330, 0, 0); // iMiev
            else if (target == 450)
                return (400, 0, 0); // BMW i5 M60 
            else if (target == 410)
            {
                if (af == 0)
                    return (355, 0, 0); // Leaf 40+
                else if (af == 1)
                    return (380, 29, 0.7f); // Leaf 20-30
            }

            return (0, 0, 0); // unknown -> use formula
        }

        static float GetEstimatedBatteryVoltageV5(float target, float soc, int af)
        {
            (var nom, var adjustBelowSoc, var adjustBelowFactor) = GetNomVoltOverridev5(target, af);
            return GetEstimatedBatteryVoltageV5_core(target, soc, nom, adjustBelowSoc, adjustBelowFactor);
        }

        static void entry(string[] args)
        {

            //Program2.Maine();

            Console.WriteLine();

            for (int x = 0; x <= 100; x += 1)
            {
                var target = 410;

                var v1g = GetEstimatedBatteryVoltageV1(target, x);
                var v2g = GetEstimatedBatteryVoltageV2(target, x);
                var v3g = GetEstimatedBatteryVoltageV3(target, x);
                var v4g_af0 = GetEstimatedBatteryVoltageV4(target, x, 0);
                var v4g_af1 = GetEstimatedBatteryVoltageV4(target, x, 1);
                var v5g_af0 = GetEstimatedBatteryVoltageV5(target, x, 0);
                var v5g_af1 = GetEstimatedBatteryVoltageV5(target, x, 1);

                //                Console.WriteLine($"soc {x}, volt v1 {(int)v1g}, v2 {(int)v2g}, v3 {(int)v3g}, v4_af0 {(int)v4g_af0}, v5_af0 {(int)v5g_af0}");
                Console.WriteLine($"soc {x}, v4_af1 {(int)v4g_af1}, v5_af1 {(int)v5g_af1}");
            }

        }
    }
}
