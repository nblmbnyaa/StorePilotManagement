using Microsoft.Data.SqlClient;
using StorePilotTables.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class User : TABLO
    {
        public User(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(uuid) },
                IsClustered = false,
                IsUnique = true,
                Name = "IX_#TABLO#_01"
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(userName) },
                IsClustered = false,
                IsUnique = true,
                Name = "IX_#TABLO#_02"
            });

            if (km != null)
            {
                km.CommandText = "select count(*) from [User] with(nolock) where userName='admin'";
                km.Parameters.Clear();
                int count = (int)km.ExecuteScalar();
                if (count == 0)
                {
                    Temizle();
                    uuid = Guid.NewGuid(); // Yeni UUID oluşturuluyor
                    userCode = "admin";
                    userName = "admin";
                    fullName = "Admin";
                    password = Yardimci.Encrypt("admin");
                    userType = (int)UserType.Admin;
                    roleUuid = Guid.Empty; // Varsayılan rol ID'si, gerekirse değiştirilebilir
                    regionUuid = Guid.Empty; // Varsayılan bölge ID'si, gerekirse değiştirilebilir
                    createdByUuid = Guid.Empty; // Oluşturan ID'si, gerekirse değiştirilebilir
                    deviceId = ""; // Cihaz ID'si boş, gerekirse değiştirilebilir
                    isActive = true; // Kullanıcı aktif olarak işaretleniyor
                    isDeleted = false; // Kullanıcı silinmemiş olarak işaretleniyor
                    isSynced = true; // Kullanıcı senkronize edilmiş olarak işaretleniyor
                    createdAt = DateTime.Now;
                    updatedAt = createdAt; // Güncelleme zamanı, oluşturma zamanı ile aynı

                    id = Insert(km);
                    Temizle();
                }
            }
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("nvarchar-20")] public string userCode { get; set; }
        [Description("nvarchar-50")] public string userName { get; set; }
        [Description("nvarchar-100")] public string fullName { get; set; }
        [Description("nvarchar-MAX")] public string password { get; set; }
        [Description("int")] public int userType { get; set; }
        [Description("uniqueidentifier")] public Guid roleUuid { get; set; }
        [Description("uniqueidentifier")] public Guid regionUuid { get; set; }
        [Description("uniqueidentifier")] public Guid createdByUuid { get; set; }
        [Description("nvarchar-255")] public string deviceId { get; set; }
        [Description("bit")] public bool isActive { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }


        public enum UserType
        {
            Admin = 1,
            Merchant = 2,
            Responsible = 3,
        }

    }
}
