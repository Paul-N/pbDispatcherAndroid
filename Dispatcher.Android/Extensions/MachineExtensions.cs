using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DataUtils;

namespace Dispatcher.Android.Extensions
{
    public static class MachineExtensions
    {
        public static bool IsSame(this Machine m1, Machine m2)
        {
            try
            {
                if (m1.name != m2.name) return false;
                if (m1.type.name != m2.type.name) return false;
                if (m1.type.iconName != m2.type.iconName) return false;
                if (m1.GetDivisionStr() != m2.GetDivisionStr()) return false;
                if (m1.sensors.Count() != m1.sensors.Count()) return false;

                if (m1.sensors.Count() > 0 && m2.sensors.Count() > 0)
                {
                    var sensor1 = m1.sensors[0];
                    var sensor2 = m2.sensors[0];

                    if (sensor1.type.iconName != sensor2.type.iconName) return false;
                    if (sensor1.mainValue != sensor2.mainValue) return false;
                    if (sensor1.type.mainValueSymbol != sensor2.type.mainValueSymbol) return false;

                    if (sensor1.additionalValue != sensor2.additionalValue) return false;
                    if (sensor1.type.additionalValueSymbol != sensor2.type.additionalValueSymbol) return false;
                }                

                if (m1.state.code != m2.state.code) return false;

                if (m1.divisionPosition.ID != m2.divisionPosition.ID) return false;
                if (m1.divisionOwner.ID != m2.divisionOwner.ID) return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return true;
        }
    }
}