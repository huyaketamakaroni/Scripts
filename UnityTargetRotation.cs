//B3デモ用に簡略化
//display関係すべてコメントアウト20190127

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using System.Collections.Generic;

public class UnityTargetRotation : MonoBehaviour
{
    //World Anchor
    //WorldAnchorStore store;
    //WorldAnchor anchor;
    //public string AnchorStoreName;
    //bool savedRoot = false;


    /*テキスト表示用オブジェクト*/
    GameObject ResultText;


    //int cnt = 0;//カウント
    //int flag_text = 1;//フラグ_Space呼び出し
    /**Text用変数**/
    TextMesh scoreText;



    public AudioSource audio1;
    public AudioSource audio2;
    public AudioSource audio3;
    public AudioSource audioOK;
    public AudioSource audioNG;

    private static bool gameOverFlag = false;

    //配列の最大数を決定するための変数
    private static int TARGET_MAX = 3;
    //private static int TARGET_MAX = 10;
    private static int OBJECT_MAX = 3;

    /*    //配列の最大数を決定するための変数
	private static int TARGET_MAX = 12;
	//private static int TARGET_MAX = 10;
	private static int OBJECT_MAX = 40;
	*/
    private static GameObject cursor;
    //Displayを制御するための変数
    public static GameObject display;
    public static GameObject DebugLog;
    bool is_debug_mode = true;
    Color BoxInvisibleColor = Color.white;

    //右下のテキスト
    public Text textTarget;

    //クリックイベントの数を数える変数
    public static int phase;
    //画角内に数字があるかを判定
    public static bool viewPortFlag;

    // GameObject管理用
    //裸眼探索モードの探索オブジェクトを格納するための配列
    public static GameObject[] BoxAnnotations;
    //周辺視ガイド付き探索モードの探索オブジェクトを格納するための配列
    public static GameObject[] ledGuideTargetArray;
    //中心視ガイド付き探索モードの探索オブジェクトを格納するための配列
    public static GameObject[] cgGuideTargetArray;
    //周辺視＋中心視ガイド付き探索モードの探索オブジェクトを格納するための配列
    public static GameObject[] bothGuideTargetArray;

    //周辺視ガイド付き探索モードの探索順に番号を格納するための配列
    public static int[] ledGuideTargetSelectNum;
    //中心視ガイド付き探索モードの探索順に番号を格納するための配列
    public static int[] cgGuideTargetSelectNum;
    //周辺視＋中心視ガイド付き探索モードの探索順に番号を格納するための配列
    public static int[] bothGuideTargetSelectNum;

    /*    //棚の番号配置
	public static int[] BoxIds = new int[] {
		10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
		20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
		30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
		40, 41, 42, 43, 44, 45, 46, 47, 48, 49};
	*/

    //棚の番号配置
    public static int[] BoxIds = new int[] { 1, 2, 3 };
    //裸眼探索モードの探索順に番号を格納するための配列
    //public static int[] patternA = new int[] { 25, 42, 35, 11, 26, 13, 43, 31, 45, 16, 33, 22 };
    //public static int[] patternB = new int[] { 22, 33, 16, 45, 31, 43, 13, 26, 11, 35, 42, 25 };
    //public static int[] patternA = new int[] { 22, 16, 35, 29, 11, 43, 24, 12, 48, 30, 47, 37 };
    //public static int[] patternB = new int[] { 37, 47, 30, 48, 12, 24, 43, 11, 29, 35, 16, 22 };
    public static int[] patternA = new int[] { 1, 2, 3 };
    public static int[] patternB = new int[] { 3, 2, 1 };
    public static int[] AnnotationIds = patternA;
    bool is_current_patternA = true;

    //ターゲットをTextに表示させるためのカウンタ変数
    public static int CurrentAnnotationId = 0;
    public double duration = 0.0;
    public float accumuDeltaTime = 0.0f;
    public Texture targetTexture;

    //void AnchorSotreReady(WorldAnchorStore store_)
    //{
    //    this.store=store_;
    //    //LoadGame();
    //    //string[] ids = store.GetAllIds();
    //    //for (int index = 0; index<ids.Length; index++)
    //    //{
    //    //    Debug.Log(ids[index]);
    //    //    if (ids[index]==AnchorStoreName)
    //    //    {
    //    //        anchor = store.Load(ids[index], gameObject);
    //    //        break;
    //    //    }
    //    //}
    //}
    //void Awake() { 
    //    Application.targetFrameRate = 60; //FPS設定
    //}

