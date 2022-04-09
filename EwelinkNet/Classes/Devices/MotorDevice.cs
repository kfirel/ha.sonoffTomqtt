using EwelinkNet.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace EwelinkNet.Classes
{
    public class MotorDevice : SwitchDevice
    {
        public void SetClose()
        {

            dynamic data = new ExpandoObject();
            ExpandoHelpers.AddProperty(data, "motorTurn", 2);

            UpdateDevice(data);
        }
        public void SetOpen()
        {
            dynamic data = new ExpandoObject();
            ExpandoHelpers.AddProperty(data, "motorTurn", 1);

            UpdateDevice(data);
        }

        public void stop()
        {
            dynamic data = new ExpandoObject();
            ExpandoHelpers.AddProperty(data, "motorTurn", 0);

            UpdateDevice(data);
        }

    }
}
