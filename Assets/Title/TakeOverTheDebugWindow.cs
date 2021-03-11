using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//デバッグパネル用のシングルトン
//このスクリプト付けたオブジェクトはシーン遷移で破壊されない
public class TakeOverTheDebugWindow : MonoBehaviour
{

    static public TakeOverTheDebugWindow instance;
    void Awake()
    {
        if (instance == null)//他に同じインスタンスがない場合
        {

            instance = this;
            DontDestroyOnLoad(gameObject);//シーン遷移で破壊されない
        }
        else
        {

            Destroy(gameObject);//同じオブジェクトが出現したら破壊される
        }
    }
}
