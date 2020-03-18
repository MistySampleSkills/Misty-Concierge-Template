using WemoNet;

namespace Misty.Services
{
    /// <summary>
    /// This service call the Wemo Device .NET library to turn ON and OFF a smart device plug.
    /// </summary>
    public class MistySmartDevicesApi
    {
        private Wemo _wemo;

        public MistySmartDevicesApi()
        {
            _wemo = new Wemo();
        }

        public void TurnONPlug()
        {
            _wemo.TurnOnWemoPlugAsync(MistyApiInfo.WemoDeviceIpAddress).GetAwaiter().GetResult();
        }

        public void TurnOFFPlug()
        {
            _wemo.TurnOffWemoPlugAsync(MistyApiInfo.WemoDeviceIpAddress).GetAwaiter().GetResult();
        }
    }
}
