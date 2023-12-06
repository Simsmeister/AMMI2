using UnityEngine;

public class RainbowLight : MonoBehaviour
{
    public Light rainbowLight;
    public float colorChangeSpeed = 1f;

    private void Update()
    {
        // Calculate the hue value based on time
        float hue = Time.time * colorChangeSpeed % 1f;

        // Set the light color using HSV color space
        rainbowLight.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}
