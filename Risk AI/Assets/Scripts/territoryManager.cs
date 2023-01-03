using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
public class territoryManager : MonoBehaviour
{

    public static territoryManager instance;

    public GameObject attackPanel;
    public GameObject gamePanel;
    public GameObject cancelBtn;

    public List<GameObject> territoryList = new List<GameObject>();
    public Dictionary<string, GameObject> territoryDict = new Dictionary<string, GameObject>();

    public bool attacking = false;

    public GameObject attacker;

    public enum turn
    {
        PLAYER1,
        PLAYER2,
        PLAYER3,
        PLAYER4
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        attackPanel.SetActive(false);
        AddTerritoryData();
    }

    void AddTerritoryData()
    {
        GameObject[] theArray = GameObject.FindGameObjectsWithTag("Territory") as GameObject[];
        foreach (GameObject territory in theArray)
        {
            territoryList.Add(territory);
            territoryDict.Add(territory.name, territory);
        }
        tintTerritories();
    }

    void tintTerritories()
    {
        System.Random rnd = new System.Random();
        for (int i = 0;i<territoryList.Count; i++)
        {
            territoryHandler terrHandler = territoryList[i].GetComponent<territoryHandler>();
            var playerDict = new Dictionary<int, Territory.thePlayers>
            {
                { 1, Territory.thePlayers.PLAYER1 },
                { 2, Territory.thePlayers.PLAYER2 },
                { 3, Territory.thePlayers.PLAYER3 },
                { 4, Territory.thePlayers.PLAYER4 }
            };
            terrHandler.territory.player = playerDict[rnd.Next(1, 5)];
            terrHandler.TerrTxt.text = (rnd.Next(1, 7)).ToString();
            terrHandler.territory.troops = int.Parse(terrHandler.TerrTxt.text);
            if (terrHandler.territory.player == Territory.thePlayers.UNCLAIMED)
            {
                terrHandler.TintColor(new Color32(0,128,19,225));
                terrHandler.oldColor = new Color32(0, 128, 19, 225);
            }
            if (terrHandler.territory.player == Territory.thePlayers.PLAYER1)
            {
                terrHandler.TintColor(new Color32(127, 255, 212, 225));
                terrHandler.oldColor = new Color32(127, 255, 212, 225);
            }
            if (terrHandler.territory.player == Territory.thePlayers.PLAYER2)
            {
                terrHandler.TintColor(new Color32(65, 105, 225, 225));
                terrHandler.oldColor = new Color32(65, 105, 225, 225);
            }
            if (terrHandler.territory.player == Territory.thePlayers.PLAYER3)
            {
                terrHandler.TintColor(new Color32(178, 34, 34, 255));
                terrHandler.oldColor = new Color32(178, 34, 34, 255);
            }
            if (terrHandler.territory.player == Territory.thePlayers.PLAYER4)
            {
                terrHandler.TintColor(new Color32(255, 20, 147, 255));
                terrHandler.oldColor = new Color32(255, 20, 147, 255);
            }
        }

    }

