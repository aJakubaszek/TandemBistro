using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance{get; private set;}

    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public Sprite GetSprite(int index){
        if(index >= sprites.Count || index<0){
            return sprites[0];
        }
        return sprites[index];
    }
}
