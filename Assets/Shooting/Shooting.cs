using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//的の配置・弾の発射・スコア計算に用いる値の管理を行っているスクリプト
public class Shooting : MonoBehaviour, IInputClickHandler
{
    //弾UI
    public Text BulletLabel;
    //的数UI
    public Text TargetLabel;

    float Depth;//的の生成範囲(奥行)

    float Width;//的の生成範囲(横幅)

    //使用弾数
    public static int ShotCount = 0;

    //破壊的数
    public static int TargetCount = 0;

    //bullet prehab
    public GameObject Bullet;

    //Target prehab
    public GameObject Target;

    //照準宣言
    public Transform BulletCircle;

    //ホロレンズ宣言
    public Transform MixedRealityCamera;

    //弾丸の速度
    public float Speed = 1500;

    //射撃音取り込み
    private AudioSource AudioSource;
    private AudioClip AudioClip;

    // Use this for initialization
    void Start()
    {
        //UI表示
        BulletLabel.text = "銃弾：" + ShotCount;
        TargetLabel.text = "ターゲット：" + TargetCount;

        // 全てのジェスチャーイベントをキャッチできるようにする
        InputManager.Instance.AddGlobalListener(gameObject);

        //音読み込み
        AudioSource = this.GetComponent<AudioSource>();
        AudioClip = this.GetComponent<AudioSource>().clip;


        // Instantiateの引数にPrefabを渡すことでインスタンスを生成する
        GameObject Targets = Instantiate(Target) as GameObject;

        Depth = Random.Range(0.0f, 4.0f);//的が生成される奥行

        Width = Random.Range(-0.25f * Depth, 0.25f * Depth);//的が生成される横幅

        // ランダムな場所に配置する
        Target.transform.position = new Vector3(Width, Random.Range(-1.0f, 0.5f), Depth);
    }

    // Update is called once per frame
    void Update()
    {

        if (DestroyTarget.TargetDestroyed == true)
        {
            // Instantiateの引数にPrefabを渡すことでインスタンスを生成する
            GameObject Targets = Instantiate(Target) as GameObject;

            Depth = Random.Range(0.0f, 4.0f);//的が生成される奥行

            Width = Random.Range(-0.25f * Depth, 0.25f * Depth);//的が生成される横幅

            // ランダムな場所に配置する
            Target.transform.position = new Vector3(Width, Random.Range(-1.0f, 0.5f), Depth);

            DestroyTarget.TargetDestroyed = false;
        }

        //的数表示
        TargetLabel.text = "ターゲット：" + TargetCount;

    }


    //AirTapを検出したとき
    public void OnInputClicked(InputClickedEventData eventData)
    {


        //2.0でなく2.2で割るのはBullet Circleより画像が大きいため円の外に弾が生成されるのを防ぐための調整から
        float radius = BulletCircle.GetComponent<Renderer>().bounds.size.x * Random.Range(0, 1.0f) / 2.2f;

        float angle = Random.Range(0, 360);
        Vector3 vector = BulletCircle.transform.position;

        //射撃音再生
        AudioSource.PlayOneShot(AudioClip);

        //弾丸の複製
        GameObject Bullets = Instantiate(Bullet) as GameObject;


        //弾丸の位置を調整
        Bullets.transform.position = MixedRealityCamera.position;

        //弾道のベクトル　MRカメラと発射点の延長線上
        Vector3 Ballistic = (GetPosition(angle, radius, vector) - MixedRealityCamera.position).normalized; ;

        Vector3 force;
        //球の向きと速度設定
        force = Ballistic * Speed;


        //Rigidbodyに力を加えて発射
        Bullets.GetComponent<Rigidbody>().AddForce(force);

        //5秒後に弾丸消滅
        GameObject.Destroy(Bullets, 5.0f);

        //shotCount(残弾数)の数値を１ずつ減らす。
        ShotCount++;

        BulletLabel.text = "銃弾：" + ShotCount;




    }

    //発射点計算
    public Vector3 GetPosition(float angle, float radius, Vector3 vector)
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        return new Vector3(x + vector.x, y + vector.y, vector.z);
    }



}
