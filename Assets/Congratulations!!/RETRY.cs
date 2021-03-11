using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using HoloToolkit.Unity.InputModule;
using System;

//リトライの実装

public class RETRY : MonoBehaviour
{



    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);//ボタンクリックを認識できるように
    }

    public void OnClickTapEvent()//ボタン押下を認識した時
    {
        SceneManager.LoadScene("Title");//タイトルシーンに遷移する

    }
}

