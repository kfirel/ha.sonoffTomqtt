﻿using EwelinkNet.Classes;
using EwelinkNet.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EwelinkNet.Constants;
using Newtonsoft.Json;

namespace EwelinkNet.API
{
    public static class Rest
    {
        public static async Task<string> GetCredentials(string url, string email, string password)
        {
            var client = new RestClient(url);

            var payload = new Payloads.CredentialsPayload(email, password);
            var body = JsonConvert.SerializeObject(payload);
            var signature = CryptoHelper.MakeAuthorizationSign(body);

            var request = new RestRequest("/user/login", Method.POST);
            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddHeader("Authorization", $"Sign {signature}");
            request.RequestFormat = DataFormat.Json;

            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
            return response.Content;
        }

        public static async Task<string> GetDevices(string url, string accessToken)
        {
            var client = new RestClient(url);
            Ewelink.logger("getDevices client");
            var request = new RestRequest("/user/device", Method.GET);
            Ewelink.logger("getDevices request" + request);
            request.AddQueryParameter("lang", "en");
            request.AddQueryParameter("getTags", "1");
            request.AddQueryParameter("version", AppData.VERSION);
            request.AddQueryParameter("ts", EwelinkHelper.MakeTimestamp());
            request.AddQueryParameter("appid", AppData.APP_ID);
            request.AddQueryParameter("imei", EwelinkHelper.MakeFakeImei());
            request.AddQueryParameter("os", AppData.OS );
            request.AddQueryParameter("model", AppData.MODEL );
            request.AddQueryParameter("romVersion", AppData.ROM_VERSION);
            request.AddQueryParameter("appVersion", AppData.APP_VERSION);

            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.RequestFormat = DataFormat.Json;
            Ewelink.logger("getDevices cancellationTokenSource1" );
            var cancellationTokenSource = new CancellationTokenSource();
            Ewelink.logger("getDevices cancellationTokenSource2");
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
            Ewelink.logger("getDevices response" + response);

            return response.Content;
        }
    }
}
