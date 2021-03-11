using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

//脳波関連のデータをここでまとめている
//ゲームに使う各種値の算出・データを記録するか・ゲームシーンへ遷移するためのスタートボタンの活性化を行う
//後付けでPC上で脳波を受け取らなくても動くように変更(簡単な動作確認用)


public class ReadBuffer : MonoBehaviour
{
    float baratio;//β/α比

    private BufferManager BufferManager;//送られてきたパケットの中身を取りに行く
    //private DebugManager DebugManager;//デバッグ用
    private bool isExcuted = true;
    public static float AvBaRatio = 0;//β/α比の平均値
    private float SumBaRatio = 0;//BaseBaRatio算出時の分子
    private float Count = 0;//BaseBaRatio算出時の分母
    public static float time = 0;//時間
    public float CalculateTime;//BaseBaRatio算出するための時間
    public bool ButtonActivate = false;//スタートボタンの活性・非活性
    private bool StartCulculation = false;//計測開始
    public int Windowsize;//移動平均窓長
    float[] ArrayBaratio;//移動平均に使うためのバッファ
    public static float MABaRatio;//移動平均
    private int NoBaRatio = 0;//送られてきたパケット数 Number of Baratio
    public static double BaRatio;//β/α比
    public static float MinBaRatio = 2.0f;//β/α比の最小値　初期値は十分に大きい値とした
    public static bool Recode = false;//記録のするか
    public static bool log = false;//ログ出力で同じデータ用いないように
    private float RandomNumberGenerationInterval = 0.0f;//β/α比が送られてくるタイミングと同じ1/16秒毎に乱数生成
    public bool RunOnPC;//PC上で動作確認するか
    void Start()
    {
        BufferManager = GameObject.Find("ValueFromDataProcessor").GetComponent<BufferManager>();
        //DebugManager = GameObject.Find("ValueFromDataProcessor").GetComponent<DebugManager>();
        ArrayBaratio = new float[Windowsize];
        Task.Run(GetPayload);
        //DebugManager.BufferDebug.Enqueue($"[ReadBuffer] windowsize:{windowsize.ToString()}");
    }

    private void Update()
    {
        if (RunOnPC == false)//実機で動かすか(実機で動かす場合false)
        {

            if (StartCulculation == true)
                time += Time.deltaTime;//経過時間

            if (BufferManager.BaRatioBuffer.IsEmpty == false)//パケットに値があるか
            {
                //DebugManager.BufferDebug.Enqueue($"[ReadBuffer] β/α比受け取り開始");


                var result = BufferManager.BaRatioBuffer.TryDequeue(out BaRatio);//送られてきたパケットにβ/α比が含まれているか（含まれていたらbaRatioにコピー）
                Recode = true;//ログの記録開始

                if (result == true)//送られてきたパケットにβ/α比が含まれていたら
                {
                    StartCulculation = true;//初めて脳波が送られてきたときに計測開始
                    baratio = (float)BaRatio;
                    log = true;//ログに記録


                    moveingaverage();//リングバッファを用いた移動平均算出

                    minbaratio();//β/α比の最低値取得

                    averagebaratio();//β/α比の平均算出
                }


                //ベースラインの算出が終わったら
                if (time > CalculateTime)
                {
                    ButtonActivate = true;//スタートボタンの活性化
                    //debugManager.bufferDebug.Enqueue($"[ReadBuffer] buttonactivate:{buttonactivate}");
                }
            }

            if (SceneManager.GetActiveScene().name == "Congratulations")
            { // Congratulationsシーンでのみやりたい処理
                Recode = false;
            }
            else
            { // それ以外のシーンでやりたい処理

            }
        }


        /*-----------------------------------------------------
        PC上で動かす場合
        ほぼ↑と同じ
        脳波ではなく発生させた乱数を使う
        簡単に動作確認するため
       -----------------------------------------------------*/
        else if (RunOnPC == true) //実機で動かすか
        {

            if (StartCulculation == true)
                time += Time.deltaTime;


            RandomNumberGenerationInterval += Time.deltaTime;
            StartCulculation = true;

            if (RandomNumberGenerationInterval >= 0.0625f)//本来脳波が送られてくるタイミングと合わせる
            {
                baratio = UnityEngine.Random.Range(0.5f, 1.5f); //作品提出用に脳波の代わりに乱数使用
                RandomNumberGenerationInterval = 0.0f;
            }
            log = true;
            //DebugManager.BufferDebug.Enqueue($"[ReadBuffer] BaRatio:{baratio.ToString()}");


            moveingaverage();//リングバッファを用いた移動平均算出

            minbaratio();//β/α比の最低値取得

            averagebaratio();//β/α比の平均算出

            if (time > CalculateTime)
            {
                ButtonActivate = true;
                //DebugManager.BufferDebug.Enqueue($"[ReadBuffer] buttonactivate:{buttonactivate}");
            }

        }

    }

    public void averagebaratio()//β/α比の平均値算出
    {
        if (time >= 10.0f)//最初の10秒間はアプリ起動直後のため計測しない
        {
            if (time <= CalculateTime)//加算平均処理
            {
                Count++;
                SumBaRatio += baratio;
                AvBaRatio = SumBaRatio / Count;
            }
        }
    }

    public void minbaratio()//β/α比の平均値算出
    {
        if (time >= 10.0f)//最初の10秒間はアプリ起動直後のため計測しない
        {
            if (time <= CalculateTime)
            {
                if (MinBaRatio >= baratio)//新しく送られてきたβ/α比が最も低い値だったら
                    MinBaRatio = baratio;//β/α比の最低値更新
            }
        }
    }

    public void moveingaverage()//リングバッファを用いた移動平均算出
    {
        if (ButtonActivate == true)//ベースラインの算出が終わったら
        {
            //ある一定以上のβ/α比を用いた方が集中してるかどうかの有意差が出やすいため、便宜的に平均+0.1以上の値を用いて移動平均を行う
            if (baratio >= AvBaRatio + 0.1f)
            {
                ArrayBaratio[NoBaRatio % Windowsize] = baratio;//一番古いデータに新しいデータを上書き処理
            }
            else
            {
                ArrayBaratio[NoBaRatio % Windowsize] = MinBaRatio;
            }

            MABaRatio = ArrayBaratio.Average();//配列の平均
            NoBaRatio++;//送られてきたデータ数をインクリメント
        }
    }


    private async Task GetPayload()//ひたすら脳波受け取る
    {
        while (isExcuted)
            BufferManager.RecvBaratioFromPayload();
    }


    //以下値の受け渡し
    public float GetBaratio()//β/α比の受け渡し
    {
        //DebugManager.BufferDebug.Enqueue($"[ReadBuffer] BaRatio:{baratio.ToString()}");
        return baratio;

    }

    public float GetAvBaratio()//β/α比の平均値の受け渡し
    {

        //DebugManager.BufferDebug.Enqueue($"[ReadBuffer] AvBaRatio:{AvBaRatio.ToString()}");
        return AvBaRatio;

    }

    public bool GetActivateButton()//スタートボタンの活性状態の受け渡し
    {
        //if(buttonactivate == true)
        //    DebugManager.BufferDebug.Enqueue($"[ReadBuffer] buttonactivate:{buttonactivate}");
        return ButtonActivate;

    }

    public float GetMovingAverageBaRatio()//β/α比の移動平均値の受け渡し
    {
        return MABaRatio;
    }

}
