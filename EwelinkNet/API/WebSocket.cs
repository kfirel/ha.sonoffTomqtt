using EwelinkNet.API.Responses;
using EwelinkNet.Helpers.Extensions;
using EwelinkNet.Payloads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using Mapster;
using EwelinkNet.Classes.Events;

namespace EwelinkNet.API
{
    public class WebSocket
    {
        WebSocketSharp.WebSocket websocket;

        public event EventHandler<EventWebsocketMessage> OnMessage;

        internal bool IsConnected { get; private set; } = false;

     
        public void Connect(string accessToken, string apiKey, string region)
        {
            var url = Constants.URLs.GetWebsocketUrl(region);
            websocket = new WebSocketSharp.WebSocket(url);

            websocket.OnMessage += Websocket_OnMessage;
            websocket.OnClose += Websocket_OnClose;
            websocket.Connect();
            IsConnected = true;

            var wsCredentialsPayload = new WsCredentialsPayload(accessToken, apiKey);
            websocket.Send(wsCredentialsPayload.AsJson());
        }

        private void Websocket_OnClose(object sender, CloseEventArgs e)
        {
            websocket.Connect();
            //throw new NotImplementedException();
        }

        public void UpdateDevice(string apiKey, string deviceId, object @params)
        {
            var wsUpdatePayload = new WsUpdatePayload(deviceId, apiKey, @params);
            var tt = wsUpdatePayload.AsJson();
            if (!websocket.IsAlive)
                websocket.Connect();
            websocket.Send(tt);
        }


        public void Disconnect()
        {
            websocket.Close();
            IsConnected = false;
        }

        private void Websocket_OnMessage(object sender, MessageEventArgs e)
        {
            var response = e.Data.FromJson<ExpandoObject>();
            Console.WriteLine(e.Data.ToString());
            object message;
            if (ExpandoHelpers.HasProperty(response, "error")) message = response.Adapt<WsLoginResponse>();
            else message = response.Adapt<WsUpdateResponse>();

            OnMessage.Emit(this, new EventWebsocketMessage() { Message = message });
        }
    }
}