using UnityEngine;

public static partial class R
{
    public static bool isInited = false;
    public static VoiceSO normalVoice;
    public static VoiceSO ubiVoice;

    public static void InitAll()
    {
        isInited = true;
        normalVoice = Resources.Load<VoiceSO>("normalVoice");
        ubiVoice = Resources.Load<VoiceSO>("UbiVoice");
        R.InitAudio();
    }
}
