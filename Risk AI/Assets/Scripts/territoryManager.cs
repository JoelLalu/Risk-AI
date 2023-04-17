using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System.Linq;


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

    public bool AIGameStart = false;
    public bool startSelect = false;
    public bool startAssign = false;
    public bool assignTroopsAI = false;
    public bool startAttackAI = false;
    public bool fortifyTerritoriesAI = false;

    Dictionary<Territory.thePlayers, string> PlayerTypeDict = new Dictionary<Territory.thePlayers, string>()
    {
        {Territory.thePlayers.PLAYER1, "player"},
        {Territory.thePlayers.PLAYER2, "ai"},
        {Territory.thePlayers.PLAYER3, "ai"},
        {Territory.thePlayers.PLAYER4, "ai"}
    };

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
        // print("added " + territoryName + " to " + player);
    }

    public void removeTerrFromPlayer(Territory.thePlayers player, string territoryName)
    {
        playerTerritories[player].Remove(territoryName);
        // print("removed " + territoryName + " from " + player);
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
            PlayerTypeDict.Remove(player);
            print(playerList[0] + " you won");
            gameover = true;           
        }
        else{
            PlayerTypeDict.Remove(player);
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
    }

    void Update()
    {
        if (!gameover)
        {
            if (AIGameStart){
                AIGameStart = false;
                startSelect = true;
            }
            else if (startSelect)
            {
                startSelect = false;
                StartCoroutine(AIStartSelect());
            }
            else if (startAssign)
            {
                startAssign = false;
                StartCoroutine(AIStartAssign());
            }
            else if (assignTroopsAI) {
                assignTroopsAI = false;
                print("Assign Phase");
                StartCoroutine(AIAsignLogic());
            }
            else if (startAttackAI) {
                startAttackAI = false;
                print("Attack Phase");
                StartCoroutine(AIAttackLogic());
            }      
            else if (fortifyTerritoriesAI) {
                fortifyTerritoriesAI = false;
                print("Fortify Phase");
                StartCoroutine(AIFortifyLogic());
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        playerTxt.text = "Player: " + turn.ToString();
        attackPanel.SetActive(false);
        AddTerritoryData();
        //start code
        if(currentScene.name == "Game")
        {
            //code to generate game states at start of run
            for (int i = 0; i < territoryList.Count; i++)
            {
                territoryHandler terrHandler = territoryList[i].GetComponent<territoryHandler>();
                terrHandler.territory.setPlayer(Territory.thePlayers.UNCLAIMED);
                terrHandler.setTroopNo(0);
                cancelBtn.SetActive(false);
                transferBtn.SetActive(false);
            }
            tintTerritories();
            // gameState = GameState.MainState;
            // gamePhase = GamePhase.Assign;
            // foreach (var terr in territoryList)
            // {
            //     territoryHandler terrhandler = terr.GetComponent<territoryHandler>();
            //     int playerNum = UnityEngine.Random.Range(1,5);
            //     int troopNum = UnityEngine.Random.Range(1,7);
            //     terrhandler.setTroopNo(troopNum);
            //     Dictionary<int, Territory.thePlayers> playerDict  = new Dictionary<int, Territory.thePlayers>()
            //     {
            //         {1, Territory.thePlayers.PLAYER1},
            //         {2, Territory.thePlayers.PLAYER2},
            //         {3, Territory.thePlayers.PLAYER3},
            //         {4, Territory.thePlayers.PLAYER4}
            //     };
            //     terrhandler.territory.setPlayer(playerDict[playerNum]);
            //     addTerrToPlayer(playerDict[playerNum], terrhandler.territory.name);
            // }
            // playerTerritories.Remove(Territory.thePlayers.PLAYER3);
            // playerTerritories.Remove(Territory.thePlayers.PLAYER4);
            // eanblePlayerTerr(Territory.thePlayers.PLAYER1);
            // tintTerritories();
            turn = Territory.thePlayers.PLAYER1;
            playerTxt.text = "Player: " + turn.ToString();
            // gameState = GameState.MainState;
            // gamePhase = GamePhase.Assign;
            // playerTroops[turn] = newTroopAmount(turn);
            // troopsleft = playerTroops[turn];
            mapState = saveMapState();  
        }
        else if (currentScene.name == "GameAI")
        {
            for (int i = 0; i < territoryList.Count; i++)
            {
                territoryHandler terrHandler = territoryList[i].GetComponent<territoryHandler>();
                terrHandler.territory.setPlayer(Territory.thePlayers.UNCLAIMED);
                terrHandler.setTroopNo(0);
                cancelBtn.SetActive(false);
                transferBtn.SetActive(false);
            }
            // foreach (var terr in territoryList)
            // {
            //     territoryHandler terrhandler = terr.GetComponent<territoryHandler>();
            //     int playerNum = UnityEngine.Random.Range(1,5);
            //     int troopNum = UnityEngine.Random.Range(1,7);
            //     terrhandler.setTroopNo(troopNum);
            //     Dictionary<int, Territory.thePlayers> playerDict  = new Dictionary<int, Territory.thePlayers>()
            //     {
            //         {1, Territory.thePlayers.PLAYER1},
            //         {2, Territory.thePlayers.PLAYER2},
            //         {3, Territory.thePlayers.PLAYER3},
            //         {4, Territory.thePlayers.PLAYER4}
            //     };
            //     terrhandler.territory.setPlayer(playerDict[playerNum]);
            //     addTerrToPlayer(playerDict[playerNum], terrhandler.territory.name);
            //     disableTerritory(terr);
            // }
            // removePlayer(Territory.thePlayers.PLAYER3);
            // removePlayer(Territory.thePlayers.PLAYER4);
            // playerTerritories.Remove(Territory.thePlayers.PLAYER3);
            // playerTerritories.Remove(Territory.thePlayers.PLAYER4);
            foreach(var terr in territoryList)
            {
                disableTerritory(terr);
            }
            turn = Territory.thePlayers.PLAYER1;
            playerTxt.text = "Player: " + turn.ToString();
            gameState = GameState.StartSelect;
            gamePhase = GamePhase.Assign;
            troopsleft = playerTroops[turn];
            tintTerritories();
            AIGameStart = true;
        }
        else if (currentScene.name == "PlayerVsAI")
        {
            //code to generate game states at start of run
            for (int i = 0; i < territoryList.Count; i++)
            {
                territoryHandler terrHandler = territoryList[i].GetComponent<territoryHandler>();
                terrHandler.territory.setPlayer(Territory.thePlayers.UNCLAIMED);
                terrHandler.setTroopNo(0);
                cancelBtn.SetActive(false);
                transferBtn.SetActive(false);
                enableTerritory(territoryList[i]);
            }
            turn = Territory.thePlayers.PLAYER1;
            playerTxt.text = "Player: " + turn.ToString();
            tintTerritories();
            //autoselect terrs
            // foreach (var terr in territoryList)
            // {
            //     territoryHandler terrhandler = terr.GetComponent<territoryHandler>();
            //     int playerNum = UnityEngine.Random.Range(1,5);
            //     int troopNum = UnityEngine.Random.Range(1,7);
            //     terrhandler.setTroopNo(troopNum);
            //     Dictionary<int, Territory.thePlayers> playerDict  = new Dictionary<int, Territory.thePlayers>()
            //     {
            //         {1, Territory.thePlayers.PLAYER1},
            //         {2, Territory.thePlayers.PLAYER2},
            //         {3, Territory.thePlayers.PLAYER3},
            //         {4, Territory.thePlayers.PLAYER4}
            //     };
            //     terrhandler.territory.setPlayer(playerDict[playerNum]);
            //     addTerrToPlayer(playerDict[playerNum], terrhandler.territory.name);
            // }
            // gameState = GameState.MainState;
            // gamePhase = GamePhase.Assign;
            // playerTroops[turn] = newTroopAmount(turn);
            // troopsleft = playerTroops[turn];
            if(PlayerTypeDict[turn] == "ai")
            {
                AIGameStart = true;
            }
            mapState = saveMapState();  
        }
    }

    
    IEnumerator AIStartSelect()
    {
        bool aiNext = true;
        yield return new WaitForSeconds(1);
        Scene currentScene = SceneManager.GetActiveScene();
        print(gameState);
        if (gameState == GameState.StartSelect)
        {
            foreach (var terr in territoryList)
            {
                print("test");
                territoryHandler terrScript = terr.GetComponent<territoryHandler>();
                if (terrScript.territory.getPlayer() == Territory.thePlayers.UNCLAIMED)
                {
                    terrScript.territory.setPlayer(turn);
                    terrScript.setTroopNo(terrScript.territory.troops + 1);
                    tintTerritory(terr);
                    addTerrToPlayer(turn, terrScript.GetComponent<territoryHandler>().territory.name);
                    playerTroops[turn] -= 1;
                    turn = nextTurn[turn];
                    playerTxt.color = nextTurnColor[turn];
                    playerTxt.text = "Player: " + turn.ToString();
                    if (currentScene.name == "PlayerVsAI")
                    {
                        if(PlayerTypeDict[turn] == "player" )
                        {
                            aiNext = false;
                            showAvailable();    
                        }                 
                    }
                    print("break");
                    break;
                }
            }
            if (getTerrtoriesOwnedNo(Territory.thePlayers.UNCLAIMED) == 0)
            {
                gameState = GameState.StartAssign;
                turn = Territory.thePlayers.PLAYER1;
                playerTxt.text = "Player: " + turn.ToString();
                troopsleft = playerTroops[turn];
                if (currentScene.name == "PlayerVsAI")
                {
                    if(PlayerTypeDict[turn] == "player" )
                    {
                            showAvailable();
                            eanblePlayerTerr(turn);
                            //add logic to switch to player select
                    }
                    else{
                        startAssign = true;
                    } 
                }
                else
                {
                    startAssign = true;
                } 
            }else
            {
                if (aiNext)
                {
                    startSelect = true;
                }
            }
        }
    }

    IEnumerator AIStartAssign()
    {
        yield return new WaitForSeconds(1);
        if (gameState == GameState.StartAssign)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            int remainder = troopsleft % playerTerritories[turn].Count;
            int troopSplit = (troopsleft - remainder) / playerTerritories[turn].Count;
            foreach (var terr in playerTerritories[turn])
            {
                territoryHandler terrScript= territoryDict[terr].GetComponent<territoryHandler>();
                terrScript.setTroopNo(terrScript.territory.troops + troopSplit);
                troopsleft = troopsleft - troopSplit;
                if (terrScript.name == territoryDict[playerTerritories[turn][0]].GetComponent<territoryHandler>().name)
                {
                    terrScript.setTroopNo(terrScript.territory.troops + remainder);
                    troopsleft = troopsleft - remainder;
                }
            }
            playerTroops[turn]  = troopsleft;
            turn = nextTurn[turn];
            troopsleft = playerTroops[turn];
            playerTxt.color = nextTurnColor[turn];
            playerTxt.text = "Player: " + turn.ToString();
            eanblePlayerTerr(turn);
            bool noTroopsLeft = true;
            foreach (var (key, value) in playerTroops)
            {
                if (value != 0)
                {
                    noTroopsLeft = false;
                }
            }
            foreach (var (key, value) in playerTroops)
            {
                print(key);
                print(value);   
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
                if(currentScene.name == "PlayerVsAI")
                {
                    if(PlayerTypeDict[turn] == "player")
                    {
                        eanblePlayerTerr(turn);
                    }
                    else
                    {
                        foreach(var terr in territoryList)
                        {
                            disableTerritory(terr);
                        }
                        assignTroopsAI = true;
                    }          
                }
                else
                {
                    foreach(var terr in territoryList)
                    {
                        disableTerritory(terr);
                    }
                    assignTroopsAI = true;
                }
            }
            else
            {            
                if(currentScene.name == "PlayerVsAI")
                {
                    if(PlayerTypeDict[turn] == "player")
                    {
                        eanblePlayerTerr(turn);
                    }
                    else{
                        startAssign = true;
                    }
                }
                else
                {
                    startAssign = true;
                }
            }
        }
    }

    IEnumerator AIAsignLogic()
    {
        if (gameState == GameState.MainState)
        {
            yield return new WaitForSeconds(1);
            if (gamePhase == GamePhase.Assign)
            {
                List<string> ownedterritories = new List<string>();
                ownedterritories = playerTerritories[turn];
                string largestTerritory = ownedterritories[0];
                for (int counter = 0; counter < ownedterritories.Count; counter++)
                {

                    territoryHandler pScript = territoryDict[ownedterritories[counter]].GetComponent<territoryHandler>();
                    territoryHandler largestTScript = territoryDict[largestTerritory].GetComponent<territoryHandler>();
                    if (pScript.territory.troops > largestTScript.territory.troops)
                    {
                        largestTerritory = pScript.territory.name;
                    }
                }
                print(troopsleft + " troops assigned to " + largestTerritory);
                AIAssign(largestTerritory, troopsleft);
                gamePhase = GamePhase.Attack;
                startAttackAI = true;
            }
        }
    }

    IEnumerator AIAttackLogic()
    {
        if (gameState == GameState.MainState && gamePhase == GamePhase.Attack)
        {
            List<string> ownedterritories = new List<string>();
            ownedterritories = playerTerritories[turn];
            for (int i = 0; i < ownedterritories.Count; i++)
            {
                territoryHandler pScript = territoryDict[ownedterritories[i]].GetComponent<territoryHandler>();
                if (pScript.territory.troops > 1)
                {
                    List<string> attacktargets = attackDict[ownedterritories[i]];
                    List<GameObject> desiredTargetList = new List<GameObject>();
                    List<string> sortedTerrsString = new List<string>();
                    foreach (var eterr in attacktargets)
                    {
                        territoryHandler eScript = territoryDict[eterr].GetComponent<territoryHandler>();
                        if (pScript.territory.troops > eScript.territory.troops)
                        {
                            desiredTargetList.Add(territoryDict[eterr]);
                            var sortedTerrs = desiredTargetList.OrderBy(o => o.GetComponent<territoryHandler>().territory.troops).ToList();
                            sortedTerrsString = sortedTerrs.Select(o => o.GetComponent<territoryHandler>().territory.name).ToList();
                        }
                    }
                    waitforAttack(sortedTerrsString, turn, ownedterritories[i]);
                }
            }
            gamePhase = GamePhase.Fortify;
            fortifyTerritoriesAI = true;
            yield return new WaitForSeconds(1); 
        }
    }

    IEnumerator AIFortifyLogic()
    {
        if(gameState == GameState.MainState && gamePhase == GamePhase.Fortify)
        {
            yield return new WaitForSeconds(1);
            List<string> ownedterritories = new List<string>();
            ownedterritories = playerTerritories[turn];
            string donater = null;
            string receiver = null;
            int donateAmount = 0;
            int donatedAmount = 0;
            for (int counter = 0; counter < ownedterritories.Count; counter++)
            {
                bool canDonate = true;
                List<string> attacktargets = attackDict[ownedterritories[counter]];
                territoryHandler pScript = territoryDict[ownedterritories[counter]].GetComponent<territoryHandler>();
                int donatableAmount = pScript.territory.troops;
                foreach (var eterr in attacktargets)
                {
                    territoryHandler eScript = territoryDict[eterr].GetComponent<territoryHandler>();
                    if (pScript.territory.getPlayer() == eScript.territory.getPlayer()){
                        if (pScript.territory.troops - 1 < donatableAmount)
                        {
                            // print(pScript.name + " has spare " + (pScript.territory.troops - (eScript.territory.troops + 1)));
                            donatableAmount = pScript.territory.troops - 1;
                        }
                    }
                    if (pScript.territory.getPlayer() != eScript.territory.getPlayer())
                    {
                        if (pScript.territory.troops <= eScript.territory.troops + 1)
                        {
                            canDonate = false;
                        }
                        if (pScript.territory.troops - (eScript.territory.troops + 1) < donatableAmount && pScript.territory.troops - (eScript.territory.troops + 1) > 0)
                        {
                            // print(pScript.name + " has spare " + (pScript.territory.troops - (eScript.territory.troops + 1)));
                            donatableAmount = pScript.territory.troops - (eScript.territory.troops + 1);
                        }
                    }
                }
                if (canDonate)
                {
                    if (donatableAmount > donateAmount)
                    {
                        donater = ownedterritories[counter];
                        donateAmount = donatableAmount;
                    }
                }
            }
            // print(donater + " is donating " + donateAmount);
            donatedAmount = 0;
            for (int counter = 0; counter < ownedterritories.Count; counter++)
            {
                bool canAttack = true;
                List<string> attacktargets = attackDict[ownedterritories[counter]];
                territoryHandler pScript = territoryDict[ownedterritories[counter]].GetComponent<territoryHandler>();
                int troopsneeded = 0;
                foreach (var eterr in attacktargets)
                {
                    territoryHandler eScript = territoryDict[eterr].GetComponent<territoryHandler>();
                    if (pScript.territory.getPlayer() != eScript.territory.getPlayer())
                    {
                        
                        if (pScript.territory.troops < eScript.territory.troops + 1)
                        {
                            canAttack = false;
                        }
                        if (pScript.territory.troops - (eScript.territory.troops + 1) < 0)
                        {
                            if (pScript.territory.troops - (eScript.territory.troops + 1) < troopsneeded)
                            {
                                // print(pScript.name + " needs " + (pScript.territory.troops - (eScript.territory.troops + 1)));
                                troopsneeded = pScript.territory.troops - (eScript.territory.troops + 1);
                            }
                        }
                    }

                }
                if (!canAttack)
                {
                    if (troopsneeded < donatedAmount && troopsneeded >= donateAmount * -1 )
                    {
                        donatedAmount = troopsneeded;
                        receiver = pScript.name;
                    }
                }
            }
            print(receiver + " needs " + donatedAmount * -1);
            if (donater != null && receiver != null)
            {
                AIFortify(donater, receiver, donateAmount);
            }
            else
            {
                print("skipped fortification");
            }
            //needs to change description text
            Scene currentScene = SceneManager.GetActiveScene();
            descriptionTxt.text = "Assign you troops to prepare to attack";
            turn = nextTurn[turn];
            playerTxt.color = nextTurnColor[turn];
            playerTxt.text = "Player: " + turn.ToString();
            gamePhase = GamePhase.Assign;
            playerTroops[turn] = newTroopAmount(turn);
            troopsleft = playerTroops[turn];
            tintTerritories();
            if (currentScene.name == "PlayerVsAI")
            {
                if(PlayerTypeDict[turn] == "player")
                {
                    eanblePlayerTerr(turn);
                    mapState = saveMapState();
                }
                else
                {
                    assignTroopsAI = true;
                }
            }
            else
            {
                assignTroopsAI = true;
            }
        }
    }

    public void AIAttack(Territory.thePlayers player, string terr1, string terr2)
    {
        System.Random rnd = new System.Random();
        AttackScript gui = attackPanel.GetComponent<AttackScript>();
        territoryHandler pterrhandler = territoryDict[terr1].GetComponent<territoryHandler>();
        territoryHandler eterrhandler = territoryDict[terr2].GetComponent<territoryHandler>();
        if (pterrhandler.territory.getPlayer() != eterrhandler.territory.getPlayer())
        {
            int ptroopAmount = pterrhandler.territory.troops;
            int etroopAmount = eterrhandler.territory.troops;
            bool battleWon = false;
            int pDiceAmount = 0;
            int eDiceAmount = 0;
            List<string> attackStats = new List<string>();
            while (ptroopAmount > 1 && battleWon == false)
            {
                List<int> PDiArray = new List<int>();
                List<int> EDiArray = new List<int>();    
                if (ptroopAmount> 3)
                {
                    pDiceAmount = 3;
                }
                else if (ptroopAmount==3)
                {
                    pDiceAmount = 2;
                }
                else if (ptroopAmount==2)
                {
                    pDiceAmount = 1;
                }
                if (ptroopAmount > etroopAmount)
                {
                    if (etroopAmount>=2)
                    {
                        eDiceAmount = 2;
                    }
                    else if (etroopAmount==1)
                    {
                        eDiceAmount = 1;
                    }
                }
                else
                {
                    eDiceAmount = pDiceAmount;
                }
                for (int i = 0; i < pDiceAmount; i++)
                {
                    PDiArray.Add(rnd.Next(1, 7));
                }
                for (int i = 0; i < eDiceAmount; i++)
                {
                    EDiArray.Add(rnd.Next(1, 7));
                }
                print(string.Format("Here's the list: ({0}).", string.Join(", ", PDiArray)));
                print(string.Format("Here's the list: ({0}).", string.Join(", ", EDiArray)));
                EDiArray.Sort();
                EDiArray.Reverse();
                PDiArray.Sort();
                PDiArray.Reverse();
                print(string.Format("Here's the list: ({0}).", string.Join(", ", PDiArray)));
                print(string.Format("Here's the list: ({0}).", string.Join(", ", EDiArray)));
                int comparisions = math.min(pDiceAmount, eDiceAmount);

                for (int i=0; i<comparisions; i++)
                {
                    if (etroopAmount > 0  && ptroopAmount > 1) 
                    { 
                        if (PDiArray[i] > EDiArray[i])
                        {
                            etroopAmount = etroopAmount - 1;
                            eterrhandler.setTroopNo(etroopAmount);
                            attackStats.Add("e-1");
                        }
                        else
                        {
                            ptroopAmount = ptroopAmount - 1;
                            pterrhandler.setTroopNo(ptroopAmount);
                            attackStats.Add("p-1");
                        }
                    }
                }
                
                if (etroopAmount == 0)
                {
                    battleWon = true;
                }

            }
            print(string.Format("Here's the list: ({0}).", string.Join(", ", attackStats)));
            string attackerName = pterrhandler.territory.name;
            string defenderName = eterrhandler.territory.name;
            if (battleWon)
            {
                removeTerrFromPlayer(eterrhandler.territory.getPlayer(), eterrhandler.territory.name);
                //logic to remove player who when they have no territories left
                List<Territory.thePlayers> removals = new List<Territory.thePlayers>();                    
                foreach (var key in playerTerritories.Keys)
                {
                    if(playerTerritories[key].Count == 0){
                        // print(key + "is removeable");
                        // print(string.Format("Here's the list: ({0}).", string.Join(", ", playerTerritories[key])));
                        removals.Add(key);
                    }
                }
                foreach(var person in removals)
                {
                    removePlayer(person);
                    print(person + "Removed");
                    playerTerritories.Remove(person);
                }
                eterrhandler.territory.setPlayer(pterrhandler.territory.getPlayer());                
                addTerrToPlayer(pterrhandler.territory.getPlayer(), eterrhandler.territory.name);
                eterrhandler.oldColor = pterrhandler.oldColor;
                eterrhandler.hoverColor = pterrhandler.hoverColor;
                eterrhandler.setTroopNo(pterrhandler.territory.troops - 1);
                pterrhandler.setTroopNo(1);
                tintTerritories();
                print(attackerName + " has defeated" + defenderName);
                if (gameover)
                {
                    descriptionTxt.text = playerList[0] + " has Won";
                }
            }else
            {
                print(attackerName + "has failed in his attack agains" + defenderName);
            }
        }
        else
        {
            // print(terr1 + "Can't attack " + terr2 + " since owned by same player");
            
        }
    }

    public void waitforAttack(List<string> sortedList, Territory.thePlayers turn, string terr1)
    {
        // print("wait!");
        territoryHandler pScript = territoryDict[terr1].GetComponent<territoryHandler>();
        int count = 0;
        while (count < sortedList.Count)
        {
            territoryHandler eScript = territoryDict[sortedList[count]].GetComponent<territoryHandler>();
            if (eScript.territory.troops < pScript.territory.troops)
            {         
                AIAttack(turn, terr1, sortedList[count]);
            }
            count = count +1;
        }
    }

    public void AIAssign(string terr1, int troopAmount)
    {
        territoryHandler terrhandler = territoryDict[terr1].GetComponent<territoryHandler>();
        if (terrhandler.territory.getPlayer() == turn)
        {
            if (troopsleft - troopAmount >= 0)
            {
                terrhandler.setTroopNo(terrhandler.territory.troops + troopAmount);
                troopsleft = troopsleft - troopAmount;
                playerTroops[turn] = troopsleft;
            }
            else
            {
                // print("not enough troops");
            }
        }
        else{
            // print("not enough troops");
        }
    }

    public void AIFortify(string terr1, string terr2, int troopAmount)
    {
        territoryHandler terrhandler = territoryDict[terr1].GetComponent<territoryHandler>();
        territoryHandler newTerrhandler = territoryDict[terr2].GetComponent<territoryHandler>();
        if (terr1 != terr2 && terrhandler.territory.getPlayer() == newTerrhandler.territory.getPlayer())
        {
            if (troopAmount < terrhandler.territory.troops)
            {
                int oldtroops = terrhandler.territory.troops;
                int oldtroopsnew = newTerrhandler.territory.troops;
                terrhandler.setTroopNo(terrhandler.territory.troops - troopAmount);
                newTerrhandler.setTroopNo(newTerrhandler.territory.troops + troopAmount);
                print(terrhandler.name + " had " + oldtroops + " and " + newTerrhandler.name + " had " + oldtroopsnew);
                print(terrhandler.name + " has " + terrhandler.territory.troops + " and " + newTerrhandler.name + " has " + newTerrhandler.territory.troops);
            }
            else
            {
                // print("Not enough troops to fortify");
            }
        }
        else
        {
            // print("these territories can't donate to eachother");
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
            foreach (var terr in territoryList)
            {
                territoryHandler terrhandler = terr.GetComponent<territoryHandler>();
                if (terrhandler.territory.getPlayer() == turn)
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
            if (terr.GetComponent<territoryHandler>().territory.getPlayer() == player)
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
        { gui.EDOneTxt, gui.EDTwoTxt};
        
        int eDice;
        territoryHandler pScript = attacker.GetComponent<territoryHandler>();
        territoryHandler eScript = defender.GetComponent<territoryHandler>();

        if (gui.EUnitValue > gui.DiceAmount)
        {
            if (gui.DiceAmount == 3)
            {
                eDice = 2;
            }
            else
            {
                eDice = gui.DiceAmount;
            }
        }
        else if(gui.EUnitValue > 3)
        {
            eDice = 2;
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
                if (i <2)
                {
                    edTextArray[i].text = EDiArray[i].ToString();
                }
            }
            else
            { 
                if (i <2)
                {
                    edTextArray[i].text = "N/A";
                }
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
            removeTerrFromPlayer(eScript.territory.getPlayer(), eScript.territory.name);
            List<Territory.thePlayers> removals = new List<Territory.thePlayers>();                    
            foreach (var key in playerTerritories.Keys)
            {
                if(playerTerritories[key].Count == 0){
                    print(key + "has no territories");
                    // print(string.Format("Here's the list: ({0}).", string.Join(", ", playerTerritories[key])));
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
        Scene currentScene = SceneManager.GetActiveScene();
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
            foreach (var (key, value) in playerTerritories)
            {
                print(key);
                print(string.Format("Here's the list: ({0}).", string.Join(", ", value)));
            }
            if (getTerrtoriesOwnedNo(Territory.thePlayers.UNCLAIMED) == 0)
            {
                gameState = GameState.StartAssign;
                turn = Territory.thePlayers.PLAYER1;
                playerTxt.text = "Player: " + turn.ToString();
                troopsleft = playerTroops[turn];

                eanblePlayerTerr(turn);
            }
            else
            {
                if(currentScene.name == "PlayerVsAI")
                {
                    if(PlayerTypeDict[turn] == "ai")
                    {
                        foreach (var terr in territoryList)
                        {
                            disableTerritory(terr);
                        }
                        startSelect = true;
                    }
                }
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
                print("notroops");
                turn = Territory.thePlayers.PLAYER1;
                eanblePlayerTerr(turn);
                playerTxt.text = "Player: " + turn.ToString();
                descriptionTxt.text = "Assign you troops to prepare to attack";
                gameState = GameState.MainState;
                gamePhase = GamePhase.Assign;
                playerTroops[turn] = newTroopAmount(turn);
                troopsleft = playerTroops[turn];
                mapState = saveMapState();
                if(currentScene.name == "PlayerVsAI")
                {
                    if(PlayerTypeDict[turn] == "ai")
                    {
                        eanblePlayerTerr(Territory.thePlayers.UNCLAIMED);
                        assignTroopsAI = true;
                    }
                }
            }else
            {
                print("aiswitch");
                if(currentScene.name == "PlayerVsAI")
                {
                    if(PlayerTypeDict[turn] == "ai")
                    {
                        foreach (var terr in territoryList)
                        {
                            disableTerritory(terr);
                        }
                        startAssign = true;
                    }
                }
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
        Scene currentScene = SceneManager.GetActiveScene();
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
            if(currentScene.name == "PlayerVsAI")
            {
                if(PlayerTypeDict[turn] == "ai")
                {
                    foreach(var terr in territoryList)
                    {
                        disableTerritory(terr);
                    }
                    assignTroopsAI = true;
                }
            }
            else
            {
                eanblePlayerTerr(turn);
            }
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
        { gui.EDOneTxt, gui.EDTwoTxt};
        for (int i = 0; i < 3; i++)
        {
            pdTextArray[i].text = "N/A";
            if (i <2)
            {
                edTextArray[i].text = "N/A";
            }
        }
    }
}
