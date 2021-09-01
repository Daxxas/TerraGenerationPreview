using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public string noiseEquation;

    [SerializeField] private TMP_InputField equationField;
    [SerializeField] private TMP_InputField seedField;
    [SerializeField] private TMP_InputField widthField;
    [SerializeField] private TMP_InputField lengthField;
    [SerializeField] private Slider waterLevelSlider;
    [SerializeField] private TMP_InputField waterLevelField;
    [SerializeField] private MapGenerator mapGenerator;

    private void Start()
    {
        // UpdateWaterLevelFromField();
        // waterLevelSlider.maxValue = mapGenerator.ChunkHeight;
    }

    // public void UpdateGenerationEquation()
    // {
    //     mapGenerator.generationEquation = equationField.text;
    // }
    //
    // public void UpdateSeed()
    // {
    //     try
    //     {
    //         mapGenerator.equationHandler.ChangeSeed(Int32.Parse(seedField.text));
    //     }
    //     catch (Exception e)
    //     {
    //         throw;
    //     }
    // }
    //
    // public void UpdateLength()
    // {
    //     try
    //     {
    //         mapGenerator.chunkSize = Int32.Parse(lengthField.text);
    //
    //     }
    //     catch (Exception e)
    //     {
    //         throw;
    //     }
    // }
    //
    // public void UpdateWidth()
    // {
    //     try
    //     {
    //         mapGenerator.chunkSize = Int32.Parse(widthField.text);
    //
    //     }
    //     catch (Exception e)
    //     {
    //         throw;
    //     }
    // }
    //
    // public void UpdateWaterLevelFromSlider()
    // {
    //     mapGenerator.waterLevel = waterLevelSlider.value;
    //     // waterLevelField.text = mapGenerator.waterLevel.ToString();
    // }
    //
    // public void UpdateWaterLevelFromField()
    // {
    //     try
    //     {
    //         // mapGenerator.waterLevel = Int32.Parse(waterLevelField.text);
    //         // mapGenerator.UpdateWaterPreview();
    //         // waterLevelSlider.value = mapGenerator.waterLevel;
    //     }
    //     catch (Exception e)
    //     {
    //         throw;
    //     }
    // }
}
