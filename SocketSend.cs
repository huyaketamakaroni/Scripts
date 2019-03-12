using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.Net.Sockets;

public class SockertSend : MonoBehaviour
{
    private static bool numFlag, durationFlag, directionFlag;
    private static string num;
    private static double duration;
    private static string direction;
    private static string answer;
    private static string mode;
    private static string phase;
    private static string is_debug;
    private static string enter;
    private static int trial;
    private static string pattern;

    //debug用
    //private static string adPC = "http://192.168.6.3:80";
    private static string adPC = "http://192.168.6.129:80";
    //private static string adPC = "http://169.254.72.139:80";
    //private static string adPC = "http://127.0.0.1:80";

    //http通信 サーバーにPOSTする
    private static IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        //Debug.Log("Status Code: " + request.responseCode);
    }

    public static void SyncDisplay(MonoBehaviour i_behaviour)
    {
        //Debug.Log("{\"num\":\"" + num + "\",\"duration\":\"" + duration +
        //    "\",\"direction\":\"" + direction +
        //    "\",\"directionFlag\":\"" + directionFlag +
        //    "\",\"durationFlag\":\"" + durationFlag + "\",\"numFlag\":\"" + numFlag + "\"}");

        //デスクトップPC用

        i_behaviour.StartCoroutine(Post(adPC,
            "{\"num\":\"" + num +
            "\",\"trial\":\"" + trial +
            "\",\"mode\":\"" + mode +
            "\",\"pattern\":\"" + pattern +
            "\",\"phase\":\"" + phase +
            "\",\"debug\":\"" + is_debug +
            "\",\"duration\":\"" + duration +
            "\",\"answer\":\"" + answer +
            "\",\"direction\":\"" + direction +
            "\",\"directionFlag\":\"" + directionFlag +
            "\",\"durationFlag\":\"" + durationFlag +
            "\",\"enter\":\"" + enter +
            "\",\"numFlag\":\"" + numFlag + "\"}"));
    }

    public static void SetNum(int n)
    {
        num = n.ToString();
    }
    public static void SetNum(string n)
    {
        num = n;
    }

    public static void SetMode(int n)
    {
        mode = n.ToString();
    }

    public static void SetPhase(int n)
    {
        phase = n.ToString();
    }
    public static void SetDebug(bool n)
    {
        is_debug = n.ToString();
    }

    public static void SetNumFlag(bool b)
    {
        numFlag = b;
    }
    public static void SetDuration(double d)
    {
        duration = d;
    }
    public static void SetAnswer(string b)
    {
        answer = b;
    }

    public static void SetDurationFlag(bool b)
    {
        durationFlag = b;
    }
    public static void SetDirection(string d)
    {
        direction = d;
    }

    public static void SetDirectionFlag(bool b)
    {
        directionFlag = b;
    }

    public static void SetEnter(bool b)
    {
        enter = b.ToString();
    }
    public static void SetPattern(string b)
    {
        pattern = b;
    }
    public static void SetTrial(int b)
    {
        trial = b;
    }

}