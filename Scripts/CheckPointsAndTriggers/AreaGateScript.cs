using UnityEngine;

public class AreaGateScript : MonoBehaviour
{
    [SerializeField] private bool isActive = false;
    private int area = 0;
    [SerializeField] private int redArea = 0;
    [SerializeField] private int greenArea = -1;


    [SerializeField] GateDetector gate1;
    [SerializeField] GateDetector gate2;
    private int dir = 0;
    private bool canChange = false;

    GameDataFile GDfile;
    EventManager EM;
    
    private void Awake()
    {
        area = redArea;
        GDfile = FindObjectOfType<GameDataFile>();
        EM = GDfile.GetComponent<EventManager>();

        enableCheck();
    }
    bool enableCheck()
    {
        if (gameState.area == greenArea || gameState.area == redArea) isActive = true;
        else isActive = false;

        return isActive;
    }
    private void FixedUpdate()
    {
        if (!enableCheck()) return;

        if (dir == 0)
        {
            if (gate1.onTrigger) dir = 1;
            else if (gate2.onTrigger) dir = -1;
        }
        if (dir == 1) IncCheck();
        else if (dir == -1) DecCheck();

        if (canChange && !gate1.onTrigger && !gate2.onTrigger) changeArea();
    }
    void IncCheck() //checks conditions to increment the area (green to red)
    {
        if (gate2.onTrigger && !gate1.onTrigger) canChange = true;
        if (gate1.onTrigger) canChange = false; //makes sure the player has to go all the way through to change area
    }
    void DecCheck() //checks conditions to decrement the area (red to green)
    {
        if (gate1.onTrigger && !gate2.onTrigger) canChange = true;
        if (gate2.onTrigger) canChange = false; //makes sure the player has to go all the way through to change area
    }

    private void changeArea()
    {
        if (dir == 1) gameState.area = redArea;
        else gameState.area = greenArea;
        EM.AreaEvent();
        canChange = false;
        dir = 0;
    }
}
