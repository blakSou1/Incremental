using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PipeController : MonoBehaviour
{
    private Tweener currentPunchTween;
    public Transform posToSpawn;

    public async UniTask AutoSpawner()
    {
        while (!G.Main.isLockDown && gameObject != null)
        {
            await UniTask.Delay(5500);
            if (G.GameState.AutoSpawnLevels > 0)
            {
                if (G.Main.isLockDown) return;
                _ = CreateItem(false);
            }

            await UniTask.Delay(5500);
            if (G.GameState.AutoSpawnLevels > 1)
            {
                if (G.Main.isLockDown) return;
                _ = CreateItem(false);
            }
            
            await UniTask.Delay(5500);
            if (G.GameState.AutoSpawnLevels > 2)
            {
                if (G.Main.isLockDown) return;
                _ = CreateItem(false);
            }
        }
        
    }

    public void CreateIt()
    {
        _ = CreateItem(true);
    }

    public async UniTask CreateItem(bool fromLever)
    {
        if (G.Main.isLockDown) return;
        if (G.Main.CheckCanSpawnNewItem())
        {
            if(fromLever) R.Audio.LevelDown.PlayAsSoundRandomPitch(0.1f);
            await UniTask.Delay(450);
            R.Audio.PipeOutNewObject.PlayAsSoundRandomPitch(0.2f);
            if (currentPunchTween != null && currentPunchTween.IsActive())
            {
                currentPunchTween.Complete();
                currentPunchTween.Kill();
            }
            currentPunchTween = transform.DOPunchScale(Vector3.one * -0.45f, 0.4f, elasticity: 0f, vibrato: 0);
            await UniTask.Delay(200);
            Instantiate(G.Main.RandomSelector.SpinRoulette(), posToSpawn.position + (Vector3.left * UnityEngine.Random.Range(-0.3f, 0.3f)), Quaternion.identity);
        }
        else
        {
            R.Audio.Wrong_Error.PlayAsSoundRandomPitch(0.2f);
        }
    }
}
