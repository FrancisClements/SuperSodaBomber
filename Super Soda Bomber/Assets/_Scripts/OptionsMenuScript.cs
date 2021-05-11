using UnityEngine;
using UnityEngine.UI;

/*
OptionsMenuScript
    responsible for the controlling the values
    at the options.
*/
public class OptionsMenuScript : MonoBehaviour
{

    // Variable Configs
    public Slider masterSl, musicSl, sfxSl;
    public Button voiceBtn;

    [Header("Voice Toggle Images")]
    [Space(20f)]

    //images for the voice toggle button
    [SerializeField] private Sprite offImg;
    [SerializeField] private Sprite onImg;
    int voiceEnable;

    void Start(){
        //automatically sets the value of the slider
        masterSl.value = PlayerPrefs.GetFloat("MasterVol", 1);
        musicSl.value = PlayerPrefs.GetFloat("MusicVol", 1);
        sfxSl.value = PlayerPrefs.GetFloat("SFXVol", 1);
        //automatically sets the value & image of the voice toggle
        // voiceEnable = PlayerPrefs.GetInt("Voice", 1);
        // if(voiceEnable == 1){voiceBtn.image.sprite = onImg;}
    }

    //setting the vars when slider changed
    public void _SetVol(Slider sl){
        
        if(sl == masterSl){
            PlayerPrefs.SetFloat("MasterVol", sl.value);
        }
        else if(sl == musicSl){
            PlayerPrefs.SetFloat("MusicVol", sl.value);
        }        
        else if(sl == sfxSl){
            PlayerPrefs.SetFloat("SFXVol", sl.value);
        }
    }

    public void _ToggleVoice(){
        //Since there's no setbool at playerprefs, we will make a make-shift one using int
        voiceEnable = voiceEnable == 1? 0: 1;
        PlayerPrefs.SetInt("Voice", voiceEnable);
        
        if(voiceEnable == 1){
            voiceBtn.image.sprite = onImg;
        }
        else{
            voiceBtn.image.sprite = offImg;
        }
    }
}