    public void ShowTargets(string selected)
    {
        var attackDict = new Dictionary<string, List<string>>() 
        {
            { "Alaska", new List<string> {"Kamchatka", "Northwest Territory", "Alberta" } },
            { "Northwest Territory",  new List<string> {  "Alaska", "Alberta", "Ontario", "Greenland" } },
            { "Greenland",  new List<string> { "Northwest Territory", "Iceland", "Ontario", "Quebec" } },
            { "Scandinavia",  new List<string> { "Iceland", "Great Britain", "Ukraine", } },
            { "Siberia",  new List<string> { "Ural", "Yakutsk", "Irkutsk",  "China", "Mongolia" } },
            { "Yakutsk",  new List<string> { "Siberia", "Irkutsk", "Kamchatka" } },
            { "Kamchatka",  new List<string> { "Yakutsk", "Irkutsk", "Mongolia", "Alaska", "Japan" } },
            { "Alberta",  new List<string> { "Alaska", "Northwest Territory", "Ontario", "Western US" } },
            { "Ontario",  new List<string> { "Western US", "Eastern US", "Quebec", "Alberta", "Northwest Territory", "Greenland" } },
            { "Quebec",  new List<string> { "Ontario", "Eastern US", "Greenland" } },
            { "Iceland",  new List<string> { "Greenland", "Scandinavia", "Great Britain" } },
            { "Ukraine",  new List<string> { "Scandinavia", "Ural", "Afghanistan", "Southern Europe", "Middle East", "Northern Europe" } },
            { "Ural",  new List<string> { "Ukraine", "Afghanistan", "China", "Siberia" } },
            { "Western US",  new List<string> { "Eastern US", "Alberta", "Ontario" , "Central America", } },
            { "Eastern US",  new List<string> { "Ontario",  "Western US", "Central America", "Quebec" } },
            { "Great Britain",  new List<string> { "Iceland", "Scandinavia", "Northern Europe", "Western Europe"  } },
            { "Irkutsk",  new List<string> { "Siberia", "Yakutsk", "Kamchatka", "Mongolia" } },
            { "Japan",  new List<string> { "Mongolia", "Kamchatka" } },
            { "Western Europe",  new List<string> { "Northern Europe", "Southern Europe", "Great Britain" } },
            { "Northern Europe",  new List<string> { "Southern Europe", "Great Britain", "Ukraine", "Western Europe" } },
            { "Afghanistan",  new List<string> { "Ukraine", "Ural", "China", "India", "Middle East" } },
            { "Mongolia",  new List<string> { "Japan", "Kamchatka" , "Irkutsk", "China", "Siberia" } },
            { "Central America",  new List<string> { "Western US", "Eastern US", "Venezuela" } },
            { "Venezuela",  new List<string> { "Central America", "Brazil", "Peru" } },
            { "Southern Europe",  new List<string> { "Western Europe", "Northern Europe", "Ukraine", "Middle East", "Egypt" } },
            { "Middle East",  new List<string> { "Ukraine", "Southern Europe", "Afghanistan",  "India", "Egypt", "East Africa" } },
            { "China",  new List<string> { "India", "Siam", "Afghanistan", "Ural", "Siberia", "Mongolia"} },
            { "Peru",  new List<string> { "Venezuela", "Brazil", "Argentina"} },
            { "Brazil",  new List<string> { "Venezuela", "Peru", "Argentina", "North Africa" } },
            { "North Africa",  new List<string> { "Brazil", "Congo", "East Africa", "Egypt", } },
            { "Egypt",  new List<string> { "Southern Europe", "East Africa", "North Africa", "Middle East" } },
            { "India",  new List<string> { "Siam", "China", "Afghanistan", "Middle East" } },
            { "Siam",  new List<string> { "India", "China", "Indonesia" } },
            { "Indonesia",  new List<string> { "Siam", "New Guinea", "Western Australia"} },
            { "New Guinea",  new List<string> { "Indonesia", "Eastern Australia", } },
            { "Argentina",  new List<string> { "Peru", "Brazil"} },
            { "Congo",  new List<string> { "North Africa", "East Africa", "South Africa"} },
            { "East Africa",  new List<string> { "Egypt", "North Africa", "Congo", "South Africa", "Madagascar", "Middle East" } },
            { "Western Australia",  new List<string> { "Indonesia", "Eastern Australia" } },
            { "Eastern Australia",  new List<string> { "New Guinea", "Western Australia" } },
            { "South Africa",  new List<string> { "Congo", "East Africa" } },
            { "Madagascar",  new List<string> { "East Africa" } }
        };
        if (attacking == false) { 
            attacker = territoryDict[selected];
        }
        attacking = true;
        cancelBtn.SetActive(true);
        var surroundTers = attackDict[selected];
        foreach (var terr in territoryList)
        {
            territoryDict[terr.name].GetComponent<PolygonCollider2D>().enabled = false;
            territoryHandler terrHandler = territoryDict[terr.name].GetComponent<territoryHandler>();
            terrHandler.TintColor(terrHandler.disabledColor);
        }
        foreach (var terrName in surroundTers) 
        {
            territoryHandler terrHandler = territoryDict[terrName].GetComponent<territoryHandler>();
            terrHandler.TintColor(terrHandler.oldColor);
            if (terrHandler.territory.player != territoryDict[selected].GetComponent<territoryHandler>().territory.player)
            {
                print("terrhandler " + terrHandler.territory.player);
                print("og " + territoryDict[selected].GetComponent<territoryHandler>().territory.player);
                territoryDict[terrName].GetComponent<PolygonCollider2D>().enabled = true;
            }
        }
    }

    public void ShowAttackPanel(string desciption, string pUnits, string eUnits )
    {
        if (attacking == true) 
        {
            attackPanel.SetActive(true);
            gamePanel.SetActive(false);
            AttackScript gui = attackPanel.GetComponent<AttackScript>();
            gui.attackbtn.enabled = false;
            gui.BattleDes.text = desciption;
            gui.PUnitNo.text = pUnits;
            gui.EUnitNo.text = eUnits;
            gui.PUnitValue = int.Parse(pUnits);
            gui.EUnitValue = int.Parse(eUnits);
            setDi();
        }
    }

