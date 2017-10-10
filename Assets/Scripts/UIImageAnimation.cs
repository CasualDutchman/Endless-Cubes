using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimation : MonoBehaviour {

    public float fps = 24.0f;
    public Sprite[] animationArray;

    Image currentImage;

    float timer;
    int index = 0;

	void Start () {
        currentImage = GetComponent<Image>();
    }
	
    void Update () {
        timer += Time.deltaTime * fps;
        if (timer >= 1) {
            timer -= 1;
            index++;
            if (index >= animationArray.Length)
                index = 0;

            currentImage.sprite = animationArray[index];
        }
	}
}
