using UnityEngine;

public class Interact_ShowText : MonoBehaviour, IInteractFunction
{
    GameDataFile GDFile;
    EventManager EM;
    [Header("Display Text")]
    [SerializeField] private string Text = "";
    [SerializeField] private inputName displayIcon;
    [SerializeField] private bool testInput = false;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        EM = GDFile.GetComponent<EventManager>();
    }
    private void Update()
    {
        if (lockState)
        {
            if (Vector3.Distance(GDFile.player.position, transform.position) >= 5f) ResetText();
        }
    }
    public void Interact() { ShowText(); }
    public string InteractType() { return "ShowText"; }
    public string InteractText() { return "to show test text"; }
    private bool lockState = false;
    void ShowText()
    {
        lockState = !lockState;
        InteractText IT = GDFile.player.GetComponent<InteractText>();
        if (testInput)
        {
            string icon = IT.getSymbol(displayIcon);
            IT.SetText("icon: " + icon + ", Input: " + displayIcon.ToString());
        }
        else { IT.SetText(Text); }
        IT.lockText(lockState);
    }
    void ResetText()
    {
        lockState = false;
        InteractText IT = GDFile.player.GetComponent<InteractText>();
        IT.ResetText();
    }
}
