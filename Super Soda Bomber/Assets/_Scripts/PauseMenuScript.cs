﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuScript : PublicScripts
{
    GameObject confirmPrompt;

    public void ReloadCheckpoint(){
        _TogglePrompt(confirmPrompt);
    }

    public void Resume(){
        _TogglePrompt(gameObject);
    }

    public void QuitLevel(){
        _TogglePrompt(confirmPrompt);
    }

}

