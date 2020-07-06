using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class testAndDelete : MonoBehaviour
{
    float delay = 1;
    public Dictionary<string, int> newDic = new Dictionary<string, int>();
    
    // Start is called before the first frame update
    void Start()
    {
        newDic.Add("a", 100);
        newDic.Add("b", 90);
        newDic.Add("z", 80);
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }
        delay = 1;
        newDic["a"] = 98;
        Debug.Log("==================");
        foreach (var a in newDic)
        {
            Debug.Log(a.Key+","+a.Value);
        }
        newDic["z"]++;
    }
}
