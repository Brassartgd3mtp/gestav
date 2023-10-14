using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : UnitManager
{
    private Character character;
    protected override Unit Unit
    {
        get { return character; }
        set { character = value is Character ? (Character)value : null; }
    }
}