using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApplication1
{
  public class LicenseClass
  {
    public int XorPattern;
    public int Length;
    public long Checksum;
    public int ProgramID;
    public long Feature;
    public DateTime Expiration;
    public bool Expired;
    public string Name;
    public bool SDS;
    public bool LSt;
    public bool FEZ;
    public bool LStFull;
    public bool Monitor;
    public bool Pro;
    public short VerMaj;
    public short VerMin;
    public string KeyString;
    public int MaxGeraete;

    public LicenseClass()
    {
      this.XorPattern = 0;
      this.Length = 0;
      this.Checksum = 0L;
      this.ProgramID = 0;
      this.Feature = 0L;
      this.Expiration = Conversions.ToDate("18.02.2074");
      this.Expired = true;
      this.Name = "DEMO";
      this.SDS = false;
      this.LSt = false;
      this.FEZ = false;
      this.LStFull = false;
      this.Monitor = false;
      this.Pro = false;
      this.VerMaj = (short) 1;
      this.VerMin = (short) 0;
      this.KeyString = "";
      this.MaxGeraete = 4;
    }

    public string SwapHex(string S)
    {
      string str1 = "";
      string str2;
      if (Strings.Len(S) % 2 != 0)
      {
        str2 = S;
      }
      else
      {
        int num = checked (Strings.Len(S) - 1);
        int Start = 1;
        while (Start <= num)
        {
          str1 = str1 + Strings.Mid(S, checked (Start + 1), 1) + Strings.Mid(S, Start, 1);
          checked { Start += 2; }
        }
        str2 = str1;
      }
      return str2;
    }

    public string HexToString(string S, int XorPat = 0)
    {
      string str1;
      if (Strings.Len(S) % 2 != 0)
      {
        str1 = "";
      }
      else
      {
        string str2 = "";
        XorPat %= 256;
        int num = Strings.Len(S);
        int Start = 1;
        while (Start <= num)
        {
          str2 += Conversions.ToString(Strings.Chr(Convert.ToInt32(Strings.Mid(S, Start, 2), 16) ^ XorPat));
          XorPat = checked (XorPat ^ Convert.ToInt32(Strings.Mid(S, Start, 2), 16) ^ XorPat + 1) % 256;
          checked { Start += 2; }
        }
        str1 = str2;
      }
      return str1;
    }

    public double GetChecksum(string S)
    {
      int num1 = Strings.Len(S);
      int Start = 1;
      double num2=0;
      while (Start <= num1)
      {
        num2 += (double) Strings.Asc(Strings.Mid(S, Start, 1));
        checked { ++Start; }
      }
      return num2 % 65536.0;
    }

    public string LizenzKey
    {
      set
      {
        try
        {
          if (Strings.Len(value) < 27)
          {
            this.ProgramID = 0;
            this.Expired = true;
          }
          else
          {
            this.KeyString = value;
            if (value.StartsWith("1$"))
            {
              string Right = value.Substring(2, 4);
              value = Strings.Mid(value, 7);
              if (Operators.CompareString(GetSHA1(value).Substring(5, 4).ToUpper(), Right, false) != 0)
              {
                this.ProgramID = 0;
                this.Expired = true;
                return;
              }
            }
            else
            {
              string Right = value.Substring(0, 4);
              value = Strings.Mid(value, 5);
              if (Operators.CompareString(this.getMd5(value).Substring(4, 4).ToUpper(), Right, false) != 0)
              {
                this.ProgramID = 0;
                this.Expired = true;
                return;
              }
            }
            value = this.SwapHex(value);
            this.XorPattern = Convert.ToInt32(Strings.Left(value, 2), 16);
            this.Checksum = (long) (Convert.ToInt32(Strings.Mid(value, 3, 4), 16) ^ checked (this.XorPattern * 256 + this.XorPattern));
            this.ProgramID = Convert.ToInt32(Strings.Mid(value, 7, 2), 16) ^ this.XorPattern;
            this.Feature = (long) (Convert.ToInt32(Strings.Mid(value, 9, 4), 16) ^ checked (this.XorPattern * 256 + this.XorPattern));
            this.Expiration = Conversions.ToDate(Strings.Format((object) (Convert.ToInt32(Strings.Mid(value, 13, 4), 16) ^ checked (this.XorPattern * 256 + this.XorPattern)), "0000") + "-" + Strings.Format((object) (Convert.ToInt32(Strings.Mid(value, 17, 2), 16) ^ this.XorPattern), "00") + "-" + Strings.Format((object) (Convert.ToInt32(Strings.Mid(value, 19, 2), 16) ^ this.XorPattern), "00"));
            this.Expired = DateTime.Compare(DateAndTime.Now, this.Expiration) > 0;
            this.VerMaj = checked ((short) (Convert.ToInt32(Strings.Mid(value, 21, 2), 16) ^ this.XorPattern));
            this.VerMin = checked ((short) (Convert.ToInt32(Strings.Mid(value, 23, 2), 16) ^ this.XorPattern));
            this.Name = this.HexToString(Strings.Mid(value, 25), this.XorPattern);
            if (this.GetChecksum(this.ProgramID.ToString("00") + this.Feature.ToString("0000") + this.Expiration.ToString("yyyyMMdd") + this.VerMaj.ToString("00").Substring(0, 2) + this.VerMin.ToString("00").Substring(0, 2) + this.HexToString(Strings.Mid(value, 25), this.XorPattern)) != (double) this.Checksum)
            {
              this.ProgramID = 0;
              this.Feature = 0L;
            }
            this.SDS = (this.Feature & 1L) == 1L;
            this.Monitor = (this.Feature & 2L) == 2L;
            this.Pro = (this.Feature & 4L) == 4L;
            this.LSt = (this.Feature & 16L) == 16L;
            this.FEZ = (this.Feature & 8L) == 8L;
            this.LStFull = this.LSt & this.FEZ;
            if ((this.Feature & 768L) == 256L)
              this.MaxGeraete = 1;
            if ((this.Feature & 768L) == 512L)
              this.MaxGeraete = 2;
            if ((this.Feature & 768L) != 768L)
              return;
            this.MaxGeraete = 3;
          }
        }
        catch (Exception ex)
        {
          ProjectData.SetProjectError(ex);
          this.ProgramID = 0;
          this.Feature = 0L;
          this.Expired = true;
          ProjectData.ClearProjectError();
        }
      }
    }


    public string GetSHA1(string input)
    {
        byte[] byteArray= SHA1Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

        string result = System.Text.Encoding.UTF8.GetString(byteArray);
        return result;
    }


    public string getMd5(string S) 
    {
        return this.ByteArrayToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(S)));
    }
      

    

    private string ByteArrayToString(byte[] arrInput)
    {
      StringBuilder stringBuilder = new StringBuilder(checked (arrInput.Length * 2));
      int num = checked (arrInput.Length - 1);
      int index = 0;
      while (index <= num)
      {
        stringBuilder.Append(arrInput[index].ToString("X2"));
        checked { ++index; }
      }
      return stringBuilder.ToString().ToLower();
    }
  }
}