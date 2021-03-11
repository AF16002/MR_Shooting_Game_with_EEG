using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//的からの効果音再生

public class SoundFromTheTarget : MonoBehaviour {

    public AudioClip SoundEffect;
    AudioSource AudioSource;//効果音
    private float SoundEffectInterval = 5.0f;//効果音再生間隔
    private float time = 0.0f;//効果音再生間隔調整用

    // Use this for initialization
    void Start () {
        AudioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {

        time += Time.deltaTime;//時間計測

        if (time >= SoundEffectInterval)//効果音再生間隔以上時間がたったら
        {
            AudioSource.PlayOneShot(SoundEffect);//効果音再生
            if (SoundEffectInterval > 1.0)//時間経過とともに効果音再生間隔が短くする
            {
                SoundEffectInterval = SoundEffectInterval - 0.2f;//効果音再生間隔を0.2秒ずつ短くする
            }
            time = 0.0f;//経過時間リセット
        }

    }
}
