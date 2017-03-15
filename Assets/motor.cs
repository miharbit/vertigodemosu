/*
    DemobyDemirkaya
    Vertigo Demo Project
    yazan: ibrahim taylan demirkaya
    
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class motor : MonoBehaviour {

    private cubes kup = new cubes();
    List<cubes> block = new List<cubes>();//new block ataması için
    List<cubes> girid = new List<cubes>();//grid küplerimiz
    List<string> colorList;//block renk listesi
    List<float> oranList;
    List<GameObject> toucheds = new List<GameObject>();//dokunulan küpler listesi
    List<Vector2> dokunmalistesi = new List<Vector2>() { new Vector2(125, 0), new Vector2(-125, 0), new Vector2(0, 125), new Vector2(0, -125) };
   
    int score;//anlık skor
    Text scoretext;
    GameObject goverobje;//game over yazısı
    Text highscoretext;
    bool touchobjecheck = false;//herhangi bir küpe dokunan olursa true alır
    bool gameover;

    int hammercheck = 0;//hammerlı dokunma için 1, hammerli dokunma işini yaptığında 2, hammer iptal 0

    Vector3 touchPosWorld;//dokunma vektörü
    void Start () {
        Camera.main.aspect = 800f / 1280f;
        scoretext = GameObject.Find("Score").GetComponent<Text>();
        highscoretext = GameObject.Find("HighScore").GetComponent<Text>();
        goverobje = GameObject.Find("Game OverPanel");
        colorList = new List<string>() { "blue", "green", "red", "orange", "purple", "turquoise", "yellow" };//yeni renk için images'e sprite at
        oranList = new List<float>() { 10f, 40f, 40f, 10f };// oranlar 1 ve 4 %10, 2 ve 3 %40
        
        playerclass.yuksekSkortexti(highscoretext);
        newGame();
        
    }
	
	void Update () {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    break;
                case TouchPhase.Stationary:
                    touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                    RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
                    if (hitInformation.collider != null && !gameover)
                    {
                        GameObject touchedObject = hitInformation.transform.gameObject;
                        if (hammercheck == 0)
                        {
                            placeCube(touchedObject);
                        } else if(hammercheck == 1)//Hammerlı dokunuş
                        {
                            hammerCube(touchedObject);
                        }
                        touchobjecheck = true;
                    }
                    break;
                case TouchPhase.Ended:
                    if (touchobjecheck)
                    {
                        touchobjecheck = false;
                        hammercheck = hammercheck == 2 ? 0 : hammercheck;
                        if (block.Count > toucheds.Count)
                        {
                            placeCubeFail();
                        }
                        else
                        {
                            toucheds.Clear();
                            randomizeNewBlock();
                            trippleorMore();
                            gameoverControl();
                        }
                    }
                    break;
            }
        }
    }
    
    private void randomizeOrSavedBlock(string[] colorArray)
    {
        if(block != null)
        {
            block.ForEach(xx => xx.sil());
            block.Clear();
        }
        /*
         * colorArray: saved blockun renkleri
         * color array yoksa random block üretilir.
         * Eğer level istenirse, gittikçe zorlaşan bir sistem için burada colorList ve oranList manipule edilebilir.
         * */
        block = colorArray.Length <1 ? cubes.newblock(oranList, colorList) : cubes.newblock(colorArray);//orana ve renk sayısına göre yeni block üretimi
    }
    
    private void randomizeNewBlock()
    {
        randomizeOrSavedBlock(new string[0]);
    }

    public void placeCube(GameObject tiklanan)
    {
        int caprazengel = 0;
        if (toucheds.Count > 0)//çapraz koymayı engel oluyor
        {
            Vector2 caprazyok = tiklanan.transform.position - toucheds.FindLast(xx => xx).transform.position;
            caprazengel = dokunmalistesi.Contains(caprazyok) ? 0 : 1;
        }
        if (!toucheds.Contains(tiklanan) && block.Count > toucheds.Count && caprazengel == 0)
        {
            toucheds.Add(tiklanan);
            kup = girid.Find(xx => xx.nesnesi == tiklanan);
            if (kup.tiprenk == "empty")
            {
                kup.colorla(block[toucheds.Count - 1].tiprenk);
            }
            else  // failed
            {
                toucheds.Remove(tiklanan);
                placeCubeFail();
            }
        }
    }
    public void placeCubeFail()
    {
        foreach(GameObject gg in toucheds)
        {
            kup = girid.Find(xx => xx.nesnesi == gg);
            kup.colorla("empty");
        }
        toucheds.Clear();
    }

    public void trippleorMore()//üç veya daha fazlasında gridi emptyle. puan kazandır
     /*
         1. döngü, var olan renklerden birini alıp listelemek için
         2. döngü, listeden bir ebe seçmek için
         3. döngü, listeyi ebeyle kıyaslamak için
         4. döngü, listeyi ebeye dokunanlar listesi ile kıyaslamak için
         Beynim yandııııııııııııııııııııııııııııııııııııııııııııııııııııııııııııııı
         ebe blocktan bir kılavuz çıkartıp 3 yada daha fazla dokunan blok varsa puanlayıp siliyoruz
     */
    {
        foreach (cubes c in girid)
        {
            if (c.tiprenk != "empty")
            {
                List<cubes> kiyastouched = new List<cubes>();
                foreach (cubes eb in girid)
                {
                    if (eb.tiprenk == c.tiprenk)
                    {
                        List<cubes> empties = girid.FindAll(xx => xx.tiprenk == eb.tiprenk);

                        List<cubes> touchedcubes = new List<cubes>();
                        touchedcubes.Add(eb);
                        int tcsay = 0;
                        while (touchedcubes.Count > tcsay)//kılavuz başlatıyoruz
                        {
                            foreach (cubes sc in empties)
                            {
                                Vector2 dokun = touchedcubes[tcsay].yer - sc.yer;
                                if (dokunmalistesi.Contains(dokun) && !touchedcubes.Contains(sc))
                                {
                                    touchedcubes.Add(sc);
                                }
                            }
                            tcsay++;
                        }
                        kiyastouched = touchedcubes.Count > kiyastouched.Count ? touchedcubes : kiyastouched;//touched listelerinin en büyüğünü buluyoruz
                    }
                    
                }
                if (kiyastouched.Count > 2)
                {
                    score += kiyastouched.Count * 10;
                    scoretext.text = score.ToString();
                    kiyastouched.ForEach(xx => xx.colorla("empty"));
                }
            }
        }
            
    }

    public void gameoverControl()
    {
        int geymsayaci = 0;
        foreach(cubes empty in girid)
        {
            if(empty.tiprenk == "empty")
            {
                List<cubes> emptylist = new List<cubes>() { empty };
                cubes ebe = empty;
                bool dongucheck = true;
                while (dongucheck)
                {
                    bool forcheck = false;
                    foreach(Vector2 kol in dokunmalistesi)
                    {
                        Vector2 varmi = ebe.yer + kol;
                        cubes yandaki = girid.Find(xx => xx.yer == varmi);
                        if(yandaki != null && yandaki.tiprenk == "empty" && !emptylist.Contains(yandaki))
                        {
                            ebe = yandaki;
                            emptylist.Add(ebe);
                            forcheck = true;
                            break;
                        }
                    }
                    dongucheck = forcheck;
                }
                geymsayaci = geymsayaci < emptylist.Count ? emptylist.Count : geymsayaci;
            }
        }
        if (geymsayaci < block.Count)//GAME OVERR
        {
            playerclass.gameOverPrefs(score);
            gameover = true;
            goverobje.SetActive(true);

        }
    }

    public void newGame()
    {
        if (girid != null)
        {
            girid.ForEach(xx => xx.sil());
            girid.Clear();
        }
        girid = kup.gridding(playerclass.loadStringDizisi("renkler"));
        randomizeOrSavedBlock(playerclass.loadStringDizisi("newblock"));
        score = playerclass.loadInt("sskor");
        scoretext.text = score.ToString();
        goverobje.SetActive(false);
        gameover = false;
        gameoverControl();

        playerclass.kaydetInt("hammer", 10);//10 hammer hakkı
        playerclass.kaydetInt("skip", 10);
    }

    void OnApplicationQuit()
    {
        var rnks = new List<string>();
        girid.ForEach(xx => rnks.Add(xx.tiprenk));
        playerclass.kaydetStringDizisi("renkler", rnks.ToArray());
        var newblockrenks = new List<string>();
        block.ForEach(xx => newblockrenks.Add(xx.tiprenk));
        playerclass.kaydetStringDizisi("newblock", newblockrenks.ToArray());
        playerclass.kaydetInt("sskor", score);

    }
    
    public void powerHammer()
    {
        if(hammercheck != 1)
        {
            if (playerclass.hammerPower())
            {
                hammercheck = 1;
            }
            else
            {
                hammercheck = 0;
            }
        }
    }
    public void hammerCube(GameObject targetkub)
    {
        kup = girid.Find(xx => xx.nesnesi == targetkub);
        kup.colorla("empty");
        touchobjecheck = false;
        hammercheck = 2;
    }
    public void powerSkip()
    {
        if (playerclass.skipPower())
        {
            randomizeNewBlock();
        }
    }
}