    public void setDi()
    {
        AttackScript gui = attackPanel.GetComponent<AttackScript>();
        if (gui.PUnitValue == 1)
        {
            gui.OneDBtn.enabled = true;
            gui.TwoDBtn.enabled = false;
            gui.ThreeDBtn.enabled = false;
        }
        else if (gui.PUnitValue == 2)
        {
            gui.OneDBtn.enabled = true;
            gui.TwoDBtn.enabled = true;
            gui.ThreeDBtn.enabled = false;
        }
        else if (gui.PUnitValue > 2)
        {
            gui.OneDBtn.enabled = true;
            gui.TwoDBtn.enabled = true;
            gui.ThreeDBtn.enabled = true;
        }
        else
        {
            gui.OneDBtn.enabled = false;
            gui.TwoDBtn.enabled = false;
            gui.ThreeDBtn.enabled = false;
        }
    }

    public void OneDi()
    {
        AttackScript gui = attackPanel.GetComponent<AttackScript>();
        gui.DiceAmount = 1;
        gui.attackbtn.enabled = true;
        cleanAttackPanel();
    }
    public void TwoDi()
    {
        AttackScript gui = attackPanel.GetComponent<AttackScript>();
        gui.DiceAmount = 2;
        gui.attackbtn.enabled = true;
        cleanAttackPanel();
    }
    public void ThreeDi()
    {
        AttackScript gui = attackPanel.GetComponent<AttackScript>();
        gui.DiceAmount = 3;
        gui.attackbtn.enabled = true;
        cleanAttackPanel();
    }

    public void StopAttack()
    {
        foreach (var terr in territoryList)
        {
            territoryDict[terr.name].GetComponent<PolygonCollider2D>().enabled = true;
            territoryHandler terrHandler = territoryDict[terr.name].GetComponent<territoryHandler>();
            terrHandler.TintColor(terrHandler.oldColor);
        }
        cancelBtn.SetActive(false);
        attacking = false;
        cleanAttackPanel();
    }

    public void Attack()
    {
        System.Random rnd = new System.Random();
        AttackScript gui = attackPanel.GetComponent<AttackScript>();
        List<int> PDiArray = new List<int>();
        List<int> EDiArray = new List<int>();
        List<TMP_Text> pdTextArray = new List<TMP_Text>()
        { gui.PDOneTxt, gui.PDTwoTxt, gui.PDThreeTxt};
        List<TMP_Text> edTextArray = new List<TMP_Text>()
        { gui.EDOneTxt, gui.EDTwoTxt, gui.EDThreeTxt};
        int eDice;

        if (gui.EUnitValue > gui.DiceAmount)
        {
            eDice = gui.DiceAmount;
        }
        else if(gui.EUnitValue > 3)
        {
            eDice = 3;
        }
        else
        {
            eDice = gui.EUnitValue;
        }
        for (int i = 0; i < gui.DiceAmount; i++)
        {
            PDiArray.Add(rnd.Next(1, 7));
        }
        for (int i = 0; i < eDice; i++)
        {
            EDiArray.Add(rnd.Next(1, 7));
        }
        for (int i = 0; i < 3; i++)
        {
            if (i < PDiArray.Count)
            {
                pdTextArray[i].text = PDiArray[i].ToString();
            }
            else
            {
                pdTextArray[i].text = "N/A";
            }
            if (i < EDiArray.Count)
            {
                edTextArray[i].text = EDiArray[i].ToString();
            }
            else
            {
                edTextArray[i].text = "N/A";
            }
        }
        EDiArray.Sort();
        PDiArray.Sort();
        int comparisions = math.min(gui.DiceAmount, eDice);
        for (int i=0; i<comparisions; i++)
        {
            if (gui.EUnitValue > 0  && gui.PUnitValue > 0) 
            { 
                if (PDiArray[i] > EDiArray[i])
                {
                    gui.EUnitValue = gui.EUnitValue - 1;
                    gui.EUnitNo.text = (gui.EUnitValue).ToString();
                }
                else
                {
                    gui.PUnitValue = gui.PUnitValue - 1;
                    gui.PUnitNo.text = (gui.PUnitValue).ToString();
                }
            }
            setDi();
        }
        gui.attackbtn.enabled = false;
    }

    public void DisableAttackPanel()
    {
        attacking = false;
        gamePanel.SetActive(true);
        attackPanel.SetActive(false);
        AttackScript gui = attackPanel.GetComponent<AttackScript>();
        StopAttack();
    }

    public void cleanAttackPanel()
    {
        AttackScript gui = attackPanel.GetComponent<AttackScript>();
        List<TMP_Text> pdTextArray = new List<TMP_Text>()
        { gui.PDOneTxt, gui.PDTwoTxt, gui.PDThreeTxt};
        List<TMP_Text> edTextArray = new List<TMP_Text>()
        { gui.EDOneTxt, gui.EDTwoTxt, gui.EDThreeTxt};
        for (int i = 0; i < 3; i++)
        {
            pdTextArray[i].text = "N/A";
            edTextArray[i].text = "N/A";
        }
    }
}
