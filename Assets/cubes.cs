/*
    DemobyDemirkaya
    Vertigo Demo Project
    yazan: ibrahim taylan demirkaya
    
*/
using System.Collections.Generic;
using UnityEngine;

public class cubes {

    public int id;
    public Vector2 yer;
    public string tiprenk;//grid için empty, block için rengi
    public GameObject nesnesi;

    public cubes()
    {
        id = 1;
    }

    public cubes(Vector2 xy, string tipp)
    {
        id = 2;
        yer = xy;
        nesnesi = Resources.Load<GameObject>("kare");
        nesnesi = Object.Instantiate(nesnesi);
        colorla(tipp);
        nesnesi.transform.position = xy;
    }

    public void sil()
    {
        Object.Destroy(nesnesi);
    }

    public void colorla(string rengi)
    {
        tiprenk = rengi;
        nesnesi.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("images/" + tiprenk);
    }

    public List<cubes> gridding(string[] renks)
    {
        List<cubes> gridcubes = new List<cubes>();
        if(renks.Length < 2)
        {
            renks = new string[25];
        }
        int diziid = -1;
        for (int x = -250; x < 375; x+=125)
        {
            for (int y = -300; y < 325; y+=125)
            {
                //empty, grid
                gridcubes.Add(new cubes(new Vector2(x, y), renks[++diziid] == null ? "empty" : renks[diziid]));//"empty"
            }
        }
        return gridcubes;
    }

    public static List<cubes> newblock(List<float> oranlar, List<string> renkler)//renkler ve block sayısı kendi oranına göre belirleniyor
    {
        float toplamoran = 0;
        oranlar.ForEach(xx => toplamoran += xx);
        float piyango = Random.Range(0, toplamoran);
        int countblock = 0;
        toplamoran = 0;
        for (int i = 0; i<oranlar.Count; i++)
        {
            if(piyango <= oranlar[i] + toplamoran)
            {
                countblock = i + 1;
                break;
            }
            toplamoran += oranlar[i];
        }
        string[] newrenkdizi = new string[countblock];
        for (int i = 0; i < countblock; i++)
        {
            newrenkdizi[i] = renkler[Random.Range(0, renkler.Count)];
        }
        return newblock(newrenkdizi);
    }

    public static List<cubes> newblock(string[] renkler)
    {
        List<cubes> yeni = new List<cubes>();
        for (int i = 0; i < renkler.Length; i++)
        {
            yeni.Add(new cubes(new Vector2((i * 50) - 50, 300), renkler[i])); //renk block
            yeni[i].nesnesi.transform.localScale = new Vector2(0.2f, 0.2f);

        }
        return yeni;
    }
}