    // Use this for initialization
    void Start()
    {

        //Textっていう名前のGameObjectをUnityから探す
        ResultText = GameObject.Find("Text");

        //テキストメッシュ変数にオブジェクトを代入
        scoreText = ResultText.gameObject.GetComponent<TextMesh>();

        //表示するテキストの中身を代入
        scoreText.text = null; ;//text描画



        //WorldAnchorStore.GetAsync(AnchorStoreReady);

        //各変数を初期化
        VarInit();
        SetDebugMode(true);
        //SetDebugMode(false);

    }

    void VarInit()
    {
        //変数の初期化
        phase = 0;
        CurrentAnnotationId = 0;
        viewPortFlag = false;

        cursor = GameObject.Find("Panel");
        //display=GameObject.Find("Display");

        BoxAnnotations = new GameObject[OBJECT_MAX];

        for (int i = 0; i < OBJECT_MAX; i++)
        {
            //GameObjectを格納
            BoxAnnotations[i] = GameObject.Find("Cube" + BoxIds[i]);
            BoxAnnotations[i].GetComponent<Renderer>().material.color = BoxInvisibleColor;
        }

        DebugLog = GameObject.Find("DebugLog");

        for (int i = 0; i < OBJECT_MAX; i++)
        {
            //テクスチャ変更
            BoxAnnotations[i].GetComponent<Renderer>().material.mainTexture = targetTexture;
        }

        //Displayと名の付くオブジェクトを代入
        //PereferalTest.arrow.SetActive(false);

        SockertSend.SetNum(0);
        SockertSend.SetMode(0);
        SockertSend.SetTrial(0);
        SockertSend.SetNumFlag(false);
        SockertSend.SetDuration(0.0);
        SockertSend.SetAnswer("00");
        SockertSend.SetDurationFlag(false);
        SockertSend.SetDirectionFlag(false);
        SockertSend.SetDirection("no");
        SockertSend.SyncDisplay(this);
    }

    //Debugモード
    void SetDebugMode(bool is_debug_mode)
    {
        //画面上から流れるデバッグメッセージ
        if (is_debug_mode)
        {
            for (int i = 0; i < OBJECT_MAX; i++)
            {
                if (BoxAnnotations[i].GetComponent<Renderer>().material.color == BoxInvisibleColor)
                    BoxAnnotations[i].GetComponent<Renderer>().material.color = Color.white;
                else
                    BoxAnnotations[i].GetComponent<Renderer>().material.color = Color.white;
            }
            //display.GetComponent<Renderer>().material.color=Color.yellow;
            BoxInvisibleColor = Color.white;
        }
        else
        {
            for (int i = 0; i < OBJECT_MAX; i++)
            {
                if (BoxAnnotations[i].GetComponent<Renderer>().material.color == BoxInvisibleColor)
                    BoxAnnotations[i].GetComponent<Renderer>().material.color = Color.white;
                else
                    BoxAnnotations[i].GetComponent<Renderer>().material.color = Color.white;
            }
            //display.GetComponent<Renderer>().material.color=new Color(0.1f, 0.1f, 0.0f);
            BoxInvisibleColor = Color.white;
        }

        //すべてのBoxAnnotation
        if (is_debug_mode)
        {
            DebugLog.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            DebugLog.GetComponent<Renderer>().material.color = Color.clear;
        }

        //画面下の状態表示
        if (is_debug_mode)
        {
            textTarget.enabled = true;
        }
        else
        {
            textTarget.enabled = false;
        }

    }

