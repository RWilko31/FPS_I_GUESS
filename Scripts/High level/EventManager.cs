using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //-------Events--------\\
    public event gameEvent death, respawn, loadLevel, area, antigravity, craft;

    public void DeathEvent()
    { death?.Invoke(); }
    public void LoadScene()
    { loadLevel?.Invoke(); }
    public void AreaEvent()
    { area?.Invoke(); }
    public void RespawnEvent()
    { respawn?.Invoke(); }
    public void AntigravityEvent()
    { antigravity?.Invoke(); }
    public void CraftEvent()
    { craft?.Invoke(); }
}
