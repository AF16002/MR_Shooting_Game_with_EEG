using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

//照準の大きさを変えている

public class SizeChange : MonoBehaviour
{
    float MABaRatio;//β/α比
    float Diameter;//照準直径

    //private DebugManager DebugManager;//debug用
    public ReadBuffer ReadBuffer;//ValueFromDataProcessorに取り付けているReadBuffer参照
    float AvBaRatio;//タイトルシーンで算出しておいたβ/α比の平均値
    float DiffBaRatio;//basebaratioとゲーム中のβ/α比の移動平均値との差分
    void Start()
    {
        ReadBuffer = GameObject.Find("ValueFromDataProcessor").GetComponent<ReadBuffer>();//ValueFromDataProcessorに取り付けているReadBuffer参照
        //DebugManager = GameObject.Find("ValueFromDataProcessor").GetComponent<DebugManager>();//debug用
    }

    private void Update()
    {
        
        MABaRatio = ReadBuffer.GetMovingAverageBaRatio();//readBufferからβ/α比の移動平均値をコピー

        AvBaRatio = ReadBuffer.GetAvBaratio();//readBufferからβ/α比の平均値をコピー

        DiffBaRatio = MABaRatio - AvBaRatio;//basebaratioとゲーム中のβ/α比の移動平均値との差分を計算


        //集中時に照準直径最小0.1m、非集中時に照準直径最大1.0mを取るように設計
        Diameter = 0.5f - 1.0f * DiffBaRatio;//1s移動平均の際

        //Diameter = 0.25f - 1.5f * DiffBaRatio;//3s移動平均の際



        if (Diameter >= 1.0f)//直径の最大値を1mに固定
        {
            Diameter = 1.0f;
        }

        else if (Diameter <= 0.1f)//直径の最小値を0.1mに固定
        {
            Diameter = 0.1f;
        }

        //照準の収縮機能を実装していた時の残骸
        //Diameter = 0.8f - (0.4f * baratio);
        //収縮直径
        //Diameter = 0.8f - (1 - sin) * (-0.2f * baratio + 0.4f);


        //照準の大きさ変更実装
        this.transform.localScale = new Vector3(Diameter, Diameter, 1);

    }

}