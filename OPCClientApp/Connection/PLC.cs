using OPCClientApp.Store;
using OPCClientApp.Util;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace OPCClientApp.Connection
{
    /// <summary>
    /// Creates an instance of S7.Net driver
    /// </summary>
    public partial class Plc : CollectionBase, IDisposable
    {
        private const int CONNECTION_TIMED_OUT_ERROR_CODE = 10060;
        const int TIMEOUT = 1000;// Debugda sure dusurulecek, canlýda 1 dk olacak
        //TCP connection to device
        private TcpClient tcpClient;
        private NetworkStream stream;
        System.Timers.Timer timerState = null;
        private IDatabase database = null;
        private ShortMessageBlog shortMessage = null;

        /// <summary>
        /// IP address of the PLC
        /// </summary>
        public string IP { get; private set; }

        public string Name { get; set; }

        /// <summary>
        /// CPU type of the PLC
        /// </summary>
        public CpuType CPU { get; private set; }

        /// <summary>
        /// Rack of the PLC
        /// </summary>
        public Int16 Rack { get; private set; }

        /// <summary>
        /// Slot of the CPU of the PLC
        /// </summary>
        public Int16 Slot { get; private set; }

        /// <summary>
        /// Max PDU size this cpu supports
        /// </summary>
        public Int16 MaxPDUSize { get; set; }

        /// <summary>
        /// Returns true if a connection to the PLC can be established
        /// </summary>
        public bool IsAvailable
        {
            //TODO: Fix This
            get
            {
                try
                {
                    Connect();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if the socket is connected and polls the other peer (the PLC) to see if it's connected.
        /// This is the variable that you should continously check to see if the communication is working
        /// See also: http://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    if (tcpClient == null)
                        return false;

                    //TODO: Actually check communication by sending an empty TPDU
                    return tcpClient.Connected;
                }
                catch { return false; }
            }
        }

        /// <summary>
        /// Creates a PLC object with all the parameters needed for connections.
        /// For S7-1200 and S7-1500, the default is rack = 0 and slot = 0.
        /// You need slot > 0 if you are connecting to external ethernet card (CP).
        /// For S7-300 and S7-400 the default is rack = 0 and slot = 2.
        /// </summary>
        /// <param name="cpu">CpuType of the PLC (select from the enum)</param>
        /// <param name="ip">Ip address of the PLC</param>
        /// <param name="rack">rack of the PLC, usually it's 0, but check in the hardware configuration of Step7 or TIA portal</param>
        /// <param name="slot">slot of the CPU of the PLC, usually it's 2 for S7300-S7400, 0 for S7-1200 and S7-1500.
        ///  If you use an external ethernet card, this must be set accordingly.</param>
        public Plc(CpuType cpu, string ip, Int16 rack, Int16 slot)
        {
            database = RedisStore.RedisCache;
            shortMessage = new ShortMessageBlog(database);

            if (!Enum.IsDefined(typeof(CpuType), cpu))
                throw new ArgumentException($"The value of argument '{nameof(cpu)}' ({cpu}) is invalid for Enum type '{typeof(CpuType).Name}'.", nameof(cpu));

            if (string.IsNullOrEmpty(ip))
                throw new ArgumentException("IP address must valid.", nameof(ip));

            CPU = cpu;
            IP = ip;
            Rack = rack;
            Slot = slot;
            MaxPDUSize = 240;
        }

        public Plc(CpuType cpu, string ip, Int16 rack, Int16 slot, string name) : this(cpu, ip, rack, slot)
        {
            this.Name = name;
        }


        /// <summary>
        /// Close connection to PLC
        /// </summary>
        public void Close()
        {
            if (tcpClient != null)
            {
                if (tcpClient.Connected) tcpClient.Close();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose Plc Object
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Plc() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region CollectionBase
        public int Add(PLCPin pin)
        {
            return this.InnerList.Add(pin);
        }

        public PLCPin this[int index]
        {
            get { return this.InnerList[index] as PLCPin; }
        }

        public void Remove(PLCPin pin)
        {
            this.InnerList.Remove(pin);
        }
        #endregion

        void timerState_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerState.Enabled = false;
                int count = 0;
                Thread.Sleep(20);

                Trace.WriteLine(this.IP + "\tOkunuyor ..");
                for (int loop = 0; loop < this.Count; loop++)
                {
                    if (int.TryParse(Read(this[loop].Adres).ToString(), out count))
                    {
                        PinValue pinValue = shortMessage.GetCounter(this.Address(loop));
                        DateTime time = Utility.UnixTimeToDateTime(pinValue.Time);
                        DateTime now = DateTime.Now;
                        Console.WriteLine(pinValue.Time);
                        if (count == pinValue.Count)
                        {
                            var times = (now - time).TotalMinutes;
                            if (times > 2)
                            {
                                Console.WriteLine("Duruþ baþlat " + time.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Duruþ varsa kapat " + (count - pinValue.Count).ToString());
                            Console.WriteLine("Log kaydý atýlacak " + (count - pinValue.Count).ToString());
                            shortMessage.AddCounter(this.Address(loop), count, (int)Utility.ConvertToUnixTime(now));
                        }

                        this[loop].Deger = count;
                        //database.StringSet(this[loop].Adres, deger);
                        Console.WriteLine(this.Address(loop));
                    }
                }

                Trace.WriteLine(this.IP + "\tOkundu.");
            }
            catch (Exception exc)
            {
                StringBuilder sErr = new StringBuilder("PLC Okuma hatasý!");
                sErr.AppendLine(exc.Message);
                sErr.AppendLine(exc.StackTrace);
                Trace.WriteLine(sErr.ToString());
            }
            finally
            {
                timerState.Enabled = true;
            }
        }

        private string Address(int index)
        {
            return string.Concat(this.IP, ".", this[index].Adres);
        }
    }
}
