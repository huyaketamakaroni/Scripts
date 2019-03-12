using UnityEngine;
using System.Collections;

public class PereferalTest : MonoBehaviour
{

    //カメラの中心をpixel単位で求める
    //左下から(0,0)で計算
    //カメラの中心pixelが(Camera.main.pixelWidth/2,Camera.main.pixelHeight/2)であらわされる
    //これをもとに条件分け
    //画面の中心座標を格納するための変数
    public static float centerX;
    public static float centerY;
    public static bool targetIsFront;
    public static GameObject arrow;

    void Start()
    {
        CenterXYInit();
        arrow = GameObject.Find("arrow");
    }

    void CenterXYInit()
    {
        centerX = Camera.main.pixelWidth / 2;
        centerY = Camera.main.pixelHeight / 2;
    }

    public static string GetDirection()
    {
        try
        {
            GameObject test = UnityTargetRotation.GetTempTargetArray();

            string direction = null;

            //探索対象のオブジェクトの２次元位置を求める
            var target2dpos = Camera.main.WorldToScreenPoint(test.transform.position);

            //カメラの投影平面上の中心点と対象オブジェクトの２点間の角度を求め、方向を決定する
            float directionX = target2dpos.x - centerX;
            float directionY = target2dpos.y - centerY;
            float radian = Mathf.Atan2(directionY, directionX) * Mathf.Rad2Deg;

            //Unityのワールド座標のy軸に対してのCameraの回転を求める
            float angle = Camera.main.transform.transform.localEulerAngles.y;

            //カメラの位置とターゲットの位置の角度を求める
            float targetX = UnityTargetRotation.GetTempTargetArray().transform.position.x - Camera.main.transform.position.x;
            float targetZ = UnityTargetRotation.GetTempTargetArray().transform.position.z - Camera.main.transform.position.z;
            float targetR = Mathf.Atan2(targetZ, targetX) * Mathf.Rad2Deg;

            if (targetR < 0)
                targetR += 360;    //マイナスのものは360を加算

            if (radian < 0)
                radian += 360;    //マイナスのものは360を加算

            /*
             *angleの回転方向と角度を補正し、targetRと合わせる 
             * 補正前：　angle →　時計の１２の方向から、右回り　→　targetR 時計の３時の方向から、左回り
             * 補正後：　angle＆targetR →　時計の３時の方向から、左回り
             */

            angle = 360 - angle;

            if (0 < angle && angle < 270)
                angle += 90;
            else
                angle = 90 - (360 - angle);

            //Debug.Log("targetR: " + targetR + ", angle: " + angle);

            //angleよりtargetRのほうが角度が大きい時
            if (angle - targetR >= 0)
            {
                if (angle - targetR < 90 || angle - targetR > 270)
                    targetIsFront = true;
                else
                    targetIsFront = false;
            }
            else
            {
                if (targetR - angle < 90 || targetR - angle > 270)
                    targetIsFront = true;
                else
                    targetIsFront = false;
            }

            //Debug.Log("targetIsFront:"+targetIsFront);
            //Debug.Log("Radian:" + radian);

            if (targetIsFront)
            {
                //方向判定
                if (radian <= 22.5f || radian > 337.5f)
                    direction = "right";
                else if (radian <= 67.5f && radian > 22.5f)
                    direction = "upperRight";
                else if (radian <= 112.5f && radian > 67.5f)
                    direction = "up";
                else if (radian <= 157.5f && radian > 112.5f)
                    direction = "upperLeft";
                else if (radian <= 202.5f && radian > 157.5f)
                    direction = "left";
                else if (radian <= 247.5f && radian > 202.5f)
                    direction = "downerLeft";
                else if (radian <= 292.5f && radian > 247.5f)
                    direction = "down";
                else if (radian <= 337.5f && radian > 292.5f)
                    direction = "downerRight";
            }
            else
            {

                if (angle - targetR >= 0)
                {
                    if (90 <= angle - targetR && angle - targetR <= 180)
                        direction = "TurnRight";
                    else
                        direction = "TurnLeft";
                }
                else
                {
                    if (90 <= targetR - angle && targetR - angle <= 180)
                        direction = "TurnLeft";
                    else
                        direction = "TurnRight";
                }
            }
            //Debug.Log(direction);
            //Debug.Log(targetIsFront);

            return direction;
        }
        catch (System.NullReferenceException)
        {
            return "null";
        }

    }
}