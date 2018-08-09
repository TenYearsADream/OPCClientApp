using OPCClientApp.Connection;
using OPCClientApp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCClientApp
{
    public partial class FormMain : Form
    {
        List<Plc> device = null;


        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            device = new List<Plc>();

            using (DataProvider db = new DataProvider(true))
            {
                db.SqlString = "SELECT [IP] FROM [ors_2016].[dbo].[PLC] WITH (NOLOCK) WHERE ADRES IS NOT NULL GROUP BY [IP]";
                using (DataTable dtip = db.GetTable())
                {
                    if (dtip != null && dtip.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtip.Rows)
                        {
                            Plc plc = new Plc(CpuType.S71200, row["IP"].ToString(), 0, 0, "");
                            db.SqlString = "SELECT [IP], [ISTASYON], [ADRES], [ACIKLAMA] FROM [ors_2016].[dbo].[PLC] WITH (NOLOCK) WHERE [IP] = N'" + row["IP"].ToString() + "'";
                            using (IDataReader dr = db.Select())
                            {
                                if (dr != null)
                                {
                                    while (dr.Read())
                                    {
                                        string adres = "";
                                        string istasyon = "";
                                        if (!dr.IsDBNull(dr.GetOrdinal("ADRES")))
                                            adres = dr.GetValue(dr.GetOrdinal("ADRES")).ToString();
                                        if (!dr.IsDBNull(dr.GetOrdinal("ISTASYON")))
                                            istasyon = dr.GetValue(dr.GetOrdinal("ISTASYON")).ToString();
                                        plc.Add(new PLCPin(adres, 0, istasyon));
                                    }
                                    dr.Close();
                                }
                            }
                            plc.Open();
                            device.Add(plc);
                        }
                    }
                }
            }

            device.TrimExcess();
            Console.WriteLine(device.Count);

            //timer1.Enabled = true;
            /*

         */

            //device = new Plc(CpuType.S71200, "192.168.145.2", 0, 0);
            //device = new Plc(CpuType.S71200, "192.168.144.1", 0, 0);
            //device.Open();
            //timer1.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            int deger = 0;
            foreach (var d in device)
            {
                for (int i = 0; i < d.Count; i++)
                {
                    if (int.TryParse(d.Read(d[i].Adres).ToString(), out deger))
                    {
                        d[i].Deger = deger;
                        Console.WriteLine(d[i].ToString());
                    }
                }
            }

            //label1.Text = device.Read("DB1.DBD0").ToString();
            //label2.Text = device.Read("DB1.DBD4").ToString();
            //label3.Text = device.Read("DB1.DBD80").ToString();
            //label4.Text = device.Read("DB1.DBD36").ToString();
            //label5.Text = device.Read("DB1.DBD40").ToString();
            //label6.Text = device.Read("DB1.DBD24").ToString();
            //label7.Text = device.Read("DB1.DBD28").ToString();
            //label8.Text = device.Read("DB1.DBD44").ToString();

            //device.Write("DB1.DBD0", 1);

            /*label1.Text = device.Read("DB1.DBD0").ToString();
            label2.Text = device.Read("DB1.DBD4").ToString();
            label3.Text = device.Read("DB1.DBD8").ToString();
            label4.Text = device.Read("DB1.DBD12").ToString();
            label5.Text = device.Read("DB1.DBD16").ToString();

            var redis = RedisStore.RedisCache;

            var smb = new ShortMessageBlog(redis);

            smb.AddUser("anton", "anton@delsink.com", "+971");
            smb.AddUser("anton", "anton@delsink.com", "+971");

            smb.AddPost("marius", "marius first post!");
            smb.AddPost("anton", "anton first post!");
            smb.AddPost("anton", "anton hello world");
            smb.AddPost("anton", "anton another post");
            smb.AddPost("anton", "anton last post");

            var all = smb.GetAllPosts();

            redis.StringSet("DB1.DBD0", label1.Text);
            redis.StringSet("DB1.DBD4", label2.Text);
            redis.StringSet("DB1.DBD8", label3.Text);
            redis.StringSet("DB1.DBD12", label4.Text);
            redis.StringSet("DB1.DBD16", label5.Text);

            */
            //if (redis.StringSet("testKey", label1.Text))
            //{
            //    var val = redis.StringGet("testKey");

            //    Console.WriteLine(val);
            //}


            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {




        }
    }
}
