using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

//β/α比のディスプレイ表示
public class DisplayBaRatio : MonoBehaviour
{
    float BaRatio;//β/α比
    Text BaratioText;//テキスト表示
    //private DebugManager DebugManager;//デバッグ用
    public ReadBuffer ReadBuffer;//ValueFromDataProcessorに取り付けているReadBuffer参照

    // Use this for initialization
    void Start()
    {
        BaratioText = GetComponentInChildren<Text>();//UIのテキストの取得の仕方
        //DebugManager = GameObject.Find("ValueFromDataProcessor").GetComponent<DebugManager>();//デバッグ用
        ReadBuffer = GameObject.Find("ValueFromDataProcessor").GetComponent<ReadBuffer>();//ReadBuffer参照
    }

    // Update is called once per frame


    private void Update()
    {
        BaRatio = ReadBuffer.GetBaratio();//ReadBufferからβ/α比受け取り
        BaratioText.text = "β/α比：" + BaRatio.ToString("F3");//β/α比のディスプレイ表示

    }
}
