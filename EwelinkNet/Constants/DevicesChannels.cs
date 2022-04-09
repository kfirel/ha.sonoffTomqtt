﻿using EwelinkNet.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EwelinkNet.Constants
{
    internal class DeviceChannels
    {
        private static Dictionary<string?, int?> data = new Dictionary<string?, int?> {
            {"SOCKET", 1},
            {"SWITCH_CHANGE", 1},
            {"GSM_UNLIMIT_SOCKET", 1},
            {"SWITCH", 1},
            {"THERMOSTAT", 1},
            {"SOCKET_POWER", 1},
            {"GSM_SOCKET", 1},
            {"POWER_DETECTION_SOCKET", 1},
            {"SOCKET_2", 2},
            {"GSM_SOCKET_2", 2},
            {"SWITCH_2", 2},
            {"SOCKET_3", 3},
            {"GSM_SOCKET_3", 3},
            {"SWITCH_3", 3},
            {"SOCKET_4", 4},
            {"GSM_SOCKET_4", 4},
            {"SWITCH_4", 4},
            {"SWITCH_6", 6},
            {"CUN_YOU_DOOR", 4}
        };

        internal static int? GetDeviceChannelsByName(string deviceName) => data.GetOrDefault(deviceName);
    }
}