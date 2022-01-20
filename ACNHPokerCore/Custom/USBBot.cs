using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{

    public class USBBot
    {

        private UsbDevice SwDevice;
        private UsbEndpointReader reader;
        private UsbEndpointWriter writer;

        public int MaximumTransferSize { get { return 468; } }

        private static readonly Encoding Encoder = Encoding.UTF8;
        private static byte[] Encode(string command, bool addrn = true) => Encoder.GetBytes(addrn ? command + "\r\n" : command);

        public static byte[] PokeRaw(uint offset, byte[] data) => Encode($"poke 0x{offset:X8} 0x{string.Concat(data.Select(z => $"{z:X2}"))}", false);

        public static byte[] PeekRaw(uint offset, int count) => Encode($"peek 0x{offset:X8} {count}", false);

        public bool Connected { get; private set; }

        private readonly object _sync = new object();

        public bool Connect()
        {
            lock (_sync)
            {
                // Find and open the usb device.
                //SwDevice = UsbDevice.OpenUsbDevice(SwFinder);
                //UsbDeviceFinder finder = new UsbDeviceFinder(1406, 12288);
                //this.SwDevice = UsbDevice.OpenUsbDevice(finder);

                foreach (UsbRegistry ur in UsbDevice.AllDevices)
                {
                    if (ur.Vid == 0x57e && ur.Pid == 0x3000 && ur.Device != null)
                    {
                        SwDevice = ur.Device;
                    }
                    //System.Diagnostics.Debug.Print(ur.Vid.ToString() + " " + ur.Pid.ToString() + " " + ur.FullName.ToString() + " " + ur.IsAlive.ToString() + "\n");
                }
                Debug.Print("UsbDevice.AllDevices -> Exception thrown: 'System.ArgumentException' in mscorlib.dll");

                //SwDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

                // If the device is open and ready

                if (SwDevice == null)
                {
                    MyMessageBox.Show("Please try to install the standalone executable of LibUsbDotNet v2.2.8.\n" +
                        "https://sourceforge.net/projects/libusbdotnet/files/LibUsbDotNet/LibUsbDotNet%20v2.2.8/ \n\n" +
                        "And then create a device filter for your Nintendo Switch.", "Device Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (SwDevice.IsOpen)
                    SwDevice.Close();
                SwDevice.Open();

                if (SwDevice is IUsbDevice wholeUsbDevice)
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    bool resagain = wholeUsbDevice.ClaimInterface(0);
                    if (!resagain)
                    {
                        wholeUsbDevice.ReleaseInterface(0);
                        wholeUsbDevice.ClaimInterface(0);
                    }
                }
                else
                {
                    Disconnect();
                    throw new Exception("Device is using WinUSB driver. Use libusbK and create a filter");
                }

                // open read write endpoints 1.
                reader = SwDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                writer = SwDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

                Connected = true;
                return true;
            }
        }

        public void Disconnect()
        {
            lock (_sync)
            {
                if (SwDevice != null)
                {
                    if (SwDevice.IsOpen)
                    {
                        if (SwDevice is IUsbDevice wholeUsbDevice)
                            wholeUsbDevice?.ReleaseInterface(0);
                        SwDevice.Close();
                    }
                }

                reader?.Dispose();
                writer?.Dispose();
                Connected = false;
            }
        }

        private int ReadInternal(byte[] buffer)
        {
            byte[] sizeOfReturn = new byte[4];

            //read size, no error checking as of yet, should be the required 368 bytes
            if (reader == null)
                throw new Exception("USB writer is null, you may have disconnected the device during previous function");

            reader.Read(sizeOfReturn, 5000, out _);

            //read stack
            reader.Read(buffer, 5000, out var lenVal);
            return lenVal;
        }

        private int SendInternal(byte[] buffer)
        {
            if (writer == null)
                throw new Exception("USB writer is null, you may have disconnected the device during previous function");

            uint pack = (uint)buffer.Length + 2;
            try
            {
                var ec = writer.Write(BitConverter.GetBytes(pack), 2000, out _);
                if (ec != ErrorCode.None)
                {
                    Disconnect();
                    throw new Exception(UsbDevice.LastErrorString);
                }
                ec = writer.Write(buffer, 2000, out var l);
                if (ec != ErrorCode.None)
                {
                    Disconnect();
                    throw new Exception(UsbDevice.LastErrorString);
                }
                return l;
            }
            catch
            {
                return 0;
            }
        }

        public int Read(byte[] buffer)
        {
            lock (_sync)
            {
                return ReadInternal(buffer);
            }
        }

        public byte[] ReadBytes(uint offset, int length)
        {
            lock (_sync)
            {
                var cmd = PeekRaw(offset, length);
                SendInternal(cmd);

                // give it time to push data back
                Thread.Sleep((length / 256) + 100);

                var buffer = new byte[length];
                var _ = ReadInternal(buffer);
                //return Decoder.ConvertHexByteStringToBytes(buffer);
                return buffer;
            }
        }

        public void WriteBytes(byte[] data, uint offset)
        {
            lock (_sync)
            {
                SendInternal(PokeRaw(offset, data));

                // give it time to push data back
                Thread.Sleep((data.Length / 256) + 100);
            }
        }

        private static T[] SubArray<T>(T[] data, int index, int length)
        {
            if (index + length > data.Length)
                length = data.Length - index;
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
