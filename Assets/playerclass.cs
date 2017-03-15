/*
    DemobyDemirkaya
    Vertigo Demo Project
    yazan: ibrahim taylan demirkaya
    
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



public class playerclass {
    
    static public Text highscoretext;

    static private int endianFark1;
    static private int endianFark2;
    static private int idx;
    static private byte[] baytBlock;
    enum ArrayType { Float, Int32, Bool, String, Vector2 }


    public static void yuksekSkortexti(Text scotext)//Yüksek Skor
    {
        highscoretext = scotext;
        prefInttoText("yuksek");
    }

    public static void highScoreyaz(int newsco)
    {
        kaydetHighS("yuksek", newsco);
        prefInttoText("yuksek");
    }

    private static void prefInttoText(string cek)
    {
        string yaz = loadInt(cek).ToString();
        highscoretext.text = yaz;
    }

    public static void gameOverPrefs(int skor)
    {
        highScoreyaz(skor);
        PlayerPrefs.DeleteKey("renkler");
        PlayerPrefs.DeleteKey("newblock");
        PlayerPrefs.DeleteKey("sskor");
        PlayerPrefs.DeleteKey("hammer");
        PlayerPrefs.DeleteKey("skip");
    }

    public static bool hammerPower()
    {
        if (PlayerPrefs.HasKey("hammer"))
        {
            if((loadInt("hammer") - 1) > 0)
            {
                return kaydetInt("hammer", loadInt("hammer") - 1);
            }
            return false;
        }
        return kaydetInt("hammer", 3);
    }
    public static bool skipPower()
    {
        if (PlayerPrefs.HasKey("skip"))
        {
            if ((loadInt("skip") - 1) > 0)
            {
                return kaydetInt("skip", loadInt("skip") - 1);
            }
            return false;
        }
        return kaydetInt("skip", 3);
    }
    /**************Buradan aşağısı PlayerPrefs Byte Yüklemesi için******************/
    public static bool kaydetStringDizisi(String key, String[] stringArray)
    {
        var bytes = new byte[stringArray.Length + 1];
        bytes[0] = Convert.ToByte(ArrayType.String);
        baslangicAyarlaması();
        
        for (var i = 0; i < stringArray.Length; i++)
        {
            if (stringArray[i] == null)
            {
                Debug.LogError("boş dizi " + key);
                return false;
            }
            if (stringArray[i].Length > 255)
            {
                Debug.LogError("255 karakter sınırı " + key);
                return false;
            }
            bytes[idx++] = (byte)stringArray[i].Length;
        }

        try
        {
            PlayerPrefs.SetString(key, Convert.ToBase64String(bytes) + "|" + String.Join("", stringArray));
        }
        catch
        {
            return false;
        }
        return true;
    }

    public static String[] loadStringDizisi(String key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var completeString = PlayerPrefs.GetString(key);
            var separatorIndex = completeString.IndexOf("|"[0]);
            if (separatorIndex < 4)
            {
                Debug.LogError("şu dosya dosya uçmuş " + key);
                return new String[0];
            }
            var bytes = System.Convert.FromBase64String(completeString.Substring(0, separatorIndex));
            if ((ArrayType)bytes[0] != ArrayType.String)
            {
                Debug.LogError(key + " bir string dizisi değil");
                return new String[0];
            }
            baslangicAyarlaması();

            var numberOfEntries = bytes.Length - 1;
            var stringArray = new String[numberOfEntries];
            var stringIndex = separatorIndex + 1;
            for (var i = 0; i < numberOfEntries; i++)
            {
                int stringLength = bytes[idx++];
                if (stringIndex + stringLength > completeString.Length)
                {
                    Debug.LogError("şu dosya dosya uçmuş " + key);
                    return new String[0];
                }
                stringArray[i] = completeString.Substring(stringIndex, stringLength);
                stringIndex += stringLength;
            }

            return stringArray;
        }
        return new String[0];
    }

    public static int loadInt(String key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var intlist = new List<int>();
            loadAna(key, intlist, ArrayType.Int32, 1, cevirInte);
            return intlist[0];
        }
        return 0;
    }

    public static bool kaydetHighS(String key, int skor)
    {
        return kaydetInt(key, loadInt(key) < skor ? skor : loadInt(key));
    }
    public static bool kaydetInt(String key, int skor)
    {
        int[] intlist = { skor };
        return kaydetAna(key, intlist, ArrayType.Int32, 1, cevirIntten);
    }

    private static bool kaydetAna<T>(String key, T array, ArrayType arrayType, int vectorNumber, Action<T, byte[], int> convert) where T : IList
    {
        var bytes = new byte[(4 * array.Count) * vectorNumber + 1];
        bytes[0] = Convert.ToByte(arrayType); 
        baslangicAyarlaması();

        for (var i = 0; i < array.Count; i++)
        {
            convert(array, bytes, i);
        }
        return baytKaydet(key, bytes);
    }

    private static void loadAna<T>(String key, T list, ArrayType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList
    {
        if (PlayerPrefs.HasKey(key))
        {
            var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
            if ((bytes.Length - 1) % (vectorNumber * 4) != 0)
            {
                Debug.LogError("bu dosya haşat olmuş " + key);
                return;
            }
            if ((ArrayType)bytes[0] != arrayType)
            {
                Debug.LogError(key + " bu " + arrayType.ToString() + " tipte bi dizi değil");
                return;
            }
            baslangicAyarlaması();

            var end = (bytes.Length - 1) / (vectorNumber * 4);
            for (var i = 0; i < end; i++)
            {
                convert(list, bytes);
            }
        }
    }

    private static void cevirInte(List<int> list, byte[] bytes)
    {
        list.Add(cevirBytetanInt32ye(bytes));
    }
    private static void cevirIntten(int[] array, byte[] bytes, int i)
    {
        cevirInt32denBytea(array[i], bytes);
    }

    private static void baslangicAyarlaması()
    {
        if (BitConverter.IsLittleEndian)
        {
            endianFark1 = 0;
            endianFark2 = 0;
        }
        else
        {
            endianFark1 = 3;
            endianFark2 = 1;
        }
        if (baytBlock == null)
        {
            baytBlock = new byte[4];
        }
        idx = 1;
    }

    private static bool baytKaydet(string key, byte[] bytes)
    {
        try
        {
            PlayerPrefs.SetString(key, Convert.ToBase64String(bytes));
        }
        catch
        {
            return false;
        }
        return true;
    }
    
    private static void cevirInt32denBytea(int i, byte[] bytes)
    {
        baytBlock = BitConverter.GetBytes(i);
        dortluByteaCevir(bytes);
    }

    private static int cevirBytetanInt32ye(byte[] bytes)
    {
        dortluBytetanCevir(bytes);
        return BitConverter.ToInt32(baytBlock, 0);
    }

    private static void dortluByteaCevir(byte[] bytes)
    {
        bytes[idx] = baytBlock[endianFark1];
        bytes[idx + 1] = baytBlock[1 + endianFark2];
        bytes[idx + 2] = baytBlock[2 - endianFark2];
        bytes[idx + 3] = baytBlock[3 - endianFark1];
        idx += 4;
    }

    private static void dortluBytetanCevir(byte[] bytes)
    {
        baytBlock[endianFark1] = bytes[idx];
        baytBlock[1 + endianFark2] = bytes[idx + 1];
        baytBlock[2 - endianFark2] = bytes[idx + 2];
        baytBlock[3 - endianFark1] = bytes[idx + 3];
        idx += 4;
    }
}