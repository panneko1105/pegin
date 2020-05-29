using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

// @date 2020/05/14 [今後修正予定]
//
// NowLoadingの動きとか
//

public class BaseSceneManager : SingletonMonoBehaviour<BaseSceneManager>, IUpdatable
{
    [SerializeField] SceneObject firstScene;                        //!< 開始するシーンを指定 (まぁ普通はTitleからだよね)
    [SerializeField] GameObject canvas;                             //!< キャンバス情報
    const int footMax = 4;
    [SerializeField] GameObject[] foot = new GameObject[footMax];   //!< 足跡Obj

    /* フェード設定関連 */
    [SerializeField] private float seconds = 0.15f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.8f;
    /* 演出制御関連 */
    [SerializeField] int waitCnt = 4;
    IEnumerator startEffect;
    //bool isUseeeeeeeeeeeeee;

    // Start is called before the first frame update
    void Start()
    {
        //isUseeeeeeeeeeeeee = false;
        // 開始
        LoadingScene.Instance.FirstLoadScene(firstScene);
    }

    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }

    // Update is called once per frame
    public void UpdateMe()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    // オン→オフ
        //    if (isUseeeeeeeeeeeeee)
        //    {
        //        // コルーチンの停止
        //        StopCoroutine(startEffect);
        //        for(int i = 0; i < footMax; i++)
        //        {
        //            foot[i].GetComponent<FadeManager>().StopAllCoroutines();
        //        }
        //        Debug.Log("BaseScene演出：OFF");
        //    }
        //    // オフ→オン
        //    else
        //    {
        //        // コルーチンの開始
        //        startEffect = StartEffect();
        //        StartCoroutine(startEffect);
        //        Debug.Log("BaseScene演出：ON");
        //    }

        //    isUseeeeeeeeeeeeee = !isUseeeeeeeeeeeeee;
        //}
    }

    //========================================
    // NowLoading画面のObjの有無
    //========================================
    public void SetObject(bool isUse)
    {
        // オフ
        if (!isUse)
        {
            // コルーチンの停止
            StopCoroutine(startEffect);
            for (int i = 0; i < footMax; i++)
            {
                foot[i].GetComponent<FadeManager>().StopAllCoroutines();
            }
            Debug.Log("BaseScene演出：OFF");
        }

        // falseになると存在を確認できないらしく、直接trueにできなかった...。
        // 仕方なく親Objからたどることに。
        canvas.transform.Find("AllObject").gameObject.SetActive(isUse);
        Debug.Log("BaseSceneObj：SetActive設定");

        // オン
        if (isUse)
        {
            // コルーチンの開始
            startEffect = StartEffect();
            StartCoroutine(startEffect);
            Debug.Log("BaseScene演出：ON");
        }
    }

    public IEnumerator StartEffect()
    {
        for (int i = 0; i < waitCnt; i++)
        {
            // 継続
            yield return null;
        }

        // 足跡1の繰り返し演出開始
        IEnumerator cor = foot[0].GetComponent<FadeManager>().StartFadeLoop(seconds, minAlpha, maxAlpha);
        StartCoroutine(cor);
        
        for(int i = 0; i < waitCnt; i++)
        {
            // 継続
            yield return null;
        }

        // 足跡2の繰り返し演出開始
        cor = foot[1].GetComponent<FadeManager>().StartFadeLoop(seconds, minAlpha, maxAlpha);
        StartCoroutine(cor);

        for (int i = 0; i < waitCnt; i++)
        {
            // 継続
            yield return null;
        }

        // 足跡3の繰り返し演出開始
        cor = foot[2].GetComponent<FadeManager>().StartFadeLoop(seconds, minAlpha, maxAlpha);
        StartCoroutine(cor);

        for (int i = 0; i < waitCnt; i++)
        {
            // 継続
            yield return null;
        }

        // 足跡4の繰り返し演出開始
        cor = foot[3].GetComponent<FadeManager>().StartFadeLoop(seconds, minAlpha, maxAlpha);
        StartCoroutine(cor);
    }
}