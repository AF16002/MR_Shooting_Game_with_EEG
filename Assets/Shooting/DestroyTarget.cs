using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//弾と的が衝突した時の処理
public class DestroyTarget : MonoBehaviour
{

    public GameObject EffectPrefab;// エフェクトプレハブのデータを入れるための箱を作る。
    public AudioClip Explosion;//爆発音
    public static bool TargetDestroyed = false;

    // このメソッドはぶつかった瞬間に呼び出される
    void OnCollisionEnter(Collision other)
    {

        // もしもぶつかった相手のTagにBulletという名前が書いてあったならば（条件）
        if (other.gameObject.CompareTag("Bullet"))
        {
            Shooting.TargetCount++;

            // このスクリプトがついているオブジェクトを破壊する（thisは省略が可能）
            Destroy(this.gameObject);

            // ぶつかってきたオブジェクトを破壊する
            Destroy(other.gameObject);

            AudioSource.PlayClipAtPoint(Explosion, transform.position);//爆発音を再生

            // エフェクトを実体化（インスタンス化）する。
            GameObject Effect = (GameObject)Instantiate(EffectPrefab, transform.position, Quaternion.identity);

            // エフェクトを２秒後に画面から消す
            Destroy(Effect, 2.0f);

            TargetDestroyed = true;


        }
    }
}