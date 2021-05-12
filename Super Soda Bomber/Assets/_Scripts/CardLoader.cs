using System.Collections.Generic;
using UnityEngine;

public class CardLoader : MonoBehaviour
{
    [SerializeField] private  List<GameObject> titleCards;
    private static GameObject shownCard;

    public void LoadCard(SceneIndex sceneIndex){
        int index = (int) sceneIndex-3;
        if (shownCard == null)
            shownCard = titleCards[index];        
    }

    public void ToggleCard(bool active){
        if (shownCard != null)
            shownCard.SetActive(active);
        else
            Debug.LogError("The card is not loaded!");
    }

    public void UnloadCard(){
        if (shownCard != null)
            shownCard = null;
    }
}
