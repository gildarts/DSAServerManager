using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Campus;
using FISCA.Authentication;
using FISCA.DSA;

namespace DSAServerManager
{
    //嘿嘿
    internal class ConnectionHelper
    {
        public static string SessionInvalidCode = "511";

        private static Dictionary<string, ConnectionHelper> Helpers { get; set; }

        private Connection InternalConnection { get; set; }

        private string TargetDSNS { get; set; }

        private object SyncRoot = new object();

        static ConnectionHelper()
        {
            Helpers = new Dictionary<string, ConnectionHelper>();
        }

        public ConnectionHelper(string uid)
        {
            DynamicCache dc = Program.GlobalSchoolCache;
            TargetDSNS = dc[uid].DSNS;
        }

        private void DoConnect()
        {
            InternalToken it = new InternalToken(DSAServices.PassportToken.PassportContent);

            InternalConnection = new Connection();
            InternalConnection.Connect(AccessPoint.Parse(TargetDSNS), "sm", it);
        }

        public Envelope CallService(string srvName, Envelope req)
        {
            try
            {
                lock (SyncRoot)
                {
                    if (InternalConnection == null)
                        DoConnect();
                }

                return InternalConnection.SendRequest(srvName, req);
            }
            catch (DSAServerException ex)
            {
                if (ex.Status == "511")
                {
                    DoConnect();
                    return InternalConnection.SendRequest(srvName, req);
                }
                else
                    throw;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ConnectionHelper GetConnection(string uid)
        {
            if (!Helpers.ContainsKey(uid))
            {
                ConnectionHelper ch = new ConnectionHelper(uid);
                Helpers.Add(uid, ch);
            }

            return Helpers[uid];
        }

        public static void ResetConnection(string uid)
        {
            if (Helpers.ContainsKey(uid))
                Helpers.Remove(uid);
        }

        class InternalToken : SecurityToken
        {
            private string TokenContent { get; set; }

            public InternalToken(string content)
            {
                TokenContent = content;
            }

            public override string TokenType
            {
                get { return "Passport"; }
            }

            protected override string XmlString { get { return TokenContent; } }
        }
    }
}
