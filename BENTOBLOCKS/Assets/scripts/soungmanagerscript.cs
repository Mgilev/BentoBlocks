using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class soungmanagerscript : MonoBehaviour
{
 
    [SerializeField] Image soundOnIcon;
    [SerializeField] Image soundOffIcon;
    private bool muted = false;
    [SerializeField] AudioSource audioSource;



    private void Start()
    {
        if (!PlayerPrefs.HasKey("muted"))
        {
            PlayerPrefs.SetInt("muted", 0);
            Load1();
        }
        else
        {
            Load1();
        }

        iconupdate();
        audioSource.mute = muted;

        
    }


    


    public void Onbuttonpress()
    {
        muted = !muted;  
        audioSource.mute = muted;  
        Save1();
        iconupdate();  
    }

    private void iconupdate()
    {
        if (muted == false)
        {
            soundOnIcon.enabled = true;
            soundOffIcon.enabled = false;
        }
        else
        {
            soundOnIcon.enabled = false;
            soundOffIcon.enabled = true;

        }

    }




    private void Load1()
    {
        muted = PlayerPrefs.GetInt("muted") == 1;

    }

    private void Save1()
    {
        PlayerPrefs.SetInt("muted", muted ? 1:0);

    }


}




