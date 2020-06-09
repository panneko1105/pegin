using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GokUtil.UpdateManager;


public class TitleManager : SingletonMonoBehaviour<TitleManager>, IUpdatable
{
    [SerializeField] SceneObject m_nextScene;       //!< 次のシーン先をInspector上で指定できるよ
    bool flg = true;

    // Use this for initialization
    void Start()
    {
        // BGMの再生
        SoundManager.Instance.PlayBgm("Stage_Asa");
        SoundManager.Instance.PlaySe("small_footsteps");
        // シーンの初期化
        LoadingScene.Instance.InitScene();
        // シーンイン演出処理
        SceneChangeManager.Instance.SceneChangeIn(SceneChangeType.FADE, 0.5f);

        // 仮取得処理
        //GameDataManager.Instance.SaveItemFlg(1, 2);
        //GameDataManager.Instance.SaveItemFlg(1, 3);
        //GameDataManager.Instance.SaveItemFlg(3, 1);
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
        //float rsv = Input.GetAxis("R_Stick_V");
        //// 確認用
        //if (rsv != 0)
        //{
        //    Debug.Log(rsv);
        //}

        if (flg)
        {
            float triggerLR = Input.GetAxis("L_R_Trigger");

            // Aボタン
            if (Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("joystick button 3")
                || Input.GetKeyDown("joystick button 4") || Input.GetKeyDown("joystick button 5") || triggerLR !=0)
            {
                flg = false;
                // シーン遷移
                SceneChangeManager.Instance.SceneChangeOut(SceneChangeType.FADE, 0.5f, m_nextScene);
                // 演出
                GameObject gameObject = GameObject.Find("PUSH ANY BUTTON");
                //gameObject.GetComponent<Text>().color = Color.red;
                TextEffect textEffect = gameObject.GetComponent<TextEffect>();
                textEffect.SetFadeInfo(0.1f, 0.2f, 1.0f);
                // SE
                SoundManager.Instance.PlaySe("凍る・コチーン");
            }
        }
    }
}