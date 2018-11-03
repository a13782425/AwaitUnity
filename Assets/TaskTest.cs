using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeSlip.Await;
using UnityEngine.UI;
using System;

public class TaskTest : MonoBehaviour
{

    public RawImage BackImage;
    public Button BuildButton;
    public Text CountText;

    public Image ProgressPic;

    public Text ProgressText;

    private Texture2D MyTexture;

    private float _progress = 0f;
    private float _allProgress = 1024f * 1024f;


    // Use this for initialization
    void Start()
    {
        BuildButton.onClick.AddListener(BuildClick);
        MyTexture = new Texture2D(1024, 1024);
        BackImage.texture = MyTexture;
        ProgressPic.gameObject.SetActive(false);
    }

    private void BuildClick()
    {
        BuildButton.enabled = false;
        int index = UnityEngine.Random.Range(0, 3);
        ProgressPic.fillAmount = 0;
        ProgressText.text = "0%";
        ProgressPic.gameObject.SetActive(true);
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

    private async void Test()
    {
        //此语句后的代码会放在子线程执行
        await new WaitForBackgroundThread();
        //等待1秒。不受Scale影响
        await new WaitForRealSeconds(1f);
        //从主线程上拿到某个游戏物体的参数
        Vector3 pos = await new WaitForFunc<Vector3>(() => { return ProgressPic.transform.position; });
        //将某个参数扔到主线程做操作
        await new WaitForAction<Vector3>((a) => { CountText.text = a.ToString(); }, pos);
        //等待1秒。受Scale影响
        await new WaitForSeconds(1f);
        //此语句后的代码会放在主线程执行
        await new WaitForUpdate();
    }

    private async void GenerateNormal()
    {
        //将大量计算放在子线程
        await new WaitForBackgroundThread();
        Color[,] colors = new Color[1024, 1024];
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                _progress = 1024 * i + j;
                colors[i, j] = new Color(Utils.RedNormal(i, j) / 255f, Utils.GreenNormal(i, j) / 255f, Utils.BlueNormal(i, j) / 255f);
            }
        }
        //计算完成后返回主线程
        await new WaitForUpdate();
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                MyTexture.SetPixel(i, j, colors[i, j]);
            }
        }
        MyTexture.Apply();
        BuildButton.enabled = true;
        ProgressPic.gameObject.SetActive(false);
    }

    private async void GenerateMandelbrot()
    {
        //把计算量巨大的放在子线程
        await new WaitForBackgroundThread();
        Color[,] colors = new Color[1024, 1024];
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                _progress = 1024 * i + j;
                colors[i, j] = new Color(Utils.RedMandelbrot(i, j) / 255f, Utils.GreenMandelbrot(i, j) / 255f, Utils.BlueMandelbrot(i, j) / 255f);
            }
        }
        //计算完成后返回主线程
        await new WaitForUpdate();
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                MyTexture.SetPixel(i, j, colors[i, j]);
            }
        }
        MyTexture.Apply();
        BuildButton.enabled = true;
        ProgressPic.gameObject.SetActive(false);
    }


    private async void GenerateMandelbrot2()
    {
        //把计算量巨大的放在子线程
        await new WaitForBackgroundThread();
        Color[,] colors = new Color[1024, 1024];
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                _progress = 1024 * i + j;
                colors[i, j] = new Color(Utils.RedMandelbrot2(i, j) / 255f, Utils.GreenMandelbrot2(i, j) / 255f, Utils.BlueMandelbrot2(i, j) / 255f);
            }
        }
        //计算完成后返回主线程
        await new WaitForUpdate();
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                MyTexture.SetPixel(i, j, colors[i, j]);
            }
        }
        MyTexture.Apply();
        BuildButton.enabled = true;
        ProgressPic.gameObject.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        //CountText.text = Time.frameCount.ToString();
        if (ProgressPic.gameObject.activeInHierarchy)
        {
            ProgressPic.fillAmount = _progress / _allProgress;
            ProgressText.text = (int)(_progress / _allProgress * 100) + "%";
        }
    }
}
