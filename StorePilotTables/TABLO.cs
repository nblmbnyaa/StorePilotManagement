using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using StorePilotTables.Utilities;

namespace StorePilotTables
{
    public class TABLO
    {
        public void CreateTable(SqlCommand cmd, string tabloAdi = "")
        {

            var dataMapper = new Dictionary<Type, string>();
            dataMapper.Add(typeof(int), "[int]");
            dataMapper.Add(typeof(int?), "[int]");

            dataMapper.Add(typeof(byte?), "[tinyint]");
            dataMapper.Add(typeof(byte), "[tinyint]");


            dataMapper.Add(typeof(string), "[nvarchar]");
            dataMapper.Add(typeof(bool), "[bit]");
            dataMapper.Add(typeof(long), "[bigint]");


            dataMapper.Add(typeof(bool?), "[bit]");
            dataMapper.Add(typeof(DateTime), "[datetime]");
            dataMapper.Add(typeof(DateTime?), "[datetime]");
            dataMapper.Add(typeof(float), "[float]");
            dataMapper.Add(typeof(float?), "[float]");
            dataMapper.Add(typeof(decimal), "[float]");
            dataMapper.Add(typeof(decimal?), "[float]");
            dataMapper.Add(typeof(Guid), "UNIQUEIDENTIFIER");
            dataMapper.Add(typeof(Guid?), "UNIQUEIDENTIFIER");
            dataMapper.Add(typeof(byte[]), "image");
            dataMapper.Add(typeof(byte?[]), "image");

            var nullMapper = new Dictionary<Type, object>();
            nullMapper.Add(typeof(int), 0);
            nullMapper.Add(typeof(int?), 0);
            nullMapper.Add(typeof(byte?), 0);
            nullMapper.Add(typeof(byte), 0);
            nullMapper.Add(typeof(string), "");
            nullMapper.Add(typeof(bool), 0);
            nullMapper.Add(typeof(long), 0);
            nullMapper.Add(typeof(bool?), 0);
            nullMapper.Add(typeof(DateTime), DateTime.Now.bostarih());
            nullMapper.Add(typeof(DateTime?), DateTime.Now.bostarih());
            nullMapper.Add(typeof(float), 0);
            nullMapper.Add(typeof(float?), 0);
            nullMapper.Add(typeof(decimal), 0);
            nullMapper.Add(typeof(decimal?), 0);
            nullMapper.Add(typeof(Guid), Guid.Empty);
            nullMapper.Add(typeof(Guid?), Guid.Empty);

            var Fields = new List<KeyValuePair<string, Type>>();
            var newFields = new List<KeyValuePair<string, Type>>();
            var list = new List<string>();
            if (tabloAdi == "")
            {
                tabloAdi = GetType().Name;
            }

            var _IDENTITY = "Id";
            foreach (PropertyInfo p in GetType().GetProperties())
            {
                var field = new KeyValuePair<string, Type>(p.Name, p.PropertyType);

                string description = Getdescription(p);
                if (string.IsNullOrEmpty(description)) continue;
                if (description.IndexOf("*") > -1)
                    _IDENTITY = p.Name;
                else
                {
                    Fields.Add(field);

                    list.Add(description.IndexOf("nvarchar") == -1 ? "" : "(" + description.Split('-')[1] + ")");

                }
            }

            var script = new StringBuilder();
            script.AppendLine(string.Format("IF NOT EXISTS (select * from sysobjects where name='{0}' and xtype='U')", tabloAdi));
            script.AppendLine(string.Format("CREATE TABLE dbo.[{0}]", tabloAdi));
            script.AppendLine("(");
            script.AppendLine(string.Format("\t [{0}] [int] IDENTITY(1,1) NOT null ", _IDENTITY));
            script.AppendLine(") ");

            for (int i = 0; i < Fields.Count; i++)
            {
                var field = Fields[i];

                if (dataMapper.ContainsKey(field.Value))
                {
                    script.AppendLine(string.Format("\t if not exists (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}' and COLUMN_NAME = '{1}')", tabloAdi, field.Key));
                    if (list[i] == "(4001)")
                    {
                        script.AppendLine(string.Format("\t ALTER TABLE [{0}] ADD [{1}] ntext NULL", tabloAdi, field.Key));
                    }
                    if (list[i] == "(4002)")
                    {
                        script.AppendLine(string.Format("\t ALTER TABLE [{0}] ADD [{1}] image NULL", tabloAdi, field.Key));
                    }
                    else
                    {
                        script.AppendLine(string.Format("\t ALTER TABLE [{0}] ADD [{1}] {2} {3} NULL", tabloAdi, field.Key, dataMapper[field.Value], list[i]));
                    }
                    script.Append(Environment.NewLine);

                    cmd.CommandText = "select count(*) from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @t and COLUMN_NAME = @c";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@t", tabloAdi);
                    cmd.Parameters.AddWithValue("@c", field.Key);
                    if (cmd.ExecuteScalar().Tamsayi() == 0)
                    {
                        newFields.Add(field);
                    }
                }
            }
            try
            {
                cmd.CommandText = script.ToString();
                cmd.Parameters.Clear();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                hatamesaji = ex.Message;

            }


            for (int i = 0; i < newFields.Count; i++)
            {
                var field = newFields[i];

                if (nullMapper.ContainsKey(field.Value))
                {

                    cmd.CommandText = "update " + tabloAdi + " set " + field.Key + "=@" + field.Key + " where " + field.Key + " is null";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@" + field.Key, nullMapper[field.Value]);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }


        private string Getdescription(PropertyInfo pi)
        {
            string description = "";
            if (pi.GetCustomAttributes(typeof(DescriptionAttribute), true).Length > 0)
                description = ((DescriptionAttribute)pi.GetCustomAttributes(typeof(DescriptionAttribute), true)[0]).Description.ToString();
            return description;
        }
        private SqlDbType Sqldbtip(string tipname)
        {
            return (SqlDbType)Enum.Parse(typeof(SqlDbType), tipname.Replace("*", ""), true);
        }
        //private SqlDataType Sqldatatip(string tipname)
        //{

        //    return (SqlDataType)Enum.Parse(typeof(SqlDataType), tipname.Replace("*", ""), true);            
        //}
        //private DataType Smodbtip(SqlDataType tip)
        //{
        //    DataType dt = new DataType(tip);
        //    return dt;
        //}
        //private DataType Smodbtip(SqlDataType tip, int len)
        //{
        //    var dt = new DataType(tip, len);


        //    return dt;
        //}

        private void CreateTableScript(SqlCommand cmd)
        {

            var dataMapper = new Dictionary<Type, string>();
            dataMapper.Add(typeof(int), "[int]");
            dataMapper.Add(typeof(int?), "[int]");

            dataMapper.Add(typeof(byte?), "[tinyint]");
            dataMapper.Add(typeof(byte), "[tinyint]");


            dataMapper.Add(typeof(string), "[nvarchar]");
            dataMapper.Add(typeof(bool), "[bit]");
            dataMapper.Add(typeof(long), "[bigint]");


            dataMapper.Add(typeof(bool?), "[bit]");
            dataMapper.Add(typeof(DateTime), "[datetime]");
            dataMapper.Add(typeof(DateTime?), "[datetime]");
            dataMapper.Add(typeof(float), "[float]");
            dataMapper.Add(typeof(float?), "[float]");
            dataMapper.Add(typeof(decimal), "[float]");
            dataMapper.Add(typeof(decimal?), "[float]");
            dataMapper.Add(typeof(Guid), "UNIQUEIDENTIFIER");
            dataMapper.Add(typeof(Guid?), "UNIQUEIDENTIFIER");



            var Fields = new List<KeyValuePair<string, Type>>();
            var list = new List<string>();
            var _className = GetType().Name;
            var _IDENTITY = "Id";
            foreach (PropertyInfo p in GetType().GetProperties())
            {
                var field = new KeyValuePair<string, Type>(p.Name, p.PropertyType);

                string description = Getdescription(p);
                if (string.IsNullOrEmpty(description)) continue;
                if (description.IndexOf("*") > -1)
                    _IDENTITY = p.Name;
                else
                {
                    Fields.Add(field);

                    list.Add(description.IndexOf("nvarchar") == -1 ? "" : "(" + description.Split('-')[1] + ")");

                }
            }

            var script = new StringBuilder();
            script.AppendLine(string.Format("IF NOT EXISTS (select * from sysobjects where name='{0}' and xtype='U')", _className));
            script.AppendLine("CREATE TABLE dbo.[" + _className + "]");
            script.AppendLine("(");
            script.AppendLine(string.Format("\t [{0}] [int] IDENTITY(1,1) NOT null ", _IDENTITY));
            script.AppendLine(") ");

            for (int i = 0; i < Fields.Count; i++)
            {
                var field = Fields[i];

                if (dataMapper.ContainsKey(field.Value))
                {
                    script.AppendLine(string.Format("\t if not exists (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}' and COLUMN_NAME = '{1}')", _className, field.Key));

                    script.AppendLine(string.Format("\t ALTER TABLE [{0}] ADD [{1}] {2} {3} NULL", _className, field.Key, dataMapper[field.Value], list[i]));


                    script.Append(Environment.NewLine);
                }
            }
            try
            {


                cmd.CommandText = script.ToString();
                cmd.Parameters.Clear();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                hatamesaji = ex.Message;

            }

        }

        //public TABLO()
        //{

        //}

        public TABLO(SqlCommand km)
        {
            if (km == null)
            {
                Temizle();
                return;
            }
            CreateTable(km);

            //string tablename = GetType().Name;
            //if (tablename=="BSR_TABLES")
            //{
            //    CreateTable(km, surumno);
            //    return;
            //}
            //int srm = 0;
            //km.CommandText = "select isnull((select surumno from BSR_TABLES with (nolock) where tablename = @table), '')";
            //km.Parameters.Clear();km.Parameters.AddWithValue("@table", tablename);
            //srm = km.ExecuteScalar().Tamsayi();
            //if (true) //sadece yüklü olan daha eski ise girsin KB
            //{
            //    Tabloyukleniyor.ShowForm();
            //    Tabloyukleniyor.Description(tablename + " Eski Sürüm " + "\r\nVeritabaný : " + srm + "\r\nGüncel : " + surumno);
            //    CreateTable(km, surumno);


            //    Tabloyukleniyor.CloseForm();



            //}
            //else
            //{
            //    if (srm > surumno) { 
            //        //MessageBox.Show("Veri Tabaný Kullandýðýnýz Programdan Daha Yeni"); 
            //    }

            //}
            Temizle();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Temizle()
        {
            PropertyInfo[] pi = GetType().GetProperties();
            for (int i = 0; i < pi.Length; i++)
            {
                string name = pi[i].Name;
                string description = Getdescription(pi[i]);
                if (description == "")
                    continue;
                if (pi[i].PropertyType == typeof(string) || pi[i].PropertyType == typeof(string))
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType("", pi[i].PropertyType), null);

                else if (pi[i].PropertyType == typeof(short) ||
                            pi[i].PropertyType == typeof(int) ||
                            pi[i].PropertyType == typeof(long) ||
                            pi[i].PropertyType == typeof(float) ||
                            pi[i].PropertyType == typeof(decimal) ||
                            pi[i].PropertyType == typeof(int) ||
                            pi[i].PropertyType == typeof(decimal) ||
                            pi[i].PropertyType == typeof(long) ||
                            pi[i].PropertyType == typeof(short) ||
                            pi[i].PropertyType == typeof(byte) ||
                            pi[i].PropertyType == typeof(decimal) ||
                            pi[i].PropertyType == typeof(decimal) ||
                            pi[i].PropertyType == typeof(float))
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType(0, pi[i].PropertyType), null);

                else if (pi[i].PropertyType == typeof(DateTime))
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType(DateTime.Now.bostarih(), pi[i].PropertyType), null);

                else if (pi[i].PropertyType == typeof(bool) || pi[i].PropertyType == typeof(bool))
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType(false, pi[i].PropertyType), null);

                else if (pi[i].PropertyType == typeof(char) || pi[i].PropertyType == typeof(char))
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType("", pi[i].PropertyType), null);

                else if (pi[i].PropertyType == typeof(Guid) || pi[i].PropertyType == typeof(Guid))
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType(Guid.Empty, pi[i].PropertyType), null);

