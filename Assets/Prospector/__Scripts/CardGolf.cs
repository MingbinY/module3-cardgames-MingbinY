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
    public SlotDef slotDef;
    public int layoutID;

    public override void OnMouseUpAsButton()
    {
        base.OnMouseUpAsButton();
    }
}
