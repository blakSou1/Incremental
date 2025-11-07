using UnityEngine;

public class Mult : MonoBehaviour
{
    [HideInInspector] public Camera MultCamera;
    public GameObject scenarioG;
    private MulticTag scenario;
    public TextWithButtonComp TwB;
    public SpriteRenderer render;
    public bool DoEndAddDist = false;
    public bool IsEndMult = false;
    private int id = 0;
    
    private void Awake()
    {
        MultCamera = FindFirstObjectByType<Camera>();
        TwB = FindFirstObjectByType<TextWithButtonComp>();
        scenario = scenarioG.GetComponent<CMSEntityPfb>().AsEntity().Get<MulticTag>();
        TwB._button.OnClick.AddListener(ButtonFunc);
        if (!IsEndMult)
        {
            G.AudioManager.PlayMusic(R.Audio.MultMusic);
        }
        else
        {
            G.AudioManager.PlayMusic(R.Audio.Horror);
        }
    }

    public void Start()
    {
        PlayLine(0);
    }

    public void PlayLine(int id)
    {
        render.sprite = scenario.lines[id].sprite;
        _ = TwB.WriteTextWithButton(scenario.lines[id].text);
        if (DoEndAddDist && id == scenario.lines.Count - 1 && !IsEndMult)
        {
            render.material.SetFloat("_DistortAmount", 0.35f);
            render.transform.Translate(new Vector3(-0.8f, -0.3f));
            R.Audio.ScarySound.PlayAsSound();
        }

        if (IsEndMult && id == scenario.lines.Count - 1)
        {
            MultCamera.GetComponent<CameraShake>().SpecialShake(7, 2);
            render.material.SetFloat("_DistortAmount", 0.05f);
            render.transform.Translate(new Vector3(-0.1f, -0.1f));
            G.AudioManager.StopMusic();
            R.Audio.MachineStopped.PlayAsSound(0.1f);
            G.Main.LockDown();
        }
    }

    private void ButtonFunc()
    {
        if (id != scenario.lines.Count - 1)
        {
            id++;
            PlayLine(id);
        }
        else
        {
            _ = G.SceneLoader.UnloadAdditive(gameObject.scene.name);

            if (IsEndMult)
                G.AudioManager.StopMusic();
            else
                G.AudioManager.PlayMusic(R.Audio.MainGameMusic);            
        }
    }
}
