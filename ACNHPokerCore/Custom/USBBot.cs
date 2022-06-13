using MonoLibUsb;
using MonoLibUsb.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ACNHPokerCore
{

    public class USBBot
    {
        private static bool debug = false;
        private const byte READPOINT = 129;
        private const byte WRITEPOINT = 1;

        public int MaximumTransferSize { get { return 468; } }

        private static readonly Encoding Encoder = Encoding.UTF8;
        private static byte[] Encode(string command, bool addrn = true) => Encoder.GetBytes(addrn ? command + "\r\n" : command);

        public static byte[] PokeRaw(uint offset, byte[] data) => Encode($"poke 0x{offset:X8} 0x{string.Concat(data.Select(z => $"{z:X2}"))}", false);
        public static byte[] PeekRaw(uint offset, int count) => Encode($"peek 0x{offset:X8} {count}", false);
        public static byte[] PokeMain(ulong offset, byte[] data) => Encode($"pokeMain 0x{offset:X16} 0x{string.Concat(data.Select(z => $"{z:X2}"))}");
        public static byte[] PeekMain(ulong offset, int count) => Encode($"peekMain 0x{offset:X16} {count}");
        public static byte[] Version() => Encode("getVersion");

        public bool Connected { get; private set; }

        private readonly object _sync = new object();

        private MonoUsbSessionHandle context;

        public USBBot(bool Debug)
        {
            debug = Debug;
        }
        public bool Connect()
        {
            lock (_sync)
            {
                if (context != null)
                {
                    if (!context.IsClosed)
                        context.Close();
                    context.Dispose();
                    context = null;
                }

                var sessionHandle = new MonoUsbSessionHandle();
                var profileList = new MonoUsbProfileList();
                profileList.Refresh(sessionHandle);

                if (debug)
                {
                    List<MonoUsbProfile> usbList = profileList.GetList();
                    string deviceList = "";
                    foreach (MonoUsbProfile profile in usbList)
                    {
                        var deviceHandle = profile.OpenDeviceHandle();
                        string VendorID = profile.DeviceDescriptor.VendorID.ToString();
                        string ProductID = profile.DeviceDescriptor.ProductID.ToString();
                        if (VendorID.Equals("1406") && ProductID.Equals("12288"))
                        {
                            deviceList += "SWITCH FOUND - " + "VendorID : " + VendorID + " " + " ProductID : " + ProductID + "\n";
                        }
                        else
                        {
                            deviceList += "VendorID : " + VendorID + " " + " ProductID : " + ProductID + "\n";
                        }
                    }

                    if (deviceList.Equals(""))
                    {
                        MyMessageBox.Show("NO USB DEVICES FOUND!", "List of USB devices", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                    else
                        MyMessageBox.Show(deviceList, "List of USB devices", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }

                context = new MonoUsbSessionHandle();
                var usbHandle = MonoUsbApi.OpenDeviceWithVidPid(context, 1406, 12288);
                if (usbHandle != null)
                {
                    if (MonoUsbApi.ClaimInterface(usbHandle, 0) == 0)
                    {
                        MonoUsbApi.ReleaseInterface(usbHandle, 0);
                        usbHandle.Close();
                        Connected = true;
                        return true;
                    }
                    usbHandle.Close();
                }
                else
                {
                    MyMessageBox.Show("Device Not Found!\nPlease also try restarting your Switch if problem persists!", "USB insertion always require at least 3 flips!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                Connected = false;
                return false;
            }
        }

        private MonoUsbDeviceHandle getUsableAndOpenUsbHandle()
        {
            lock (_sync)
            {
                if (context != null)
                {
                    if (!context.IsClosed)
                        context.Close();
                    context.Dispose();
                    context = null;
                }

                context = new MonoUsbSessionHandle();
                var usbHandle = MonoUsbApi.OpenDeviceWithVidPid(context, 1406, 12288);
                if (usbHandle != null)
                {
                    if (MonoUsbApi.ClaimInterface(usbHandle, 0) == 0)
                    {
                        return usbHandle;
                    }
                    usbHandle.Close();
                }

                return null;
            }
        }

        private void CleanUpHandle(MonoUsbDeviceHandle handle)
        {
            MonoUsbApi.ReleaseInterface(handle, 0);
            handle.Close();
            Disconnect(false);
        }

        public void Disconnect(bool setConnectionStatus = true)
        {
            lock (_sync)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
                if (setConnectionStatus)
                    Connected = false;
            }
        }

        private int ReadInternal(byte[] buffer)
        {
            var handle = getUsableAndOpenUsbHandle();
            if (handle == null)
                throw new Exception("USB writer is null, you may have disconnected the device during previous function");

            byte[] sizeOfReturn = new byte[4];

            MonoUsbApi.BulkTransfer(handle, READPOINT, sizeOfReturn, 4, out var _, 5000);

            // read stack
            MonoUsbApi.BulkTransfer(handle, READPOINT, buffer, buffer.Length, out var len, 5000);
            CleanUpHandle(handle);
            return len;
        }

        private int SendInternal(byte[] buffer)
        {
            var handle = getUsableAndOpenUsbHandle();
            if (handle == null)
                throw new Exception("USB writer is null, you may have disconnected the device during previous function");

            uint pack = (uint)buffer.Length + 2;
            byte[] packed = BitConverter.GetBytes(pack);
            var ec = MonoUsbApi.BulkTransfer(handle, WRITEPOINT, packed, packed.Length, out var _, 5000);
            if (ec != 0)
            {
                string err = MonoUsbSessionHandle.LastErrorString;
                CleanUpHandle(handle);
                throw new Exception(err);
            }
            ec = MonoUsbApi.BulkTransfer(handle, WRITEPOINT, buffer, buffer.Length, out var len, 5000);
            if (ec != 0)
            {
                string err = MonoUsbSessionHandle.LastErrorString;
                CleanUpHandle(handle);
                throw new Exception(err);
            }

            CleanUpHandle(handle);
            return len;
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
            if (length > MaximumTransferSize)
                return ReadBytesLarge(offset, length);
            lock (_sync)
            {
                var cmd = PeekRaw(offset, length);
                SendInternal(cmd);

                // give it time to push data back
                Thread.Sleep((length / 256));

                var buffer = new byte[length];
                var _ = ReadInternal(buffer);
                //return Decoder.ConvertHexByteStringToBytes(buffer);
                return buffer;
            }
        }

        public void WriteBytes(byte[] data, uint offset)
        {
            if (data.Length > MaximumTransferSize)
                WriteBytesLarge(data, offset);
            lock (_sync)
            {
                var cmd = PokeRaw(offset, data);
                SendInternal(cmd);

                // give it time to push data back
                Thread.Sleep((data.Length / 256));
            }
        }

        public void WriteBytesMain(byte[] data, uint offset)
        {
            if (data.Length > MaximumTransferSize)
                WriteBytesLarge(data, offset);
            lock (_sync)
            {
                var cmd = PokeMain(offset, data);
                SendInternal(cmd);

                // give it time to push data back
                Thread.Sleep((data.Length / 256));
            }
        }

        public void SendBytes(byte[] encodeData)
        {
            lock (_sync)
            {
                SendInternal(encodeData);
            }
        }

        public byte[] GetVersion()
        {
            lock (_sync)
            {
                var cmd = Version();
                SendInternal(cmd);

                // give it time to push data back
                Thread.Sleep(100);
                var buffer = new byte[9];
                var _ = ReadInternal(buffer);
                return buffer;
            }
        }

        private void WriteBytesLarge(byte[] data, uint offset)
        {
            int byteCount = data.Length;
            for (int i = 0; i < byteCount; i += MaximumTransferSize)
                WriteBytes(SubArray(data, i, MaximumTransferSize), offset + (uint)i);
        }

        private byte[] ReadBytesLarge(uint offset, int length)
        {
            List<byte> read = new List<byte>();
            for (int i = 0; i < length; i += MaximumTransferSize)
                read.AddRange(ReadBytes(offset + (uint)i, Math.Min(MaximumTransferSize, length - i)));
            return read.ToArray();
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
