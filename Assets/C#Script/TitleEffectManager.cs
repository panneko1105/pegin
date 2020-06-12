using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class TitleEffectManager : MonoBehaviour, IUpdatable
{
    [SerializeField] GameObject canvasData;       //!< 親Obj参照データ
    [SerializeField] GameObject effectPrefab;       //!< エフェクト用
    //private List<GameObject> myList = new List<GameObject>();
    //const int numMax = 100;                         //!< 最大数

    // Start is called before the first frame update
    void Start()
    {
        
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
        // ランダムにエフェクト生成
        if(Random.Range(1, 160) == 1)
        {
            CreateEffect();
        }

        //for (int i = 0; i < myList.Count; i++)
        //{
        //    //Destroy(list_toggle_[i]);
        //    myList[i].transform.position+= new Vector3(0, 50.0f * Time.deltaTime, 0);
        //}
    }

    void CreateEffect()
    {
        GameObject obj = Instantiate(effectPrefab);
        obj.transform.SetParent(canvasData.transform, false);
        Vector3 pos;
        pos.x = Random.Range(-1000.0f, 1000.0f);
        pos.y = -600;
        pos.z = 0;
        obj.transform.localPosition = pos;
        Vector3 scal;
        scal.x= Random.Range(1.4f, 4.4f);
        scal.y = scal.x;
        scal.z = 1;
        obj.transform.localScale = scal;

        //myList.Add(obj); 
    }
}
