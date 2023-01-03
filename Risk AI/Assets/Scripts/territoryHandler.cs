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
        ShowGUI();
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

    void ShowGUI()
    {
        string desTxt;
        string attackUnits;
        string enemyUnits;
        desTxt = ("You are attacking " + territory.name + " owned by "
                  + territory.player.ToString() + " Are you sure you want to attack");
        if (territoryManager.instance.attacking)
        {
            attackUnits = territoryManager.instance.attacker.GetComponent<territoryHandler>().TerrTxt.text;
            enemyUnits = TerrTxt.text;
            territoryManager.instance.ShowAttackPanel(desTxt, attackUnits, enemyUnits);
            territoryManager.instance.ShowTargets(name);
        }else
        {
            territoryManager.instance.ShowTargets(name);
        }

    }
}
