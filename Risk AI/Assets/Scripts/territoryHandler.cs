using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(PolygonCollider2D))]

public class territoryHandler : MonoBehaviour
{

    public Territory territory;

    private SpriteRenderer sprite;

    public TMP_Text TerrTxt;
    
    public Color32 oldColor;

    public Color32 hoverColor;

    public Color32 startColor;

    public Color32 disabledColor;


    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        //disabledColor = new Color32(76, 125, 125, 0);
    }

    public void setTroopNo(int troopAmount)
    {
        TerrTxt.text = troopAmount.ToString();
        territory.troops = troopAmount;
    }

    void OnMouseEnter()
    {
        oldColor = sprite.color;
        hoverColor = oldColor;
        hoverColor.a = 128;
        sprite.color = hoverColor;
    }

    void OnMouseExit() 
    {
        sprite.color = oldColor;
    }

    void OnMouseUpAsButton()
    {
        if (territoryManager.instance.gameState == territoryManager.GameState.Start)
        {
            var tmInstance = territoryManager.instance;
            tmInstance.transferTarget = tmInstance.territoryDict[territory.name];
            this.setTroopNo(territory.troops + 1);
            foreach (var terr in tmInstance.territoryList)
            {
                territoryManager.instance.disableTerritory(terr);
            }
            tmInstance.transferTarget.GetComponent<territoryHandler>().territory.setPlayer(tmInstance.turn);
            tmInstance.tintTerritory(tmInstance.transferTarget);
            tmInstance.cancelBtn.SetActive(true);
            tmInstance.transferBtn.SetActive(true);
        }
        else
        {
            if (territoryManager.instance.transferReady)
            {
                territoryHandler attackerScript = territoryManager.instance.attacker.GetComponent<territoryHandler>();
                territoryHandler defenderScript = territoryManager.instance.defender.GetComponent<territoryHandler>();
                if (territory.name == attackerScript.territory.name)
                {
                    print("attacker");
                    if (defenderScript.territory.troops > 1)
                    {
                        setTroopNo(attackerScript.territory.troops + 1);
                        defenderScript.setTroopNo(defenderScript.territory.troops - 1);
                    }
                }
                else if (territory.name == defenderScript.territory.name)
                {
                    print("defender");
                    if (attackerScript.territory.troops > 1)
                    {
                        setTroopNo(defenderScript.territory.troops + 1);
                        attackerScript.setTroopNo(attackerScript.territory.troops - 1);
                    }
                }
            }
            else
            {
                ShowAttackGUI();
            }
        }
    }

    private void OnDrawGizmos()
    {
        territory.name = name;
        this.tag = "Territory";
    }

    public void TintColor(Color32 color)
    {
        sprite.color = color;
    }

    void ShowAttackGUI()
    {
        if (territoryManager.instance.attacking)
        {
            string desTxt;
            string attackUnits;
            string enemyUnits;
            desTxt = ("You are attacking " + territory.name + " owned by "
                      + territory.getPlayer().ToString() + " Are you sure you want to attack");
            attackUnits = territoryManager.instance.attacker.GetComponent<territoryHandler>().TerrTxt.text;
            enemyUnits = TerrTxt.text;
            territoryManager.instance.ShowAttackPanel(desTxt, attackUnits, enemyUnits, name);
        }else
        {
            territoryManager.instance.ShowTargets(name);
        }

    }
}
