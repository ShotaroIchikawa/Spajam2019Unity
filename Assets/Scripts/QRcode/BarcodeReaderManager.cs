using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarcodeReaderManager : MonoBehaviour {
    public Text resultText; //読み取り結果
    public RawImage cameraPanel;

    public Button btnRegist;
    public Button btnCancel;

    private WebCamTexture webcamTexture;
    private WebcamCodeReader reader;

    private int width = 640;
    private int height = 480;
    private string foundString = null;

    private IEnumerator Start()
    {
        // ボタンは最初は非表示
        changeBtnVisible(false);

        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogFormat("カメラがありません。");
            yield break;
        }

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogFormat("カメラ利用が許可されていません。");
            yield break;
        }
        
        WebCamDevice userCameraDevice = WebCamTexture.devices[0];

        webcamTexture = new WebCamTexture(userCameraDevice.name, width, height);
        //webcamTexture = new WebCamTexture(userCameraDevice.name);

        cameraPanel.texture = webcamTexture;
        webcamTexture.Play();

        Debug.Log(webcamTexture.width + " " + webcamTexture.height + " " + webcamTexture.requestedFPS);

        // バーコードリーダーのセット
        this.reader = new WebcamCodeReader();
    }

    // Update is called once per frame
    void Update()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying)
        {
            return;
        }

        if (foundString == null)
        {
            // コード発見前
            // バーコード認識
            
            string codeResult = this.reader.Read(webcamTexture);
            if (codeResult == null)
            { 
                resultText.text = "<color=#111111>scanning...</color>";
            }
            else
            {
                foundString = codeResult;
                resultText.text = "<color=#111111>" + codeResult + "</color>";
            }
        }
        else
        {
            //コード発見後
            changeBtnVisible(true);
            Debug.Log(resultText.text);
        }
    }

    void changeBtnVisible(bool state)
    {
        btnRegist.gameObject.SetActive(state);
        btnCancel.gameObject.SetActive(state);
    }

    private void openBarcode(string str)
    {
        if (str.StartsWith("http"))
        {
            Application.OpenURL(str);
        }
        else
        {
            Application.OpenURL("http://www.google.com/search?q=" + str);
        }
    }

    public void onBtnRegistClick()
    {
        openBarcode(foundString);
    }

    public void onBtnCancelClick()
    {
        changeBtnVisible(false);
        foundString = null;
    }
}
