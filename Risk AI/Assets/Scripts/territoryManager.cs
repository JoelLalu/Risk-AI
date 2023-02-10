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
    public GameObject transferBtn;
    public TMP_Text playerTxt;
    public TMP_Text descriptionTxt;

    public List<GameObject> territoryList = new List<GameObject>();
    public Dictionary<string, GameObject> territoryDict = new Dictionary<string, GameObject>();

    public Dictionary<Territory.thePlayers, int> playerTroops = new Dictionary<Territory.thePlayers, int>()
    {
        {Territory.thePlayers.PLAYER1, 30},
        {Territory.thePlayers.PLAYER2, 30},
        {Territory.thePlayers.PLAYER3, 30},
        {Territory.thePlayers.PLAYER4, 30}
    };

    private Dictionary<string, List<string>> continentDict = new Dictionary<string, List<string>>()
    {
        { "Asia", new List<string> {"Middle East", "Afghanistan", "Ural", "Siberia", "Yakutsk", "Kamchatka",
                                    "Irkutsk", "China", "Mongolia", "Japan", "India", "Siam" } },
        { "North America",  new List<string> {"Alaska", "Northwest Territory", "Alberta", "Ontario", "Quebec",
                                              "Greenland", "Western US", "Eastern US", "Central America" } },
        { "Europe",  new List<string> { "Great Britain", "Iceland", "Western Europe", "Scandinavia", "Northern Europe",
                                        "Southern Europe", "Ukraine" } },
        { "Africa",  new List<string> { "North Africa", "Egypt", "Congo", "East Africa", "South Africa", "Madagascar" } },
        { "South America",  new List<string> { "Venezuela", "Brazil", "Peru", "Argentina"} },
        { "Australia",  new List<string> { "Indonesia", "New Guinea", "Western Australia", "Eastern Australia" } },
    };

    public bool attacking = false;
    public bool transferReady = false;
    public bool allClaimed;
    public GameObject transferTarget;

    public GameObject defender;
    public GameObject attacker;
    public GameState gameState = GameState.StartSelect;
    public Territory.thePlayers turn = Territory.thePlayers.PLAYER1;
    public int troopsleft = 0;

    public enum GameState
    {
        StartSelect,
        StartAssign,
        Fortify,
        Attack,
        Move,
    }

    public Dictionary<Territory.thePlayers, Territory.thePlayers> nextTurn = new Dictionary<Territory.thePlayers, Territory.thePlayers>()
    {
        { Territory.thePlayers.PLAYER1, Territory.thePlayers.PLAYER2 },
        { Territory.thePlayers.PLAYER2, Territory.thePlayers.PLAYER3 },
        { Territory.thePlayers.PLAYER3, Territory.thePlayers.PLAYER4 },
        { Territory.thePlayers.PLAYER4, Territory.thePlayers.PLAYER1 }
    };

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerTxt.text = "Player: " + turn.ToString();
        attackPanel.SetActive(false);
        AddTerritoryData();
        for (int i = 0; i < territoryList.Count; i++)
        {
            territoryHandler terrHandler = territoryList[i].GetComponent<territoryHandler>();
            terrHandler.territory.setPlayer(Territory.thePlayers.UNCLAIMED);
            terrHandler.setTroopNo(0);
            tintTerritories();
            cancelBtn.SetActive(false);
            transferBtn.SetActive(false);
        }

    }

    void AddTerritoryData()
    {
        GameObject[] theArray = GameObject.FindGameObjectsWithTag("Territory") as GameObject[];
        foreach (GameObject territory in theArray)
        {
            territoryList.Add(territory);
            territoryDict.Add(territory.name, territory);
        }
    }

    public void tintTerritories()
    {
        System.Random rnd = new System.Random();
        for (int i = 0;i<territoryList.Count; i++)
        {
            tintTerritory(territoryList[i]);
        }

    }

    public void tintTerritory(GameObject terr)
    {
        territoryHandler terrHandler = terr.GetComponent<territoryHandler>();
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.UNCLAIMED)
        {
            terrHandler.TintColor(new Color32(0, 128, 19, 225));
            terrHandler.oldColor = new Color32(0, 128, 19, 225);
        }
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.PLAYER1)
        {
            terrHandler.TintColor(new Color32(127, 255, 212, 225));
            terrHandler.oldColor = new Color32(127, 255, 212, 225);
        }
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.PLAYER2)
        {
            terrHandler.TintColor(new Color32(65, 105, 225, 225));
            terrHandler.oldColor = new Color32(65, 105, 225, 225);
        }
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.PLAYER3)
        {
            terrHandler.TintColor(new Color32(178, 34, 34, 255));
            terrHandler.oldColor = new Color32(178, 34, 34, 255);
        }
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.PLAYER4)
        {
            terrHandler.TintColor(new Color32(255, 20, 147, 255));
            terrHandler.oldColor = new Color32(255, 20, 147, 255);
        }
    }

    public bool isContinentOwned(string continent, Territory.thePlayers player)
    {
        bool continentNotOwned = false;
        List<string> countryList = continentDict[continent];
        foreach (var terr in countryList)
        {
            territoryHandler terrHandler = territoryDict[terr].GetComponent<territoryHandler>();
            if (continent == "Asia")
            {
                terrHandler.TintColor(new Color32(0, 128, 19, 225));
                terrHandler.oldColor = new Color32(0, 128, 19, 225);
            }
            if (continent == "North America")
            {
                terrHandler.TintColor(new Color32(127, 255, 212, 225));
                terrHandler.oldColor = new Color32(127, 255, 212, 225);
            }
            if (continent == "Europe")
            {
                terrHandler.TintColor(new Color32(65, 105, 225, 225));
                terrHandler.oldColor = new Color32(65, 105, 225, 225);
            }
            if (continent == "Africa")
            {
                terrHandler.TintColor(new Color32(178, 34, 34, 255));
                terrHandler.oldColor = new Color32(178, 34, 34, 255);
            }
            if (continent == "South America")
            {
                terrHandler.TintColor(new Color32(255, 20, 147, 255));
                terrHandler.oldColor = new Color32(255, 20, 147, 255);
            }
            if (continent == "Australia")
            {
                terrHandler.TintColor(new Color32(255, 0, 0, 0));
                terrHandler.oldColor = new Color32(255, 0, 0, 0);
            }
            if (territoryDict[terr].GetComponent<territoryHandler>().territory.getPlayer() != player)
            {
                continentNotOwned = true;
            }
        }
        return continentNotOwned;
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
            if (terrHandler.territory.getPlayer() != territoryDict[selected].GetComponent<territoryHandler>().territory.getPlayer())
            {
                territoryDict[terrName].GetComponent<PolygonCollider2D>().enabled = true;
            }
        }
    }

    public void ShowAttackPanel(string desciption, string pUnits, string eUnits, string eName)
    {
        if (attacking == true) 
        {
            attackPanel.SetActive(true);
            gamePanel.SetActive(false);
            AttackScript gui = attackPanel.GetComponent<AttackScript>();
            defender = territoryDict[eName];
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
        if (gui.PUnitValue == 2)
        {
            gui.OneDBtn.enabled = true;
            gui.TwoDBtn.enabled = false;
            gui.ThreeDBtn.enabled = false;
        }
        else if (gui.PUnitValue == 3)
        {
            gui.OneDBtn.enabled = true;
            gui.TwoDBtn.enabled = true;
            gui.ThreeDBtn.enabled = false;
        }
        else if (gui.PUnitValue > 3)
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

    public void StopAction()
    {
        if (gameState == GameState.StartSelect)
        {
            territoryHandler terrHandler = transferTarget.GetComponent<territoryHandler>();
            terrHandler.setTroopNo(terrHandler.territory.troops - 1);
            if (terrHandler.territory.troops == 0)
            {
                terrHandler.territory.setPlayer(Territory.thePlayers.UNCLAIMED);
                tintTerritory(transferTarget);
            }
            cancelBtn.SetActive(false);
            transferBtn.SetActive(false);
            showAvailable();
        }
        else if (gameState == GameState.StartAssign)
        {
            territoryHandler terrHandler = transferTarget.GetComponent<territoryHandler>();
            terrHandler.setTroopNo(terrHandler.territory.troops - 1);
            foreach (var terr in territoryList)
            {
                if (terr.GetComponent<territoryHandler>().territory.getPlayer() == turn)
                {
                    enableTerritory(terr);
                    terr.GetComponent<territoryHandler>().setTroopNo(1);
                }
            }
            troopsleft = playerTroops[turn];
            cancelBtn.SetActive(false);
            transferBtn.SetActive(false);
        }
        else if (gameState == GameState.Fortify)
        {
            print("hi");
        }
        else if (gameState == GameState.Attack)
        {
            foreach (var terr in territoryList)
            {
                enableTerritory(terr);
            }
            cancelBtn.SetActive(false);
            attacking = false;
            cleanAttackPanel();
        }
    }

    public void enableTerritory(GameObject terr)
    {
        terr.GetComponent<PolygonCollider2D>().enabled = true;
        territoryHandler terrHandler = terr.GetComponent<territoryHandler>();
        terrHandler.TintColor(terrHandler.oldColor);
    }

    public void disableTerritory(GameObject terr)
    {
        terr.GetComponent<PolygonCollider2D>().enabled = false;
        territoryHandler terrHandler = terr.GetComponent<territoryHandler>();
        //terrHandler.TintColor
        //terrHandler.TintColor(terrHandler.oldColor = );
    }

    public int getTerrtoriesOwnedNo(Territory.thePlayers playername)
    {
        int amount = 0;
        foreach (var terr in territoryList)
        {
            Territory.thePlayers terrPlayer = terr.GetComponent<territoryHandler>().territory.getPlayer();
            if (terrPlayer == playername)
            {
                amount += 1;
            }
        }
        return amount;
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
        territoryHandler pScript = attacker.GetComponent<territoryHandler>();
        territoryHandler eScript = defender.GetComponent<territoryHandler>();

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
        EDiArray.Reverse();
        PDiArray.Sort();
        PDiArray.Reverse();
        int comparisions = math.min(gui.DiceAmount, eDice);
        for (int i=0; i<comparisions; i++)
        {
            if (gui.EUnitValue > 0  && gui.PUnitValue > 1) 
            { 
                if (PDiArray[i] > EDiArray[i])
                {
                    gui.EUnitValue = gui.EUnitValue - 1;
                    gui.EUnitNo.text = (gui.EUnitValue).ToString();
                    eScript.setTroopNo(gui.EUnitValue);

                }
                else
                {
                    gui.PUnitValue = gui.PUnitValue - 1;
                    gui.PUnitNo.text = (gui.PUnitValue).ToString();
                    pScript.setTroopNo(gui.PUnitValue);
                }
            }
            setDi();
        }
        if (gui.PUnitValue == 1)
        {
            DisableAttackPanel();
        }
        if (gui.EUnitValue == 0)
        {
            eScript.territory.setPlayer(pScript.territory.getPlayer());
            eScript.oldColor = pScript.oldColor;
            eScript.hoverColor = eScript.oldColor;
            eScript.setTroopNo(pScript.territory.troops - 1);
            pScript.setTroopNo(1);
            transferReady = true;
            transferBtn.SetActive(true);
            transferTroops();
        }

        gui.attackbtn.enabled = false;
    }

    public void FinishTransfer()
    {
        if (gameState == GameState.StartSelect)
        {
            playerTroops[turn] -= 1;
            turn = nextTurn[turn];
            playerTxt.text = "Player: " + turn.ToString();
            foreach (var terr in territoryList)
            {
                enableTerritory(terr);
            }
            cancelBtn.SetActive(false);
            transferBtn.SetActive(false);
            showAvailable();
            if (getTerrtoriesOwnedNo(Territory.thePlayers.UNCLAIMED) == 0)
            {
                print("GameState Changed!");
                gameState = GameState.StartAssign;
                troopsleft = playerTroops[turn];
                foreach (var terr in territoryList)
                {
                    if (terr.GetComponent<territoryHandler>().territory.getPlayer() == turn)
                    {
                        enableTerritory(terr);
                    }
                }
            }
        }
        else if (gameState == GameState.StartAssign)
        {
            print("turn: " + turn);
            playerTroops[turn]  = troopsleft;
            turn = nextTurn[turn];
            troopsleft = playerTroops[turn];
            playerTxt.text = "Player: " + turn.ToString();
            foreach (var terr in territoryList)
            {
                if (terr.GetComponent<territoryHandler>().territory.getPlayer() == turn)
                {
                    enableTerritory(terr);
                }
                else
                {
                    disableTerritory(terr);
                }
            }
            cancelBtn.SetActive(false);
            transferBtn.SetActive(false);
            print("troopsleft" + troopsleft);
            print("player" + turn);
            print("playerTroops" + playerTroops[turn]);
            bool noTroopsLeft = true;
            foreach (var (key, value) in playerTroops)
            {
                if (value != 0)
                {
                    noTroopsLeft = false;
                }
            }
            if (noTroopsLeft)
            {
                print("GameState Changed!");
                turn = Territory.thePlayers.PLAYER4;
                gameState = GameState.Fortify;
            }
        }
        else if(gameState == GameState.Fortify)
        {
                isContinentOwned("Asia", turn);
                isContinentOwned("North America", turn);
                isContinentOwned("Europe", turn);
                isContinentOwned("Africa", turn);
                isContinentOwned("South America", turn);
                isContinentOwned("Australia", turn);
        }
        else if(gameState == GameState.Attack)
        {
            foreach (var terr in territoryList)
            {
                enableTerritory(terr);
            }
            transferReady = false;
            transferBtn.SetActive(false);
        }
    }

    public void showAvailable()
    {
        foreach (var terr in territoryList)
        {
            Territory.thePlayers terrPlayer = terr.GetComponent<territoryHandler>().territory.getPlayer();
            if (terr.GetComponent<territoryHandler>().territory.getPlayer() == Territory.thePlayers.UNCLAIMED)
            {
                territoryManager.instance.enableTerritory(terr);
            }
            else
            {
                territoryManager.instance.disableTerritory(terr);
            }
        }
    }

    public void DisableAttackPanel()
    {
        attacking = false;
        gamePanel.SetActive(true);
        attackPanel.SetActive(false);
        StopAction();
    }

    public void transferTroops()
    {
        attacking = false;
        gamePanel.SetActive(true);
        attackPanel.SetActive(false);
        string attackerName = attacker.GetComponent<territoryHandler>().territory.name;
        string defenderName = defender.GetComponent<territoryHandler>().territory.name;
        print(attackerName);
        print(defenderName);
        foreach (var terr in territoryList)
        {
            disableTerritory(terr);
        }
        enableTerritory(territoryDict[attackerName]);
        enableTerritory(territoryDict[defenderName]);
        cancelBtn.SetActive(false);
        attacking = false;
        cleanAttackPanel();
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