    void Update()
    {
        float fps = 1f / Time.deltaTime;
        //Debug.LogFormat("{0}fps", fps);

        if (phase == 0)
            scoreText.text = "PRESS SPACE KEY";

        if (gameOverFlag)
        {
            scoreText.text = "Thank You";
            return;
        }

        RayCastTest.CheckRaycast();
        string dir = PereferalTest.GetDirection();
        SockertSend.SetDirection(dir);
        SockertSend.SetNum(AnnotationIds[CurrentAnnotationId]);
        SockertSend.SetDurationFlag(false);
        SockertSend.SetMode(PlayModeSelecter.GetMode());
        SockertSend.SetPhase(phase);
        SockertSend.SetDebug(is_debug_mode);
        SockertSend.SetEnter(false);
        SockertSend.SetTrial(CurrentAnnotationId);

        if (Input.GetKeyDown(KeyCode.Alpha0) ||
            Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetKeyDown(KeyCode.Alpha4)
        )
        {
            SetDebugMode(is_debug_mode);
            CurrentAnnotationId = 0;
            phase = 0;
            duration = 0.0;
            CurrentAnnotationId = 0;
            gameOverFlag = false;
        }
        if (Input.GetKeyDown(KeyCode.Home))
        {
            LoadGame();
            Debug.Log("Key: Home");
        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            SaveGame();
            Debug.Log("Key: End");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            is_debug_mode = !is_debug_mode;
            SetDebugMode(is_debug_mode);
            Debug.Log("Key: P");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ForwardTempArray();
            Debug.Log("Key: F");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BackTempArray();
            Debug.Log("Key: B");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            ProceesPhase();
            Debug.Log("Key: Space");
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (is_current_patternA)
            {
                AnnotationIds = patternB;
                is_current_patternA = false;
            }
            else
            {
                AnnotationIds = patternA;
                is_current_patternA = true;
            }

            Debug.Log("Key: Tab");
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SockertSend.SetEnter(true);
            SetDebugMode(is_debug_mode);
            CurrentAnnotationId = 0;
            phase = 0;
            duration = 0.0;
            CurrentAnnotationId = 0;
            Debug.Log("Key: Enter");
        }

        if (is_current_patternA)
        {
            SockertSend.SetPattern("A");
        }
        else
        {
            SockertSend.SetPattern("B");
        }

        //フラグを全部見てから、最後にSocketを送る
        SockertSend.SyncDisplay(this);

        if (accumuDeltaTime > 1.0f / 30)
        {
            SockertSend.SyncDisplay(this);
            accumuDeltaTime = 0.0f;
        }
        accumuDeltaTime += Time.deltaTime;


        //Status Bar


        try
        {
            string objectName = RayCastTest.GetSelectedGameObject().name;
            textTarget.text = "mode: " + PlayModeSelecter.GetMode() + ", "
                + "phase: " + phase + ", "
                + AnnotationIds[CurrentAnnotationId] + "-" + objectName + ", dir: " + dir;
        }
        catch (System.NullReferenceException)
        {
            textTarget.text = "mode: " + PlayModeSelecter.GetMode() + ", "
                + "phase: " + phase + ", "
                + "-" + "null" + ", dir: " + dir; ;
        }


    }

    //スペースを押したときの動作
    void ProceesPhase()
    {
        GameObject box = GameObject.Find("Cube" + AnnotationIds[CurrentAnnotationId]);

        /*
        //モニタを見てるときのみトライアル開始
		if (phase==0)
		{
			if (RayCastTest.GetSelectedGameObject()==display)
			{
				Debug.Log("-----------------");
				Debug.Log("phase 1");
				Debug.Log("\tBox name: Cube"+AnnotationIds[CurrentAnnotationId]);
				phase++;
			}
			else
			{
				audioNG.Play();
			}
		}*/

        //待機状態→探索フェーズ
        if (phase == 0)
        {
            /*
			if(CurrentAnnotationId == 0)
				scoreText.text = "「赤い箱」を探せ";
			if(CurrentAnnotationId == 1)
				scoreText.text = "「黄色い箱」を探せ";
			if(CurrentAnnotationId == 2)
				scoreText.text = "「青い箱」を探せ";
			*/

            //カーソルを無効化
            //cursor.SetActive(false);

            //時間計測スタート

            duration = Time.time;
            SockertSend.SetNumFlag(true);
            int mode = PlayModeSelecter.GetMode();
            if (mode == 2 || mode == 4 || mode == 0)
            {
                if (CurrentAnnotationId == 0)
                {
                    scoreText.text = "「赤い箱」を探せ";
                    BoxAnnotations[0].GetComponent<Renderer>().material.color = Color.red;
                }
                if (CurrentAnnotationId == 1)
                {
                    scoreText.text = "「緑の箱」を探せ";
                    BoxAnnotations[1].GetComponent<Renderer>().material.color = Color.green;

                }
                if (CurrentAnnotationId == 2)
                {
                    scoreText.text = "「青い箱」を探せ";
                    BoxAnnotations[2].GetComponent<Renderer>().material.color = Color.blue;
                }

            }

            if (mode == 3 || mode == 4 || mode == 0)
            {
                SockertSend.SetDirectionFlag(true);
            }
            else
            {
                SockertSend.SetDirectionFlag(false);
            }

            audio1.Play();
            Debug.Log("-----------------");
            Debug.Log("phase 2");
            phase = 2;
        }

        //探索フェーズ→回答フェーズ
        else if (phase == 2)
        {
            if (CurrentAnnotationId == 0)
            {
                if (RayCastTest.GetSelectedGameObject() != box)
                {
                    return;
                }
            }

            if (CurrentAnnotationId == 1)
            {
                if (RayCastTest.GetSelectedGameObject() != box)
                {
                    return;
                }

            }


            if (CurrentAnnotationId == 2)
            {
                if (RayCastTest.GetSelectedGameObject() != box)
                {
                    return;
                }
            }

            //カーソルを有効に
            cursor.SetActive(true);
            PereferalTest.arrow.SetActive(false);
            audio2.Play();

            box.GetComponent<Renderer>().material.color = BoxInvisibleColor;
            SockertSend.SetDirectionFlag(false);

            SockertSend.SetNumFlag(false);
            duration = Time.time - duration;
            SockertSend.SetDuration(duration);
            Debug.Log("-----------------");
            Debug.Log("phase 3: " + duration);


            //phase3
            //正解／不正解にかかわらずいずれかのBoxを見ている場合は次にすすむ
            bool is_looking_at_box = false;


            for (int i = 0; i < OBJECT_MAX; i++)
            {
                if (RayCastTest.GetSelectedGameObject() == BoxAnnotations[i])
                {
                    is_looking_at_box = true;
                    break;
                }
            }


            //正誤送信
            if (is_looking_at_box)
            {
                string name = RayCastTest.GetSelectedGameObject().name.ToString().Substring(4);
                //SockertSend.SetMode(PlayModeSelecter.GetMode());
                SockertSend.SetAnswer(name);
                SockertSend.SetDurationFlag(true);
            }
            //Boxを見ていない場合
            else
            {
                audioNG.Play();
                return;
            }


            //いずれかのBoxを見ていた場合
            audioOK.Play();
            box.GetComponent<Renderer>().material.color = BoxInvisibleColor;

            //ディスプレイを見たとき
            //数字を表示，時間を記録しない
            CurrentAnnotationId++;



            //修正前　最初に戻る
            //修正後　Endと表示
            if (CurrentAnnotationId == TARGET_MAX)
            {
                SockertSend.SetNumFlag(true);
                SockertSend.SetNum("End");
                gameOverFlag = true;
                Debug.Log("-----------------");
                Debug.Log("-----------------");
                Debug.Log("----END----");
                Debug.Log("-----------------");
                Debug.Log("-----------------");
                //Debug.Log(gameOverFlag);//201901227
                //CurrentAnnotationId--;

            }

            phase = 0;
        }

        //回答フェーズ（見つけたBoxにカーソルを合わせる）
        else if (phase == 3)
        {

        }
    }

