using System.Collections.Generic;
using System.Threading.Tasks;
using RCDiWheel.Models;

namespace RCDiWheel.Interfaces
{
    public interface IBluetoothDeviceHelper
    {
        Task<bool> Connect(string deviceName);
        List<BluetoothDevice> GetBondedDevices();
        bool Connected { get; set; }
        Task SendMessageAsync(string data);
    }
}
