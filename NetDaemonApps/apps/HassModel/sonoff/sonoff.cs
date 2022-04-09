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
    static ILogger<MttEntitySubscriptionApp> _logger;
    private readonly IMqttEntityManager _entityManager;
    private readonly Ewelink _eWelink;

    static void Logger(string log)
    {
        if(_logger != null)
        _logger.LogInformation(log);
;    }
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
        Ewelink.logger += Logger;
        await _eWelink.GetCredentials();
        await _eWelink.GetDevices();
            _eWelink.OpenWebSocket();
        await loadDevices(_eWelink.Devices);

    }


        async Task loadMultiSwitch(MultiSwitchDevice multiSw)
    {
        for (int i = 0; i < multiSw.NumChannels; i++)
        {
            var entityId = "switch." + multiSw.name.Replace(" ", "") + "_" + i;

            int channel = i;
            await _entityManager.CreateAsync(entityId,
                new EntityCreationOptions(Name: multiSw.name + " " + i, PayloadOn: "on", PayloadOff: "off", UniqueId: multiSw.deviceid + i))
               .ConfigureAwait(false);
            (await _entityManager.PrepareCommandSubscriptionAsync(entityId).ConfigureAwait(false)).
                 Subscribe(new Action<string>(async status => { turnSwitchOnOrOff(multiSw, status, entityId, channel); }));
        }
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
                    int ch = multiSw.NumChannels;
                    await loadMultiSwitch(multiSw);
                }
                catch (Exception)
                {

                    _logger.LogInformation("unsupported device", device.name, device.deviceName);
                }
            }


            var motorDevice = device as MotorDevice;
            if (motorDevice != null)
            {
                var entityId = "cover."+ motorDevice.name.Replace(" ", "");

                await _entityManager.CreateAsync(entityId,
                    new EntityCreationOptions( Name: motorDevice.name, UniqueId: motorDevice.deviceid))
                   .ConfigureAwait(false);
                (await _entityManager.PrepareCommandSubscriptionAsync(entityId).ConfigureAwait(false)).
                     Subscribe(new Action<string>(async status => { turnMotorOnOrOff(motorDevice, status, entityId); }));

            }
        }
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

    void turnMotorOnOrOff(MotorDevice motor, string status, string entityId)
    {

        if (motor == null)
            return;
        if (status == "OPEN")
            motor.SetOpen();
        else if (status == "CLOSE")
            motor.SetClose();
        else if (status == "STOP")
            motor.stop();
        _entityManager.SetStateAsync(entityId, status);

    }


}