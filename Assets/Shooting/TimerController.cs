using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//制限時間のカウントダウンのスクリプト
//時間切れ時の処理も行う
public class TimerController : MonoBehaviour
{
    public Text TimerText;//テキスト表示

    public float TotalTime;//制限時間
    int Seconds;//残り時間(int型で表示する用)

    public static float RemainingTime;//残り時間

    // Use this for initialization
    void Start()
    {
        RemainingTime = TotalTime;//残り時間最大
    }

    // Update is called once per frame
    void Update()
    {
        RemainingTime -= Time.deltaTime;//残り時間をカウントダウン

        if (RemainingTime <= 0.0f)//時間切れ時
        {
            RemainingTime = 0.0f;//残り時間がマイナスにならないように
            Invoke("GoToCongratulations", 0.5f);//0.5秒後にスコアシーンへ遷移
        }

        Seconds = (int)RemainingTime;//残り時間をint型に
        TimerText.text = Seconds.ToString();//残り時間のディスプレイ表示

    }


    void GoToCongratulations()//スコアシーンへ遷移させる
    {
        SceneManager.LoadScene("Congratulations");
    }

    public static float getRemainingTime()//残り時間の受け渡し
    {
        return RemainingTime;
    }


}