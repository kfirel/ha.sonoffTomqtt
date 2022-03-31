using System.Threading;
using System.Threading.Tasks;
using EwelinkNet;
using EwelinkNet.Classes;
using EwelinkNet.Classes.Events;
using NetDaemon.Extensions.MqttEntityManager;

namespace DebugHost.apps.Extensions;

[NetDaemonApp]
public class MttEntitySubscriptionApp : IAsyncInitializable
{
    private readonly IHaContext _ha;
    private readonly ILogger<MttEntitySubscriptionApp> _logger;
    private readonly IMqttEntityManager _entityManager;
    private readonly Ewelink _eWelink;

    public MttEntitySubscriptionApp(IHaContext ha, ILogger<MttEntitySubscriptionApp> logger,
        IMqttEntityManager entityManager)
    {
        _ha = ha;
        _logger = logger;
        _entityManager = entityManager;
        _eWelink = new Ewelink("kfirel@gmail.com", "ottomanew0", "as");
        _eWelink.OnDeviceChanged += EventHandler;
       


    }
    void EventHandler<TEventArgs>(object? sender, TEventArgs e)
    {
        
        var multiSw = (e as EvendDeviceUpdate).Device as MultiSwitchDevice;
        if (multiSw == null)
            return;
       
        for (int i = 0; i < multiSw.NumChannels; i++)
        {
            var entityId = "switch." + multiSw.name + "_" + i;
            _entityManager.SetStateAsync(entityId, multiSw.GetState(i));
        }
    }
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _eWelink.GetCredentials();
        await _eWelink.GetDevices();
            _eWelink.OpenWebSocket();
        await loadDevices(_eWelink.Devices);

    }
    async Task loadDevices(Device[] devices)
    {
        foreach (var device in devices)
        {

            var multiSw = device as MultiSwitchDevice;
            if (multiSw != null)
            {
                try
                {
                    for (int i = 0; i < multiSw.NumChannels; i++)
                    {
                        var entityId = "switch." + device.name.Replace(" ", "") + "_" + i;

                        int channel = i;
                        await _entityManager.CreateAsync(entityId,
                            new EntityCreationOptions(Name: device.name + " " + i, PayloadOn: "on", PayloadOff: "off", UniqueId: device.deviceid + i))
                           .ConfigureAwait(false);
                        (await _entityManager.PrepareCommandSubscriptionAsync(entityId).ConfigureAwait(false)).
                             Subscribe(new Action<string>(async status => { turnSwitchOnOrOff(multiSw, status, entityId, channel); }));
                    }
                }
                catch (Exception)
                {

                    _logger.LogDebug("unsupported device", device.name, device.deviceName);
                }
            }
            var curtainDevice = device as CurtainDevice;
            if (curtainDevice != null)
            {
                var entityId = "Cover." + device.name.Replace(" ", "");

                await _entityManager.CreateAsync(entityId,
                    new EntityCreationOptions(Name: device.name , PayloadOn: "open", PayloadOff: "close", UniqueId: device.deviceid))
                   .ConfigureAwait(false);
                (await _entityManager.PrepareCommandSubscriptionAsync(entityId).ConfigureAwait(false)).
                     Subscribe(new Action<string>(async status => { openOrCloseCurtain(curtainDevice, status, entityId); }));
            }
        }
    }
    void openOrCloseCurtain(CurtainDevice curtain, string status, string entityId)
    {

        if (curtain == null)
            return;
        if(status == "open")
            curtain.SetOpen(100);
        else
            curtain.SetClose(0);

    }
    void turnSwitchOnOrOff(MultiSwitchDevice multiSw, string status,string entityId,int channel)
    {

        if (multiSw == null)
            return;
        if (status == "on")
            multiSw.TurnOn(channel);
        else
            multiSw.TurnOff(channel);
        _entityManager.SetStateAsync(entityId, status);

    }
   
}