using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public static DialogueManager instance;

    public enum DialogueState
    {
        inactive, dialogueStart, fishTalks, playerAnswer
    }
    public DialogueState state;

    public FishFile currentFish;
    public List<FishFile> fish;

    public enum Fish
    {
        kissingGourami, clownfish, seaBunnySlug, pufferFish, moorishIdol, cafish, piranha, blobfish
    }


    [System.Serializable]
    public struct TextFileSplitted
    {
        List<string> textLines;
    }

    //[Space]
    //public List<FishFiles> fishFiles;

    private int index = 0;
    private bool set;

    List<GameObject> answers = new List<GameObject>(9);
    [Space]
    public GameObject textField;
    public DialogueBlock currentDialogueBlock;
    List<string> currentSplittedTextFile;

    public bool resetFishAtStart;


    // Use this for initialization
    void Awake () {

        if(instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }


        if (Application.isEditor && resetFishAtStart)
        {
            foreach (FishFile file in fish)
            {
                file.redAnswers = 0;
                file.blueAnswers = 0;
                file.greenAnswers = 0;
                file.dateProgress = 0;
            }
        }


        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Answer").Length; i++)
        {
            answers.Add(gameObject);

        }


        foreach(GameObject answer in GameObject.FindGameObjectsWithTag("Answer"))
        {
            string text = answer.name.Substring(answer.name.Length -1 , 1);
            int num;
            int.TryParse(text, out num);
            answers[num -1] = answer;
            answer.SetActive(false);
        }



        textField.SetActive(false);

	}

	// Update is called once per frame
	void Update () {
    

        switch (state)
        {
            case DialogueState.inactive:
                Inactive();
                break;

            case DialogueState.dialogueStart:
                DialogueStart();
                break;

            case DialogueState.fishTalks:
                FishTalks();
                break;

            case DialogueState.playerAnswer:
                PlayerAnswer();
                break;
        }

    }

    void Inactive()
    {
        if (!set)
        {
            textField.SetActive(false);
            foreach (GameObject answer in answers)
            {
                answer.SetActive(false);
            }
        }
    }

    void DialogueStart()
    {
        print(currentFish.dateProgress);

        foreach (GameObject answer in answers)
        {
            answer.SetActive(false);
        }

        currentDialogueBlock = currentFish.dates.Find(x => x.dateNumber == currentFish.dateProgress).startingDialogue;
        textField.SetActive(true);
        SplitAndType();

        state = DialogueState.fishTalks;
    }


    void FishTalks()
    {
        foreach (GameObject answer in answers)
        {
            answer.SetActive(false);
        }

        // Player click next line
        if (Input.GetMouseButtonDown(0))
        {
            SplitAndType();
        }
        if (index >= currentSplittedTextFile.Count)
        {
            index = 0;

            if (currentDialogueBlock.answers.Count != 0)
            {
                state = DialogueState.playerAnswer;
            }
            else
            {
                EndDate();
            }
        }

    }

    void PlayerAnswer()
    {
        for (int i = 0; i < currentDialogueBlock.answers.Count; i++)
        {
            answers[i].SetActive(true);
            answers[i].GetComponentInChildren<Text>().text = currentDialogueBlock.answers[i].playerAnswer;
        }

    }


    public void ChangeCurrentFish(FishFile fish)
    {
        currentFish = fish;
    }

    //public void StartDialogue()
    //{
    //    if(startDialogue)
    //    {
    //        //currentDialogueBlock = fishFiles.Find(x => x.fish == currentFish)
    //        //                                .dates.Find(x => x.dateNumber == fishFiles.Find(z => z.fish == currentFish).dateProgress)
    //        //                                .startingDialogue;

    //        //currentSplittedTextFile = new List<string>(currentDialogueBlock.dialogue.Split('/'));
    //        //textField.GetComponent<Text>().text = currentSplittedTextFile[1];
    //        //currentDialogueBlock.dialogue = "heyyy";

    //        startDialogue = false;
    //    }
    //}

    public void Answer(int answerNumber)
    {
        if (currentDialogueBlock.answers[answerNumber].nextBlockFrom != null 
            && currentDialogueBlock.answers.Count != 0)
        {
            switch (currentDialogueBlock.answers[answerNumber].color)
            {
                case DialogueBlock.Answer.AnswerColor.red:
                    currentFish.redAnswers++;
                    break;

                case DialogueBlock.Answer.AnswerColor.blue:
                    currentFish.blueAnswers++;
                    break;

                case DialogueBlock.Answer.AnswerColor.green:
                    currentFish.greenAnswers++;
                    break;
            }
            currentDialogueBlock = currentDialogueBlock.answers[answerNumber].nextBlockFrom;
            state = DialogueState.fishTalks;
            SplitAndType();
        }
        else
        {
            //Adria specialcase
            if (currentDialogueBlock.name == "Adria_Intro" 
                || currentDialogueBlock.name != "1Toss"
                || currentDialogueBlock.name != "2Toss"
                || currentDialogueBlock.name != "3Toss"
                || currentDialogueBlock.name != "4Toss")
            {
                TossBackAdria();
            }
            else if(currentDialogueBlock.name != "5Toss")
            {
                print("Adria gameover??");
            }
            else
            {
                EndDate();
            }
        }
    }

    public void EndDate()
    {
        if (currentFish.dateProgress > 999)
        {
            currentFish.dateProgress = 0;
        }

        //normal
        currentFish.dateProgress += 1;
        state = DialogueState.inactive;
        print("End Date");
        GameManager.instance.state = GameManager.State.fishing;
        
    }

    void SplitAndType()
    {
        if (currentDialogueBlock != null)
        {
            currentSplittedTextFile = new List<string>(currentDialogueBlock.dialogue.Split('\n'));

            if (index < currentSplittedTextFile.Count)
            {
                textField.GetComponent<TypingEffect>().NextText(currentSplittedTextFile[index]);
            }
        }
        index += 1;
    }

    void TossBackAdria()
    {
        currentFish.dateProgress += 1000;
        state = DialogueState.inactive;
        print("Toss Back Adrian never works");
    }
}
