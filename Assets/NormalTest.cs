using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeSlip.Await;
using UnityEngine.UI;
using System;

public class NormalTest : MonoBehaviour
{
    public RawImage BackImage;
    public Button BuildButton;
    public Text CountText;
    private Texture2D MyTexture;

    // Use this for initialization
    void Start()
    {
        BuildButton.onClick.AddListener(BuildClick);
        MyTexture = new Texture2D(1024, 1024);
        BackImage.texture = MyTexture;
    }

    private void BuildClick()
    {
        int index = UnityEngine.Random.Range(0, 3);
        switch (index)
        {
            case 2:
                GenerateMandelbrot2();
                break;
            case 1:
                GenerateMandelbrot();
                break;
            case 0:
            default:
                GenerateNormal();
                break;
        }
    }

    private void GenerateNormal()
    {
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                Color color = new Color(Utils.RedNormal(i, j) / 255f, Utils.GreenNormal(i, j) / 255f, Utils.BlueNormal(i, j) / 255f);
                MyTexture.SetPixel(i, j, color);
            }
        }
        MyTexture.Apply();
    }

    private void GenerateMandelbrot()
    {
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                Color color = new Color(Utils.RedMandelbrot(i, j) / 255f, Utils.GreenMandelbrot(i, j) / 255f, Utils.BlueMandelbrot(i, j) / 255f);
                MyTexture.SetPixel(i, j, color);
            }
        }
        MyTexture.Apply();
    }


    private void GenerateMandelbrot2()
    {
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                Color color = new Color(Utils.RedMandelbrot2(i, j) / 255f, Utils.GreenMandelbrot2(i, j) / 255f, Utils.BlueMandelbrot2(i, j) / 255f);
                MyTexture.SetPixel(i, j, color);
            }
        }
        MyTexture.Apply();
    }
    // Update is called once per frame
    void Update()
    {
        CountText.text = Time.frameCount.ToString();
    }



}
