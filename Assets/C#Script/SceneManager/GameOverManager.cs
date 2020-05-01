using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @date 2020/05/01 [今後修正予定]
//
// １つ前のシーンに戻る
//

public class GameOverManager : SingletonMonoBehaviour<GameOverManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //SceneManager.LoadScene(TransitionManager.previous);
        }
    }
}
