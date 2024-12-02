using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Cursor Sprites")]
    [SerializeField] Sprite defaultCursor; 
    [SerializeField] Sprite pickUpCursor;
    [SerializeField] Sprite putDownCursor;
    [SerializeField] Sprite giveCursor;
    [SerializeField] Sprite trashCursor;

    [Header("Cursor settings")]
    [SerializeField] private float defaultSize = 20f;
    [SerializeField] private float largeSize = 50f;

    private Image cursorImage;

    private void Awake(){
        if (Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start(){
        cursorImage = GetComponent<Image>();
        SetCursor(defaultCursor);
    }

    public void SetCursor(Sprite sprite){
        if (cursorImage != null && cursorImage.sprite != sprite){
            cursorImage.sprite = sprite;
            if(sprite != defaultCursor){
            cursorImage.rectTransform.sizeDelta = new Vector2(largeSize, largeSize);
            }
            else{
                cursorImage.rectTransform.sizeDelta = new Vector2(defaultSize, defaultSize);
            }
        }
    }
    public void ResetCursor(){
        SetCursor(defaultCursor);
    }

    public Sprite GetPickupCursor(){
        return pickUpCursor;
    }
    public Sprite GetTrashCursor(){
        return trashCursor;
    }
    public Sprite GetPutDownCursor(){
        return putDownCursor;
    }
    public Sprite GetGiveCursor(){
        return giveCursor;
    }
}

