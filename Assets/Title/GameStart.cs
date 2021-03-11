using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // この１行を忘れないこと。
using HoloToolkit.Unity.InputModule;

//ゲームシーンへ遷移させる

public class GameStart : MonoBehaviour
{



    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);//ボタンへのクリック認識
    }


    // メソッドに「public」が付いていることを確認する（ポイント）
    public void OnClickTapEvent()//startボタンがクリックされたら
    {

        SceneManager.LoadScene("ShootingGame");//ゲームシーンへ遷移

    }
}