    public static GameObject GetTempTargetArray()
    {
        try
        {
            return GameObject.Find("Cube" + AnnotationIds[CurrentAnnotationId]);
        }
        catch
        {
            return GameObject.Find("Cube" + 1);
        }
    }

    public static void ResetTempTargetArray(int mode)
    {
        //display.SetActive(true);
        SockertSend.SetNumFlag(false);
        CurrentAnnotationId = 0;
        phase = 0;
        gameOverFlag = false;
    }

    public static void ForwardTempArray()
    {
        //display.SetActive(true);

        //要修正
        BoxAnnotations[CurrentAnnotationId].SetActive(false);
        phase = 0;
        CurrentAnnotationId++;
        if (CurrentAnnotationId == TARGET_MAX)
            CurrentAnnotationId = TARGET_MAX - 1;

        //要修正
        BoxAnnotations[CurrentAnnotationId].SetActive(true);

    }

    public static void BackTempArray()
    {
        //display.SetActive(true);

        //要修正
        BoxAnnotations[CurrentAnnotationId].SetActive(false);
        phase = 0;
        CurrentAnnotationId--;
        if (CurrentAnnotationId < 0)
            CurrentAnnotationId = 0;

        //要修正
        BoxAnnotations[CurrentAnnotationId].SetActive(true);

    }

    private void SaveGame()
    {
        // Save data about holograms positioned by this world anchor
        //if (!this.savedRoot) // Only Save the root once
        //{
        //    anchor = gameObject.AddComponent<WorldAnchor>();
        //    string name = gameObject.name.ToString();
        //    Debug.Log("game object name:" + name);
        //    this.store.Delete(name);
        //    bool wasSaved = this.savedRoot=this.store.Save(name, anchor);
        //    if (wasSaved)
        //    {
        //        Debug.Log("Saved world anchor");
        //    }
        //    else
        //    {
        //        Debug.Log("Could not save world anchor");
        //    }

        //}
    }

    private void LoadGame()
    {
        // Save data about holograms positioned by this world anchor
        //this.savedRoot = this.store.Load(gameObject.name.ToString(), gameObject);
        //if (!this.savedRoot)
        //{
        //     Debug.Log("Could not load world anchor");
        //}
    }
}