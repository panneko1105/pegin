using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using GokUtil.UpdateManager;

// 呼び出し用
public enum SceneChangeType
{
    FADE,
    SLIDE_RIGHT,
    SLIDE_UP,
}

public class SceneChangeManager : SingletonMonoBehaviour<SceneChangeManager>
{
    [SerializeField] private GameObject fadePrefab;
    [SerializeField] private GameObject slidePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //=======================================================================================
    // Out演出 [秒]指定→シーン遷移を開始する
    //=======================================================================================
    public void SceneChangeOut(SceneChangeType type, float seconds, string sceneName)
    {
        switch (type)
        {
            case SceneChangeType.FADE:
                // コルーチン開始→シーン遷移
                StartCoroutine(FadeOutLoadScene(seconds, sceneName));
                break;
            default:
                Debug.Log("SceneChangeOut : false");
                LoadingScene.Instance.LoadScene(sceneName);
                break;
        }
    }

    //=======================================================================================
    // In演出 [秒]指定
    //=======================================================================================
    public void SceneChangeIn(SceneChangeType type, float seconds)
    {
        switch (type)
        {
            case SceneChangeType.FADE:
                //!< フェード用Obj生成
                GameObject obj = Instantiate(fadePrefab);
                // キャンバスに設置
                obj.transform.SetParent(GameObject.Find("Canvas").transform, false);
                //!< コンポーネントの取得
                FadeManager fadeManager = obj.GetComponent<FadeManager>();
                // 外部コルーチン開始
                StartCoroutine(fadeManager.SceneFadeIn(seconds));
                break;
            case SceneChangeType.SLIDE_RIGHT:
                StartCoroutine(SlideIn_RIGHT(seconds));
                break;
            case SceneChangeType.SLIDE_UP:
                StartCoroutine(SlideIn_UP(seconds));
                break;
            default:
                Debug.Log("SceneChangeIn : false");
                break;
        }
    }

    //==================================================
    // フェードアウト[秒] → シーン遷移
    //==================================================
    IEnumerator FadeOutLoadScene(float seconds, string sceneName)
    {
        //!< フェード用Obj生成
        GameObject obj = Instantiate(fadePrefab);
        // キャンバスに設置
        obj.transform.SetParent(GameObject.Find("Canvas").transform, false);
        //!< コンポーネントの取得
        FadeManager fadeManager = obj.GetComponent<FadeManager>();
        Debug.Log("フェードアウト開始");

        // Objのフェードアウト処理が完了するまで待機
        yield return StartCoroutine(obj.GetComponent<FadeManager>().SceneFadeOut(seconds));
        Debug.Log("フェードアウト終了、シーン遷移");

        // フェードアウトしたところでロード開始
        LoadingScene.Instance.LoadScene(sceneName);
    }

    IEnumerator SlideIn_RIGHT(float seconds)
    {
        // 時間計測開始
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        //!< Obj生成
        GameObject obj = Instantiate(slidePrefab);
        // キャンバスに設置
        obj.transform.SetParent(GameObject.Find("Canvas").transform, false);
        obj.transform.localScale = new Vector3(1.1f, 1.1f, 1);
        obj.transform.Rotate(0.0f, 0.0f, 3.0f, Space.World);

        //!< 保存用
        Vector3 pos = obj.transform.localPosition;


        //!< ラジアンに変換 (3.0度→)
        float rad = 3.0f * Mathf.Deg2Rad;  

        while (seconds > processTimer.TotalSeconds)
        {

            float move = Easing.QuintOut(processTimer.TotalSeconds, seconds, 0, 2050.0f);
            obj.transform.localPosition = new Vector3(pos.x + move * Mathf.Cos(rad), pos.y + move * Mathf.Sin(rad), 0);
            // 継続
            yield return null;
        }

        Debug.Log("スライドイン終了");
        // いらん
        Destroy(obj);
    }

    IEnumerator SlideIn_UP(float seconds)
    {
        // 時間計測開始
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        //!< Obj生成
        GameObject obj = Instantiate(slidePrefab);
        // キャンバスに設置
        obj.transform.SetParent(GameObject.Find("Canvas").transform, false);

        while (seconds > processTimer.TotalSeconds)
        {
            float y = Easing.QuintOut(processTimer.TotalSeconds, seconds, 0, 1080.0f);
            obj.transform.localPosition = new Vector3(0, y, 0);
            // 継続
            yield return null;
        }

        // いらん
        Destroy(obj);

        Debug.Log("スライドイン終了");
    }
}