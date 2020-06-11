using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class BaseSceneManager : SingletonMonoBehaviour<BaseSceneManager>, IUpdatable
{
    [SerializeField] GameObject loadCamera;                         //!< NOWLOADING...用のカメラ
    [SerializeField] SceneObject firstScene;                        //!< 開始するシーンを指定 (まぁ普通はTitleからだよね)
    [SerializeField] GameObject canvas;                             //!< キャンバス情報
    const int footMax = 4;
    [SerializeField] GameObject[] foot = new GameObject[footMax];   //!< 足跡Obj

    /* フェード設定関連 */
    [SerializeField] private float seconds = 0.25f;
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
        // カメラ
        loadCamera.SetActive(isUse);
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
        FadeManager fadeManager = foot[0].GetComponent<FadeManager>();
        fadeManager.SetFadeInfo(seconds, minAlpha, maxAlpha);
        IEnumerator cor = fadeManager.StartFadeLoop();
        StartCoroutine(cor);

        for (int i = 0; i < waitCnt; i++)
        {
            // 継続
            yield return null;
        }

        // 足跡2の繰り返し演出開始
        fadeManager = foot[1].GetComponent<FadeManager>();
        fadeManager.SetFadeInfo(seconds, minAlpha, maxAlpha);
        cor = fadeManager.StartFadeLoop();
        StartCoroutine(cor);

        for (int i = 0; i < waitCnt; i++)
        {
            // 継続
            yield return null;
        }

        // 足跡3の繰り返し演出開始
        fadeManager = foot[2].GetComponent<FadeManager>();
        fadeManager.SetFadeInfo(seconds, minAlpha, maxAlpha);
        cor = fadeManager.StartFadeLoop();
        StartCoroutine(cor);

        for (int i = 0; i < waitCnt; i++)
        {
            // 継続
            yield return null;
        }

        // 足跡4の繰り返し演出開始
        fadeManager = foot[3].GetComponent<FadeManager>();
        fadeManager.SetFadeInfo(seconds, minAlpha, maxAlpha);
        cor = fadeManager.StartFadeLoop();
        StartCoroutine(cor);
    }
}