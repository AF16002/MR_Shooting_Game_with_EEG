using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;


//FPS確認
//計算は詳しくは理解していない…
//以前フレームレート落ちていないか確認した際に作成
//現在は使用していない
public class FpsDisplay : MonoBehaviour
{

    // 変数
    int FrameCount;
    float prevTime;
    float fps;
    Text myText;

    // 初期化処理
    void Start()
    {
        myText = GetComponentInChildren<Text>();//UIのテキストの取得の仕方
        // 変数の初期化
        FrameCount = 0;
        prevTime = 0.0f;
    }

    // 更新処理
    void Update()
    {
        FrameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            fps = FrameCount / time;
            Debug.Log(fps);

            FrameCount = 0;
            prevTime = Time.realtimeSinceStartup;

        }
    }

    // 表示処理
    private void OnGUI()
    {
        myText.text = "FPS：" + fps.ToString();//FPSのディスプレイ表示
    }
}