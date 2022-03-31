using EwelinkNet.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace EwelinkNet.Classes
{
    public class CurtainDevice : SwitchDevice
    {
        public void SetClose(int percent)
        {
            dynamic data = new ExpandoObject();
            ExpandoHelpers.AddProperty(data, "setClose", percent);

            UpdateDevice(data);
        }
        public void SetOpen(int percent)
        {
            dynamic data = new ExpandoObject();
            ExpandoHelpers.AddProperty(data, "setOpen", percent);

            UpdateDevice(data);
        }

        public void stop(int percent)
        {
            dynamic data = new ExpandoObject();
            ExpandoHelpers.AddProperty(data, "setOpen", percent);

            UpdateDevice(data);
        }

    }
}
