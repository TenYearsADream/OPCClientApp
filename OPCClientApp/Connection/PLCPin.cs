using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCClientApp.Connection
{
    public class PLCPin : IDisposable
    {
        private readonly object lockObject = new object();

        public PLCPin() { }

        public PLCPin(string adres,int id, string istasyonKod)
        {
            this.Adres = adres;
            this.IstasyonId = id;
            this.IstasyonKod = istasyonKod;
        }

        public string Adres { get; set; }
        public int IstasyonId { get; set; }
        public string IstasyonKod { get; set; }

        public int Deger { get; set; }
        public DateTime Saat { get; set; }

        #region IDisposable
        ~PLCPin()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
            }
            disposed = true;
        }
        #endregion

        public override string ToString()
        {
            return new StringBuilder(this.Adres).Append("-->").Append(Deger).ToString();
        }

    }

    public struct PinAdres
    {
        public string Ad { get; set; }
        public string Adres { get; set; }
    }
}