                else if (pi[i].PropertyType.IsEnum)
                {
                    object enumval = Enum.Parse(pi[i].PropertyType, "0");
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType(enumval, pi[i].PropertyType), null);
                }
                else if (pi[i].PropertyType == typeof(Guid))
                {
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType(Guid.Empty, pi[i].PropertyType), null);
                }
                else
                    GetType().GetProperty(name).SetValue(this, Convert.ChangeType(null, pi[i].PropertyType), null);

            }
        }

        public void WhiteSpaceClear()
        {
            PropertyInfo[] pi = GetType().GetProperties();
            for (int i = 0; i < pi.Length; i++)
            {
                string name = pi[i].Name;
                string description = Getdescription(pi[i]);
                if (description == "")
                    continue;
                if (pi[i].PropertyType == typeof(string) || pi[i].PropertyType == typeof(string))
                {
                    if (GetType().GetProperty(name).GetValue(this, null) != null)
                    {
                        string vl = GetType().GetProperty(name).GetValue(this, null).getstring();
                        vl = System.Text.RegularExpressions.Regex.Replace(vl, @"\s", "");
                        GetType().GetProperty(name).SetValue(this, Convert.ChangeType(vl, pi[i].PropertyType), null);
                    }
                }
            }
        }


        /// <summary>
        /// Ýnsert Ýþlemi 
        /// </summary>
        /// <param name="km"></param>
        /// <returns></returns>
        public int Insert(SqlCommand km, string _tablename = "")
        {
            string tablename = GetType().Name;

            if (!string.IsNullOrEmpty(_tablename))
            {
                tablename = _tablename;
            }

            tablename = $"dbo.[{tablename}]";

            km.CommandText = "insert into " + tablename + " (#kolon) VALUES (#value) select SCOPE_IDENTITY()";
            km.Parameters.Clear();

            bool idvar = false;

            PropertyInfo[] pi = GetType().GetProperties();
            for (int i = 0; i < pi.Length; i++)
            {
                string description = Getdescription(pi[i]);
                if (description == "")
                    continue;

                string name = pi[i].Name;
                if (description.IndexOf("*") > 0)
                {
                    idvar = true;
                    continue;
                }

                string tip = "";
                if (description.Split('-').Length > 1)
                    tip = description.Split('-')[0].ToString();
                else
                    tip = description;
                if (GetType().GetProperty(name).GetValue(this, null) != null)
                {
                    SqlDbType sqltip = Sqldbtip(tip);
                    km.CommandText = km.CommandText.Replace("#kolon", "[" + name + "]" + ", #kolon").Replace("#value", "@" + name + ", #value");
                    if (description == "nvarchar-4002")
                    {
                        km.Parameters.Add("@" + name, SqlDbType.Image, ((byte[])GetType().GetProperty(name).GetValue(this, null)).Length).Value = GetType().GetProperty(name).GetValue(this, null);
                    }
                    else
                    {
                        var obj = GetType().GetProperty(name).GetValue(this, null);
                        if (sqltip == SqlDbType.DateTime) obj = obj.getdate();
                        km.Parameters.Add("@" + name, sqltip).Value = obj;
                    }
                }
                else
                {
                    km.Parameters.AddWithValue("@" + name, DBNull.Value);
                }

            }
            if (!idvar)
            {
                km.CommandText = km.CommandText.Replace("select SCOPE_IDENTITY()", "");
            }

            km.CommandText = km.CommandText.Replace(", #kolon", "").Replace(", #value", "");
            try
            {
                hatamesaji = "";

                if (!idvar)
                {
                    km.ExecuteScalar();
                    return 1;
                }
                else
                {
                    return km.ExecuteScalar().Tamsayi();
                }
            }
            catch (Exception ex)
            {
                hatamesaji = ex.Message;
                return 0;
            }
        }
        public bool Update(SqlCommand km, string _tablename = "")
        {
            string tablename = GetType().Name;
            if (!string.IsNullOrEmpty(_tablename))
            {
                tablename = _tablename;
            }

            km.CommandText = "update dbo.[" + tablename + "] set #kolon = #value  where #id = @#id";
            km.Parameters.Clear();

            PropertyInfo[] pi = GetType().GetProperties();
            for (int i = 0; i < pi.Length; i++)
            {
                string description = Getdescription(pi[i]);
                if (description == "")
                    continue;

                string name = pi[i].Name;
                if (description.IndexOf("*") <= 0)
                {
                    string tip = "";
                    if (description.Split('-').Length > 1)
                        tip = description.Split('-')[0].ToString();
                    else
                        tip = description;

                    SqlDbType sqltip = Sqldbtip(tip);
                    km.CommandText = km.CommandText.Replace("#kolon", "[" + name + "]").Replace("#value", "@" + name + ", #kolon = #value");
                    //km.Parameters.Add("@" + name, sqltip).Value = (int)typeof(TABLO).GetProperty(name).GetValue(this, null);
                    //Type T = t.GetType().GetProperty(name).PropertyType;
                    if (GetType().GetProperty(name).GetValue(this, null) != null)
                    {
                        var obj = GetType().GetProperty(name).GetValue(this, null);
                        if (sqltip == SqlDbType.DateTime) obj = obj.getdate();
                        km.Parameters.Add("@" + name, sqltip).Value = obj;
                    }
                    else
                    {
                        km.Parameters.Add("@" + name, sqltip).Value = DBNull.Value;
                    }
                }
                else
                {
                    km.CommandText = km.CommandText.Replace("#id", name);
                    string tip = "";
                    if (description.Split('-').Length > 1)
                        tip = description.Split('-')[0].ToString();
                    else
                        tip = description;
                    SqlDbType sqltip = Sqldbtip(tip);
                    if (GetType().GetProperty(name).GetValue(this, null) != null)
                    {
                        km.Parameters.Add("@" + name, sqltip).Value = GetType().GetProperty(name).GetValue(this, null);
                    }
                    else
                    {
                        km.Parameters.Add("@" + name, sqltip).Value = DBNull.Value;
                    }
                }
            }
            if (km.CommandText.IndexOf("#id") > 0)
            {
                km.CommandText = km.CommandText.Replace("#id", pi[0].Name);
            }
            km.CommandText = km.CommandText.Replace(", #kolon = #value", "");
            try
            {
                hatamesaji = "";
                km.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                hatamesaji = ex.Message;
                ////MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool Delete(SqlCommand km, int recno)
        {
            PropertyInfo[] pi = GetType().GetProperties();
            string name = "";
            km.Parameters.Clear();
            string tablename = GetType().Name;
            km.CommandText = "delete  from " + tablename + " where ";
            for (int i = 0; i < pi.Length; i++)
            {
                string description = Getdescription(pi[i]);
                if (description == "")
                    continue;
                if (description.IndexOf("*") > 0)
                {
                    name = pi[i].Name;
                    break;
                }
            }
            km.CommandText += name + " = @" + name;
            km.Parameters.AddWithValue("@" + name, recno);
            try
            {
                hatamesaji = "";
                km.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                hatamesaji = ex.Message;
                ////MessageBox.Show(ex.Message);
                return false;
            }

        }

        public bool ReadData(SqlCommand km, Guid guid)
        {
            PropertyInfo[] pi = GetType().GetProperties();
            string name = "";

            km.Parameters.Clear();
            string tablename = GetType().Name;
            km.CommandText = "select top 1 * from " + tablename + " where ";
            for (int i = 0; i < pi.Length; i++)
            {
                string description = Getdescription(pi[i]);
                if (description == "")
                    continue;
                if (description.IndexOf("*") > 0)
                {
                    name = pi[i].Name;
                    break;
                }
            }
            km.CommandText += name + " = @" + name;
            km.Parameters.AddWithValue("@" + name, guid);

            return ReadData(km);
        }

        public bool ReadData(SqlCommand km, int recno)
        {
            PropertyInfo[] pi = GetType().GetProperties();
            string name = "";

            km.Parameters.Clear();
            string tablename = GetType().Name;
            km.CommandText = "select top 1 * from " + tablename + " where ";
            for (int i = 0; i < pi.Length; i++)
            {
                string description = Getdescription(pi[i]);
                if (description == "")
                    continue;
                if (description.IndexOf("*") > 0)
                {
                    name = pi[i].Name;
                    break;
                }
            }
            km.CommandText += name + " = @" + name;
            km.Parameters.AddWithValue("@" + name, recno);

            return ReadData(km);
        }
        public bool ReadData(SqlCommand km)
        {
            bool oku = false;
            PropertyInfo[] pi = GetType().GetProperties();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(km);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (PropertyInfo t in pi)
                    {
                        if (string.IsNullOrEmpty(Getdescription(t))) continue;
                        string name = t.Name;
                        if (dt.Columns[name] == null) continue;
                        if (dr[name] == DBNull.Value)
                        {
                            switch (t.PropertyType.Name)
                            {
                                case "Int32":
                                case "Int16":
                                case "Int64":
                                case "Double":
                                case "Decimal":
                                case "Byte":
                                    dr[name] = 0;
                                    break;
                                case "String":
                                    dr[name] = "";
                                    break;
                                case "DateTime":
                                    dr[name] = DateTime.Now.bostarih();
                                    break;
                                case "Boolean":
                                    dr[name] = false;
                                    break;
                                case "Guid":
                                    dr[name] = Guid.Empty;
                                    break;
                                case "Byte[]":
                                    dr[name] = new byte[0];
                                    break;

                            }
                        }
                        GetType().GetProperty(name).SetValue(this, Convert.ChangeType(dr[name], t.PropertyType), null);

                        oku = true;
                    }
                }
            }
            else { oku = false; }
            return oku;
        }

        [JsonIgnore]
        public string hatamesaji { get; set; }

        public string ToStr()
        {
            string toStrSonuc = "";
            PropertyInfo[] pi = GetType().GetProperties();
            for (int i = 0; i < pi.Length; i++)
            {
                string name = pi[i].Name;
                if (GetType().GetProperty(name).GetValue(this, null) != null)
                {
                    var obj = GetType().GetProperty(name).GetValue(this, null);
                    toStrSonuc += name + ":" + obj.getstring() + Environment.NewLine;
                }
                else
                {
                    toStrSonuc += name + ":" + "null" + Environment.NewLine;
                }
            }
            return toStrSonuc;
        }

        public enum TabloHareketTipi
        {
            create = 0,
            alter = 1,
        }

        public string InsertColumns()
        {
            string sonuc = "#kolon";
            PropertyInfo[] pi = GetType().GetProperties();
            for (int i = 0; i < pi.Length; i++)
            {
                string description = Getdescription(pi[i]);
                if (description == "")
                    continue;
                if (description.IndexOf("*") > 0)
                {
                    continue;
                }
                string name = pi[i].Name;

                sonuc = sonuc.Replace("#kolon", "[" + name + "]" + ", #kolon").Replace("#value", "@" + name + ", #value");
            }


            sonuc = sonuc.Replace(", #kolon", "");
            return sonuc;
        }


        public class TabloIndex
        {
            public string Name { get; set; }
            public List<string> IndexColumns { get; set; }
            public bool IsUnique { get; set; }
            public bool IsClustered { get; set; }
        }

        public void IndexCreate(SqlCommand km, TabloIndex index)
        {
            if (km==null)
            {
                return;
            }
            string tablename = GetType().Name;
            string IndexColumns = "";
            foreach (var item in index.IndexColumns)
            {
                IndexColumns += item + " ASC,";
            }
            if (IndexColumns.Length == 0) return;
            IndexColumns = IndexColumns.Substring(0, IndexColumns.Length - 1);
            string indexName = index.Name.Replace("#TABLO#", tablename);

            km.CommandText = @"if not exists(select * from sys.indexes where name='" + indexName + @"')
begin

CREATE NONCLUSTERED INDEX[" + indexName + @"] ON[dbo].[" + tablename + @"]
(

   " + IndexColumns + @"
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)

end";
            km.ExecuteNonQuery();
        }
    }
}
