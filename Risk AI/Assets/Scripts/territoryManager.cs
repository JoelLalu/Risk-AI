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
    public GameObject NextPhaseBtn;
    public TMP_Text playerTxt;
    public TMP_Text descriptionTxt;
    public bool terrSelected = false;
    public bool gameover = false;

    public List<GameObject> territoryList = new List<GameObject>();
    public Dictionary<string, GameObject> territoryDict = new Dictionary<string, GameObject>();

    public Dictionary<Territory.thePlayers, int> playerTroops = new Dictionary<Territory.thePlayers, int>()
    {
        {Territory.thePlayers.PLAYER1, 30},
        {Territory.thePlayers.PLAYER2, 30},
        {Territory.thePlayers.PLAYER3, 30},
        {Territory.thePlayers.PLAYER4, 30}
    };
    
    private Dictionary<Territory.thePlayers, List<string>> playerTerritories = new Dictionary<Territory.thePlayers, List<string>>()
    {
        {Territory.thePlayers.PLAYER1, new List<string>()},
        {Territory.thePlayers.PLAYER2, new List<string>()},
        {Territory.thePlayers.PLAYER3, new List<string>()},
        {Territory.thePlayers.PLAYER4, new List<string>()},
    };

    public void addTerrToPlayer(Territory.thePlayers player, string territoryName)
    {
        playerTerritories[player].Add(territoryName);
    }

    public void removeTerrToPlayer(Territory.thePlayers player, string territoryName)
    {
        playerTerritories[player].Remove(territoryName);
    }

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
    public GamePhase gamePhase = GamePhase.Assign;
    public Territory.thePlayers turn = Territory.thePlayers.PLAYER1;

    public Dictionary<string, int> mapState = new  Dictionary<string, int>(){};
    public int troopsleft = 0;

    public List<Territory.thePlayers> playerList =  new List<Territory.thePlayers> 
        {Territory.thePlayers.PLAYER1,
         Territory.thePlayers.PLAYER2,
         Territory.thePlayers.PLAYER3,
         Territory.thePlayers.PLAYER4};

    public enum GameState
    {
        StartSelect,
        StartAssign,
        MainState,
    }

    public enum GamePhase
    {
        Assign,
        Attack,
        Fortify
    }

    public Dictionary<Territory.thePlayers, Territory.thePlayers> nextTurn = new Dictionary<Territory.thePlayers, Territory.thePlayers>()
    {
        { Territory.thePlayers.PLAYER1, Territory.thePlayers.PLAYER2 },
        { Territory.thePlayers.PLAYER2, Territory.thePlayers.PLAYER3 },
        { Territory.thePlayers.PLAYER3, Territory.thePlayers.PLAYER4 },
        { Territory.thePlayers.PLAYER4, Territory.thePlayers.PLAYER1 }
    };
    public Dictionary<Territory.thePlayers, Color32> nextTurnColor = new Dictionary<Territory.thePlayers, Color32>()
    {
        { Territory.thePlayers.PLAYER1, new Color32(127, 255, 212, 225) },
        { Territory.thePlayers.PLAYER2, new Color32(65, 105, 225, 225) },
        { Territory.thePlayers.PLAYER3, new Color32(178, 34, 34, 255) },
        { Territory.thePlayers.PLAYER4, new Color32(255, 20, 147, 255) }
    };

    public void removePlayer(Territory.thePlayers player)
    {
        if (playerList.Count ==1)
        {
            print("no more players");
        }
        else if ((playerList.Count ==2))
        {
            playerList.Remove(player);
            print(playerList[0] + " you won");
            gameover = true;           
        }
        else{
            playerList.Remove(player);
        }
        var dictCorrection = new Dictionary<Territory.thePlayers, Territory.thePlayers>();
        for (int i = 0; i < playerList.Count; i++)
        {
            if(i==playerList.Count -1)
            {
                dictCorrection.Add(playerList[i], playerList[0]);
            }
            else
            {
                dictCorrection.Add(playerList[i], playerList[i+1]);
            }
        }
        nextTurn = dictCorrection;
        foreach (var (key, value) in nextTurn)
        {
                print(key + "test" + value);
        }
    }

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
            cancelBtn.SetActive(false);
            transferBtn.SetActive(false);
        }
        tintTerritories();
        gameState = GameState.MainState;
        gamePhase = GamePhase.Assign;
        foreach (var terr in territoryList)
        {
            territoryHandler terrhandler = terr.GetComponent<territoryHandler>();
            int playerNum = UnityEngine.Random.Range(1,5);
            int troopNum = UnityEngine.Random.Range(1,7);
            terrhandler.setTroopNo(troopNum);
            Dictionary<int, Territory.thePlayers> playerDict  = new Dictionary<int, Territory.thePlayers>()
            {
                {1, Territory.thePlayers.PLAYER1},
                {2, Territory.thePlayers.PLAYER2},
                {3, Territory.thePlayers.PLAYER2},
                {4, Territory.thePlayers.PLAYER2}
            };
            terrhandler.territory.setPlayer(playerDict[playerNum]);
            addTerrToPlayer(playerDict[playerNum], terrhandler.territory.name);
        }
        playerTerritories.Remove(Territory.thePlayers.PLAYER3);
        playerTerritories.Remove(Territory.thePlayers.PLAYER4);
        removePlayer(Territory.thePlayers.PLAYER3);
        removePlayer(Territory.thePlayers.PLAYER4);
        eanblePlayerTerr(Territory.thePlayers.PLAYER1);
        tintTerritories();
        turn = Territory.thePlayers.PLAYER1;
        playerTxt.text = "Player: " + turn.ToString();
        gameState = GameState.MainState;
        gamePhase = GamePhase.Assign;
        playerTroops[turn] = newTroopAmount(turn);
        troopsleft = playerTroops[turn];
        mapState = saveMapState();
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
            terrHandler.disabledColor = terrHandler.oldColor;
            terrHandler.disabledColor.a = 30;
        }
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.PLAYER1)
        {
            terrHandler.TintColor(new Color32(127, 255, 212, 225));
            terrHandler.oldColor = new Color32(127, 255, 212, 225);
            terrHandler.disabledColor = terrHandler.oldColor;
            terrHandler.disabledColor.a = 30;
        }
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.PLAYER2)
        {
            terrHandler.TintColor(new Color32(65, 105, 225, 225));
            terrHandler.oldColor = new Color32(65, 105, 225, 225);
            terrHandler.disabledColor = terrHandler.oldColor;
            terrHandler.disabledColor.a = 30;
        }
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.PLAYER3)
        {
            terrHandler.TintColor(new Color32(178, 34, 34, 255));
            terrHandler.oldColor = new Color32(178, 34, 34, 255);
            terrHandler.disabledColor = terrHandler.oldColor;
            terrHandler.disabledColor.a = 30;
        }
        if (terrHandler.territory.getPlayer() == Territory.thePlayers.PLAYER4)
        {
            terrHandler.TintColor(new Color32(255, 20, 147, 255));
            terrHandler.oldColor = new Color32(255, 20, 147, 255);
            terrHandler.disabledColor = terrHandler.oldColor;
            terrHandler.disabledColor.a = 30;
        }
    }

    public bool isContinentOwned(string continent, Territory.thePlayers player)
    {
        bool continentNotOwned = true;
        List<string> countryList = continentDict[continent];
        foreach (var terr in countryList)
        {
            territoryHandler terrHandler = territoryDict[terr].GetComponent<territoryHandler>();
            if (continentDict[continent].Contains(terrHandler.territory.name))
            {
                if (territoryDict[terr].GetComponent<territoryHandler>().territory.getPlayer() != player)
                {
                    continentNotOwned = false;
                }
            }
        }
        return continentNotOwned;
    }

    public void ShowTargets(string selected)
    {
        Dictionary<string, List<string>> attackDict = new Dictionary<string, List<string>>() 
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
            disableTerritory(terr);
            territoryHandler terrHandler = terr.GetComponent<territoryHandler>();
            terrHandler.TintColor(terrHandler.disabledColor);
        }
        foreach (var terrName in surroundTers) 
        {
            territoryHandler terrHandler = territoryDict[terrName].GetComponent<territoryHandler>();
            terrHandler.TintColor(terrHandler.oldColor);
            if (terrHandler.territory.getPlayer() != territoryDict[selected].GetComponent<territoryHandler>().territory.getPlayer())
            {
                enableTerritory(territoryDict[terrName]);
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

    public Dictionary<string, int> saveMapState()
    {
        Dictionary<string, int> tempMap = new Dictionary<string, int>();
        foreach (var terr in territoryList)
        {
            tempMap[terr.GetComponent<territoryHandler>().territory.name] = terr.GetComponent<territoryHandler>().territory.troops;
        }
        return tempMap;
    }

    public void restoreMapState(Dictionary<string, int> mapstate)
    {
        foreach (var (key, value) in mapstate)
        {
            territoryDict[key].GetComponent<territoryHandler>().setTroopNo(value);
        }
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
        else if (gameState == GameState.MainState)
        {
            if (gamePhase == GamePhase.Assign)
            {
                restoreMapState(mapState);
                eanblePlayerTerr(turn);
                troopsleft = playerTroops[turn];
                cancelBtn.SetActive(false);
                transferBtn.SetActive(false);
            }
            else if (gamePhase == GamePhase.Attack)
            {
                eanbleAttackPlayerTerr(turn);
                cancelBtn.SetActive(false);
                NextPhaseBtn.SetActive(true);
                attacking = false;
                tintTerritories();
                cleanAttackPanel();
                gamePanel.SetActive(true);
                attackPanel.SetActive(false);
            }
            else if(gamePhase == GamePhase.Fortify)
            {
                restoreMapState(mapState);
                terrSelected = false;
                transferReady = false;
                eanblePlayerTerr(turn);
                troopsleft = playerTroops[turn];
                cancelBtn.SetActive(false);
                transferBtn.SetActive(false);
                NextPhaseBtn.SetActive(true);
                tintTerritories();
                foreach (var terr in territoryList)
                {
                    disableTerritory(terr);
                    territoryHandler terrHandler = terr.GetComponent<territoryHandler>();
                    terrHandler.TintColor(terrHandler.disabledColor);
                    if (terrHandler.territory.getPlayer() == turn && terrHandler.territory.troops > 1)
                    {
                        enableTerritory(terr);
                    }
                }
            }
        }
    }

    public void eanblePlayerTerr(Territory.thePlayers player)
    {
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
    }

    public void eanbleAttackPlayerTerr(Territory.thePlayers player)
    {
        foreach (var terr in territoryList)
        {
            territoryHandler terrhandler = terr.GetComponent<territoryHandler>();
            if (terrhandler.territory.getPlayer() == turn && terrhandler.territory.troops >1)
            {
                enableTerritory(terr);
            }
            else
            {
                disableTerritory(terr);
            }
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
        // terrHandler.TintColor(terrHandler.disabledColor);
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
            removeTerrToPlayer(eScript.territory.getPlayer(), eScript.territory.name);
            List<Territory.thePlayers> removals = new List<Territory.thePlayers>();                    
            foreach (var key in playerTerritories.Keys)
            {
                if(playerTerritories[key].Count == 0){
                    print(key + "test");
                    print(string.Format("Here's the list: ({0}).", string.Join(", ", playerTerritories[key])));
                    removals.Add(eScript.territory.getPlayer());
                }
            }
            foreach(var person in removals)
            {
                removePlayer(person);
                playerTerritories.Remove(person);
            }
            if (gameover) 
            {
                eScript.territory.setPlayer(pScript.territory.getPlayer());
                addTerrToPlayer(pScript.territory.getPlayer(), eScript.territory.name);
                eScript.oldColor = pScript.oldColor;
                eScript.hoverColor = eScript.oldColor;
                eScript.setTroopNo(pScript.territory.troops - 1);
                pScript.setTroopNo(1);
                transferReady = true;
                transferBtn.SetActive(true);
                string attackerName = attacker.GetComponent<territoryHandler>().territory.name;
                string defenderName = defender.GetComponent<territoryHandler>().territory.name;
                attacking = false;
                tintTerritories();
                cleanAttackPanel();
                gamePanel.SetActive(true);
                attackPanel.SetActive(false);
                descriptionTxt.text = playerList[0] + " has Won";
                foreach (var terr in territoryList)
                {
                    disableTerritory(terr);
                }
                
                cancelBtn.SetActive(false);
                transferBtn.SetActive(false);
                NextPhaseBtn.SetActive(false);
            }
            else
            {
                eScript.territory.setPlayer(pScript.territory.getPlayer());
                addTerrToPlayer(pScript.territory.getPlayer(), eScript.territory.name);
                eScript.oldColor = pScript.oldColor;
                eScript.hoverColor = eScript.oldColor;
                eScript.setTroopNo(pScript.territory.troops - 1);
                pScript.setTroopNo(1);
                transferReady = true;
                transferBtn.SetActive(true);
                string attackerName = attacker.GetComponent<territoryHandler>().territory.name;
                string defenderName = defender.GetComponent<territoryHandler>().territory.name;
                transferTroops(attackerName, defenderName);                
            }
        }

        gui.attackbtn.enabled = false;
    }


    //TODO: Alter the instruction text with changes
    public void FinishTransfer()
    {
        if (gameState == GameState.StartSelect)
        {
            addTerrToPlayer(turn, transferTarget.GetComponent<territoryHandler>().territory.name);
            playerTroops[turn] -= 1;
            turn = nextTurn[turn];
            playerTxt.color = nextTurnColor[turn];
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
                gameState = GameState.StartAssign;
                turn = Territory.thePlayers.PLAYER1;
                playerTxt.text = "Player: " + turn.ToString();
                troopsleft = playerTroops[turn];
                eanblePlayerTerr(turn);
            }
        }
        else if (gameState == GameState.StartAssign)
        {
            playerTroops[turn]  = troopsleft;
            turn = nextTurn[turn];
            troopsleft = playerTroops[turn];
            playerTxt.color = nextTurnColor[turn];
            playerTxt.text = "Player: " + turn.ToString();
            eanblePlayerTerr(turn);
            cancelBtn.SetActive(false);
            transferBtn.SetActive(false);
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
                turn = Territory.thePlayers.PLAYER1;
                playerTxt.text = "Player: " + turn.ToString();
                descriptionTxt.text = "Assign you troops to prepare to attack";
                gameState = GameState.MainState;
                gamePhase = GamePhase.Assign;
                playerTroops[turn] = newTroopAmount(turn);
                troopsleft = playerTroops[turn];
                mapState = saveMapState();
            }
        }
        else if(gameState == GameState.MainState)
        {
            if(gamePhase == GamePhase.Assign)
            {
                playerTroops[turn] = troopsleft;
                cancelBtn.SetActive(false);
                transferBtn.SetActive(false);
                descriptionTxt.text = "Select you territory then a territory to attack";
                NextPhaseBtn.SetActive(true);
                gamePhase = GamePhase.Attack;
                print(gamePhase);
                eanbleAttackPlayerTerr(turn);
                tintTerritories();
            }
            else if(gamePhase == GamePhase.Attack)
            {
                eanbleAttackPlayerTerr(turn);
                transferReady = false;
                transferBtn.SetActive(false);
                NextPhaseBtn.SetActive(true);
                tintTerritories();
            }
            else if(gamePhase == GamePhase.Fortify)
            {
                nextPhase();
            }
        }
    }

    public void nextPhase()
    {
        if (gamePhase == GamePhase.Attack)
        {
            descriptionTxt.text = "Select territory to Fortify";
            gamePhase = GamePhase.Fortify;
            eanblePlayerTerr(turn);
            foreach (var terr in territoryList)
            {
                disableTerritory(terr);
                territoryHandler terrHandler = terr.GetComponent<territoryHandler>();
                terrHandler.TintColor(terrHandler.disabledColor);
                if (terrHandler.territory.getPlayer() == turn && terrHandler.territory.troops > 1)
                {
                    enableTerritory(terr);
                }
            }
            mapState = saveMapState();

        }
        else if (gamePhase == GamePhase.Fortify)
        {
            descriptionTxt.text = "Assign you troops to prepare to attack";
            turn = nextTurn[turn];
            playerTxt.color = nextTurnColor[turn];
            playerTxt.text = "Player: " + turn.ToString();
            eanblePlayerTerr(turn);
            terrSelected = false;
            transferReady = false;
            transferBtn.SetActive(false);
            cancelBtn.SetActive(false);
            NextPhaseBtn.SetActive(false);
            gamePhase = GamePhase.Assign;
            playerTroops[turn] = newTroopAmount(turn);
            troopsleft = playerTroops[turn];
            mapState = saveMapState();
            tintTerritories();
        }
    }

    public int newTroopAmount(Territory.thePlayers player)
    {
        int troopsAmount = 0; 
        if (isContinentOwned("Asia", player))
        {
            troopsAmount += 7;
        }
        if (isContinentOwned("North America", player))
        {
            troopsAmount += 5;
        }
        if (isContinentOwned("Europe", player))
        {
            troopsAmount += 5;   
        }
        if (isContinentOwned("Africa", player))
        {
            troopsAmount += 4;
        }
        if (isContinentOwned("South America", player))
        {
            troopsAmount += 2;
        }
        if (isContinentOwned("Australia", player))
        {
            troopsAmount += 2;
        }
        troopsAmount += playerTerritories[player].Count/3;

        if (troopsAmount < 3)
        {
            troopsAmount = 3;
        }
        return troopsAmount;
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

    public void transferTroops(string TerritoryA, string TerritoryB)
    {
        attacking = false;
        gamePanel.SetActive(true);
        attackPanel.SetActive(false);
        foreach (var terr in territoryList)
        {
            disableTerritory(terr);
            terr.GetComponent<territoryHandler>().TintColor(terr.GetComponent<territoryHandler>().disabledColor);
        }
        enableTerritory(territoryDict[TerritoryA]);
        enableTerritory(territoryDict[TerritoryB]);
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
