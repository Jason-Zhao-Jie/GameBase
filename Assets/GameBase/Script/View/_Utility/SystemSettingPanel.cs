using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemSettingPanel : MonoBehaviour
{
    public Toggle musicMute;
    public Slider musicVolume;
    public Toggle soundMute;
    public Slider soundVolume;
    public InputField inputAIDelay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnSaveSetting()
    {
        
    }

    public void OnClickClose()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
