using System;
using System.Collections.Generic;
using System.Diagnostics;
using RCDiWheel.Interfaces;
using RCDiWheel.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RCDiWheel.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private readonly BluetoothDevice _btDevice;
        private readonly IBluetoothDeviceHelper _bluetoothDeviceHelper;
        private readonly int _timerDelay = 250;
        
        public MainPage(BluetoothDevice btDevice)
        {
            InitializeComponent();

            if (btDevice != null)
            {
                MessageLabel.Text = $"Trying to connect to Bluetooth device {btDevice.Name}...";
                _btDevice = btDevice;
                _bluetoothDeviceHelper = DependencyService.Get<IBluetoothDeviceHelper>();
                Connect2BluetoothDevice();
            }
            else
            {
                MessageLabel.Text = "No Bluetooth device found.";
            }

            Device.StartTimer(TimeSpan.FromMilliseconds(_timerDelay), OnTimerTick);
        }

        async void Connect2BluetoothDevice()
        {
            var connected = await _bluetoothDeviceHelper.Connect(_btDevice.Address);
            MessageLabel.Text = connected ? $"Connected to {_btDevice.Name}" : $"Cannot connect to {_btDevice.Name}!";
        }

        private bool OnTimerTick()
        {
            var msg = $"{UpDownLeft.Value},{UpDownRight.Value}|";
            Debug.WriteLine(msg);

            if (_bluetoothDeviceHelper != null && _bluetoothDeviceHelper.Connected)
            {
                _bluetoothDeviceHelper.SendMessageAsync(msg);
            }

            return true;
        }      

        private bool Connected => _bluetoothDeviceHelper != null && _bluetoothDeviceHelper.Connected;
    }
}