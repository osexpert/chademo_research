using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanlogParser
{
    internal class BatteryVoltage
    {

        static float GetEstimatedBatteryVoltageV5_ze0(float target, float soc, float nomVolt = 0)
        {
            float maxVolt = target - 10;

            if (nomVolt == 0)
            {
                nomVolt = 0.875f * target + 6.25f;
            }

            float halfRange = maxVolt - nomVolt; // pack is symmetric around nomVolt

            float volt20 = nomVolt - 0.35f * halfRange;  // delta 20-50
            float volt80 = nomVolt + 0.55f * halfRange;  // delta 50-80

            //   float voltLow0 = nomVolt - 2.10f * halfRange;    // 338, extrapolated, no measurement below 7%
            float voltLow0 = nomVolt - 2.56f * halfRange;   // 328, assumed like ZE1 at 0%

            float voltLow12 = nomVolt - 1.00f * halfRange;   // 360, knee: steep rise ends
            float voltMid0 = nomVolt - 1.455f * halfRange;   // 350.9, extended line for 12-29%

            if (soc < 12.0f)
            {
                float lowFraction = soc / 12.0f;
                return voltLow0 + lowFraction * (voltLow12 - voltLow0);
            }
            else if (soc < 29.0f)
            {
                float midFraction = soc / 29.0f;
                return voltMid0 + midFraction * (volt20 - voltMid0);
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






        static float GetEstimatedBatteryVoltageV5_core(float target, float soc, float nomVolt = 0)
        {
            float maxVolt = target - 10;

            if (nomVolt == 0)
            {
                // Based on 2 extreme data points: iMiev 370 -> 330 and BMW i5 M60: 450 -> 400
                // For 410 -> 365 (leaf 40+ is 355, so 365 is not a bad estimate)
                // PS: For known targets, we set nomVolt override, so this will only be used for unknown targets
                nomVolt = 0.875f * target + 6.25f;
            }

            float halfRange = maxVolt - nomVolt; // pack is symmetric around nomVolt

            float volt20 = nomVolt - 0.35f * halfRange;  // delta 20-50
            float volt80 = nomVolt + 0.55f * halfRange;  // delta 50-80

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


        static int GetNomVoltOverridev5(float target, int af = 0)
        {
            // Get nomvolt for some known targets
            if (target == 370)
                return (330); // iMiev
            else if (target == 450)
                return (400); // BMW i5 M60 
            else if (target == 410)
            {
                if (af == 0)
                    return (355); // Leaf 40+
                else if (af == 1)
                    return (380); // Leaf 20-30
            }

            return (0); // unknown -> use formula
        }

        static float GetEstimatedBatteryVoltageV5(float target, float soc, int af)
        {
            var nom = GetNomVoltOverridev5(target, af);
            if (af == 1)
                return GetEstimatedBatteryVoltageV5_ze0(target, soc, nom);
            //return GetEstimatedBatteryVoltageV5_ze0(target, soc, nom);
            else
                 return GetEstimatedBatteryVoltageV5_core(target, soc, nom);
            
            //return GetEstimatedBatteryVoltageV5_core(target, soc, nom);
        }

        public static void entry(string[] args)
        {

            //Program2.Maine();

            Console.WriteLine();

            for (int x = 0; x <= 100; x += 1)
            {
                var target = 410;

                               
                
                
                var v5g_af0 = GetEstimatedBatteryVoltageV5(target, x, 0);
                var v5g_af1 = GetEstimatedBatteryVoltageV5(target, x, 1);

                Console.WriteLine($"soc {x}, af0 {(int)v5g_af0}, af1 {(int)v5g_af1}");

            }

        }
    }
}
