using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "FishFile")]
public class FishFile : ScriptableObject{

    public enum WhatKindOfFish { adenPufferfish, adriaKissingGourami, caspianPiranha, icariClownfish, jerryBlobfish, karaSeaBunnySlug, thraciMoorishIdol, tyrrhenCatfish};
    public WhatKindOfFish whatFish;
    [Tooltip("Related to the date number")]
    public int dateProgress;
    public int redAnswers;
    public int blueAnswers;
    public int greenAnswers;
    [Space]
    [Space]
    public List<DateBlock> dates;
}
