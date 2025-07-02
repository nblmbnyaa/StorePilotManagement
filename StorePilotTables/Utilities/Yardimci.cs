using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Utilities
{
    public static class Yardimci
    {
        public static string Encrypt(string toEncrypt)
        {
            if (toEncrypt == null) toEncrypt = "";
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            string key = "tannblm";
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string cipherString)
        {
            try
            {
                if (cipherString == null)
                    cipherString = "";
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);
                string key = "tannblm";
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();


                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock
                        (toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return null;
            }
        }



        #region Dönüşüm Fonksiyonları

        public static string getstring(this object nesne)
        {
            string sonuc = "";
            try { sonuc = Convert.ToString(nesne); }
            catch (Exception) { }
            return sonuc;
        }

        public static string getSubString(this string nesne, int baslangic, int len)
        {
            if (baslangic > nesne.Length)
            {
                return "";
            }
            if (nesne.Length < baslangic + len)
            {
                return nesne.Substring(baslangic);
            }
            else
            {
                return nesne.Substring(baslangic, len);
            }
        }

        public static Guid getguid(this object nesne)
        {
            Guid sonuc = Guid.Empty;
            try
            { sonuc = Guid.Parse(nesne.getstring()); }
            catch (Exception) { }
            return sonuc;
        }
        public static Int32 Tamsayi(this object nesne)
        {
            int sonuc = 0;
            try { sonuc = Convert.ToInt32(nesne); }
            catch (Exception) { }
            return sonuc;
        }
        public static long tolong(this object nesne)
        {
            long sonuc = 0;
            try { sonuc = Convert.ToInt64(nesne); }
            catch (Exception) { }
            return sonuc;
        }
        public static byte getbyte(this object nesne)
        {
            byte sonuc = 0;
            try { sonuc = Convert.ToByte(nesne); }
            catch (Exception) { }
            return sonuc;
        }
        public static byte[] getbytedizi(this object nesne)
        {
            try
            {
                return (byte[])nesne;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static byte[] StringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static bool getbool(this object nesne)
        {
            bool sonuc = false;
            if (nesne.Tamsayi() >= 1) return true;
            try
            {
                sonuc = Convert.ToBoolean(nesne);
            }
            catch (Exception)
            {

            }
            return sonuc;
        }
        public static DateTime getdate(this object nesne)
        {
            DateTime sonuc = new DateTime(1899, 12, 30);
            try { sonuc = Convert.ToDateTime(nesne); }
            catch (Exception) { }
            if (sonuc < new DateTime(1899, 12, 30))
            {
                sonuc = new DateTime(1899, 12, 30);
            }
            return sonuc;
        }
        public static decimal getdeci(this object nesne)
        {
            decimal sonuc = 0;
            try { sonuc = Convert.ToDecimal(nesne); }
            catch (Exception) { }
            return sonuc;
        }
        public static double duble(this object nesne)
        {
            double sonuc = 0;
            try { sonuc = Convert.ToDouble(nesne); }
            catch (Exception) { }
            return sonuc;
        }
        public static DateTime bostarih(this DateTime nesne)
        {
            return new DateTime(1899, 12, 30);
        }

        #endregion
    }
}
