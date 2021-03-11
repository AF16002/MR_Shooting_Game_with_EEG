using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

#if UNITY_UWP
using Windows.Storage;
using System.Threading.Tasks;
#endif

//ログ出力(実機のみ)
//アプリ起動後、初回プレイのみログ出力する
//HoloLensのデバイスポータルに出力される

public class OutputLog : MonoBehaviour
{
    private List<string> GameTime = new List<string>();//経過時間
    private List<string> Baratio = new List<string>();//β/α比
    private List<string> TargetCount = new List<string>();//破壊的数
    private List<string> ShotCount = new List<string>();//使用弾数
    private List<string> MABaratio = new List<string>();//β/α比の移動平均
    private List<string> AvBaRatio = new List<string>();//β/α比のベースライン
    private List<string> MinBaRatio = new List<string>();//β/α比の最低値
    private List<string> UnixTime = new List<string>();//Unixtime
    private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);//Unixtime用
    private bool SaveFlag = false;//記録するか
    //private DebugManager DebugManager;//デバッグ用


    // Use this for initialization
    void Start()
    {
        //DebugManager = GameObject.Find("ValueFromDataProcessor").GetComponent<DebugManager>();//デバッグ用
    }


    // Update is called once per frame
    void Update()
    {
        
        if (ReadBuffer.Recode == true && ReadBuffer.log == true)
        {
            GameTime.Add(ReadBuffer.time.ToString());//経過時間を格納
            Baratio.Add(ReadBuffer.BaRatio.ToString());//β/α比を格納
            TargetCount.Add((Shooting.TargetCount).ToString());//破壊的数を格納
            ShotCount.Add(Shooting.ShotCount.ToString());//使用弾数を格納
            MABaratio.Add(ReadBuffer.MABaRatio.ToString());//β/α比の移動平均を格納
            AvBaRatio.Add(ReadBuffer.AvBaRatio.ToString());//β/α比のベースラインを格納
            MinBaRatio.Add(ReadBuffer.MinBaRatio.ToString());//β/α比の最低値を格納
            UnixTime.Add(((DateTime.Now - UnixEpoch).TotalSeconds).ToString());//Unixtimeを格納
            ReadBuffer.log = false;//同じデータを重複して記録しないように
        }

        if (SceneManager.GetActiveScene().name == "Congratulations" && SaveFlag == false)//スコアシーンに遷移したら
        {
            SaveFlag = true;//セーブフラグをtrueに
            CreateCSV();//CSV作成
            //DebugManager.BufferDebug.Enqueue($"[OutputLog] :CreateCSV()");//デバッグ用
        }
       
    }


    public void CreateCSV()//CSV作成
    {
        var columns = new[] { "GameTime", "Baratio", "TargetCount", "ShotCount", "MABaratio", "AvBaRatio", "MinBaRatio", "UnixTime" };//記録項目(20-26行目記載)
        var filename = (DateTime.Now).ToString().Replace(':', '-').Replace('/', '-') + ".csv";//CSVファイル名
        var result = string.Join(",", columns);//記録データをカンマ区切りで結合
        result = result + "\n";//改行
        //DebugManager.BufferDebug.Enqueue($"[OutputLog] :CreatFile");
        for (int num = 0; num < GameTime.Count; num++)//ゲームシーンが終わるまでのデータをひたすら記録
        {
            var value = new[] { GameTime[num], Baratio[num], TargetCount[num], ShotCount[num], MABaratio[num], AvBaRatio[num], MinBaRatio[num], UnixTime[num] };//記録データをvalueに格納
            var values = string.Join(",", value);//記録データをカンマ区切りで結合
            values = values + "\n";//改行
            result += values;//記録データの更新
        }

#if UNITY_UWP
        Task.Run(async ()=>
        {

            // ローカルフォルダー
            // 「User Files\LocalAppData\<アプリ名>\LocalState」 以下にできる
            {
                //  var filename = ((DateTime.Now - UnixEpoch).TotalSeconds).ToString() + ".csv";
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DocumentLibraryTest", CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(result);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
                   //DebugManager.BufferDebug.Enqueue($"[OutputLog] :Task.Run");
        });
#endif
    }
}