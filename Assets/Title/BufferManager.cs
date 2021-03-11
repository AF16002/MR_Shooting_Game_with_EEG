using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// バッファ用のクラス
/// TCPから受信したものを別スレッドからメインスレッドに引き渡すために、別スレッドで動いているTCPReceiverから
/// バッファに追加して、メインスレッドで動いているスクリプトからバッファの中身を取り出している
/// 以下はバッファから取り出す処理をまとめている
/// </summary>

public class BufferManager : MonoBehaviour
{
    public ConcurrentQueue<Payload> PayloadBuffer = new ConcurrentQueue<Payload>();//送られてきたパケット
    public ConcurrentQueue<double> BaRatioBuffer = new ConcurrentQueue<double>();//送られてきたパケットを入れるキュー
    //private DebugManager DebugManager;//debug用



    // Use this for initialization
    void Start()
    {
        //DebugManager = GameObject.Find("ValueFromDataProcessor").GetComponent<DebugManager>();//debug用
                                                                                              //debugManager.bufferDebug.Enqueue("[BufferManager] BufferManager Start");

    }

    public void RecvBaratioFromPayload()
    {
        Payload Payload;
        //DebugManager.BufferDebug.Enqueue("[BufferManager] RecvBaratioFromPayload()");


        if (PayloadBuffer.IsEmpty)
        {
            //DebugManager.BufferDebug.Enqueue("[BufferManager] PayloadBuffer.IsEmpty");
            return;
        }

        var result = PayloadBuffer.TryDequeue(out Payload);
        if (result == false)
        {
            //DebugManager.BufferDebug.Enqueue("[BufferManager] result == false");
            return;
        }

        if (Double.IsNaN(Payload.Data.BetaAlpha))
        {
        }
        else
        {
            double BaRatio = Payload.Data.BetaAlpha;//パケット内にβ/α比があったらbaRatioに取り出し

            BaRatioBuffer.Enqueue(BaRatio);//BaRatioBufferにβ/α比を格納

        }
    }

}