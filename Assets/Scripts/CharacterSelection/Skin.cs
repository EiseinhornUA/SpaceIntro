using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skin
{
    [field: SerializeField] public Color skinColor {  get; private set; }
    [field: SerializeField] public Color beardColor {  get; private set; }
    [field: SerializeField] public Color lipsColor {  get; private set; }

}