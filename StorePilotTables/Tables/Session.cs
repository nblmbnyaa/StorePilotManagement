using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class Session : TABLO
    {
        public Session(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("nvarchar-100")] public string sessionId { get; set; }
        [Description("uniqueidentifier")] public Guid userUuid { get; set; }
        [Description("nvarchar-50")] public string userName { get; set; }
        [Description("nvarchar-100")] public string fullName { get; set; }
        [Description("nvarchar-255")] public string token { get; set; }
        [Description("nvarchar-255")] public string refreshToken { get; set; }
        [Description("datetime")] public DateTime tokenExpiry { get; set; }
        [Description("nvarchar-255")] public string roles { get; set; }
        [Description("nvarchar-255")] public string permissions { get; set; }
        [Description("datetime")] public DateTime loginAt { get; set; }
        [Description("nvarchar-100")] public string deviceId { get; set; }
        [Description("nvarchar-100")] public string deviceModel { get; set; }
        [Description("nvarchar-50")] public string appVersion { get; set; }
        [Description("nvarchar-max")] public string errorMessage { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }


        public bool TokenKontrol(SqlCommand km, Guid token)
        {
            km.CommandText = "SELECT * FROM Session WITH(NOLOCK) WHERE token=@token";
            km.Parameters.Clear();
            km.Parameters.AddWithValue("@token", token);
            if (!ReadData(km))
            {
                hatamesaji = "Geçersiz oturum. Lütfen tekrar giriş yapın.";
                return false;
            }
            else
            {
                if (tokenExpiry < DateTime.Now)
                {
                    hatamesaji = "Oturum süresi dolmuş. Lütfen tekrar giriş yapın.";
                    return false;
                }

                return true;

            }

        }
    }
}
