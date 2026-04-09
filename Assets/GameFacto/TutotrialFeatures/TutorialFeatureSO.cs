using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Scriptable Objects/Tutorial Feature SO")]
public class TutorialFeatureSO : ScriptableObject
{
    public TutorialID TutorialID;
    public bool IsTutorial;
    public string Title;
    public string Description;
    public VideoClip VideoClip;
    public Sprite FillImage;
    public Sprite FeatureImage;
    public int MinLevelRequirement, MaxLevelRequirement;
    public int Stage;
    public bool IsFinal;
}
