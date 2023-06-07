using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    public Material[] skyboxes;
    private int currentIndex = 1;

    public void OnButtonClicked() {
        ChangeSkybox();
    }
    public void ChangeSkybox() {
        RenderSettings.skybox = skyboxes[currentIndex];

        currentIndex++;
        if (currentIndex >= skyboxes.Length)
        {
            currentIndex = 0;
        }
    }

}
