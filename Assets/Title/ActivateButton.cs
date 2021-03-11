using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
 //startボタンの活性化を行っている

public class ActivateButton : MonoBehaviour
{
    public ReadBuffer ReadBuffer;//ReadBufferスクリプト参照
    Button StartButton;//startボタン
    //private DebugManager DebugManager;//debug用

    // Use this for initialization
    void Start()
        {
        //DebugManager = GameObject.Find("ValueFromDataProcessor").GetComponent<DebugManager>();//debug用
        ReadBuffer = GameObject.Find("ValueFromDataProcessor").GetComponent<ReadBuffer>();//ReadBufferスクリプト参照
        StartButton = GameObject.Find("GameStart").GetComponent<Button>();
        StartButton.interactable = false;//ボタン非活性に
        //debugManager.bufferDebug.Enqueue($"[ActivateButton] Start().btn.interactable:{btn.interactable}");
    }

        void Update()
        {
        StartButton.interactable = ReadBuffer.GetActivateButton();//ReadBufferでボタンの活性化を制御(ベースラインのβ/α比を算出し終わったらボタン活性化に)
        //debugManager.bufferDebug.Enqueue($"[ActivateButton] btn.interactable:{btn.interactable}");
    }

    }