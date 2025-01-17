using UnityEngine;

public class InfoBehaviour : MonoBehaviour
{
    const float SPEED = 6f;
    private const float SCALE_THRESHOLD = 0.01f;

    [SerializeField]
    Transform SectionInfo;
    [SerializeField] public AudioSource voiceOver;

    Vector3 desiredScale = Vector3.zero;
    private bool isScaling = false;

    void Update()
    {
        // Only perform scaling if necessary
        if (isScaling)
        {
            Vector3 newScale = Vector3.Lerp(SectionInfo.localScale, desiredScale, SPEED * Time.deltaTime);
            
            // Check if we're close enough to desired scale
            if (Vector3.Distance(newScale, desiredScale) < SCALE_THRESHOLD)
            {
                SectionInfo.localScale = desiredScale;
                isScaling = false;
            }
            else
            {
                SectionInfo.localScale = newScale;
            }
        }
    }

    public void OpenInfo()
    {
        desiredScale = Vector3.one;
        isScaling = true;
    }

    public void CloseInfo()
    {
        desiredScale = Vector3.zero;
        isScaling = true;
        if (voiceOver != null)
        {
            voiceOver.Play();
        }
    }
}
