using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//スコア計算とその表示
//1発1発集中して撃ってもらうために使用弾数をスコア計算に取り入れているが
//スコア計算に使われる値は深く考えて設定していません

public class Congratulations : MonoBehaviour {

    int TargetCount;
    int ShotCount;
    public Text PointLabel;
    int TargetCoefficient = 550;
    int BulletCoefficient = 50;

    int Score;

	// Use this for initialization
	void Start () {

        //壊した的数
        TargetCount = Shooting.TargetCount;

        //残弾数
        ShotCount = Shooting.ShotCount;


        //スコア計算
        Score = TargetCount * TargetCoefficient - ShotCount * BulletCoefficient;

        if (Score < 0)
            Score = 0;

        //スコア表示
        PointLabel.text = "壊した的数:" + TargetCount + "\n"
            + "使用弾数:" + ShotCount + "\n"
            + "\n"
            + "Score:" + Score;


    }
}
