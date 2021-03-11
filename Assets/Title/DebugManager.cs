using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デバック用のクラス
/// バッファーに追加することで、別スレッドでの処理でもHoloLens上で表示することができる
/// デバックしたいところに queueを追加すればOK
/// </summary>

public class DebugManager : MonoBehaviour
{
    public ConcurrentQueue<string> BufferDebug = new ConcurrentQueue<string>();
    public TextMesh DebugText;//debug文入れるテキスト

    void Update()
    {
        if (BufferDebug.IsEmpty == false)
        {
            string msg;//debug文
            var result = BufferDebug.TryDequeue(out msg);//debug文取り出し
            if (result == true)//debug文があったら
            {
                //Debug.Log($"{msg}\n");
                if (DebugText != null)
                {
                    DebugText.text += $"Debug : {msg}\n";//debug文出力
                }
            }
        }
    }
}