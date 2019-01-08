using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "DialogueBlock")]
public class DialogueBlock : ScriptableObject
{
    [TextArea]
    public string dialogue;
    //public List<string> dialogueParts;
    [Space]
    public List<Answer> answers;

    [System.Serializable]
    public struct Answer
    {
        public string playerAnswer;
        public DialogueBlock nextBlockFrom;
        public enum AnswerColor{ red, blue, green, colorless}
        public AnswerColor color;
    }
}
