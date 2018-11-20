using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryLine : MonoBehaviour
{

    public TextMesh paperHint;
    public string[] hintTexts;
    public TextMesh globalHint;
    private int step;
    //private bool isGlobalHintDisplay;

    // Use this for initialization
    void Start()
    {
        step = -1;
        //isGlobalHintDisplay = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (step >= 0)
        {
            globalHint.text = "";
        }
    }

    public void UpdateHintText(int currentStep)
    {
        this.step = currentStep;
        if (step < hintTexts.Length)
        {
            paperHint.text = hintTexts[step].Replace("\\n", "\n");
        }
    }

    public void SetCurrentStep(int currentStep)
    {
        this.step = currentStep;
    }
}
