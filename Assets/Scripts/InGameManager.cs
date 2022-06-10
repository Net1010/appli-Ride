using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : SManager
{
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void LoadScene(string sceneName)
    {
        AudioManager.instance.PlayClick();
        SceneCtrl.Instance.LoadScene(sceneName);
    }
}
