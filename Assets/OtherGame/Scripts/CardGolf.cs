using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum golfCardState
{
    drawpile,
    tableau,
    target,
    discard
}
public class CardGolf : Card
{
    public golfCardState state = golfCardState.drawpile;
    public List<CardGolf> hiddenBy = new List<CardGolf>();
    public SlotDef slotDef;
    public int layoutID;
    public Color greyoutColor;

    public override void OnMouseUpAsButton()
    {
        Golf.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }
}
