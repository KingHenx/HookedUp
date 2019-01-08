using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DateBlock")]
public class DateBlock : ScriptableObject {

    [Tooltip("First date, second date, .etc")]
    public int dateNumber;
    [Space]
    public int redAnswersNeeded;
    public int blueAnswersNeeded;
    public int greenAnswersNeeded;
    [Space]
    public DialogueBlock startingDialogue;
}
