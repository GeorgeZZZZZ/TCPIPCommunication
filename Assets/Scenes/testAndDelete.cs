using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Timers;

public class testAndDelete : MonoBehaviour
{
    Timer tmr;
    public float CheckAliveDelayTime = 2f;
    private float CheckAliveDelayTimeCache;
    // Start is called before the first frame update
    void Start()
    {
        tmr = new Timer();
        tmr.Interval = 5000;
        tmr.Elapsed += reached;
        tmr.Start();
        Debug.Log("Time Start");
    }
    public bool timeReached;
    // Update is called once per frame
    public bool startAgain;
    void Update()
    {
        Debug.Log(tmr.Enabled);
        if (timeReached)
        {
            startAgain = true;
            timeReached = false;
            tmr.Stop();
            CheckAliveDelayTimeCache = CheckAliveDelayTime;
        }
        if (startAgain)
        {
            if (CheckAliveDelayTimeCache > 0)
                CheckAliveDelayTimeCache -= Time.deltaTime;
            else
            {
                tmr.Start();
                startAgain = false;
                Debug.Log("====Start Again!!========");
            }

        }
    }

    public void reached(object sender, EventArgs e)
    {
        timeReached = true;
        Debug.Log("Time Reached");
    }
}
