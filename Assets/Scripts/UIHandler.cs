using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public string noiseEquation;

    [SerializeField] private TMP_InputField equationField;
    [SerializeField] private TMP_InputField seedField;
    [SerializeField] private MapGenerator mapGenerator;

    public void UpdateGenerationEquation()
    {
        mapGenerator.generationEquation = equationField.text;
    }

    public void UpdateSeed()
    {
        mapGenerator.seed = Int32.Parse(seedField.text);
    }
    
}
