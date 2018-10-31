using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TimeSlip.Await;

public class MyTest : MonoBehaviour
{

    public Image TestImage;

    public Text TestText;

    public Text NumText;

    public int n;

    public int num;
    public bool _isCancel = false;
    //private Task _currentTask;
    //CancellationTokenSource cancellationTokenSource;
    CancellationTokenSource cancellationTokenSource;
    // Use this for initialization
    void Start()
    {
        //num = 10;
        cancellationTokenSource = new CancellationTokenSource();
        RunAsync(cancellationTokenSource.Token);

        //Test();
        StartCoroutine(TestAAA());
        Task.Run(Test);

        //Debug.LogError();
        //TestButton.onClick.AddListener(ButtonClick);
    }

    private IEnumerator TestAAA()
    {
        yield return new WaitForSeconds(5f);
        Debug.LogError("aaaaa");
        cancellationTokenSource.Cancel();
    }

    private async Task Test()
    {
        while (n < 1000)
        {
            //try
            //{

            await new WaitForBackgroundThread();
            n++;
            await new WaitForUpdate();
            NumText.text = n.ToString();
            //}
            //catch (Exception ex)
            //{
            //    Debug.LogError(ex.Message);

            //    throw;
            //}

        }

    }

    private async void RunAsync(CancellationToken token)
    {
        while (num > 0 && !_isCancel)
        {
            TestText.text = num.ToString();
            num--;
            await RunTest();
        }
    }

    private async Task RunTest()
    {
        TestImage.color = GetColor();
        await new WaitForRealSeconds(1f);
    }

    private Color GetColor()
    {
        float red = UnityEngine.Random.Range(0.0f, 1.0f);
        float green = UnityEngine.Random.Range(0.0f, 1.0f);
        float blue = UnityEngine.Random.Range(0.0f, 1.0f);
        return new Color(red, green, blue);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    NumText.text = n.ToString();
    //    n++;
    //}
}
